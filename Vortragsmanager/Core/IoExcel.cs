using DevExpress.Xpf.Core;
using OfficeOpenXml;
using OfficeOpenXml.Table;
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
                            v.Zeit.Set(2019, DayOfWeeks.Sonntag, z19);
                            if (z20 != z19)
                                v.Zeit.Set(2020, DayOfWeeks.Sonntag, z20);

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

        public static void CreateReportOverviewTalkCount(bool openReport)
        {
            Log.Info(nameof(CreateReportOverviewTalkCount), "");
            var tempFile = Path.GetTempFileName();
            var excel = new FileInfo(tempFile);
            var vers = DataContainer.MeineVersammlung;
            var kreis = vers.Kreis;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add($"Vorträge");

                sheet.Column(1).Width = 10;
                sheet.Column(2).Width = 50;
                sheet.Column(3).Width = 10;
                sheet.Column(5).Width = 15;

                sheet.Cells[1, 1].Value = "Anzahl der Ausarbeitungen der Vorträge";

                sheet.Cells[2, 1, 2, 4].Style.Font.Bold = true;
                sheet.Cells[2, 1].Value = "Nr";
                sheet.Cells[2, 2].Value = "Thema";
                sheet.Cells[2, 3].Value = "Versammlung";
                sheet.Cells[2, 4].Value = "Kreis";
                sheet.Cells[2, 5].Value = "zuletzt gehört";

                var row = 3;
                foreach (var v in TalkList.Get())
                {
                    sheet.Cells[row, 1].Value = v.Nummer;
                    sheet.Cells[row, 2].Value = v.Thema;
                    sheet.Cells[row, 3].Value = DataContainer.Redner.Where(x => x.Versammlung == vers && x.Vorträge.Select(y => y.Vortrag).Contains(v)).Count();
                    sheet.Cells[row, 4].Value = DataContainer.Redner.Where(x => x.Versammlung.Kreis == kreis && x.Vorträge.Select(y => y.Vortrag).Contains(v)).Count();
                    var wochen = DataContainer.MeinPlan.Where(x => x.Vortrag?.Vortrag?.Nummer == v.Nummer);
                    if (wochen.Any())
                    {
                        var zuletzt = wochen.Select(x => x.Kw).Max();
                        sheet.Cells[row, 5].Value = Core.Helper.CalculateWeek(zuletzt);
                    }

                    row++;
                }

                //create a range for the table
                ExcelRange range = sheet.Cells[2, 1, row - 1, 5];
                ExcelTable tab = sheet.Tables.Add(range, "Table1");
                tab.TableStyle = TableStyles.Medium2;

                range = sheet.Cells[2, 5, row - 1, 5];
                range.Style.Numberformat.Format = "dd.mm.yyyy";

                package.SaveAs(excel);
            }
            File.Save(tempFile, "Vortragsthemen.xlsx", openReport);
        }

        public static void CreateReportSpeakerOverview(bool openReport)
        {
            Log.Info(nameof(CreateReportSpeakerOverview), "");
            var tempFile = Path.GetTempFileName();
            var excel = new FileInfo(tempFile);
            var vers = DataContainer.MeineVersammlung;
            var kreis = vers.Kreis;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add($"Redner");

                sheet.Column(1).Width = 7;
                sheet.Column(2).Width = 25;
                sheet.Column(3).Width = 25;
                sheet.Column(4).Width = 7;
                sheet.Column(5).Width = 7;
                sheet.Column(6).Width = 10;
                sheet.Column(7).Width = 11;
                sheet.Column(8).Width = 30;
                sheet.Column(9).Width = 30;
                sheet.Column(10).Width = 30;
                sheet.Column(11).Width = 15;
                sheet.Column(12).Width = 15;
                sheet.Column(13).Width = 20;
                sheet.Column(14).Width = 20;

                sheet.Cells[1, 1, 1, 14].Style.Font.Bold = true;
                sheet.Cells[1, 1].Value = "Kreis";
                sheet.Cells[1, 2].Value = "Versammlung";
                sheet.Cells[1, 3].Value = "Name";
                sheet.Cells[1, 4].Value = "DAG";
                sheet.Cells[1, 5].Value = "Aktiv";
                sheet.Cells[1, 6].Value = "Einladen";
                sheet.Cells[1, 7].Value = "Datum letzte Einladung";
                sheet.Cells[1, 8].Value = "Vorträge";
                sheet.Cells[1, 9].Value = "Mail";
                sheet.Cells[1, 10].Value = "JwPub";
                sheet.Cells[1, 11].Value = "Telefon";
                sheet.Cells[1, 12].Value = "Mobil";
                sheet.Cells[1, 13].Value = "Kommentar Privat";
                sheet.Cells[1, 14].Value = "Kommentar Öffentlich";

                var row = 2;
                var rednerListe = DataContainer.MeinPlan.Where(x => x.Status == EventStatus.Zugesagt).Cast<Invitation>();
                foreach (var v in DataContainer.Redner.OrderBy(x => x.Versammlung.Name).ThenBy(x => x.Name))
                {
                    sheet.Cells[row, 1].Value = v.Versammlung.Kreis;
                    sheet.Cells[row, 2].Value = v.Versammlung.Name;
                    sheet.Cells[row, 3].Value = v.Name;
                    sheet.Cells[row, 4].Value = v.Ältester ? "" : "X";
                    sheet.Cells[row, 5].Value = v.Aktiv ? "X" : "";
                    sheet.Cells[row, 6].Value = v.Einladen ? "X" : "";

                    var b = rednerListe.Cast<Invitation>()?.Where(x => x.Ältester == v);
                    var letzterVortrag = "n.a.";
                    if (b.Any())
                    {
                        Helper.CalculateWeek(b.Select(x => x.Kw)?.Max() ?? -1).ToShortDateString();
                    }

                    sheet.Cells[row, 7].Value = letzterVortrag;

                    var vortragsliste = string.Empty;
                    foreach (var item in v.Vorträge)
                    {
                        vortragsliste += item.Vortrag.Nummer + ", ";
                    };
                    if (vortragsliste.Length >= 2)
                        vortragsliste = vortragsliste.Substring(0, vortragsliste.Length - 2);
                    sheet.Cells[row, 8].Value = vortragsliste;
                    sheet.Cells[row, 9].Value = v.Mail;
                    sheet.Cells[row, 10].Value = v.JwMail;
                    sheet.Cells[row, 11].Value = v.Telefon;
                    sheet.Cells[row, 12].Value = v.Mobil;
                    sheet.Cells[row, 13].Value = v.InfoPrivate;
                    sheet.Cells[row, 14].Value = v.InfoPublic;
                    row++;
                }

                //create a range for the table
                ExcelRange range = sheet.Cells[1, 1, row - 1, 14];
                ExcelTable tab = sheet.Tables.Add(range, "Table1");
                tab.TableStyle = TableStyles.Medium2;

                package.SaveAs(excel);
            }
            File.Save(tempFile, "Vortragsredner.xlsx", openReport);
        }

        public static void CreateReportExchangeRednerList(bool openReport)
        {
            Log.Info(nameof(CreateReportExchangeRednerList), "");
            var jahr = DateTime.Today.Year;
            var row = 12;
            var tempFile = Path.GetTempFileName();
            var excel = new FileInfo(tempFile);
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add($"Redner");

                sheet.Column(1).Width = 30;
                sheet.Column(2).Width = 25;
                sheet.Column(3).Width = 30;

                sheet.Cells[1, 1, 1, 3].Style.Font.Bold = true;
                sheet.Cells[1, 1].Value = "Versammlungsdaten";
                sheet.Cells[2, 1].Value = $"{DataContainer.MeineVersammlung.Name} ({DataContainer.MeineVersammlung.Kreis})";
                row = 3;
                if (!string.IsNullOrEmpty(DataContainer.MeineVersammlung.Anschrift1))
                {
                    sheet.Cells[row, 1].Value = DataContainer.MeineVersammlung.Anschrift1;
                    row++;
                }
                if (!string.IsNullOrEmpty(DataContainer.MeineVersammlung.Anschrift2))
                {
                    sheet.Cells[row, 1].Value = DataContainer.MeineVersammlung.Anschrift2;
                    row++;
                }
                if (!string.IsNullOrEmpty(DataContainer.MeineVersammlung.Anreise))
                {
                    sheet.Cells[row, 1].Value = DataContainer.MeineVersammlung.Anreise;
                    row++;
                }
                if (!string.IsNullOrEmpty(DataContainer.MeineVersammlung.Zoom))
                {
                    sheet.Cells[row, 1].Value = DataContainer.MeineVersammlung.Zoom;
                    row++;
                }
                if (!string.IsNullOrEmpty(DataContainer.MeineVersammlung.Telefon))
                {
                    sheet.Cells[row, 1].Value = DataContainer.MeineVersammlung.Telefon;
                    row++;
                }
                var maxRow = row;

                sheet.Cells[1, 3].Value = "Koordinatordaten";
                sheet.Cells[2, 3].Value = DataContainer.MeineVersammlung.Koordinator;
                row = 3;
                if (!string.IsNullOrEmpty(DataContainer.MeineVersammlung.KoordinatorTelefon))
                {
                    sheet.Cells[row, 3].Value = DataContainer.MeineVersammlung.KoordinatorTelefon;
                    row++;
                }
                if (!string.IsNullOrEmpty(DataContainer.MeineVersammlung.KoordinatorMobil))
                {
                    sheet.Cells[row, 3].Value = DataContainer.MeineVersammlung.KoordinatorMobil;
                    row++;
                }
                if (!string.IsNullOrEmpty(DataContainer.MeineVersammlung.KoordinatorMail))
                {
                    sheet.Cells[row, 3].Value = DataContainer.MeineVersammlung.KoordinatorMail;
                    row++;
                }
                if (!string.IsNullOrEmpty(DataContainer.MeineVersammlung.KoordinatorJw))
                {
                    sheet.Cells[row, 3].Value = DataContainer.MeineVersammlung.KoordinatorJw;
                    row++;
                }
                row = Math.Max(row, maxRow) + 1;

                sheet.Cells[row, 1].Style.Font.Bold = true;
                sheet.Cells[row, 1].Value = "Zusammenkunftszeiten";
                row++;

                sheet.Cells[row, 1].Value = $"{jahr}: {DataContainer.MeineVersammlung.Zeit.Get(jahr)}";
                row++;

                sheet.Cells[row, 1].Value = $"{jahr + 1}: {DataContainer.MeineVersammlung.Zeit.Get(jahr + 1)}";
                row += 2;

                sheet.Cells[row, 1, row, 3].Style.Font.Bold = true;
                sheet.Cells[row, 1].Value = "Redner";
                sheet.Cells[row, 2].Value = "Telefonnummer";
                sheet.Cells[row, 3].Value = "Vorträge";
                row++;

                var anzahlDag = 0;
                var anzahlEingeschränkt = 0;

                foreach (var redner in DataContainer.Redner.Where(x => x.Versammlung == DataContainer.MeineVersammlung && x.Aktiv))
                {
                    sheet.Cells[$"A{row}:C{row}"].Style.WrapText = true;

                    var name = redner.Name;
                    if (!redner.Ältester)
                    {
                        name += " #";
                        anzahlDag += 1;
                    }

                    sheet.Cells[row, 1].Value = name;

                    sheet.Cells[row, 2].Value = redner.Telefon ?? redner.Mobil;
                    var vorträge = string.Empty;
                    foreach (var v in redner.Vorträge)
                    {
                        vorträge += $"{v.Vortrag.Nummer}, ";
                    }
                    sheet.Cells[row, 3].Value = vorträge.TrimEnd().TrimEnd(',');
                    row++;
                }

                row++;
                if (anzahlDag > 0)
                {
                    sheet.Cells[row, 1].Value = "# Dienstamtgehilfe";
                    row++;
                }
                if (anzahlEingeschränkt > 0)
                {
                    sheet.Cells[row, 1].Value = "* Eingeschränkter Reiseradius";
                }

                sheet.Cells[row, 1].Value = $"Stand: {DateTime.Today:dd.MMMM.yyyy}";

                package.SaveAs(excel);
            }
            File.Save(tempFile, "Rednerliste.xlsx", openReport);
        }

        public static void CreateReportContactList(bool openReport)
        {
            Log.Info(nameof(CreateReportContactList), "");
            var tempFile = Path.GetTempFileName();
            var excel = new FileInfo(tempFile);
            using (ExcelPackage package = new ExcelPackage())
            {
                var maxJahr = DataContainer.MeinPlan.Select(x => x.Kw).Max();
                maxJahr /= 100;
                for (var i = DateTime.Today.Year; i <= maxJahr; i++)
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets.Add($"Mein Plan {i}");
                    //Überschriften
                    sheet.Cells[1, 1].Value = "Datum";
                    sheet.Cells[1, 2].Value = "Vortrag";
                    sheet.Cells[1, 3].Value = "Redner";
                    sheet.Cells[1, 4].Value = "Versammlung";
                    sheet.Cells[1, 5].Value = "Redner Telefon";
                    sheet.Cells[1, 6].Value = "Redner Mobil";
                    sheet.Cells[1, 7].Value = "Redner Mail";
                    sheet.Cells[1, 8].Value = "Redner JwPub";
                    sheet.Cells[1, 9].Value = "Koordinator";
                    sheet.Cells[1, 10].Value = "Koordinator Telefon";
                    sheet.Cells[1, 11].Value = "Koordinator Mobil";
                    sheet.Cells[1, 12].Value = "Koordinator Mail";
                    sheet.Cells[1, 13].Value = "Koordinator JwPub";
                    using (var range = sheet.Cells[1, 1, 1, 13])
                    {
                        range.Style.Font.Bold = true;
                    }
                    //Daten
                    var startDate = new DateTime(i, 1, 1);
                    var endDate = new DateTime(i, 12, 31);
                    while ((int)startDate.DayOfWeek != (int)Helper.Wochentag)
                        startDate = startDate.AddDays(1);
                    var row = 2;
                    while (startDate < endDate)
                    {
                        var kw = Helper.CalculateWeek(startDate);
                        sheet.Cells[row, 1].Value = startDate;
                        var einladung = DataContainer.MeinPlan.FirstOrDefault(x => x.Kw == kw);
                        if (einladung is null)
                        {
                            row++;
                            startDate = startDate.AddDays(7);
                            continue;
                        }
                        var typ = einladung.Status;
                        if (typ == EventStatus.Ereignis)
                        {
                            //ToDo: in die Kontaktliste SpecialEvents eintragen
                            var special = (einladung as SpecialEvent);
                            sheet.Cells[row, 2].Value = special.Vortrag?.Vortrag.Thema ?? special.Anzeigetext;
                            sheet.Cells[row, 3].Value = special.Vortragender ?? special.Thema;
                            sheet.Cells[row, 4].Value = special.Name ?? special.Typ.ToString();
                        }
                        else
                        {
                            var details = (einladung as Invitation);
                            sheet.Cells[row, 2].Value = details.Vortrag.Vortrag.Thema;
                            sheet.Cells[row, 3].Value = details.Ältester.Name;
                            sheet.Cells[row, 4].Value = details.Ältester.Versammlung.Name;
                            sheet.Cells[row, 5].Value = details.Ältester.Telefon;
                            sheet.Cells[row, 6].Value = details.Ältester.Mobil;
                            sheet.Cells[row, 7].Value = details.Ältester.Mail;
                            sheet.Cells[row, 8].Value = details.Ältester.JwMail;
                            sheet.Cells[row, 9].Value = details.Ältester.Versammlung.Koordinator;
                            sheet.Cells[row, 10].Value = details.Ältester.Versammlung.KoordinatorTelefon;
                            sheet.Cells[row, 11].Value = details.Ältester.Versammlung.KoordinatorMobil;
                            sheet.Cells[row, 12].Value = details.Ältester.Versammlung.KoordinatorMail;
                            sheet.Cells[row, 13].Value = details.Ältester.Versammlung.KoordinatorJw;
                        }
                        row++;
                        startDate = startDate.AddDays(7);
                    }
                    sheet.Cells[$"A1:A{row}"].Style.Numberformat.Format = "dd.MM.yyyy";
                    sheet.Cells[$"A1:L{row}"].AutoFitColumns(20);
                }
                package.SaveAs(excel);
            }
            File.Save(tempFile, "Kontaktdaten.xlsx", openReport);
        }

        public static void CreateReportAushang(bool openReport)
        {
            Log.Info(nameof(CreateReportAushang), "");
            //laden der Excel-Datei
            var template = $"{Helper.TemplateFolder}AushangExcel.xlsx";
            var tempFile = Path.GetTempFileName();
            System.IO.File.Copy(template, tempFile, true);
            var excel = new FileInfo(tempFile);

            using (ExcelPackage package = new ExcelPackage(excel))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Aushang"];
                var titel = $"Öffentliche Vorträge der Versammlung {DataContainer.MeineVersammlung.Name}";
                worksheet.Cells[1, 1].Value = titel;
                var row = 3;
                var aktuelleKw = Helper.CalculateWeek(DateTime.Today);
                var next10 = DataContainer.MeinPlan.Where(x => x.Kw >= aktuelleKw).OrderBy(x => x.Kw).Take(10).ToList();

                foreach (var evt in next10)
                {
                    var sonntagEinteilung = DataContainer.AufgabenPersonKalender.FirstOrDefault(x => x.Kw == evt.Kw);

                    worksheet.Cells[row, 1].Value = Helper.CalculateWeek(evt.Kw); //Datum

                    if (evt.Status == EventStatus.Ereignis)
                    {
                        var sonntag = (evt as SpecialEvent);
                        var zeile1 = sonntag.Name ?? sonntag.Typ.ToString();
                        if (sonntag.Vortrag != null)
                            zeile1 += ":" + sonntag.Vortrag.Vortrag.Thema;

                        worksheet.Cells[row, 2].Value = zeile1; //EventName

                        worksheet.Cells[row, 6].Value = sonntagEinteilung?.Vorsitz?.PersonName; //Rechts: Vorsitz 
                        row++;
                        worksheet.Cells[row, 3].Value = sonntag.Vortragender; //Vortragsredner
                        worksheet.Cells[row, 4].Value = sonntag.Thema; //Versammlung
                        worksheet.Cells[row, 6].Value = sonntagEinteilung?.Leser?.PersonName; //Rechts: Leser
                        row++;
                        worksheet.Cells[row, 2].Value = DataContainer.GetRednerAuswärts(sonntag.Kw);//auswärts
                        row++;
                        row++;
                    }
                    else
                    {
                        var sonntag = (evt as Invitation);
                        var themaMitLied = sonntag.Vortrag.Vortrag.Thema;
                        //Lieder des Redners abfragen
                        var v = sonntag.Ältester.Vorträge.FirstOrDefault(x => x.Vortrag.Nummer == sonntag.Vortrag.Vortrag.Nummer);
                        if (v == null)
                            v = sonntag.Vortrag;
                        worksheet.Cells[row, 2].Value = v.VortragMitLied; //Vortragsthema
                        worksheet.Cells[row, 6].Value = sonntagEinteilung?.Vorsitz?.PersonName;
                        row++;
                        worksheet.Cells[row, 3].Value = sonntag.Ältester?.Name; //Vortragsredner
                        worksheet.Cells[row, 4].Value = sonntag.Ältester?.Versammlung?.Name; //Vortragsredner, Versammlung
                        worksheet.Cells[row, 6].Value = sonntagEinteilung?.Leser?.PersonName;
                        row++;
                        worksheet.Cells[row, 2].Value = DataContainer.GetRednerAuswärts(sonntag.Kw);//auswärts
                        row++;
                        row++;
                    }
                }
                package.Save();
            }
            File.Save(tempFile, "Aushang.xlsx", openReport);
        }
    }
}