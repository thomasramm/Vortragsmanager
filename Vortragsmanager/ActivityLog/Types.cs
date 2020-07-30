namespace Vortragsmanager.ActivityLog
{
    /* Ein neuer Type benötigt ergänzungen in folgenden Klassen:
     * Activities.cs -> SetFilter()
     * AddActivities.cs
     * ActivityItem.xaml.cs -> SetToolTip()
     * Symbols.cs -> GetImage()
     * Types.cs
     */

    public enum Types
    {
        Sonstige,
        ExterneAnfrageAblehnen,
        ExterneAnfrageBestätigen,
        ExterneAnfrageListeSenden,
        SendMail,
        RednerAnfrageBestätigt,
        RednerAnfrageAbgesagt,
        RednerAnfragen,
    }
}