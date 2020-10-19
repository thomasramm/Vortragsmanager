using DevExpress.Mvvm;
using OfficeOpenXml;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;
using Vortragsmanager.UserControls;

namespace Vortragsmanager.MeinPlan
{
    class SonntagViewModel : ViewModelBase
    {
        private DateTime _month;
        private readonly DateTime _currentMonth;

        public SonntagViewModel()
        {
            _currentMonth = DateTime.Today;
            _currentMonth = _currentMonth.AddDays((_currentMonth.Day - 1) * -1);
            Monat = _currentMonth;
            ChangeMonth = new DelegateCommand<int>(ChangeTheMonth);
            Einstellungen = new DelegateCommand(OpenEinstellungen);
            Hauptseite = new DelegateCommand(OpenHauptseite);
            PlanAusgeben = new DelegateCommand(PlanErstellen);

            OpenHauptseite();
        }

        public GridLength HauptseiteWidth { get; set; }

        public GridLength EinstellungenWidth { get; set; }

        private void OpenEinstellungen() 
        {
            EinstellungenWidth = new GridLength(1, GridUnitType.Star);
            HauptseiteWidth = new GridLength(0);
            RaisePropertiesChanged(nameof(HauptseiteWidth));
            RaisePropertiesChanged(nameof(EinstellungenWidth));
        }

        private void OpenHauptseite()
        {
            HauptseiteWidth = new GridLength(1, GridUnitType.Star);
            EinstellungenWidth = new GridLength(0);
            RaisePropertiesChanged(nameof(HauptseiteWidth));
            RaisePropertiesChanged(nameof(EinstellungenWidth));
            WochenLoad();
        }

        public DateTime Monat
        {
            get
            {
                return _month;
            }
            set
            {
                _month = value;
                RaisePropertyChanged();
                WochenLoad();
            }
        }

        public DelegateCommand<int> ChangeMonth { get; private set; }

        private void ChangeTheMonth(int direction)
        {
            if (direction != 0)
            {
                Monat = Monat.AddMonths(direction);
            }
            else
            {
                Monat = _currentMonth;
            }
        }

        public ObservableCollection<SonntagItem> Wochen { get; private set; } = new ObservableCollection<SonntagItem>();

        private void WochenLoad()
        {
            Wochen.Clear();
            var tag = Core.Helper.GetSunday(Monat);
            var nMonat = Monat.AddMonths(Properties.Settings.Default.SonntagAnzeigeMonate);
            while (tag < nMonat)
            {
                Wochen.Add(new SonntagItem(tag));
                tag = tag.AddDays(7);
            }
        }

        private void PlanErstellen()
        {
            //laden der Excel-Datei
            var template = $"{Helper.TemplateFolder}AushangExcel.xlsx";
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
                    var sonntagEinteilung = DataContainer.AufgabenPersonKalender.FirstOrDefault(x => x.Datum == evt.Datum);

                    worksheet.Cells[row, 1].Value = evt.Datum; //Datum

                    if (evt.Status == EventStatus.Ereignis)
                    {
                        var sonntag = (evt as SpecialEvent);
                        worksheet.Cells[row, 2].Value = sonntag.Name ?? sonntag.Typ.ToString(); // EventName
                        worksheet.Cells[row, 6].Value = sonntagEinteilung?.Vorsitz?.PersonName;   // Vorsitz
                        row++;
                        worksheet.Cells[row, 3].Value = sonntag.Vortragender; // Vortragsredner
                        worksheet.Cells[row, 4].Value = sonntag.Thema;        // Vortragsredner, Versammlung
                        worksheet.Cells[row, 6].Value = sonntagEinteilung?.Leser?.PersonName; //Leser
                        row++;
                        worksheet.Cells[row, 2].Value = DataContainer.GetRednerAuswärts(sonntag.Datum); // Auswärts
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
                        worksheet.Cells[row, 2].Value = DataContainer.GetRednerAuswärts(sonntag.Datum);//auswärts
                        row++;
                        row++;
                    }
                }
                package.Save();
            }
            IoExcel.File.Save(tempFile, "Aushang.xlsx", true);
        }

        public DelegateCommand Einstellungen { get; private set; }

        public DelegateCommand Hauptseite { get; private set; }

        public DelegateCommand PlanAusgeben { get; private set; }
    }
}
