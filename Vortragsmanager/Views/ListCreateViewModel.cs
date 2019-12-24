using DevExpress.Mvvm;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Vortragsmanager.Core;
using Vortragsmanager.Models;

namespace Vortragsmanager.Views
{
    public class ListCreateViewModel : ViewModelBase
    {
        public ListCreateViewModel()
        {
            CreateAushangCommand = new DelegateCommand(CreateAushang);
        }

        public DelegateCommand CreateAushangCommand { get; private set; }

        private string GetRednerAuswärts(DateTime datum)
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

        public void CreateAushang()
        {
            //laden der Excel-Datei
            var template = @"C:\Daten\Thomas\Projekte\Vortragsmanager\Rohdaten\TemplateAushangExcel.xlsx";
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

                    if (evt.Status == InvitationStatus.Ereignis)
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

            var saveFileDialog1 = new SaveFileDialog
            {
                Filter = "Excel Datei (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.Delete(saveFileDialog1.FileName);
                File.Move(tempFile, saveFileDialog1.FileName);
            }

            saveFileDialog1.Dispose();
        }
    }
}