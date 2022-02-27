using System.Collections.Generic;
using DevExpress.Mvvm;
using Vortragsmanager.Datamodels;
using Vortragsmanager.DataModels;
using Vortragsmanager.Helper;

namespace Vortragsmanager.PageModels
{
    /// <summary>
    /// Liste der Redner einer Versammlung
    /// </summary>
    public class GroupSpeaker : ViewModelBase
    {
        public GroupSpeaker()
        {
            Gewählt = true;
        }

        public Speaker Redner { get; set; }

        public List<GroupTalk> Vorträge { get; } = new List<GroupTalk>();

        public int SelectedIndex { get; set; }

        public int LetzteEinladungKw { get; set; }

        public Invitation LetzterVortrag { get; set; }

        public bool InZukunft { get; set; }

        public bool InVergangenheit { get; set; }

        public string Name => Redner?.Name;

        public bool Gewählt
        {
            get { return GetProperty(() => Gewählt); }
            set { SetProperty(() => Gewählt, value); }
        }

        public string LetzterBesuch => LetzteEinladungKw == -1 ? string.Empty : DateCalcuation.CalculateWeek(LetzteEinladungKw).ToString("dd.MM.yyyy", Helper.Helper.German);

        public string InfoPrivate => Redner.InfoPrivate;
    }
}