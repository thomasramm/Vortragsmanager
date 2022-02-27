using Vortragsmanager.Datamodels;
using Vortragsmanager.Enums;

namespace Vortragsmanager.Interface
{
    public interface IEvent
    {
        int Kw { get; set; }

        EventStatus Status { get; }

        string Anzeigetext { get; }

        TalkSong Vortrag { get; set; }
    }
}