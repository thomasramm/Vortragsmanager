﻿using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortragsmanager.Properties;

namespace Vortragsmanager.MeineVerwaltung
{
    public class VorträgeViewModel
    {
        public ObservableCollection<Datamodels.Talk> Talks { get; private set; }

        public VorträgeViewModel()
        {
            Talks = new ObservableCollection<Datamodels.Talk>(Datamodels.TalkList.Get());
            ResetCommand = new DelegateCommand(Reset);
            CreateOverviewTalkCountCommand = new DelegateCommand(CreateOverviewTalkCount);
        }

        public DelegateCommand ResetCommand { get; private set; }

        public DelegateCommand CreateOverviewTalkCountCommand { get; private set; }

        public bool ListeÖffnen
        {
            get => Settings.Default.ListCreate_OpenFile;
            set
            {
                Settings.Default.ListCreate_OpenFile = value;
                Settings.Default.Save();
            }
        }

        public void Reset()
        {
            var defaultTalks = Core.Initialize.LoadDefaultTalks();
            var gültigeVortragsNr = new List<int>(200);

            //Vortragsnummern die in der default-Liste vorkommen auf default zurücksetzen
            foreach (var defaultTalk in defaultTalks)
            {
                gültigeVortragsNr.Add(defaultTalk.Nummer);
                var lokalerTalk = Talks.FirstOrDefault(x => x.Nummer == defaultTalk.Nummer);
                lokalerTalk.Thema = defaultTalk.Thema;
                lokalerTalk.Gültig = defaultTalk.Gültig;
            }

            //Vortragsnummern die NICHT in der default-Liste vorkommen
            foreach(var lokalTalk in Talks.Where(x => !gültigeVortragsNr.Contains(x.Nummer)))
            {
                lokalTalk.Gültig = false;
            }
        }

        public void CreateOverviewTalkCount()
        {
            Core.IoExcel.CreateReportOverviewTalkCount(ListeÖffnen);
        }
    }
}
