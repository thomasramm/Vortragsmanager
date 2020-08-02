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
        BuchungLöschen,
        EreignisLöschen,
    }

    /*ToDo: MeinPlan Command:
     BuchungLöschen-> TeilA: Invitation, TeilB: Ereignisse wird alles angezeigt, nach Ereignistyp und Vortrag=null filtern!
     BuchungVerschieben
     RednerSuchen
     RednerEintragen
     EreignisEintragen
     BuchungBearbeiten
     BuchungErinnern

     AnfrageBearbeiten -> ok;
     */
}