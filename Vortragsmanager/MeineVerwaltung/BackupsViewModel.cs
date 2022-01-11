using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Vortragsmanager.Properties;

namespace Vortragsmanager.MeineVerwaltung
{
    public class BackupsViewModel : ViewModelBase
    {
        private List<Core.BackupItem> _backups;
        private bool showToday = true;
        private bool showThisWeek = true;
        private bool showThisMonth = true;
        private bool showThisYear = true;
        private bool showOld = true;

        public ObservableCollection<Core.BackupItem> VisibleBackups { get; private set; }

        public BackupsViewModel()
        {
            _backups = Core.Backup.List();
            VisibleBackups = new ObservableCollection<Core.BackupItem>(_backups);

            DeleteBackupCommand = new DelegateCommand<string>(Delete);
            RestoreBackupCommand = new DelegateCommand<string>(Restore);
        }

        public DelegateCommand<string> DeleteBackupCommand { get; private set; }

        public DelegateCommand<string> RestoreBackupCommand { get; private set; }

        public void Delete(string backup)
        {
            Core.Backup.Remove(backup);
            ReadBackups();
        }

        public void Restore(string backup)
        {
            var zeit = backup.Replace(".sqlite3", "");
            var dt = DateTime.ParseExact(zeit, "yyyy-MM-dd-HH-mm-ss", null);
            zeit = dt.ToString("D") + " " + dt.ToString("T");
            if (Core.Backup.Restore(backup))
            {
                ThemedMessageBox.Show($"Wiederherstellung", 
                    $"Deine Planung wurde auf den Stand vom {zeit} zurückgesetzt.\nDeine vorherige Planung wurde als neue Sicherung hinzugefügt.", 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Information);
            }
            else
            {
                ThemedMessageBox.Show($"Wiederherstellung",
                    $"Fehler beim zurücksetzen auf den Stand vom {zeit}.\nDeine vorherige Planung wurde als neue Sicherung hinzugefügt.",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
            ReadBackups();
        }

        public bool ShowToday
        {
            get => showToday;
            set
            {
                showToday = value;
                UpdateVisibleBackups();
            }
        }
        public bool ShowThisWeek
        {
            get => showThisWeek;
            set
            {
                showThisWeek = value;
                UpdateVisibleBackups();
            }
        }
        public bool ShowThisMonth
        {
            get => showThisMonth;
            set
            {
                showThisMonth = value;
                UpdateVisibleBackups();
            }
        }
        public bool ShowThisYear
        {
            get => showThisYear;
            set
            {
                showThisYear = value;
                UpdateVisibleBackups();
            }
        }
        public bool ShowOld
        {
            get => showOld;
            set
            {
                showOld = value;
                UpdateVisibleBackups();
            }
        }

        public bool MakeBackups
        {
            get => Settings.Default.SaveBackups;
            set
            {
                Settings.Default.SaveBackups = value;
                Settings.Default.Save();
            }
        }

        private void UpdateVisibleBackups()
        {
            VisibleBackups.Clear();

            if (ShowToday)
                foreach (var backup in _backups.Where(x => x.Age == Core.BackupAge.Heute))
                {
                    VisibleBackups.Add(backup);
                }
            if (ShowThisWeek)
                foreach (var backup in _backups.Where(x => x.Age == Core.BackupAge.Diese_Woche))
                {
                    VisibleBackups.Add(backup);
                }
            if (ShowThisMonth)
                foreach (var backup in _backups.Where(x => x.Age == Core.BackupAge.Dieser_Monat))
                {
                    VisibleBackups.Add(backup);
                }
            if (ShowThisYear)
                foreach (var backup in _backups.Where(x => x.Age == Core.BackupAge.Dieses_Jahr))
                {
                    VisibleBackups.Add(backup);
                }
            if (ShowOld)
                foreach (var backup in _backups.Where(x => x.Age == Core.BackupAge.Älter))
                {
                    VisibleBackups.Add(backup);
                }
        }

        private void ReadBackups()
        {
            _backups = Core.Backup.List();
            UpdateVisibleBackups();
        }
    }
}
