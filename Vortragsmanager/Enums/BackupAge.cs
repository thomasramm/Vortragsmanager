using System.Diagnostics.CodeAnalysis;

namespace Vortragsmanager.Enums
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum BackupAge
    {
        Heute,
        Diese_Woche,
        Dieser_Monat,
        Dieses_Jahr,
        Älter
    }
}