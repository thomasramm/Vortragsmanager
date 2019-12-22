using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Vortragsmanager.Models;
using System.Linq;

namespace Vortragsmanager.Views
{
    public class UpdateDialogViewModel : ViewModelBase
    {
        public UpdateDialogViewModel()
        {
            CloseCommand = new DelegateCommand<ICloseable>(Schließen);
        }

        public DelegateCommand<ICloseable> CloseCommand { get; private set; }

        public static void Schließen(ICloseable window)
        {
            if (window != null)
                window.Close();
        }

        private Version _localVersion;
        public Version LocalVersion
        {
            get 
            { 
                return _localVersion; 
            }

            set 
            { 
                _localVersion = value;
                RaisePropertyChanged(nameof(CurrentVersion));
            }
        }

        public Version ServerVersion
        {
            get { return GetProperty(() => LocalVersion); }
            set { SetProperty(() => LocalVersion, value); }
        }

        private Ini _serverIni = new Ini();
        public Ini ServerIni
        {
            get { return _serverIni; }
            set { 
                _serverIni = value;
                if (_serverIni == null)
                    return;

                RaisePropertyChanged(nameof(Changelog));
                RaisePropertyChanged(nameof(NeueVersion));
                
            }
        }

        public string NeueVersion => $"Es ist eine neue Version verfügbar! ({ServerVersion}).";

        public string CurrentVersion => $"Aktuell ist Version {LocalVersion} installiert.";

        public string Changelog
        {
            get 
            {
                var changeLog = string.Empty;
                var versionen = ServerIni.GetSections();
                foreach (var version in versionen)
                {
                    var v = new Version(version);
                    if (v <= LocalVersion)
                        return changeLog;

                    changeLog += $"Version {version}\n--------------------\n\n";
                    var keys = ServerIni.GetKeys(version);
                    foreach(var k in keys)
                    {
                        var value = ServerIni.GetValue(k, version);
                        changeLog += $"{k}: {value}\n";
                    }
                    changeLog += Environment.NewLine + Environment.NewLine;
                }
                return changeLog;
            }
        }

    }
}