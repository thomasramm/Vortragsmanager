[27.08.2020]

Redner: Anzeige der letzen 10 Aktivitäten
Mailtexte: können im Programm geändert werden, die geänderten Mailtexte werden dann auch im Activitäten Log gespeichert.
kleinere Fehler behoben und Stabilitätsverbesserungen

[09.08.2020]

Aktivitäten Log:
* Ab sofort werden alle wichtigen Änderungen deiner Planungen mitgeschrieben und können im Log nachvollzogen werden.

Sonstiges:
* Kleinere Fehlerbehebungen

[05.08.2020]

Hotfix: Fehlertoleranterer Updatemechanismus.

[24.07.2020]

Neues System zum speichern der Vorlagen. Als neue Vorlage wurde "Signatur" hinzugefügt. Die Signatur kann wiederum in allen anderen Vorlagen benutzt werden.
Wird eine Vorlage verändert, kann sie nun wieder auf den Originaltext zurückgesetzt werden.  
__Achtung:__ Durch die Änderungen müssen leider alle Vorlagen zurückgesetzt und nochmals von dir angepasst werden!  

Neue Redner Einstellungen:
* Es kann nun zu jedem Redner auch das Lied + Ersatzlied zu seinen Vorträgen gespeichert werden.
* Es kann nun zu jedem Redner die jwpub Adresse gespeichert werden.

Neue Versammlungseinstellungen:
* Zu jeder Versammlung können die Zoom Zugangsdaten gespeichert werden.
* Versammlungen können nun zusammengelegt werden.

Neuer Menübereich:
* Auf dem Dashboard gibt es einen neuen Menüpunkt "Redner" mit Schnellzugriff auf alle Redner.

Sonstige Fehlerbehebungen und Verbesserungen:
* Ungültige Vorträge werden nicht mehr angezeigt
* Fehler beim Verschieben von Ereignissen behoben
* Fehler in Rednersuche behoben (Filtereinstellungen)
* Fehlerbehebung in Berechnung wann ein Vortrag zuletzt gehalten wurde
* Handbuch Anzeige im Web optimiert
* Verbessertes löschen von Rednern und Versammlungen (intern)

[14.05.2020]
Handbuch Ergänzungen

[27.04.2020]

Mein Plan: 
* Neue Funktion: Senden einer Erinnerungsmail mit Hinweisen, z.B. Zoom Zugangsdaten
* kleine Fehlerbehebungen

[15.04.2020]
* Antwort Eintragen: Datumsfeld mit erstem Datum vorausgewählt 
* Updater: Beim Wizard wurde die Dateiversion falsch gesetzt

[12.04.2020]
* UpdateDialog: Zeilenumbruch bei Anzeige langer Texte
* Redner suchen: Zukünftig werden die generierten Mail-Text einer Vortragsanfrage gespeichert (Ich wollte einen Koordinator an eine ausstehende Einladung erinnern, die Mail war in jwpub aber schon gelöscht...) und es gibt einen kleinen Button zur Anzeige in der Anzeige der offenen Anfragen. Wird die Anfrage bestätigt oder gelöscht, wird der Mailtext mit gelöscht.

[03.03.2020]
Mein Plan: 
 * Direktes Eintragen von Vortragsbuchungen
 * Detailansicht bei Buchungen wird jetzt über das neue X oben rechts im Popup geschlossen, bei Klick auf eine Information (z.B. Telefonnr des Koordinator) wird diese in die Zwischenablage kopiert. Das Kopieren wird durch eine kurzzeitige optische Hervorhebung signalisiert.
 
[24.02.2020]
* kleine Bugs behoben beim Listen erstellen
* Verbesserter Log zur Fehlersuche

[18.02.2020]
* Redner Extern planen:
  * Doppeltes speichern verhindern
* Redner suchen: 
  * Neuer Filter Vortrags-Nr
* Listen Erstellen
  *  Dateien nach dem erstellen direkt öffnen
  * Liste der Vorträge erweitert um Spalte "zuletzt gehört"
* Redner bearbeiten
  * Neuen Vortrag eingeben und speichern mit Enter (beide Eingabemöglichkeiten)
  * Die Box wird nur noch bei klick auf den Header geschlossen

[10.02.2020]
Fehlerbehebung:
* Beim öffnen einer Datenbank werden Absagen nun vor dem einlesen korrekt geleert.
* Das Excel-Template ist nun im Setup enthalten
* Redner suchen:
  * Liste der Versammlungen ist Alphabetisch sortiert
  * gewählte Kreise und der Status des Parameters "zukünftige Vorträge..." wird nun gemerkt
* Mein Plan:
  * Beim Klick auf einen Eintrag (linke Maustaste) wird nun je nach Typ des Eintrags ein Dialog aufgerufen.
    * Ereigniss: Ereignis bearbeiten
    * offener Eintrag: Rednersuche
    * Buchung: Info-Dialog mit Kontaktinformationen. Bei einem Klick auf das Feld, wird dieses wieder geschlossen.

[02.02.2020]
Fehlerbehebung: 
* Abgesagte Vorträge wurden noch nicht gespeichert.
Verbesserungen:
* Fehlerlog erweitert, mehr Fehler werden aufgezeichnet. 
* Standard-Log-Level ist jetzt auf "Fehler" eingestellt.
* Changelog von Versionsnummern auf Datum umgestellt, damit können Benutzer meistens mehr anfangen.
* Changelog auf Github verschoben.

[27.01.2020]
Neue Funktion:
* Buchung verschieben.
* Programmversion und geöffnete Datei werden im Fenstertitel angezeigt.
* Neue Möglichkeit eine Anfrage für ein anderes freies Datum zu bestätigen.
* Möglichkeit zum aktivieren eines Log zur Fehlersuche
* Rednersuche: Neuer Filter: Redner für die in der Zukunft eine Absage erteilt wurde nicht nochmal anfragen
* Bei der Vortragsanzeige unter "Meine Redner" werden auch die Planungen in meiner eigenen Versammlung mit angezeigt.
* Neuer Vortrag 56
* Neues Attribut für Redner "Einladen" - wirkt genauso wie Aktiv. Wenn ein Redner zwar aktiv ist, aber z.B. nicht so weit bis zu deiner Versammlung reisen will, kannst du ihn bei "Einladen" deaktivieren. Wenn dann nach 1-2 Jahren eine neue Liste kommt, musst du nicht überlegen ob ein ehemals deaktivierter Redner wieder aktiv reist oder warum er bei dir deaktiviert ist...
* Neue Liste: Alle Redner die im System hinterlegt sind ausgeben.
* Neue Einstellmöglichkeit, ob beim Speichern eine Sicherungsdatei angelegt werden soll.
Bugs: Viele kleinere Programmverbesserung und Fehlerbehebungen

[13.01.2020]
* Anzeige der nächsten Ereignisse direkt auf der Startseite
  * Einladung am Sonntag
  * Nächster externer Redner
* Externe Anfrage:
  * prüfen ob alle Felder korrekt gefüllt sind
  * kleine UI Fehler behoben
  * Hinweise anzeigen die gegen die Einladung sprechen: Redner ist bereits verplant, Redner hat innerhalb 1 Monats mehr als 1 Einladung, an dem angefragten Datum sind bereits 2 oder mehr Redner auswärts unterwegs.
* Rednersuche
  * Offene Anfragen für Versammlungen werden jetzt berücksichtigt, neuer Filter
  * Anzeige des letzten Besuch eines Redners in meiner Versammlung
* kleine UI (Oberflächen) Verbesserungen 


[06.01.2020]
* Rednersuche berücksichtigt nun auch offene Anfragen + deaktivierte Redner
* Im Bereich der Versammlungen->Rednerverwaltung kann man bei neuen Rednern die Vorträge jetzt auch als kommagetrennte Liste eingeben, was wesentlich schneller geht.

[05.01.2020]
* Fehler im Einrichtungsassistent, import von vplanung.net Dateien war nicht möglich
* Neue Listen: 
 * Rednerdaten der Versammlung zum Versenden an Koordinatoren 
 * Liste aller Vorträge mit Anzahl der Ausarbeitungen
* kleinere Programmverbesserungen der Oberfläche

[04.01.2020]
* Kritischer Bug bei Funktion "andere Datenbank öffnen", Anfragen wurden verdoppelt
* Mail an Koordinatoren im Kreis erweitert um neue Funktion "Mail an Koordinatoren < x km Entfernung"
* Redner verwalten, hinzufügen von Vorträgen zum Rednern vereinfacht, die Vortragsnummer kann im offenen DropDown über die Tastatur eingegeben werden.

[28.12.2019]
Erste Testversion (Beta)
