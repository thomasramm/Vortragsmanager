using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.LayoutControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Vortragsmanager.Models;

namespace Vortragsmanager.Views
{
    public class SpeakersViewModelCollection : ObservableCollection<SpeakerViewModel>
    {
        public SpeakersViewModelCollection(Conregation versammlung)
        {
            var redner = Core.DataContainer.Redner.Where(x => x.Versammlung == versammlung).OrderBy(x => x.Name);
            foreach (Speaker r in redner)
                Add(new SpeakerViewModel(r));
        }
    }

    public class ConregationsViewModelCollection : ObservableCollection<ConregationViewModel>
    {
        public ConregationsViewModelCollection() : this(Core.DataContainer.Versammlungen)
        {
        }

        public ConregationsViewModelCollection(IEnumerable<Conregation> versammlungen)
        {
            if (versammlungen is null)
                throw new NullReferenceException();

            var v = versammlungen.OrderBy(x => x, new EigeneKreisNameComparer());

            foreach (Conregation versammlung in v)
                Add(new ConregationViewModel(versammlung));

            AddConregationCommand = new DelegateCommand(AddConregation);
        }

        internal class EigeneKreisNameComparer : IComparer<Conregation>
        {
            public int Compare(Conregation x, Conregation y)
            {
                var eigene = Core.DataContainer.MeineVersammlung;
                var eigenerKreis = eigene.Kreis;
                string value1 = ((x.Kreis == eigenerKreis) ? "0" : "1") + ((x == eigene) ? "0" : "1") + x.Kreis + x.Name;
                string value2 = ((y.Kreis == eigenerKreis) ? "0" : "1") + ((y == eigene) ? "0" : "1") + y.Kreis + y.Name;
                return string.Compare(value1, value2, StringComparison.InvariantCulture);
            }
        }

        public DelegateCommand AddConregationCommand { get; private set; }

        public void AddConregation()
        {
            var vers = Core.DataContainer.FindOrAddConregation("Neue Versammlung");
            var model = new ConregationViewModel(vers);
            Add(model);
            model.Select(true);
        }
    }
}