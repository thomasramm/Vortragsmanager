using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Vortragsmanager.Models;

namespace Vortragsmanager.Views
{
    public class SetupWizardDialogViewModel : ViewModelBase
    {
        public SetupWizardDialogViewModel()
        {
            CloseCommand = new DelegateCommand<ICloseable>(Schließen);
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand<ICloseable> CloseCommand { get; private set; }

        public static void Schließen(ICloseable window)
        {
            if (window != null)
                window.Close();
        }
    }
}