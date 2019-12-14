﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vortragsmanager.Core
{
    public class Anonymisieren
    {
        public static void Start()
        {
            _ = new Anonymisieren();
        }
        public Anonymisieren()
        {
            LoadNamen();
            LoadCity();
            DatenAnonymisieren();
        }

        private List<string> Namen { get; set; }

        private List<string> Städte { get; set; }

        public void LoadNamen()
        {
            Namen = new List<string>(100);
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
        }

        public void LoadCity()
        {
            Städte = new List<string>(100);
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

        }

        private void DatenAnonymisieren()
        {
            var i = 0;
            foreach (var redner in DataContainer.Redner)
            {
                redner.Name = Namen[i];
                redner.Mail = Namen[i].Replace(" ", ".") + "@mail.de";
                redner.Mobil = "+49 150 123456";
                redner.Telefon = "0561-123 456";
                i++;
            }

            i = 0;
            foreach (var versammlung in DataContainer.Versammlungen)
            {
                versammlung.Name = Städte[i];
                versammlung.Anschrift1 = "Zum Königreich 1";
                versammlung.Anschrift2 = "00000 " + Städte[i];
                versammlung.Anreise = "https://goo.gl/maps/mCihsiqLXaoFUhF46";
                versammlung.Koordinator = Namen[i];
                versammlung.KoordinatorJw = Namen[i].Replace(" ", ".") + "@jwpub.de";
                versammlung.KoordinatorMail = Namen[i].Replace(" ", ".") + "@mail.de";
                versammlung.KoordinatorMobil = "+49 150 123456";
                versammlung.KoordinatorTelefon = "0561-123 456";
                i++;
            }
        }
    }
}
