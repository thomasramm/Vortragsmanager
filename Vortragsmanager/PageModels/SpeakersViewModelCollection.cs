﻿using System.Collections.ObjectModel;
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
}