using DevExpress.Xpf.Core;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vortragsmanager.Datamodels;
using System.Windows.Forms;
using Vortragsmanager.Properties;

namespace Vortragsmanager.Core
{
    internal class IoExcel
    {
        public static class File
        {
            public static void Save(string tempName, string sugestedName, bool open)
            {
                Log.Info(nameof(Save), $"tempName={tempName}, sugestedName={sugestedName}");
                var saveFileDialog1 = new SaveFileDialog
                {
                    Filter = Resources.DateifilterExcel,
                    FilterIndex = 1,
                    RestoreDirectory = false,
                    FileName = sugestedName,
                };

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Log.Info(nameof(Save), $"{saveFileDialog1.FileName}");
                    var fi = new FileInfo(saveFileDialog1.FileName);
                    var filename = fi.FullName;
                    var i = 0;
                    try
                    {
                        System.IO.File.Delete(filename);
                    }
                    catch
                    {
                        while (System.IO.File.Exists(filename))
                        {
                            i++;
                            filename = $"{fi.DirectoryName}\\{fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length)} ({i}){fi.Extension}";
                        }
                    }
                    finally
                    {
                        System.IO.File.Move(tempName, filename);
                        if (open)
                            System.Diagnostics.Process.Start(filename);
                    }
                }
                saveFileDialog1.Dispose();
            }
        }

        public static void UpdateSpeakers(string File)
        {
            Log.Info(nameof(UpdateSpeakers), File);
            var fi = new FileInfo(File);

            using (ExcelPackage package = new ExcelPackage(fi))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Redner"];
                var row = 2;
                var id = DataContainer.Redner.Count > 0 ? DataContainer.Redner.Select(x => x.Id).Max() + 1 : 1;
                while (true)
                {
                    var vers = worksheet.Cells[row, 1].Value;
                    var name = worksheet.Cells[row, 2].Value;
                    var vort = worksheet.Cells[row, 3].Value;

                    if (vers == null)
                        break;

                    //Versammlung
                    var rednerVersammlung = DataContainer.ConregationFindOrAdd(vers.ToString());

                    var s = DataContainer.SpeakerFind(name.ToString(), rednerVersammlung);
                    if (s == null)
                    {
                        s = new Speaker
                        {
                            Name = name.ToString(),
                            Id = id,
                            Versammlung = rednerVersammlung,
                        };
                        DataContainer.Redner.Add(s);
                        id++;
                    }

                    //Vorträge
                    var meineVotrgäge = vort.ToString().Split(new[] { ';', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var v in meineVotrgäge)
                    {
                        var nr = int.Parse(v, Helper.German);
                        var t = TalkList.Find(nr);
                        if (!(t is null) && !s.Vorträge.Select(y => y.Vortrag).Contains(t))
                            s.Vorträge.Add(new TalkSong(t));
                    }

                    row++;
                }
            } // the using statement automatically calls Dispose() which closes the package.
        }

        internal static class Vplanung
        {
            public static int Kreis { get; set; } = -1;

            public static List<Conregation> Conregations = new List<Conregation>();

            public static bool ImportKoordinatoren(string filename)
            {
                Log.Info(nameof(ImportKoordinatoren), filename);
                var file = new FileInfo(filename);
                Conregations = new List<Conregation>();

                try
                {
                    using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (ExcelPackage package = new ExcelPackage(fs))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                        var pos = worksheet.Name.LastIndexOf(' ');
                        var kreisString = worksheet.Name.Substring(pos + 1, worksheet.Name.Length - pos - 1);
                        Kreis = int.Parse(kreisString, Helper.German);

                        var row = 2;
                        var id = 1;
                        if (DataContainer.Versammlungen.Count > 0)
                            id = DataContainer.Versammlungen.Max(x => x.Id) + 1;
                        while (true)
                        {
                            var vers = worksheet.Cells[row, 1].Value;
                            var saal = worksheet.Cells[row, 2].Value;
                            var zeit2019 = worksheet.Cells[row, 3].Value;
                            var zeit2020 = worksheet.Cells[row, 4].Value;
                            var koord = worksheet.Cells[row, 5].Value;
                            var tel = worksheet.Cells[row, 6].Value;
                            var handy = worksheet.Cells[row, 7].Value;
                            var mail = worksheet.Cells[row, 8].Value;
                            var jw = worksheet.Cells[row, 9].Value;

                            if (vers is null)
                                break;

                            var verString = vers.ToString();
                            if (verString.Length > 23 && verString.Substring(0, 23) == "Hinweis zum Datenschutz")
                                break;

                            var v = new Conregation
                            {
                                Id = id,
                                Kreis = Kreis,
                                Name = vers.ToString(),
                                Koordinator = koord.ToString(),
                                KoordinatorTelefon = tel?.ToString(),
                                KoordinatorMobil = handy?.ToString(),
                                KoordinatorMail = mail?.ToString(),
                                KoordinatorJw = jw?.ToString(),
                            };

                            // Anschrift
                            var adr = saal.ToString().Split(Environment.NewLine.ToCharArray());
                            v.Anschrift1 = adr[0];
                            v.Anschrift2 = adr[1];
                            v.Telefon = adr[2];

                            // Zusammenkunftszeiten
                            var z19 = zeit2019.ToString();
                            var z20 = zeit2020.ToString();
                            if (z20 == "liegt nicht vor")
                                z20 = z19;
                            v.SetZusammenkunftszeit(2019, z19);
                            if (z20 != z19)
                                v.SetZusammenkunftszeit(2020, z19);

                            Conregations.Add(v);

                            row++;
                            id++;
                        }
                    }
                }
                catch (Exception e)
                {
                    ThemedMessageBox.Show("Fehler",
                        $"Beim Einlesen der Excel-Datei ist es zu folgendem Fehler gekommen\n:{e.Message}",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                    return false;
                }
                return true;
            }

            public static List<IEvent> MeinPlan { get; } = new List<IEvent>();

            public static List<Outside> ExternerPlan { get; } = new List<Outside>();

            public static bool ImportEigenePlanungen(string filename)
            {
                Log.Info(nameof(ImportEigenePlanungen), filename);
                try
                {
                    var file = new FileInfo(filename);

                    using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (ExcelPackage package = new ExcelPackage(fs))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[2];

                        var row = 2;
                        while (true)
                        {
                            var datum = worksheet.Cells[row, 1].Value;
                            var redner = worksheet.Cells[row, 2].Value;
                            var vortrag = worksheet.Cells[row, 3].Value;
                            var thema = worksheet.Cells[row, 4].Value;
                            var versammlung = worksheet.Cells[row, 5].Value;
                            var rednerTelefon = worksheet.Cells[row, 6].Value?.ToString();
                            var rednerHandy = worksheet.Cells[row, 7].Value?.ToString();
                            row++;

                            if (datum == null) //Die Liste ist zu Ende
                                break;

                            if (datum.ToString() == "Datum")
                                continue;

                            if (redner == null && thema == null) //Keine Planung eingetragen
                                continue;

                            var kw = Core.Helper.CalculateWeek(DateTime.Parse(datum.ToString(), Helper.German));

                            if (redner == null) //Special Event eintragen
                            {
                                var se = new SpecialEvent
                                {
                                    Kw = kw,
                                    Typ = SpecialEventTyp.Sonstiges,
                                };
                                var typ = thema.ToString();
                                if (typ.Contains("Weltzentrale"))
                                {
                                    se.Typ = SpecialEventTyp.Streaming;
                                    se.Name = typ;
                                }
                                else if (typ.Contains("Kreiskongress"))
                                {
                                    se.Typ = SpecialEventTyp.Kreiskongress;
                                }
                                else if (typ.Contains("Sondervortrag"))
                                {
                                    se.Typ = SpecialEventTyp.Streaming;
                                    se.Name = typ;
                                }
                                else if (typ.Contains("Regionaler Kongress"))
                                {
                                    se.Typ = SpecialEventTyp.RegionalerKongress;
                                }
                                MeinPlan.Add(se);
                                continue;
                            }

                            var i = new Invitation
                            {
                                Kw = kw
                            };

                            //Versammlung
                            var v1 = versammlung?.ToString() ?? "Unbekannt";

                            if (v1 == "Kreisaufseher")
                            {
                                var se = new SpecialEvent
                                {
                                    Kw = kw,
                                    Typ = SpecialEventTyp.Dienstwoche,
                                    Vortragender = redner?.ToString() ?? "Kreisaufseher",
                                    Thema = thema?.ToString()
                                };
                                MeinPlan.Add(se);
                                continue;
                            }

                            var v = DataContainer.ConregationFindOrAdd(v1);
                            var r = DataContainer.SpeakerFindOrAdd(redner.ToString(), v);
                            i.Ältester = r;

                            if (string.IsNullOrEmpty(i.Ältester.Telefon) && !string.IsNullOrEmpty(rednerTelefon))
                                i.Ältester.Telefon = rednerTelefon;
                            if (string.IsNullOrEmpty(i.Ältester.Mobil) && !string.IsNullOrEmpty(rednerHandy))
                                i.Ältester.Mobil = rednerHandy;

                            //Vortrag
                            var vn = int.Parse(vortrag.ToString(), Helper.German);
                            var t = TalkList.Find(vn);
                            i.Vortrag = new TalkSong(t);
                            if (!i.Ältester.Vorträge.Select(y => y.Vortrag).Contains(t))
                                i.Ältester.Vorträge.Add(i.Vortrag);

                            MeinPlan.Add(i);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(nameof(ImportEigenePlanungen), e.Message);
                    ThemedMessageBox.Show("Fehler",
                        $"Beim Einlesen der Excel-Datei ist es zu folgendem Fehler gekommen\n:{e.Message}",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                    MeinPlan.Clear();
                    return false;
                }
                return true;
            }

            public static bool ImportRednerPlanungen(string filename)
            {
                Log.Info(nameof(ImportRednerPlanungen), filename);
                try
                {
                    var file = new FileInfo(filename);
                    Conregations = new List<Conregation>();

                    using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (ExcelPackage package = new ExcelPackage(fs))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                        var row = 2;
                        while (true)
                        {
                            var datum = worksheet.Cells[row, 1].Value;
                            var redner = worksheet.Cells[row, 2].Value;
                            var vortrag = worksheet.Cells[row, 3].Value;
                            var thema = worksheet.Cells[row, 4].Value;
                            var versammlung = worksheet.Cells[row, 5].Value;
                            row++;

                            if (datum == null)
                                break;

                            if (datum.ToString() == "Datum")
                                continue;

                            if (redner == null)
                                continue;

                            var kw = Core.Helper.CalculateWeek(DateTime.Parse(datum.ToString(), Helper.German));

                            var i = new Outside
                            {
                                Kw = kw
                            };

                            //Redner
                            var r = DataContainer.SpeakerFindOrAdd(redner.ToString(), DataContainer.MeineVersammlung);
                            if (r == null)
                                continue;

                            i.Ältester = r;

                            //Gast-Versammlung
                            var v1 = versammlung?.ToString() ?? "Unbekannt";
                            if (v1 == "Urlaub")
                                i.Reason = OutsideReason.NotAvailable;
                            var v = DataContainer.ConregationFindOrAdd(v1);
                            i.Versammlung = v;

                            //Vortrag
                            var vn = string.IsNullOrEmpty(vortrag?.ToString()) ? -1 : int.Parse(vortrag.ToString(), Helper.German);
                            var t = TalkList.Find(vn);
                            i.Vortrag = new TalkSong(t);
                            if (!i.Ältester.Vorträge.Select(y => y.Vortrag).Contains(t))
                                i.Ältester.Vorträge.Add(i.Vortrag);

                            ExternerPlan.Add(i);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(nameof(ImportRednerPlanungen), e.Message);
                    ThemedMessageBox.Show("Fehler",
                        $"Beim Einlesen der Excel-Datei ist es zu folgendem Fehler gekommen\n:{e.Message}",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                    ExternerPlan.Clear();
                    return false;
                }
                return true;
            }
        }
    }
}