using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Vortragsmanager.Models
{
    public class Invitation : IEvent
    {
        #region Interface

        public DateTime Datum { get; set; }

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

        public Talk Vortrag { get; set; }

        public Conregation AnfrageVersammlung { get; set; }

        public DateTime LetzteAktion { get; set; }

        public string Kommentar { get; set; }

        public override string ToString() => $"{Ältester}\n{Vortrag}";
    }

    public enum EventStatus
    {
        Anfrage,
        Zugesagt,
        Abgesagt,
        Ereignis
    }

    public class SpecialEvent : IEvent
    {
        #region Interface

        public DateTime Datum { get; set; }

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

        public Talk Vortrag { get; set; }

        public SpecialEvent Clone()
        {
            SpecialEvent s = new SpecialEvent
            {
                Typ = Typ,
                Name = Name,
                Thema = Thema,
                Vortragender = Vortragender,
                Datum = Datum
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

        public Talk Vortrag
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        private DateTime _datum = new DateTime(1);

        public DateTime Datum
        {
            get { return _datum; }
            set { _datum = new DateTime(1); }
        }

        //Angefragte Termin
        public ObservableCollection<DateTime> Wochen { get; } = new ObservableCollection<DateTime>();

        public string Kommentar { get; set; }
    }

    public interface IEvent
    {
        DateTime Datum { get; set; }

        EventStatus Status { get; }

        string Anzeigetext { get; }

        Talk Vortrag { get; set; }
    }
}