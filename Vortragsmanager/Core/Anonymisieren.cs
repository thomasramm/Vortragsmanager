using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortragsmanager.Properties;

namespace Vortragsmanager.Core
{
    public class Anonymisieren
    {
        public static void Start()
        {
            //_ = new Anonymisieren();
        }

        public Anonymisieren()
        {
            LoadNamen();
            LoadCity();
            DatenAnonymisieren();
            Core.Initialize.DemoAktualisieren();

            var file = Settings.Default.sqlite;

            IoSqlite.SaveContainer(file, false);
            Settings.Default.sqlite = file;
            Settings.Default.Save();
        }

        private List<string> Namen { get; set; }

        private List<string> Städte { get; set; }

        public void LoadNamen()
        {
            Namen = new List<string>(160);
            Namen.Add("Aaron Müller");
            Namen.Add("Alexander Schmidt");
            Namen.Add("Andreas Schneider");
            Namen.Add("Anton Fischer");
            Namen.Add("Archie Weber");
            Namen.Add("Ben Meyer");
            Namen.Add("Benjamin Wagner");
            Namen.Add("Christian Becker");
            Namen.Add("Daniel Schulz");
            Namen.Add("David Hoffmann");
            Namen.Add("Elias Schäfer");
            Namen.Add("Emil Koch");
            Namen.Add("Fabian Bauer");
            Namen.Add("Felix Richter");
            Namen.Add("Finn Klein");
            Namen.Add("Florian Wolf");
            Namen.Add("Jakob Schröder (Schneider)");
            Namen.Add("Jan Neumann");
            Namen.Add("Jonas Schwarz");
            Namen.Add("Jonathan Zimmermann");
            Namen.Add("Joris Braun");
            Namen.Add("Julian Krüger");
            Namen.Add("Leo Hofmann");
            Namen.Add("Leon Hartmann");
            Namen.Add("Levi Lange");
            Namen.Add("Liam Schmitt");
            Namen.Add("Linus Werner");
            Namen.Add("Lio Schmitz");
            Namen.Add("Lorenz Krause");
            Namen.Add("Luca Meier");
            Namen.Add("Lukas Lehmann");
            Namen.Add("Marcel Schmid");
            Namen.Add("Markus Schulze");
            Namen.Add("Marvin Maier");
            Namen.Add("Matteo Köhler");
            Namen.Add("Maximilian Herrmann");
            Namen.Add("Michael König");
            Namen.Add("Milan Walter");
            Namen.Add("Milo Mayer");
            Namen.Add("Moritz Huber");
            Namen.Add("Niklas Kaiser");
            Namen.Add("Noah Fuchs");
            Namen.Add("Paul Peters");
            Namen.Add("Philipp Lang");
            Namen.Add("Samuel Scholz");
            Namen.Add("Simon Möller");
            Namen.Add("Theo Weiß");
            Namen.Add("Thomas Jung");
            Namen.Add("Tim Hahn");
            Namen.Add("Valentin Schubert");
            Namen.Add("Lucas Vogel");
            Namen.Add("Timm Friedrich");
            Namen.Add("Kevin Keller");
            Namen.Add("Tobias Günther");
            Namen.Add("Denis Frank");
            Namen.Add("Niclas Berger");
            Namen.Add("Patrik Winkler");
            Namen.Add("Sebastian Roth");
            Namen.Add("Yannik Beck");
            Namen.Add("Timo Lorenz");
            Namen.Add("Marwin Baumann");
            Namen.Add("Niko Franke");
            Namen.Add("Mark Albrecht");
            Namen.Add("Lennart Schuster");
            Namen.Add("Tom Simon");
            Namen.Add("Max Ludwig");
            Namen.Add("Pascal Böhm");
            Namen.Add("Nils Winter");
            Namen.Add("Dominik Kraus");
            Namen.Add("Christopher Martin");
            Namen.Add("Lars Schumacher");
            Namen.Add("Marko Krämer");
            Namen.Add("Robin Vogt");
            Namen.Add("Swen Stein");
            Namen.Add("Christoph Jäger");
            Namen.Add("Thorben Otto");
            Namen.Add("Malte Sommer");
            Namen.Add("Johannes Groß");
            Namen.Add("Jacob Seidel");
            Namen.Add("Mike Heinrich");
            Namen.Add("Aaron Brandt");
            Namen.Add("Alexander Haas");
            Namen.Add("Andreas Schreiber");
            Namen.Add("Anton Graf");
            Namen.Add("Archie Schulte");
            Namen.Add("Ben Dietrich");
            Namen.Add("Benjamin Ziegler");
            Namen.Add("Christian Kuhn");
            Namen.Add("Daniel Kühn");
            Namen.Add("David Pohl");
            Namen.Add("Elias Engel");
            Namen.Add("Emil Horn");
            Namen.Add("Fabian Busch");
            Namen.Add("Felix Bergmann");
            Namen.Add("Finn Thomas");
            Namen.Add("Florian Voigt");
            Namen.Add("Jakob Sauer");
            Namen.Add("Jan Arnold");
            Namen.Add("Jonas Wolff");
            Namen.Add("Jonathan Pfeiffer");
            Namen.Add("Joris Müller");
            Namen.Add("Julian Schmidt");
            Namen.Add("Leo Schneider");
            Namen.Add("Leon Fischer");
            Namen.Add("Levi Weber");
            Namen.Add("Liam Meyer");
            Namen.Add("Linus Wagner");
            Namen.Add("Lio Becker");
            Namen.Add("Lorenz Schulz");
            Namen.Add("Luca Hoffmann");
            Namen.Add("Lukas Schäfer");
            Namen.Add("Marcel Koch");
            Namen.Add("Markus Bauer");
            Namen.Add("Marvin Richter");
            Namen.Add("Matteo Klein");
            Namen.Add("Maximilian Wolf");
            Namen.Add("Michael Schröder (Schneider)");
            Namen.Add("Milan Neumann");
            Namen.Add("Milo Schwarz");
            Namen.Add("Moritz Zimmermann");
            Namen.Add("Niklas Braun");
            Namen.Add("Noah Krüger");
            Namen.Add("Paul Hofmann");
            Namen.Add("Philipp Hartmann");
            Namen.Add("Samuel Lange");
            Namen.Add("Simon Schmitt");
            Namen.Add("Theo Werner");
            Namen.Add("Thomas Schmitz");
            Namen.Add("Tim Krause");
            Namen.Add("Valentin Meier");
            Namen.Add("Lucas Lehmann");
            Namen.Add("Timm Schmid");
            Namen.Add("Kevin Schulze");
            Namen.Add("Tobias Maier");
            Namen.Add("Denis Köhler");
            Namen.Add("Niclas Herrmann");
            Namen.Add("Patrik König");
            Namen.Add("Sebastian Walter");
            Namen.Add("Yannik Mayer");
            Namen.Add("Timo Huber");
            Namen.Add("Marwin Kaiser");
            Namen.Add("Niko Fuchs");
            Namen.Add("Mark Peters");
            Namen.Add("Lennart Lang");
            Namen.Add("Tom Scholz");
            Namen.Add("Max Möller");
            Namen.Add("Pascal Weiß");
            Namen.Add("Nils Jung");
            Namen.Add("Dominik Hahn");
            Namen.Add("Christopher Schubert");
            Namen.Add("Lars Vogel");
            Namen.Add("Marko Friedrich");
            Namen.Add("Robin Keller");
            Namen.Add("Swen Günther");
            Namen.Add("Christoph Frank");
            Namen.Add("Thorben Berger");
            Namen.Add("Malte Winkler");
            Namen.Add("Johannes Roth");
            Namen.Add("Jacob Beck");
            Namen.Add("Mike Lorenz");
        }

        public void LoadCity()
        {
            Städte = new List<string>(152);
            Städte.Add("Frankfurt am Main");
            Städte.Add("Wiesbaden");
            Städte.Add("Kassel");
            Städte.Add("Darmstadt");
            Städte.Add("Offenbach am Main");
            Städte.Add("Hanau");
            Städte.Add("Gießen");
            Städte.Add("Marburg");
            Städte.Add("Fulda");
            Städte.Add("Rüsselsheim am Main");
            Städte.Add("Bad Homburg vor der Höhe");
            Städte.Add("Wetzlar");
            Städte.Add("Oberursel (Taunus)");
            Städte.Add("Rodgau");
            Städte.Add("Dreieich");
            Städte.Add("Bensheim");
            Städte.Add("Hofheim am Taunus");
            Städte.Add("Maintal");
            Städte.Add("Langen (Hessen)");
            Städte.Add("Neu-Isenburg");
            Städte.Add("Limburg an der Lahn");
            Städte.Add("Mörfelden-Walldorf");
            Städte.Add("Viernheim");
            Städte.Add("Dietzenbach");
            Städte.Add("Bad Vilbel");
            Städte.Add("Lampertheim");
            Städte.Add("Bad Nauheim");
            Städte.Add("Taunusstein");
            Städte.Add("Bad Hersfeld");
            Städte.Add("Friedberg (Hessen)");
            Städte.Add("Kelkheim (Taunus)");
            Städte.Add("Mühlheim am Main");
            Städte.Add("Rödermark");
            Städte.Add("Baunatal");
            Städte.Add("Hattersheim am Main");
            Städte.Add("Griesheim");
            Städte.Add("Butzbach");
            Städte.Add("Heppenheim (Bergstraße)");
            Städte.Add("Weiterstadt");
            Städte.Add("Groß-Gerau");
            Städte.Add("Friedrichsdorf");
            Städte.Add("Pfungstadt");
            Städte.Add("Obertshausen");
            Städte.Add("Idstein");
            Städte.Add("Riedstadt");
            Städte.Add("Korbach");
            Städte.Add("Dillenburg");
            Städte.Add("Gelnhausen");
            Städte.Add("Bad Soden am Taunus");
            Städte.Add("Karben");
            Städte.Add("Büdingen");
            Städte.Add("Flörsheim am Main");
            Städte.Add("Eschborn");
            Städte.Add("Stadtallendorf");
            Städte.Add("Seligenstadt");
            Städte.Add("Groß-Umstadt");
            Städte.Add("Herborn");
            Städte.Add("Bruchköbel");
            Städte.Add("Nidderau");
            Städte.Add("Eschwege");
            Städte.Add("Haiger");
            Städte.Add("Heusenstamm");
            Städte.Add("Kronberg im Taunus");
            Städte.Add("Pohlheim");
            Städte.Add("Vellmar");
            Städte.Add("Schwalmstadt");
            Städte.Add("Frankenberg (Eder)");
            Städte.Add("Hochheim am Main");
            Städte.Add("Nidda");
            Städte.Add("Bad Wildungen");
            Städte.Add("Eltville am Rhein");
            Städte.Add("Kelsterbach");
            Städte.Add("Babenhausen");
            Städte.Add("Ginsheim-Gustavsburg");
            Städte.Add("Königstein im Taunus");
            Städte.Add("Künzell");
            Städte.Add("Hünfeld");
            Städte.Add("Seeheim-Jugenheim");
            Städte.Add("Bürstadt");
            Städte.Add("Reinheim");
            Städte.Add("Kirchhain");
            Städte.Add("Raunheim");
            Städte.Add("Michelstadt");
            Städte.Add("Alsfeld");
            Städte.Add("Petersberg");
            Städte.Add("Schlüchtern");
            Städte.Add("Dieburg");
            Städte.Add("Bad Arolsen");
            Städte.Add("Schwalbach am Taunus");
            Städte.Add("Hofgeismar");
            Städte.Add("Witzenhausen");
            Städte.Add("Ober-Ramstadt");
            Städte.Add("Köln");
            Städte.Add("Düsseldorf");
            Städte.Add("Dortmund");
            Städte.Add("Essen");
            Städte.Add("Duisburg");
            Städte.Add("Bochum");
            Städte.Add("Wuppertal");
            Städte.Add("Bielefeld");
            Städte.Add("Bonn");
            Städte.Add("Münster");
            Städte.Add("Mönchengladbach");
            Städte.Add("Gelsenkirchen");
            Städte.Add("Aachen");
            Städte.Add("Krefeld");
            Städte.Add("Oberhausen");
            Städte.Add("Hagen");
            Städte.Add("Hamm");
            Städte.Add("Mülheim an der Ruhr");
            Städte.Add("Leverkusen");
            Städte.Add("Solingen");
            Städte.Add("Herne");
            Städte.Add("Neuss");
            Städte.Add("Paderborn");
            Städte.Add("Bottrop");
            Städte.Add("Recklinghausen");
            Städte.Add("Bergisch Gladbach");
            Städte.Add("Remscheid");
            Städte.Add("Moers");
            Städte.Add("Siegen");
            Städte.Add("Gütersloh");
            Städte.Add("Witten");
            Städte.Add("Iserlohn");
            Städte.Add("Düren");
            Städte.Add("Ratingen");
            Städte.Add("Lünen");
            Städte.Add("Marl");
            Städte.Add("Velbert");
            Städte.Add("Minden");
            Städte.Add("Viersen");
            Städte.Add("Rheine");
            Städte.Add("Gladbeck");
            Städte.Add("Troisdorf");
            Städte.Add("Dorsten");
            Städte.Add("Detmold");
            Städte.Add("Arnsberg");
            Städte.Add("Castrop-Rauxel");
            Städte.Add("Lüdenscheid");
            Städte.Add("Bocholt");
            Städte.Add("Lippstadt");
            Städte.Add("Dinslaken");
            Städte.Add("Herford");
            Städte.Add("Kerpen");
            Städte.Add("Dormagen");
            Städte.Add("Grevenbroich");
            Städte.Add("Herten");
            Städte.Add("Bergheim");
            Städte.Add("Wesel");
            Städte.Add("Hürth");
            Städte.Add("Langenfeld (Rheinland)");
            Städte.Add("Unna");
        }

        private void DatenAnonymisieren()
        {
            var i = 0;
            var erweiterung = string.Empty;
            foreach (var redner in Datamodels.DataContainer.Redner)
            {
                redner.Name = Namen[i] + erweiterung;
                redner.Mail = redner.Name.Replace(" ", ".") + "@mail.de";
                redner.Mobil = "+49 150 123456";
                redner.Telefon = "0561-123 456";
                redner.InfoPrivate = "";
                redner.JwMail = redner.Name.Replace(" ", ".") + "@jwpub.org";

                i++;
                if (i >= Namen.Count)
                {
                    erweiterung = " jun.";
                    i = 0;
                }

                if (i >= Namen.Count*2)
                {
                    erweiterung = " sen.";
                    i = 0;
                }
            }

            i = 0;
            var lauf = 1;
            foreach (var versammlung in Datamodels.DataContainer.Versammlungen)
            {
                var teil = "";
                if (versammlung.Name.EndsWith("Nord"))
                    teil = "-Nord";
                if (versammlung.Name.EndsWith("Süd"))
                    teil = "-Süd";
                if (versammlung.Name.EndsWith("West"))
                    teil = "-West";
                if (versammlung.Name.EndsWith("Ost"))
                    teil = "-Ost";
                versammlung.Name = Städte[i] + teil + ((lauf > 1) ? $" {lauf}" : "");
                versammlung.Anschrift1 = $"Zum Königreich {i}";
                versammlung.Anschrift2 = "00000 " + Städte[i];
                versammlung.Anreise = "https://goo.gl/maps/mCihsiqLXaoFUhF46";
                versammlung.Koordinator = Namen[i];
                versammlung.KoordinatorJw = Namen[i].Replace(" ", ".") + "@jwpub.de";
                versammlung.KoordinatorMail = Namen[i].Replace(" ", ".") + "@mail.de";
                versammlung.KoordinatorMobil = "+49 150 123456";
                versammlung.KoordinatorTelefon = "0561-123 456";
                versammlung.Zoom = "Id: 123 4567 8901 Passwort: geheim";

                if (versammlung.Kreis == 209)
                    versammlung.Kreis = 1;

                i++;

                if (i >= Städte.Count)
                {
                    i = 0;
                    lauf++;
                }
            }

            i = 0;
            foreach (var m in Datamodels.DataContainer.AufgabenPersonZuordnung)
            {
                if (m.VerknüpftePerson != null)
                    m.PersonName = m.VerknüpftePerson.Name;
                else
                {
                    m.PersonName = Namen[i++];
                }
            }

            Datamodels.DataContainer.MeineVersammlung.Name = "Meine Versammlung";
        }
    }
}