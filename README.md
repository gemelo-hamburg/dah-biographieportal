# Biographien-Portal
**Biographien-Portal** für das *Deutsche Auswandererhaus* in Bremerhaven


# Inhaltsverzeichnis

1. [Kurzbeschreibung](#kurzbeschreibung)

1. [Installation](#installation)

1. [Benutzung](#benutzung)

1. [Credits](#credits)

1. [Lizenz](#lizenz)

1. [Credits / Projektpartner](#creditsprojektpartner)


# Kurzbeschreibung

Diese auf Basis von c#/WPF/XAML errichtete Microsoft .net core 6.0 Applikation ermöglicht dem *Deutschen Auswandererhaus* die Darstellung eines Multimedia-Exponats mit folgendem Zweck:

Anhand von Fotos, Texten und Videos können die Nutzer hier einen Einblick in ganz persönliche Aus- und Einwanderungsgeschichten gewinnen.
			
Mehr als 3.000 solcher Familiengeschichten und die dazugehörigen Konvolute – mündlich oder schriftlich überlieferte Biographien, Dokumente, Fotos und persönliche Erinnerungsobjekte – hat das *Deutsche Auswandererhaus* seit seiner Gründung im Jahr 2005 in seiner Sammlung zusammengetragen.
			
Die Nutzer können darüber hinaus mit ihrer eigenen Geschichte Teil des Portals werden, egal ob sie bzw. ihre Vorfahren für kurz oder lang, freiwillig oder unfreiwillig ihren Heimatort verlassen und andernorts eine neue Heimat gefunden haben. Über eine Maske können die Nutzer Informationen zu ihrer eigenen Geschichte sowie ihre Kontaktdaten eingeben. Hierfür wird jeweils ein Textfile angelegt, welches aus Datenschutzgründen natürlich unter einem besonders geschützten Bereich abgelegt werden muss.
			
			
Das Biographien-Portal ist entstanden im Verbundprojekt 
**museum4punkt0** - Digitale Strategien für das Museum der Zukunft,
Teilprojekt *Deutsches Auswandererhaus* - Migrationsgeschichte digital erleben. 

Das Projekt museum4punkt0 wird gefördert durch die Beauftragte der Bundesregierung für Kultur und Medien aufgrund eines Beschlusses des Deutschen Bundestages.

Weitere Informationen: [museum4punkt0](www.museum4punkt0.de)

# Installation

## Hardware / Betriebssystem
Die Software läuft auf einem handelsüblichen PC unter dem Betriebssystem Microsoft Windows 10. Sie benötigt das Microsoft .net Framework 4.8. An die Hardware werden keine speziellen Anforderungen gestellt, am rechenintensivsten ist die Darstellung von Videos.

## Schriften
Folgende Schriftarten müssen auf dem PC installiert sein:
* Helvetica
* Weissenhof Grotesk

jeweils mit den Schriftschnitten Bold, Italic und Light.

## Einstellungen
In der [.settings Datei](Data/Biographieportal.json) können folgende Einstellungen vorgenommen werden:

* `RestartInterval`
Intervall als Timespan, nach der das Exponat bei Nichtbenutzung in den Timeout geht und die Startseite anzeigt.

* `WaitIfContentReadingError`
Flag (bool), welches gesetzt werden kann, um in der Phase der Content Erstellung Fehler besser bemerken zu können.

* `CheckNewContentInterval`
Zeitspanne (Timespan), nach der nach aktualisierten Inhalten gesucht wird.

* `VisitorMessagesPath`
Pfad zu dem geschützten Datenbereich, unter dem die Geschichten der Besucher abgelegt werden.

* `ContentDataPath`
Pfad zu den eigentlichen Inhalten. 
Hier müssen zwei Ordner angelegt sein.

1. `ContentDefinition`
Hier muss sich eine Excel Datei (xlsx) mit den eigentlichen Texten und Metadaten befinden.

2. `Media`
Hier müssen die Medienfiles wie Bilder, Videos, Audios abgelegt sein.

Siehe nächster Absatz.


## Eigentliche Inhalte
Die eigentlichen Inhalte bestehen im Wesentlichen aus einer selbsterklärenden Excel-Datei ([siehe Ordner Example Content](ExampleContent)).

Diese besteht aus 6 Tabellen:

Der Haupttabelle und 5 relational verknüpften Untertabellen, in der Zeiträume, Migrationsarten, Migrationsgründe, Folgen und Historische Hintergründe definiert werden. Nach diesen kann der Besucher filtern.

In der Haupttabelle werden die Daten sequentiell aufgelistet.

Am Anfang eines Eintrages wird der Typ ´meta´ definiert. Hier erfolgen relationale Verknüpfungen auf die jeweiligen Untertabellen.

Es können die Typen Teaser, Image und Movie folgen, in der jeweils beschreibende Texte und die Dateinamen definiert sind.

### Videos
Die Bitrate der Videos sollte bei 25Mbit/sec liegen, je nach Leistungsfähigkeit der verwendeten Hardware. 
Kodierung: H.264 
Auflösung: Full-HD

# Benutzung
Nach Programmstart und Begrüßungsbildschirm wird dem Besucher eine umfangreiche Liste mit allen Biographien angezeigt. Diese kann der Besucher wie oben beschrieben über die Filter eingrenzen. Innerhalb der Biographie können weitere passende Biographien verlinkt sein.

Der Besucher hat außerdem die Möglichkeit, über ein Formular eine eigene Geschichte zu hinterlegen und um Kontaktaufnahme durch Mitarbeiter des Museums zu bitten.

#### Beispielhafte Darstellung nach ähnlichen Biographien
![Beispielbild 001](/ExampleScreenshots/001.jpg)

#### Beispielhafte Darstellung nach der Migrationsart "Auswanderung"
![Beispielbild 002](/ExampleScreenshots/002.jpg)

#### Formular zur Einreichung eigener Geschichten
![Beispielbild 003](/ExampleScreenshots/003.jpg)

#### beispielhafte Darstellung eines Inhalts
![Beispielbild 006](/ExampleScreenshots/006.jpg)

#### Detailansicht
![Beispielbild 007](/ExampleScreenshots/007.jpg)



# Lizenz
Copyright © 2019/2020, Deutsches Auswandererhaus / Design: Andreas Heller Architects and Designers, Hamburg; Programmierung: gemelo GmbH, Hamburg, Germany

Die vom Auftragnehmer im Rahmen des Projektes erstellten Programmcodes und erstellten/verwendeten Assets werden im Rahmen der MIT License bereitgestellt. Davon ausgenommen sind Elemente, an denen der Auftragnehmer keine Rechte besitzt.

MIT license

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


# Credits/Projektpartner

Diese Anwendung ist entstanden im Verbundprojekt museum4punkt0 – Digitale Strategien für das Museum der Zukunft, Teilprojekt *Deutsches Auswandererhaus* - Migrationsgeschichte digital erleben. Weitere Informationen: www.museum4punkt0.de.

Das Projekt museum4punkt0 wird gefördert durch die Beauftragte der Bundesregierung für Kultur und Medien aufgrund eines Beschlusses des Deutschen Bundestages.

Auftraggeber und inhaltliches Konzept: Deutsches Auswandererhaus Bremerhaven, Columbusstraße 65, 27568 Bremerhaven

Design: Andreas Heller Architects and Designers, Hamburg

Auftragnehmer Programmierung: gemelo GmbH, interactive Media, Stresemannstraße 375, 22761 Hamburg, Telefon +49/40/3553060
Ansprechpartner: Thies Reinhold, info@gemelo.de


