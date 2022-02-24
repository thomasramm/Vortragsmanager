using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Views;

namespace Vortragsmanager.MeineVerwaltung
{
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