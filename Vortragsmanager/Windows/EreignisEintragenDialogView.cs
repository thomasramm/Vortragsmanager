﻿using System.Collections.ObjectModel;
using System.Globalization;
using DevExpress.Mvvm;
using Vortragsmanager.DataModels;
using Vortragsmanager.Enums;
using Vortragsmanager.Interface;

namespace Vortragsmanager.Windows
{
    public class EreignisEintragenCommandDialogView : ViewModelBase
    {
        private SpecialEvent _event;

        public SpecialEvent Event
        {
            get => _event;
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
            Vortragsliste = new ObservableCollection<Talk>(TalkList.Get());
        }

        public DelegateCommand<ICloseable> CloseCommand { get; }

        public DelegateCommand<ICloseable> SaveCommand { get; }

        public ObservableCollection<Talk> Vortragsliste { get; }

        public Talk NeuerVortrag
        {
            get => _event?.Vortrag?.Vortrag;
            set
            {
                if (value == null)
                {
                    _event.Vortrag = null;
                    if (ShowVortragRadio)
                        VortragFreitextIsChecked = true;
                }
                else
                {
                    _event.Vortrag = new TalkSong(value);
                    if (ShowVortragRadio)
                        VortragDropDownIsChecked = true;
                }
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
            _eventOriginal.Typ = _event.Typ;
            _eventOriginal.Vortragender = _event.Vortragender;

            _eventOriginal.Thema = (_vortragDropDownIsChecked) ? string.Empty : _event.Thema;
            _eventOriginal.Vortrag = (_vortragDropDownIsChecked) ? _event.Vortrag : null;

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
                case 0: //Dienstwoche
                    _event.Typ = SpecialEventTyp.Dienstwoche;
                    ShowVortragFreitext = true;
                    ShowVortragDropDown = false;
                    _vortragDropDownIsChecked = false;
                    ShowEreignisName = false;
                    EreignisName = "Dienstwoche";
                    break;

                case 1: //Name nicht sichtbar
                    _event.Typ = SpecialEventTyp.RegionalerKongress;
                    EreignisName = "Regionaler Kongress";
                    ShowVortragFreitext = false;
                    ShowVortragDropDown = false;
                    _vortragDropDownIsChecked = false;
                    ShowEreignisName = false;
                    break;

                case 2: //Name nicht sichtbar
                    _event.Typ = SpecialEventTyp.Kreiskongress;
                    EreignisName = "Kreiskongress";
                    ShowVortragFreitext = false;
                    ShowVortragDropDown = false;
                    _vortragDropDownIsChecked = false;
                    ShowEreignisName = false;
                    break;

                case 3: //Alles Sichtbar
                    _event.Typ = SpecialEventTyp.Streaming;
                    EreignisName = "Streaming";
                    ShowVortragFreitext = true;
                    ShowVortragDropDown = true;
                    ShowEreignisName = true;
                    _vortragDropDownIsChecked = (_event.Vortrag != null);
                    break;

                case 4://Alles Sichtbar
                    _event.Typ = SpecialEventTyp.Sonstiges;
                    EreignisName = "Sonstiges";
                    ShowVortragFreitext = true;
                    ShowVortragDropDown = true;
                    ShowEreignisName = true;
                    _vortragDropDownIsChecked = (_event.Vortrag != null);
                    break;
            }

            RaisePropertyChanged(nameof(VortragFreitextIsChecked));
            RaisePropertyChanged(nameof(VortragDropDownIsChecked));
        }

        public bool ShowVortragFreitext
        {
            get { return GetProperty(() => ShowVortragFreitext); }
            set
            {
                SetProperty(() => ShowVortragFreitext, value);
                RaisePropertyChanged(nameof(ShowVortragRadio));
            }
        }

        public bool ShowVortragRadio => ShowVortragDropDown && ShowVortragFreitext;

        public bool ShowVortragDropDown
        {
            get { return GetProperty(() => ShowVortragDropDown); }
            set
            {
                SetProperty(() => ShowVortragDropDown, value);
                RaisePropertyChanged(nameof(ShowVortragRadio));
            }
        }

        private bool _vortragDropDownIsChecked;

        public bool VortragFreitextIsChecked
        {
            get => !_vortragDropDownIsChecked;
            set
            {
                if (_vortragDropDownIsChecked == value)
                {
                    _vortragDropDownIsChecked = !value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool VortragDropDownIsChecked
        {
            get => _vortragDropDownIsChecked;
            set
            {
                if (_vortragDropDownIsChecked != value)
                {
                    _vortragDropDownIsChecked = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool ShowEreignisName
        {
            get { return GetProperty(() => ShowEreignisName); }
            set { SetProperty(() => ShowEreignisName, value); }
        }

        public string EreignisName
        {
            get => _event?.Name;
            set
            {
                _event.Name = value;
                RaisePropertyChanged();
            }
        }

        public string VortragName
        {
            get => _event?.Vortragender;
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
                if (VortragName != Helper.Helper.GlobalSettings.NameKreisaufseher)
                {
                    Helper.Helper.GlobalSettings.NameKreisaufseher = VortragName;
                    Helper.Helper.GlobalSettings.Save();
                }
            }
        }

        public string VortragThema
        {
            get => _event?.Thema;
            set
            {
                _event.Thema = value;
                if (!string.IsNullOrWhiteSpace(_event.Thema))
                    VortragFreitextIsChecked = true;
                RaisePropertyChanged();
            }
        }

        public bool CanSave => true;
    }
}