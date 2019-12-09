﻿using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortragsmanager.Core;
using static Vortragsmanager.Core.Templates;

namespace Vortragsmanager.Views
{
    public class VorlagenViewModel : ViewModelBase
    {
        public VorlagenViewModel()
        {
            Vorlagen = Core.Templates.Vorlagen;
            RaisePropertyChanged(nameof(Vorlagen));
            Speichern = new DelegateCommand<bool>(VorlagentextSpeichern);
        }

        public DelegateCommand<bool> Speichern { get; private set; }

        public Dictionary<TemplateName, Template> Vorlagen { get; }

        public void VorlagentextSpeichern(bool speichern)
        {
            if (speichern)
                SelectedVorlage.Value.Inhalt = SelectedVorlageInhalt;
            else
                SelectedVorlageInhalt = SelectedVorlage.Value.Inhalt;
        }

        public KeyValuePair<TemplateName, Template> SelectedVorlage 
        {
            get { return GetProperty(() => SelectedVorlage); }
            set { SetProperty(() => SelectedVorlage, value, ChangeParameterList); }
        }

        private void ChangeParameterList()
        {
            SelectedVorlageParameter = SelectedVorlage.Value.Parameter;
            SelectedVorlageInhalt = SelectedVorlage.Value.Inhalt;
            SelectedVorlageBeschreibung = SelectedVorlage.Value.Beschreibung;
        }

        public string SelectedVorlageInhalt
        {
            get { return GetProperty(() => SelectedVorlageInhalt); }
            set { SetProperty(() => SelectedVorlageInhalt, value); }
        }

        public string SelectedVorlageBeschreibung
        {
            get { return GetProperty(() => SelectedVorlageBeschreibung); }
            set { SetProperty(() => SelectedVorlageBeschreibung, value); }
        }

        public Dictionary<string, string> SelectedVorlageParameter
        {
            get { return GetProperty(() => SelectedVorlageParameter); }
            private set { SetProperty(() => SelectedVorlageParameter, value); }
        }

        public KeyValuePair<string, string> SelectedParameter
        {
            get { return GetProperty(() => SelectedParameter); }
            set { SetProperty(() => SelectedParameter, value, ChangeParameterText); }
        }

        private void ChangeParameterText()
        {
            ParameterBeschreibung = SelectedParameter.Value;
        }

        public string ParameterBeschreibung 
        {
            get { return GetProperty(() => ParameterBeschreibung); }
            set { SetProperty(() => ParameterBeschreibung, value); }
        }
    }
}
