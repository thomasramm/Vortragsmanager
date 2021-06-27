using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Views;

namespace Vortragsmanager.MeineVerwaltung
{
    public class SpeakersViewModelCollection : ObservableCollection<SpeakerViewModel>
    {
        public SpeakersViewModelCollection(Conregation versammlung)
        {
            var redner = DataContainer.Redner.Where(x => x.Versammlung == versammlung).OrderBy(x => x.Name);
            foreach (Speaker r in redner)
                Add(new SpeakerViewModel(r));
        }
    }

    public class ConregationsViewModelCollection : ObservableCollection<ConregationViewModel>
    {
        public ConregationsViewModelCollection() : this(DataContainer.Versammlungen)
        {
        }

        public ConregationsViewModelCollection(IEnumerable<Conregation> versammlungen)
        {
            if (versammlungen is null)
                throw new NullReferenceException();

            var v = versammlungen.OrderBy(x => x, new Helper.EigeneKreisNameComparer());

            foreach (Conregation versammlung in v)
                Add(new ConregationViewModel(versammlung, this));

            AddConregationCommand = new DelegateCommand(AddConregation);
        }

        public DelegateCommand AddConregationCommand { get; private set; }

        public void AddConregation()
        {
            var vers = DataContainer.ConregationFindOrAdd("Neue Versammlung");
            var model = new ConregationViewModel(vers, this);
            Add(model);
            model.Select(true);
        }
    }
}