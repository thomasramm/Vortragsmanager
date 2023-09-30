using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using Vortragsmanager.DataModels;
using Vortragsmanager.Interface;
using Vortragsmanager.UserControls;

namespace Vortragsmanager.PageModels
{
    public class ConregationsViewModelCollection : ObservableCollection<ConregationViewModel>
    {
        private readonly INavigation _parentNavigationService;

        public ConregationsViewModelCollection(INavigation parentModel) : this(parentModel, DataContainer.Versammlungen)
        {
        }

        public ConregationsViewModelCollection(INavigation parentModel, IEnumerable<Conregation> versammlungen)
        {
            _parentNavigationService = parentModel;

            if (versammlungen is null)
                throw new NullReferenceException();

            var v = versammlungen.OrderBy(x => x, new Helper.EigeneKreisNameComparer());

            foreach (Conregation versammlung in v)
                Add(new ConregationViewModel(_parentNavigationService, versammlung, this));

            AddConregationCommand = new DelegateCommand(AddConregation);
        }

        public DelegateCommand AddConregationCommand { get; }

        public void AddConregation()
        {
            var vers = DataContainer.ConregationFindOrAdd("Neue Versammlung");
            var model = new ConregationViewModel(_parentNavigationService, vers, this);
            Add(model);
            model.Select(true);
        }
    }
}