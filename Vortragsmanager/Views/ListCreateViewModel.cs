using DevExpress.Mvvm;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Vortragsmanager.Core;
using Vortragsmanager.Models;
using Vortragsmanager.Properties;

namespace Vortragsmanager.Views
{
    public class ListCreateViewModel : ViewModelBase
    {
        private static string templateFolder;

        public ListCreateViewModel()
        {
            CreateAushangCommand = new DelegateCommand(CreateAushang);
            CreateContactListCommand = new DelegateCommand(CreateContactList);
            CreateExchangeRednerListCommand = new DelegateCommand(CreateExchangeRednerList);
            CreateOverviewTalkCountCommand = new DelegateCommand(CreateOverviewTalkCount);
            templateFolder = AppDomain.CurrentDomain.BaseDirectory + @"Templates\";
        }

        public DelegateCommand CreateAushangCommand { get; private set; }

        public DelegateCommand CreateContactListCommand { get; private set; }

        public DelegateCommand CreateExchangeRednerListCommand { get; private set; }

        public DelegateCommand CreateOverviewTalkCountCommand { get; private set; }

        private static string GetRednerAuswärts(DateTime datum)
        {
            var e = DataContainer.ExternerPlan.Where(x => x.Datum == datum).ToList();
            if (e.Count == 0)
                return "";

            var ausgabe = "Redner Auswärts: ";
            foreach (var r in e)
            {
                ausgabe += $"{r.Ältester.Name} in {r.Versammlung.Name}, ";
            }

            return ausgabe.Substring(0, ausgabe.Length - 2);
        }

        public static void CreateAushang()
        {
            //laden der Excel-Datei
            var template = $"{templateFolder}AushangExcel.xlsx";
            var tempFile = Path.GetTempFileName();
            File.Copy(template, tempFile, true);
            var excel = new FileInfo(tempFile);

            using (ExcelPackage package = new ExcelPackage(excel))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Aushang"];
                var titel = $"Öffentliche Vorträge der Versammlung {DataContainer.MeineVersammlung.Name}";
                worksheet.Cells[1, 1].Value = titel;
                var row = 3;
                var next10 = DataContainer.MeinPlan.Where(x => x.Datum > DateTime.Today).OrderBy(x => x.Datum).Take(10).ToList();
                foreach (var evt in next10)
                {
                    worksheet.Cells[row, 1].Value = evt.Datum; //Datum

                    if (evt.Status == EventStatus.Ereignis)
                    {
                        var sonntag = (evt as SpecialEvent);
                        worksheet.Cells[row, 2].Value = sonntag.Name; //EventName
                        row++;
                        worksheet.Cells[row, 3].Value = sonntag.Vortragender; //Vortragsredner
                        worksheet.Cells[row, 4].Value = sonntag.Thema; //Vortragsredner, Versammlung
                                                                       //worksheet.Cells[row, 6].Value = wt-leser;
                        row++;
                        worksheet.Cells[row, 2].Value = GetRednerAuswärts(sonntag.Datum);//auswärts
                        row++;
                        row++;
                    }
                    else
                    {
                        var sonntag = (evt as Invitation);
                        worksheet.Cells[row, 2].Value = sonntag.Vortrag.Thema; //Vortragsthema
                                                                               //worksheet.Cells[row, 6].Value = vorsitz;
                        row++;
                        worksheet.Cells[row, 3].Value = sonntag.Ältester?.Name; //Vortragsredner
                        worksheet.Cells[row, 4].Value = sonntag.Ältester?.Versammlung?.Name; //Vortragsredner, Versammlung
                                                                                             //worksheet.Cells[row, 6].Value = wt-leser;
                        row++;
                        worksheet.Cells[row, 2].Value = GetRednerAuswärts(sonntag.Datum);//auswärts
                        row++;
                        row++;
                    }
                }
                package.Save();
            }

            SaveExcelFile(tempFile, "Aushang.xlsx");
        }

        public static void CreateContactList()
        {
            var tempFile = Path.GetTempFileName();
            var excel = new FileInfo(tempFile);
            using (ExcelPackage package = new ExcelPackage())
            {
                var maxJahr = DataContainer.MeinPlan.Select(x => x.Datum.Year).Max();
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
                    sheet.Cells[1, 8].Value = "Koordinator";
                    sheet.Cells[1, 9].Value = "Koordinator Telefon";
                    sheet.Cells[1, 10].Value = "Koordinator Mobil";
                    sheet.Cells[1, 11].Value = "Koordinator Mail";
                    sheet.Cells[1, 12].Value = "Koordinator JwPub";
                    using (var range = sheet.Cells[1, 1, 1, 12])
                    {
                        range.Style.Font.Bold = true;
                    }
                    //Daten
                    var startDate = new DateTime(i, 1, 1);
                    var endDate = new DateTime(i, 12, 31);
                    while (startDate.DayOfWeek != DayOfWeek.Sunday)
                        startDate = startDate.AddDays(1);
                    var row = 2;
                    while (startDate < endDate)
                    {
                        sheet.Cells[row, 1].Value = startDate;
                        var einladung = DataContainer.MeinPlan.FirstOrDefault(x => x.Datum == startDate);
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
                            sheet.Cells[row, 2].Value = special.Vortrag?.Thema ?? special.Anzeigetext;
                            sheet.Cells[row, 3].Value = special.Vortragender ?? special.Thema;
                            sheet.Cells[row, 4].Value = special.Name ?? special.Typ.ToString();
                        }
                        else
                        {
                            var details = (einladung as Invitation);
                            sheet.Cells[row, 2].Value = details.Vortrag.Thema;
                            sheet.Cells[row, 3].Value = details.Ältester.Name;
                            sheet.Cells[row, 4].Value = details.Ältester.Versammlung.Name;
                            sheet.Cells[row, 5].Value = details.Ältester.Telefon;
                            sheet.Cells[row, 6].Value = details.Ältester.Mobil;
                            sheet.Cells[row, 7].Value = details.Ältester.Mail;
                            sheet.Cells[row, 8].Value = details.Ältester.Versammlung.Koordinator;
                            sheet.Cells[row, 9].Value = details.Ältester.Versammlung.KoordinatorTelefon;
                            sheet.Cells[row, 10].Value = details.Ältester.Versammlung.KoordinatorMobil;
                            sheet.Cells[row, 11].Value = details.Ältester.Versammlung.KoordinatorMail;
                            sheet.Cells[row, 12].Value = details.Ältester.Versammlung.KoordinatorJw;
                        }
                        row++;
                        startDate = startDate.AddDays(7);
                    }
                    sheet.Cells[$"A1:A{row}"].Style.Numberformat.Format = "dd.MM.yyyy";
                    sheet.Cells[$"A1:L{row}"].AutoFitColumns(20);
                }
                package.SaveAs(excel);
            }
            SaveExcelFile(tempFile, "Kontaktdaten.xlsx");
        }

        public static void CreateExchangeRednerList()
        {
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

                sheet.Cells[row, 1].Value = $"{jahr}: {DataContainer.MeineVersammlung.GetZusammenkunftszeit(jahr)}";
                row++;

                sheet.Cells[row, 1].Value = $"{jahr + 1}: {DataContainer.MeineVersammlung.GetZusammenkunftszeit(jahr + 1)}";
                row = row + 2;

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
                        vorträge += $"{v.Nummer}, ";
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
            SaveExcelFile(tempFile, "Rednerliste.xlsx");
        }

        public static void CreateOverviewTalkCount()
        {
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

                sheet.Cells[1, 1].Value = "Anzahl der Ausarbeitungen der Vorträge";

                sheet.Cells[2, 1, 2, 4].Style.Font.Bold = true;
                sheet.Cells[2, 1].Value = "Nr";
                sheet.Cells[2, 2].Value = "Thema";
                sheet.Cells[2, 3].Value = "Versammlung";
                sheet.Cells[2, 4].Value = "Kreis";

                var row = 3;
                foreach (var v in DataContainer.Vorträge.OrderBy(x => x.Nummer))
                {
                    sheet.Cells[row, 1].Value = v.Nummer;
                    sheet.Cells[row, 2].Value = v.Thema;
                    sheet.Cells[row, 3].Value = DataContainer.Redner.Where(x => x.Versammlung == vers && x.Vorträge.Contains(v)).Count();
                    sheet.Cells[row, 4].Value = DataContainer.Redner.Where(x => x.Versammlung.Kreis == kreis && x.Vorträge.Contains(v)).Count();
                    row++;
                }

                //create a range for the table
                ExcelRange range = sheet.Cells[2, 1, row - 1, 4];
                ExcelTable tab = sheet.Tables.Add(range, "Table1");
                tab.TableStyle = TableStyles.Medium2;

                package.SaveAs(excel);
            }
            SaveExcelFile(tempFile, "Vortragsthemen.xlsx");
        }

        private static void SaveExcelFile(string tempName, string sugestedName)
        {
            var saveFileDialog1 = new SaveFileDialog
            {
                Filter = Resources.DateifilterExcel,
                FilterIndex = 1,
                RestoreDirectory = false,
                FileName = sugestedName,
            };

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var fi = new FileInfo(saveFileDialog1.FileName);
                var filename = fi.FullName;
                var i = 0;
                try
                {
                    File.Delete(filename);
                }
                catch
                {
                    while (File.Exists(filename))
                    {
                        i++;
                        filename = $"{fi.DirectoryName}\\{fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length)} ({i}){fi.Extension}";
                    }
                }
                finally
                {
                    File.Move(tempName, filename);
                }
            }
            saveFileDialog1.Dispose();
        }
    }
}