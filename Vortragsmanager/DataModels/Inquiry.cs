﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Vortragsmanager.Enums;

namespace Vortragsmanager.Datamodels
{
    public class Inquiry : IEvent
    {
        public int Id { get; set; }

        //Ich habe eine Gruppe Redner + Vortrag
        //Ich habe eine Gruppe Daten
        public Dictionary<Speaker, Talk> RednerVortrag { get; } = new Dictionary<Speaker, Talk>();

        //Datum der Anfrage
        public DateTime AnfrageDatum { get; set; }

        //Angefragte Versammlung
        public Conregation Versammlung { get; set; }

        public EventStatus Status => EventStatus.Anfrage;

        public string Anzeigetext => Versammlung.Name;

        public TalkSong Vortrag
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        private int _datum = -1;

        public int Kw
        {
            get { return _datum; }
            //ToDo: Kann das richtig sein?
            set { _datum = -1; }
        }

        //Angefragte Termin
        public ObservableCollection<int> Kws { get; } = new ObservableCollection<int>();

        public string Kommentar { get; set; }

        public string Mailtext { get; set; }
    }
}