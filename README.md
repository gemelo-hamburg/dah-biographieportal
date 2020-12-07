# dah-biographieportal
Biographieportal für das Deutsche Auswandererhaus in Bremerhaven


# Inhaltsverzeichnis

1. Kurzbeschreibung

1. Installation

1. Benutzung/Usage

1. Credits

1. Lizenz


# Kurzbeschreibung

Diese auf Basis von c#/WPF/XAML errichtete .net 4.8 Applikation ermöglicht dem Deutschen Auswandererhaus die Darstellung eines multimedia Exponats mit folgendem Zweck:

Anhand von Fotos, Texten und Videos können Sie hier einen Einblick in ganz persönliche Aus- und Einwanderungsgeschichten gewinnen
			
Mehr als 3.000 solcher Familiengeschichten und die dazugehörigen Konvolute – mündlich oder schriftlich überlieferte Biographien, Dokumente, Fotos und persönliche Erinnerungsobjekte – hat das Deutsche Auswandererhaus seit seiner Gründung im Jahr 2005 in seiner Sammlung zusammengetragen.
			
Werden Sie mit Ihrer Geschichte Teil des Portals. Egal, ob Sie bzw. Ihre Vorfahren für kurz oder lang, freiwillig oder unfreiwillig Ihren Heimatort verlassen und andernorts eine neue Heimat gefunden haben – erzählen Sie uns Ihre Geschichte! Klicken Sie hierzu auf den Briefumschlag, der Ihnen auf den nächsten Seiten immer wieder begegnen wird.

Besucher können außerdem eine Eingabe erstellen und aus ihrer eigenen Geschichte erzählen. Hierfür wird jeweils ein Textfile angelegt, welches aus Datenschutzgründen natürlich unter einem besonders geschützten Bereich abgelegt werden muss.
			
			
Dieses Biographieportal ist entstanden im Verbundprojekt museum4punkt0
* Digitale Strategien für das Museum der Zukunft, Teilprojekt Deutsches Auswandererhaus
* Migrationsgeschichte digital erleben. 

Das Projekt museum4punkt0 wird gefördert durch die Beauftragte der Bundesregierung für Kultur und Medien aufgrund eines Beschlusses des Deutschen Bundestages.

Weitere Informationen: [museum4punkt0](www.museum4punkt0.de)

# Installation

## Hardware / Betriebssystem
Die Software läuft auf einem handelsüblichen PC unter dem Betriebssystem Microsoft Windows 10. Sie benötigt das Microsoft .net Framework 4.8. An die Hardware werden keine speziellen Anforderungen gestellt, am rechnenintensivsten ist die Darstellung von Videos.

## Schriften
Folgende Schriftarten müssen auf dem PC installiert sein:
* Helvetica
* Weissenhof Grotesk

jeweils mit den Schriftschnitten Bold, Italic und Light.

## Einstellungen
In der .settings Datei können folgende Einstellungen vorgenommen werden:

* `RestartInteval`
Intervall als Timespan, nach der das Exponat bei Nichtbenutzung in den Timeout geht und die Startseite anzeigt.

* `VisitorMessagesPath`
Pfad zu dem geschützten Dateinbereich, unter der die Geschichten der Besucher abgelegt werden.

* `ContentDataPath`
Pfad zu den eigentlichen Inhalten. 
Hier müssen zwei Ordner angelegt sein

1. `ContentDefinition`
Hier muss sich eine Excel Datei (xlsx) mit den eigentlichen Texten und Metadaten befinden.

2. `Media`
Hier müssen die Medienfiles wie Bilder, Videos, Audios abgelegt sein.

Siehe nächster Absatz.


## eigentliche Inhalte
Die eigentlichen Inhalte bestehen im wesentlichen aus einer selbsterklärenden Excel Datei (siehe Ordner Example Content).

Diese besteht aus ein 6 Tabellen:

Der Haupttabelle und 5 relational verknüpften Untertabellen, in der Zeiträume, Migrationsarten, Migrationsgründe, Folgen und Historische Hintergründe definiert werden. Nach diesen kann der Besucher filtern.

In der Haupttabelle werden die Daten sequentiell aufgelistet.

Am Anfang eines Eintrages wird der Typ ´meta´ definiert. Hier erfolgen relationale Verknpüfungen auf die jeweiligen Untertabellen.

Es können die Typen Teaser, Image und Movie folgen, in der jeweils beschreibende Texte und die Dateinamen definiert sind.


# Benutzung/Usage
Nach Programmstart und Begrüßungsbildschirm wird dem Besucher eine umfangreiche Liste mit allen Biographie angezeigt. Diese kann der Besucher wie oben beschrieben über die Filter eingrenzen. Innerhalb der Biographie können weitere passenden Biographien verlinkt sein.

Der Besucher hat außerdem die Möglichkeit, über ein Formular eine eigene Geschichte zu hinterlegen und um Kontaktaufnahme zu bitten.

# Credits

# Lizenz

