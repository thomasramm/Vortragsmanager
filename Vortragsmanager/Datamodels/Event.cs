using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Vortragsmanager.Datamodels
{
    public class Invitation : IEvent
    {
        #region Interface

        public int Kw { get; set; }

        public EventStatus Status { get; set; } = EventStatus.Zugesagt;

        public string Anzeigetext
        {
            get
            {
                if (Status == EventStatus.Anfrage)
                    return AnfrageVersammlung.Name;
                if (Status == EventStatus.Zugesagt)
                    return Ältester?.Name ?? "unbekannt";
                return "FEHLER";
            }
        }

        #endregion Interface

        public Speaker Ältester { get; set; }

        public TalkSong Vortrag { get; set; }

        public Conregation AnfrageVersammlung { get; set; }

        public DateTime LetzteAktion { get; set; }

        public bool ErinnerungsMailGesendet { get; set; }

        public string Kommentar { get; set; }

        public override string ToString() => $"{Ältester}\n{Vortrag}";
    }

    public enum EventStatus
    {
        Anfrage,
        Zugesagt,
        Ereignis
    }

    public class SpecialEvent : IEvent
    {
        #region Interface

        public int Kw { get; set; }

        public EventStatus Status { get; } = EventStatus.Ereignis;

        public string Anzeigetext
        {
            get
            {
                return string.IsNullOrEmpty(Name) ? Typ.ToString() : Name;
            }
        }

        #endregion Interface

        public SpecialEventTyp Typ { get; set; }

        public string Name { get; set; }

        public string Thema { get; set; }

        public string Vortragender { get; set; }

        public TalkSong Vortrag { get; set; }

        public SpecialEvent Clone()
        {
            SpecialEvent s = new SpecialEvent
            {
                Typ = Typ,
                Name = Name,
                Thema = Thema,
                Vortragender = Vortragender,
                Kw = Kw,
                Vortrag = Vortrag,
            };
            return s;
        }
    }

    public enum SpecialEventTyp
    {
        Dienstwoche = 1,
        RegionalerKongress = 2,
        Kreiskongress = 3,
        Streaming = 4,
        Sonstiges = 5,
    }

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

    public interface IEvent
    {
        int Kw { get; set; }

        EventStatus Status { get; }

        string Anzeigetext { get; }

        TalkSong Vortrag { get; set; }
    }

    public class Cancelation
    {
        public Cancelation()
        {
        }

        public Cancelation(int kw, Speaker person, EventStatus status)
        {
            Kw = kw;
            Ältester = person;
            LetzterStatus = status;
        }

        public int Kw { get; set; }

        public Speaker Ältester { get; set; }

        public EventStatus LetzterStatus { get; set; }
    }
}