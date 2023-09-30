using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Vortragsmanager.DataModels;
using Vortragsmanager.Enums;
using Vortragsmanager.Helper;
using Vortragsmanager.PageModels;

namespace Vortragsmanager.UserControls
{
    class SonntagItemViewModel : ViewModelBase
    {
        public SonntagItemViewModel(int kw)
        {
            Kalenderwoche = kw;
            LoadData();
        }

        public int Kalenderwoche { get; set; }

        public DateTime Datum => DateCalcuation.CalculateWeek(Kalenderwoche);

        private void RecalculateNewActivityPerson(AufgabenZuordnung removed, AufgabenZuordnung added)
        {
            if (removed == added)
                return;
            //Neue Person
            if (added != null)
            {
                added.LetzterEinsatz = Kalenderwoche + added.Häufigkeit;
            }

            //Removed Person
            if (removed != null)
            {
                int kwLetzterEinsatz;

                var a1 = DataContainer.AufgabenPersonKalender
                    .Where(x => (x.Leser == removed || x.Vorsitz == removed) && x.Kw != Kalenderwoche).ToList();
                if (a1.Any())
                {
                    var a2 = a1.Select(x => x.Kw);
                    kwLetzterEinsatz = a2.Max();
                }
                else
                    kwLetzterEinsatz = -1;

                removed.LetzterEinsatz = kwLetzterEinsatz + removed.Häufigkeit;
            }
        }

        public AufgabenZuordnung SelectedLeser
        {
            get => _zuteilung?.Leser;
            set
            {
                var neuerWert = (value?.Id == 0) ? null : value;
                RecalculateNewActivityPerson(_zuteilung.Leser, neuerWert);
                FilterOtherList(Vorsitz, _zuteilung.Leser, neuerWert);
                _zuteilung.Leser = neuerWert;
                RaisePropertyChanged(nameof(SelectedLeser));
            }
        }

        public AufgabenZuordnung SelectedVorsitz
        {
            get => _zuteilung?.Vorsitz;
            set
            {
                var neuerWert = (value?.Id == 0) ? null : value;
                RecalculateNewActivityPerson(_zuteilung.Vorsitz, neuerWert);
                FilterOtherList(Leser, _zuteilung.Vorsitz, neuerWert);
                _zuteilung.Vorsitz = neuerWert;
                RaisePropertyChanged(nameof(SelectedVorsitz));
            }
        }

        public string Vortragsredner { get; set; }

        public string AuswärtigeRedner { get; set; }

        private void LoadData()
        {
            //Vortragsredner
            var redner = DataContainer.MeinPlan.FirstOrDefault(x => x.Kw == Kalenderwoche);

            if (redner != null)
            {
                switch (redner.Status)
                {
                    case EventStatus.Zugesagt:
                        if (redner is Invitation zu) Vortragsredner = zu.Ältester.Name;
                        break;
                    case EventStatus.Ereignis:
                        if (redner is SpecialEvent er)
                        {
                            Vortragsredner = er.Anzeigetext;
                            switch (er.Typ)
                            {
                                case SpecialEventTyp.Kreiskongress:
                                case SpecialEventTyp.RegionalerKongress:
                                    IsLeser = false;
                                    IsVorsitz = false;
                                    break;
                                case SpecialEventTyp.Dienstwoche:
                                    IsLeser = false;
                                    break;
                            }
                        }

                        break;
                }
            }

            //Auswärtige Redner
            var auswärts = DataContainer.ExternerPlan.Where(x => x.Kw == Kalenderwoche).ToList();
            AuswärtigeRedner = string.Empty;
            foreach (var item in auswärts)
            {
                AuswärtigeRedner += item.Ältester + ", ";
            }
            if (AuswärtigeRedner.Length > 2)
                AuswärtigeRedner = AuswärtigeRedner.Substring(0, AuswärtigeRedner.Length - 2);

            //zuteilungen
            _zuteilung = DataContainer.AufgabenPersonKalenderFindOrAdd(Kalenderwoche);

            //DropDown Vorsitz + Leser
            foreach (var az in DataContainer.AufgabenPersonZuordnung)
            {
                if (az.VerknüpftePerson != null)
                {
                    if (redner?.Status == EventStatus.Zugesagt)
                    {
                        if (redner is Invitation ereignis && (ereignis.Ältester == az.VerknüpftePerson))
                        {
                            continue;
                        }

                    }

                    if (auswärts.Any(x => x.Ältester == az.VerknüpftePerson))
                    {
                        continue;
                    }

                    //Urlaub...
                    if (DataContainer.Abwesenheiten.Any(x => x.Kw == Kalenderwoche && x.Redner == az.VerknüpftePerson))
                    {
                        continue;
                    }
                }

                if (az.IsLeser) // && az) != zuteilung.Vorsitz)
                {
                    Leser.Add(az);
                }

                if (az.IsVorsitz) // && az != zuteilung.Leser)
                {
                    Vorsitz.Add(az);
                }
            }

            FilterOtherList(Leser, null, SelectedVorsitz);
            FilterOtherList(Vorsitz, null, SelectedLeser);


            if (!IsLeser)
                SelectedLeser = null;

            if (!IsVorsitz)
                SelectedVorsitz = null;
        }

        private void FilterOtherList(ObservableCollection<AufgabenZuordnung> liste, AufgabenZuordnung oldValue, AufgabenZuordnung newValue)
        {
            if (oldValue == newValue)
                return;

            //entfernen der gewählten Person aus der Liste
            if (newValue != null && newValue.Id > 0 && liste.Contains(newValue))
                liste.Remove(newValue);

            //hinzufügen der frei gewordenen Person
            if ((oldValue != null) && !liste.Contains(oldValue) && ((oldValue.IsLeser && liste == Leser) || (oldValue.IsVorsitz && liste == Vorsitz)))
                liste.Add(oldValue);
        }

        public ObservableCollection<AufgabenZuordnung> Leser { get; } = new ObservableCollection<AufgabenZuordnung>();

        public ObservableCollection<AufgabenZuordnung> Vorsitz { get; } = new ObservableCollection<AufgabenZuordnung>();

        private AufgabenKalender _zuteilung;

        public bool IsVorsitz { get; set; } = true;

        public bool IsLeser { get; set; } = true;

    }
}
