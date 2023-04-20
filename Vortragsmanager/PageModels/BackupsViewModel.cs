using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using Vortragsmanager.DataModels;
using Vortragsmanager.Enums;

namespace Vortragsmanager.PageModels
{
    public class BackupsViewModel : ViewModelBase
    {
        private List<BackupItem> _backups;
        private bool _showToday = true;
        private bool _showThisWeek = true;
        private bool _showThisMonth = true;
        private bool _showThisYear = true;
        private bool _showOld = true;

        public ObservableCollection<BackupItem> VisibleBackups { get; }

        public BackupsViewModel()
        {
            _backups = Backup.List();
            VisibleBackups = new ObservableCollection<BackupItem>(_backups);

            DeleteBackupCommand = new DelegateCommand<string>(Delete);
            RestoreBackupCommand = new DelegateCommand<string>(Restore);
        }

        public DelegateCommand<string> DeleteBackupCommand { get; }

        public DelegateCommand<string> RestoreBackupCommand { get; }

        public void Delete(string backup)
        {
            Backup.Remove(backup);
            ReadBackups();
        }

        public void Restore(string backup)
        {
            var zeit = backup.Replace(".sqlite3", "");
            var dt = DateTime.ParseExact(zeit, "yyyy-MM-dd-HH-mm-ss", null);
            zeit = dt.ToString("D") + " " + dt.ToString("T");
            if (Backup.Restore(backup))
            {
                ThemedMessageBox.Show("Wiederherstellung",
                    $"Deine Planung wurde auf den Stand vom {zeit} zurückgesetzt.\nDeine vorherige Planung wurde als neue Sicherung hinzugefügt.",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            else
            {
                ThemedMessageBox.Show("Wiederherstellung",
                    $"Fehler beim zurücksetzen auf den Stand vom {zeit}.\nDeine vorherige Planung wurde als neue Sicherung hinzugefügt.",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
            ReadBackups();
        }

        public bool ShowToday
        {
            get => _showToday;
            set
            {
                _showToday = value;
                UpdateVisibleBackups();
            }
        }

        public bool ShowThisWeek
        {
            get => _showThisWeek;
            set
            {
                _showThisWeek = value;
                UpdateVisibleBackups();
            }
        }

        public bool ShowThisMonth
        {
            get => _showThisMonth;
            set
            {
                _showThisMonth = value;
                UpdateVisibleBackups();
            }
        }

        public bool ShowThisYear
        {
            get => _showThisYear;
            set
            {
                _showThisYear = value;
                UpdateVisibleBackups();
            }
        }

        public bool ShowOld
        {
            get => _showOld;
            set
            {
                _showOld = value;
                UpdateVisibleBackups();
            }
        }

        public bool MakeBackups
        {
            get => Helper.Helper.GlobalSettings.SaveBackups;
            set
            {
                Helper.Helper.GlobalSettings.SaveBackups = value;
                Helper.Helper.GlobalSettings.Save();
            }
        }

        private void UpdateVisibleBackups()
        {
            VisibleBackups.Clear();

            if (ShowToday)
                foreach (var backup in _backups.Where(x => x.Age == BackupAge.Heute))
                {
                    VisibleBackups.Add(backup);
                }
            if (ShowThisWeek)
                foreach (var backup in _backups.Where(x => x.Age == BackupAge.Diese_Woche))
                {
                    VisibleBackups.Add(backup);
                }
            if (ShowThisMonth)
                foreach (var backup in _backups.Where(x => x.Age == BackupAge.Dieser_Monat))
                {
                    VisibleBackups.Add(backup);
                }
            if (ShowThisYear)
                foreach (var backup in _backups.Where(x => x.Age == BackupAge.Dieses_Jahr))
                {
                    VisibleBackups.Add(backup);
                }
            if (ShowOld)
                foreach (var backup in _backups.Where(x => x.Age == BackupAge.Älter))
                {
                    VisibleBackups.Add(backup);
                }
        }

        private void ReadBackups()
        {
            _backups = Backup.List();
            UpdateVisibleBackups();
        }
    }
}
