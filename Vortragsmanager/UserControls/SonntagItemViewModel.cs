using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortragsmanager.Datamodels;

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

        private void RecalculateNewActivityPerson(AufgabenZuordnung removed, AufgabenZuordnung added)
        {
            if (removed == added)
                return;
            //Neue Person
            if (added != null)
            {
                var tage = Core.Helper.CurrentWeek - Kalenderwoche;
                added.LetzterEinsatz = tage * added.Häufigkeit;
            }

            //Removed Person
            if (removed != null)
            {
                int kwLetzterEinsatz;

                var a1 = DataContainer.AufgabenPersonKalender
                    .Where(x => (x.Leser == removed || x.Vorsitz == removed) && x.Kw != Kalenderwoche);
                if (a1.Any())
                {
                    var a2 = a1?.Select(x => x.Kw);
                    kwLetzterEinsatz = a2.Max();
                }
                else
                    kwLetzterEinsatz = -1;

                var tage = Core.Helper.CurrentWeek - Kalenderwoche;
                removed.LetzterEinsatz = tage * removed.Häufigkeit;
            }
        }

        public AufgabenZuordnung SelectedLeser
        {
            get { return zuteilung?.Leser; }
            set
            {
                var neuerWert = (value?.Id == -1) ? null : value;
                RecalculateNewActivityPerson(zuteilung.Leser, neuerWert);
                FilterOtherList(Vorsitz, zuteilung.Leser, neuerWert);
                zuteilung.Leser = neuerWert;
                RaisePropertyChanged(nameof(SelectedLeser));
            }
        }

        public AufgabenZuordnung SelectedVorsitz
        {
            get { return zuteilung?.Vorsitz; }
            set
            {
                var neuerWert = (value?.Id == -1) ? null : value;
                RecalculateNewActivityPerson(zuteilung.Vorsitz, neuerWert);
                FilterOtherList(Leser, zuteilung.Vorsitz, neuerWert);
                zuteilung.Vorsitz = neuerWert;
                RaisePropertyChanged(nameof(SelectedVorsitz));
            }
        }

        public string Vortragsredner { get; set; }

        public string AuswärtigeRedner { get; set; }

        private void LoadData()
        {
            //Vortragsredner
            var redner = DataContainer.MeinPlan.FirstOrDefault(x => x.Kw == Kalenderwoche);

            switch (redner.Status)
            {
                case EventStatus.Zugesagt:
                    var zu = (redner as Invitation);
                    Vortragsredner = zu.Ältester.Name;
                    break;
                case EventStatus.Ereignis:
                    var er = (redner as SpecialEvent);
                    Vortragsredner = er.Anzeigetext;
                    if (er.Typ == SpecialEventTyp.Dienstwoche 
                        || er.Typ == SpecialEventTyp.Kreiskongress
                        || er.Typ == SpecialEventTyp.RegionalerKongress)
                    {
                        IsLeser = false;
                        IsVorsitz = false;
                    }
                    if (er.Typ == SpecialEventTyp.Dienstwoche)
                        IsVorsitz = true;

                    break;
                default:
                    break;
            }

            //Auswärtige Redner
            var auswärts = DataContainer.ExternerPlan.Where(x => x.Kw == Kalenderwoche);
            AuswärtigeRedner = string.Empty;
            foreach (var item in auswärts)
            {
                AuswärtigeRedner += item.Ältester + ", ";
            }
            if (AuswärtigeRedner.Length > 2)
                AuswärtigeRedner = AuswärtigeRedner.Substring(0, AuswärtigeRedner.Length - 2);

            //zuteilungen
            zuteilung = DataContainer.AufgabenPersonKalenderFindOrAdd(Kalenderwoche);

            //DropDown Vorsitz + Leser
            foreach (var az in DataContainer.AufgabenPersonZuordnung)
            {
                if (az.VerknüpftePerson != null)
                {
                    if (redner.Status == EventStatus.Zugesagt)
                    {
                        var ereignis = (redner as Invitation);
                        if (ereignis != null && (ereignis.Ältester == az.VerknüpftePerson))
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

                if (az.IsLeser && az != zuteilung.Vorsitz)
                {
                    Leser.Add(az);
                }

                if (az.IsVorsitz && az != zuteilung.Leser)
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

            //entfernen der geählten Person aus der Liste
            if (newValue != null && liste.Contains(newValue))
                liste.Remove(newValue);

            //hinzufügen der frei gewordenen Person
            if ((oldValue != null) && !liste.Contains(oldValue) && (newValue == null || (newValue.IsLeser && liste == Leser) || (newValue.IsVorsitz && liste == Vorsitz)))
                liste.Add(oldValue);
        }

        public ObservableCollection<AufgabenZuordnung> Leser { get; } = new ObservableCollection<AufgabenZuordnung>();

        public ObservableCollection<AufgabenZuordnung> Vorsitz { get; } = new ObservableCollection<AufgabenZuordnung>();

        private AufgabenKalender zuteilung;

        public bool IsVorsitz { get; set; } = true;

        public bool IsLeser { get; set; } = true;

    }
}
