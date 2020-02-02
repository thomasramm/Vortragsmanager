using DevExpress.Mvvm;
using System;

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

        private DateTime _localVersion;

        public DateTime LocalVersion
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

        private DateTime _serverVersion;

        public DateTime ServerVersion
        {
            get
            {
                return _serverVersion;
            }

            set
            {
                _serverVersion = value;
                RaisePropertyChanged(nameof(ServerVersion));
            }
        }

        private Ini _serverIni = new Ini();

        public Ini ServerIni
        {
            get { return _serverIni; }
            set
            {
                _serverIni = value;
                if (_serverIni == null)
                    return;

                RaisePropertyChanged(nameof(Changelog));
                RaisePropertyChanged(nameof(NeueVersion));
            }
        }

        public string NeueVersion => $"Es ist eine neue Version verfügbar! (vom {ServerVersion.ToShortDateString()}).";

        public string CurrentVersion => $"Aktuell ist die Version vom {LocalVersion.ToShortDateString()} installiert.";

        public string Changelog
        {
            get
            {
                var changeLog = string.Empty;
                var versionen = ServerIni.GetSections();
                foreach (var version in versionen)
                {
                    var v = DateTime.Parse(version, Core.DataContainer.German);
                    if (v <= LocalVersion)
                        return changeLog;

                    changeLog += $"Version vom {version}\n--------------------\n\n";
                    var keys = ServerIni.GetKeys(version);
                    foreach (var k in keys)
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