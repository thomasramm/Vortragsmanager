using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Vortragsmanager.Models;

namespace Vortragsmanager.Views
{
    public class EreignisEintragenCommandDialogView : ViewModelBase
    {
        private SpecialEvent _event;

        public SpecialEvent Event
        {
            get
            {
                return _event;
            }
            set
            {
                _eventOriginal = value;
                if (value is null)
                    return;
                _event = _eventOriginal.Clone();
                var individuellerName = _event.Name;
                if (_event.Typ > 0)
                    SelectedEreignis = (int)_event.Typ - 1;
                SetEreignisTyp();
                if (!string.IsNullOrEmpty(individuellerName))
                    EreignisName = individuellerName;

                RaisePropertyChanged(nameof(EreignisName));
                RaisePropertyChanged(nameof(VortragName));
                RaisePropertyChanged(nameof(VortragThema));
                RaisePropertyChanged(nameof(NeuerVortrag));
            }
        }

        private SpecialEvent _eventOriginal;

        public EreignisEintragenCommandDialogView()
        {
            CloseCommand = new DelegateCommand<ICloseable>(Schließen);
            SaveCommand = new DelegateCommand<ICloseable>(Save);
        }

        public DelegateCommand<ICloseable> CloseCommand { get; private set; }

        public DelegateCommand<ICloseable> SaveCommand { get; private set; }

        public ObservableCollection<Talk> Vortragsliste => Core.DataContainer.Vorträge;

        public Talk NeuerVortrag
        {
            get
            {
                return _event?.Vortrag?.Vortrag;
            }
            set
            {
                if (value == null)
                    _event.Vortrag = null;
                else
                    _event.Vortrag = new TalkSong(value);
                RaisePropertyChanged();
            }
        }

        public static void Schließen(ICloseable window)
        {
            if (window != null)
                window.Close();
        }

        public bool Speichern { get; private set; }

        public void Save(ICloseable window)
        {
            Speichern = true;

            _eventOriginal.Name = _event.Name;
            _eventOriginal.Thema = _event.Thema;
            _eventOriginal.Typ = _event.Typ;
            _eventOriginal.Vortragender = _event.Vortragender;
            _eventOriginal.Vortrag = _event.Vortrag;
            if (window != null)
                window.Close();
        }

        public int SelectedEreignis
        {
            get { return GetProperty(() => SelectedEreignis); }
            set { SetProperty(() => SelectedEreignis, value, SetEreignisTyp); }
        }

        private void SetEreignisTyp()
        {
            switch (SelectedEreignis)
            {
                case 0: //Alles Sichtbar
                    _event.Typ = SpecialEventTyp.Dienstwoche;
                    ShowVortrag = true;
                    ShowVortragDropDown = false;
                    ShowEreignisName = false;
                    EreignisName = "Dienstwoche";
                    break;

                case 4://Alles Sichtbar
                    _event.Typ = SpecialEventTyp.Sonstiges;
                    ShowVortrag = true;
                    ShowVortragDropDown = true;
                    ShowEreignisName = true;
                    EreignisName = "Sonstiges";
                    break;

                case 1: //Name nicht sichtbar
                    _event.Typ = SpecialEventTyp.RegionalerKongress;
                    EreignisName = "Regionaler Kongress";
                    ShowVortrag = false;
                    ShowVortragDropDown = false;
                    ShowEreignisName = false;
                    break;

                case 2: //Name nicht sichtbar
                    _event.Typ = SpecialEventTyp.Kreiskongress;
                    EreignisName = "Kreiskongress";
                    ShowVortrag = false;
                    ShowVortragDropDown = false;
                    ShowEreignisName = false;
                    break;

                case 3: //Name nicht sichtbar
                    _event.Typ = SpecialEventTyp.Streaming;
                    EreignisName = "Streaming";
                    ShowVortrag = false;
                    ShowVortragDropDown = true;
                    ShowEreignisName = true;
                    break;
            }
        }

        public bool ShowVortrag
        {
            get { return GetProperty(() => ShowVortrag); }
            set { SetProperty(() => ShowVortrag, value); }
        }

        public bool ShowVortragDropDown
        {
            get { return GetProperty(() => ShowVortragDropDown); }
            set { SetProperty(() => ShowVortragDropDown, value); }
        }

        public bool ShowEreignisName
        {
            get { return GetProperty(() => ShowEreignisName); }
            set { SetProperty(() => ShowEreignisName, value); }
        }

        public string EreignisName
        {
            get { return _event?.Name; }
            set
            {
                _event.Name = value;
                RaisePropertyChanged();
            }
        }

        public string VortragName
        {
            get { return _event?.Vortragender; }
            set
            {
                _event.Vortragender = value;
                ChangeKreisaufseher();
                RaisePropertyChanged();
            }
        }

        public void ChangeKreisaufseher()
        {
            if (SelectedEreignis.ToString(CultureInfo.InvariantCulture) == "Dienstwoche")
            {
                if (VortragName != Properties.Settings.Default.NameKreisaufseher)
                {
                    Properties.Settings.Default.NameKreisaufseher = VortragName;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public string VortragThema
        {
            get { return _event?.Thema; }
            set
            {
                _event.Thema = value;
                RaisePropertyChanged();
            }
        }
    }
}