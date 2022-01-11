---
title: "Sicherung"
---
![Icon](images/menu_icon_13.png){: .align-right}

In regelm��igen Abst�nden werden Sicherungskopien deiner Planung erstellt und hier angezeigt. 
Du kannst die Sicherungen benutzen um versehentliche �nderungen deiner Planung R�ckg�ngig zu machen.

![Sicherungen](images/verwaltung_50.png)

Im Hauptbereich des Fensters siehst du alle verf�gbaren Sicherungen.
Der Eintrag f�r jede Sicherung ist folgenderma�en aufgebaut:
1. Ein Symbol das das ungef�hre Alter der Sicherung anzeigt.
2. Das genaue Datum der Sicherung und in der zweiten Zeile ein Hinweis wie alt diese Sicherung ist.
3. Ein rotes M�lleimer-Symbol. Damit wird die Sicherung gel�scht.
4. Ein wei�es "Wiederherstellen"-Symbol. Damit wird die Sicherung wiederhergestellt.

Rechts im Fenster werden verschiedene Optionen angezeigt.
* F�r die verschiedenen Zeitbereiche der Sicherung kann die Anzeige ausgeblendet werden, um eine einzelne Sicherung besser zu finden.
* Au�erdem kann das Erstellen der Sicherungen deaktiviert werden.

### Wann werden Sicherungen erstellt ###

* Bei jedem Beenden des Programm
* jedes mal wenn du im Programm auf die Startseite (Dashboard) zur�ckkehrst.

### Welche Sicherungen werden behalten ###

Da bei den vielen Sicherungen die �bersichtlichkeit verloren ginge, und das Backup zu gro� werden w�rde, werden nicht alle Sicherungen behalten.
Bei jedem Programmstart werden die Sicherungen gepr�ft und �ltere werden entfernt.
Welche Sicherungen bleiben erhalten?

* Alle Sicherungen des aktuellen Tages
* Eine Sicherung pro Stunde f�r alle Sicherungen der aktuellen Woche (jeweils die letzte Sicherung einer vollen Stunde)
* Eine Sicherung pro Tag f�r alle Sicherungen des aktuellen Monats (jeweils die letzte Sicherung des Tages)
* Eine Sicherung pro Monat f�r die letzten 12 Monate (jeweils die letzte Sicherung des Monats)

### Wo werden die Sicherungen abgelegt? ###

Die Sicherungen werden in einem Archiv (zip-Datei) im AppData/Local Ordner gesichert.
Du findest den Ordner, indem du im Windows Explorer als Pfad %LOCALAPPDATA% eingibtst. Suche von dort aus nach dem Ordner "Vortragsmanager DeLuxe", 
dort liegt die Datei backup.zip

[zur�ck](Redner.md){: .btn .btn--inverse} [weiter](ListenAusgeben.md){: .btn .btn--inverse}