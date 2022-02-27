using System.Collections.ObjectModel;
using System.Linq;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Interface;
using Vortragsmanager.Views;

namespace Vortragsmanager.PageModels
{
    public class SpeakersViewModelCollection : ObservableCollection<SpeakerViewModel>
    {
        public SpeakersViewModelCollection(INavigation parentModel, Conregation versammlung)
        {
            var redner = DataContainer.Redner.Where(x => x.Versammlung == versammlung).OrderBy(x => x.Name);
            foreach (Speaker r in redner)
                Add(new SpeakerViewModel(parentModel,r));
        }
    }
}