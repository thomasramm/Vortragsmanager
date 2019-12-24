using DevExpress.Xpf.Core;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Vortragsmanager.Core
{
    internal class IoExcel
    {
        public static void UpdateSpeakers(string File)
        {
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
                    var rednerVersammlung = DataContainer.FindOrAddConregation(vers.ToString());

                    var s = DataContainer.FindSpeaker(name.ToString(), rednerVersammlung);
                    if (s == null)
                    {
                        s = new Models.Speaker
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
                        var nr = int.Parse(v, DataContainer.German);
                        var t = DataContainer.FindTalk(nr);
                        if (!(t is null) && (!s.Vorträge.Contains(t)))
                            s.Vorträge.Add(t);
                    }

                    row++;
                }
            } // the using statement automatically calls Dispose() which closes the package.
        }

        internal static class Vplanung
        {
            public static int Kreis { get; set; } = -1;

            public static List<Models.Conregation> Conregations = new List<Models.Conregation>();

            public static bool ImportKoordinatoren(string filename)
            {
                file = new FileInfo(filename);
                Conregations = new List<Models.Conregation>();

                try
                {

                    using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (ExcelPackage package = new ExcelPackage(fs))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                        var pos = worksheet.Name.LastIndexOf(' ');
                        var kreisString = worksheet.Name.Substring(pos + 1, worksheet.Name.Length - pos -1);
                        Kreis = int.Parse(kreisString, DataContainer.German);

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

                            var v = new Models.Conregation
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
                catch(Exception e)
                {
                    ThemedMessageBox.Show("Fehler",
                        $"Beim Einlesen der Excel-Datei ist es zu folgendem Fehler gekommen\n:{e.Message}",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                    return false;
                }
                return true;
            }

            public static List<Models.IEvent> MeinPlan { get; } = new List<Models.IEvent>();

            public static List<Models.Outside> ExternerPlan { get; } = new List<Models.Outside>();

            public static bool ImportEigenePlanungen(string filename)
            {
                try
                {
                    file = new FileInfo(filename);

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

                            if (redner == null) //Special Event eintragen
                            {
                                var se = new Models.SpecialEvent();
                                se.Datum = DateTime.Parse(datum.ToString(), DataContainer.German);
                                var typ = thema.ToString();
                                se.Typ = Models.EventTyp.Sonstiges;
                                if (typ.Contains("Weltzentrale"))
                                {
                                    se.Typ = Models.EventTyp.Streaming;
                                    se.Name = typ;
                                }
                                else if (typ.Contains("Kreiskongress"))
                                {
                                    se.Typ = Models.EventTyp.Kreiskongress;
                                }
                                else if (typ.Contains("Sondervortrag"))
                                {
                                    se.Typ = Models.EventTyp.Streaming;
                                    se.Name = typ;
                                }
                                else if (typ.Contains("Regionaler Kongress"))
                                {
                                    se.Typ = Models.EventTyp.RegionalerKongress;
                                }
                                MeinPlan.Add(se);
                                continue;
                            }

                            var i = new Models.Invitation
                            {
                                Datum = DateTime.Parse(datum.ToString(), DataContainer.German)
                            };

                            //Versammlung
                            var v1 = versammlung?.ToString() ?? "Unbekannt";

                            if (v1 == "Kreisaufseher")
                            {
                                var se = new Models.SpecialEvent();
                                se.Datum = i.Datum;
                                se.Typ = Models.EventTyp.Dienstwoche;
                                se.Vortragender = redner?.ToString() ?? "Kreisaufseher";
                                se.Thema = thema?.ToString();
                                MeinPlan.Add(se);
                                continue;
                            }

                            var v = DataContainer.FindOrAddConregation(v1);
                            var r = DataContainer.FindOrAddSpeaker(redner.ToString(), v);
                            i.Ältester = r;

                            if (string.IsNullOrEmpty(i.Ältester.Telefon) && !string.IsNullOrEmpty(rednerTelefon))
                                i.Ältester.Telefon = rednerTelefon;
                            if (string.IsNullOrEmpty(i.Ältester.Mobil) && !string.IsNullOrEmpty(rednerHandy))
                                i.Ältester.Mobil = rednerHandy;

                            //Vortrag
                            var vn = int.Parse(vortrag.ToString(), DataContainer.German);
                            var t = DataContainer.FindTalk(vn);
                            i.Vortrag = t;
                            if (!i.Ältester.Vorträge.Contains(t))
                                i.Ältester.Vorträge.Add(t);

                            MeinPlan.Add(i);
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

            public static bool ImportRednerPlanungen(string filename)
            {
                try
                {
                    file = new FileInfo(filename);
                    Conregations = new List<Models.Conregation>();

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

                            var i = new Models.Outside
                            {
                                Datum = DateTime.Parse(datum.ToString(), DataContainer.German)
                            };

                            //Redner
                            var r = DataContainer.FindOrAddSpeaker(redner.ToString(), DataContainer.MeineVersammlung);
                            if (r == null)
                                continue;

                            i.Ältester = r;

                            //Gast-Versammlung
                            var v1 = versammlung?.ToString() ?? "Unbekannt";
                            if (v1 == "Urlaub")
                                i.Reason = Models.OutsideReason.NotAvailable;
                            var v = DataContainer.FindOrAddConregation(v1);
                            i.Versammlung = v;

                            //Vortrag
                            var vn = int.Parse(vortrag.ToString(), DataContainer.German);
                            var t = DataContainer.FindTalk(vn);
                            i.Vortrag = t;
                            if (!i.Ältester.Vorträge.Contains(t))
                                i.Ältester.Vorträge.Add(t);

                            ExternerPlan.Add(i);
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
        }
    }
 }