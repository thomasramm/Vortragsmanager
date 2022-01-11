---
title: "Sicherung"
---
![Icon](images/menu_icon_13.png){: .align-right}

In regelmäßigen Abständen werden Sicherungskopien deiner Planung erstellt und hier angezeigt. 
Du kannst die Sicherungen benutzen um versehentliche Änderungen deiner Planung Rückgängig zu machen.

![Sicherungen](images/verwaltung_50.png)

Im Hauptbereich des Fensters siehst du alle verfügbaren Sicherungen.
Der Eintrag für jede Sicherung ist folgendermaßen aufgebaut:
1. Ein Symbol das das ungefähre Alter der Sicherung anzeigt.
2. Das genaue Datum der Sicherung und in der zweiten Zeile ein Hinweis wie alt diese Sicherung ist.
3. Ein rotes Mülleimer-Symbol. Damit wird die Sicherung gelöscht.
4. Ein weißes "Wiederherstellen"-Symbol. Damit wird die Sicherung wiederhergestellt.

Rechts im Fenster werden verschiedene Optionen angezeigt.
* Für die verschiedenen Zeitbereiche der Sicherung kann die Anzeige ausgeblendet werden, um eine einzelne Sicherung besser zu finden.
* Außerdem kann das Erstellen der Sicherungen deaktiviert werden.

### Wann werden Sicherungen erstellt ###

* Bei jedem Beenden des Programm
* jedes mal wenn du im Programm auf die Startseite (Dashboard) zurückkehrst.

### Welche Sicherungen werden behalten ###

Da bei den vielen Sicherungen die Übersichtlichkeit verloren ginge, und das Backup zu groß werden würde, werden nicht alle Sicherungen behalten.
Bei jedem Programmstart werden die Sicherungen geprüft und ältere werden entfernt.
Welche Sicherungen bleiben erhalten?

* Alle Sicherungen des aktuellen Tages
* Eine Sicherung pro Stunde für alle Sicherungen der aktuellen Woche (jeweils die letzte Sicherung einer vollen Stunde)
* Eine Sicherung pro Tag für alle Sicherungen des aktuellen Monats (jeweils die letzte Sicherung des Tages)
* Eine Sicherung pro Monat für die letzten 12 Monate (jeweils die letzte Sicherung des Monats)

### Wo werden die Sicherungen abgelegt? ###

Die Sicherungen werden in einem Archiv (zip-Datei) im AppData/Local Ordner gesichert.
Du findest den Ordner, indem du im Windows Explorer als Pfad %LOCALAPPDATA% eingibtst. Suche von dort aus nach dem Ordner "Vortragsmanager DeLuxe", 
dort liegt die Datei backup.zip

[zurück](Redner.md){: .btn .btn--inverse} [weiter](ListenAusgeben.md){: .btn .btn--inverse}