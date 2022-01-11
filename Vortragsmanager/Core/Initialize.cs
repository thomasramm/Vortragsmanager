using Vortragsmanager.Datamodels;
using Vortragsmanager.Views;
using System.Collections.Generic;
using System;

namespace Vortragsmanager.Core
{
    public static class Initialize
    {
        public static void Reset()
        {
            DataContainer.Version = Update.CurrentVersion;
            DataContainer.IsInitialized = false;
            DataContainer.MeineVersammlung = null;
            DataContainer.ExternerPlan.Clear();
            DataContainer.MeinPlan.Clear();
            DataContainer.OffeneAnfragen.Clear();
            DataContainer.Redner.Clear();
            DataContainer.Versammlungen.Clear();
            TalkList.Clear();
        }

        public static void NewDatabase()
        {
            Log.Info(nameof(NewDatabase));
            Reset();
            LoadTalks();
            Templates.Load();
            DataContainer.Versammlungen.Clear();
            var wizard = new SetupWizardDialog();
            wizard.ShowDialog();

            var wizardData = (SetupWizardDialogViewModel)wizard.DataContext;
            DataContainer.IsInitialized = wizardData.IsFinished;
        }

        public static void LoadTalks()
        {
            var myTalks = LoadDefaultTalks();
            
            TalkList.Clear();
            foreach (var t in myTalks)
            {
                TalkList.Add(t);
            }
        }

        public static List<Talk> LoadDefaultTalks()
        {
            Log.Info(nameof(LoadTalks));
            var v = new List<Talk>(200);
            v.Add(new Talk(1, "Wie gut kenne ich Gott?"));
            v.Add(new Talk(2, "Die „letzten Tage“ - Wer wird sie überleben?"));
            v.Add(new Talk(3, "Mit Jehovas vereinter Organisation in Richtung Ewigkeit"));
            v.Add(new Talk(4, "Sichtbare Belege für die Existenz Gottes"));
            v.Add(new Talk(5, "Wie kann man als Familie glücklich sein?"));
            v.Add(new Talk(6, "Die Sintflut – nicht nur eine Geschichte"));
            v.Add(new Talk(7, "Sich den „Vater tiefen Mitgefühls“ zum Vorbild nehmen"));
            v.Add(new Talk(8, "Für Gott und nicht für sich selbst leben"));
            v.Add(new Talk(9, "Gottes Wort hören und danach leben"));
            v.Add(new Talk(10, "Bei allem, was wir tun, ehrlich sein"));
            v.Add(new Talk(11, "„Kein Teil der Welt“ sein – so wie Christus"));
            v.Add(new Talk(12, "Autorität – ist es Gott wichtig, wie wir darüber denken?"));
            v.Add(new Talk(13, "Gottes Ansicht über Sexualität und Ehe"));
            v.Add(new Talk(14, "Durch Sauberkeit und Reinheit Jehova ehren"));
            v.Add(new Talk(15, "Wie wir „allen Gutes tun“"));
            v.Add(new Talk(16, "Wie man sein Verhältnis zu Gott vertieft"));
            v.Add(new Talk(17, "Gott mit allem ehren, was wir haben"));
            v.Add(new Talk(18, "Ist Jehova meine „Festung“?"));
            v.Add(new Talk(19, "Wie kann man erfahren, was in Zukunft geschieht?"));
            v.Add(new Talk(20, "Ist für Gott die Zeit gekommen, die Welt zu regieren?"));
            v.Add(new Talk(21, "Das Vorrecht schätzen, zu Gottes Königreich zu gehören"));
            v.Add(new Talk(22, "Ziehen wir vollen Nutzen aus allem, wofür Jehova sorgt?"));
            v.Add(new Talk(23, "Unser Leben hat einen Sinn"));
            v.Add(new Talk(24, "„Eine besonders kostbare Perle“ – habe ich sie gefunden?"));
            v.Add(new Talk(25, "Dem Geist der Welt widerstehen"));
            v.Add(new Talk(26, "Bin ich Gott wichtig?", true)); //Dieser Vortrag sollte nicht mehr gehalten werden -> Neue Disposition
            v.Add(new Talk(27, "Ein guter Start in die Ehe"));
            v.Add(new Talk(28, "In der Ehe Liebe und Respekt zeigen"));
            v.Add(new Talk(29, "Elternsein – eine verantwortungsvolle, aber lohnende Aufgabe"));
            v.Add(new Talk(30, "Die Kommunikation in der Familie verbessern – wie?"));
            v.Add(new Talk(31, "Brauche ich Gott in meinem Leben?"));
            v.Add(new Talk(32, "Wie man mit den Sorgen des Lebens fertigwird"));
            v.Add(new Talk(33, "Wird Ungerechtigkeit jemals enden?"));
            v.Add(new Talk(34, "Trägst du das „Kennzeichen“ zum Überleben?", false)); //Dieser Vortrag sollte nicht mehr gehalten werden
            v.Add(new Talk(35, "Können wir ewig leben? Wenn ja, wie?"));
            v.Add(new Talk(36, "Ist mit dem jetzigen Leben alles vorbei?"));
            v.Add(new Talk(37, "Was bringt es, sich von Gott leiten zu lassen?"));
            v.Add(new Talk(38, "Wie kann man das Ende der Welt überleben?"));
            v.Add(new Talk(39, "Gottes Sieg mit Zuversicht erwarten"));
            v.Add(new Talk(40, "Was die nahe Zukunft bringt", true)); //Dieser Vortrag sollte nicht mehr gehalten werden -> Neue Disposition
            v.Add(new Talk(41, "„Bleibt stehen und seht, wie Jehova euch rettet“"));
            v.Add(new Talk(42, "Wie wirkt sich Gottes Königreich auf unser Leben aus?"));
            v.Add(new Talk(43, "Tue ich, was Gott von mir erwartet?"));
            v.Add(new Talk(44, "Was bringen uns die Lehren Jesu?"));
            v.Add(new Talk(45, "Den „Weg zum Leben“ gehen"));
            v.Add(new Talk(46, "Bleiben wir zuversichtlich bis zum Ende"));
            v.Add(new Talk(47, "Glaubt an die gute Botschaft", false)); //Dieser Vortrag sollte nicht mehr gehalten werden
            v.Add(new Talk(48, "Als Christ Loyalität beweisen"));
            v.Add(new Talk(49, "Eine gereinigte Erde – wer wird darauf leben?"));
            v.Add(new Talk(50, "Wie man gute Entscheidungen trifft"));
            v.Add(new Talk(51, "Verändert die Wahrheit mein Leben?"));
            v.Add(new Talk(52, "Wer ist mein Gott?"));
            v.Add(new Talk(53, "Denke ich so wie Gott?"));
            v.Add(new Talk(54, "Wie man den Glauben an Gott und seine Versprechen stärkt"));
            v.Add(new Talk(55, "Wie kann man sich einen guten Namen bei Gott machen?"));
            v.Add(new Talk(56, "Wessen Führung kann man vertrauen?"));
            v.Add(new Talk(57, "Unter Verfolgung standhaft bleiben"));
            v.Add(new Talk(58, "Woran erkennt man echte Christen?"));
            v.Add(new Talk(59, "Man erntet, was man sät"));
            v.Add(new Talk(60, "Was gibt meinem Leben Sinn?"));
            v.Add(new Talk(61, "Wessen Versprechen kann man vertrauen?"));
            v.Add(new Talk(62, "Das einzige Heilmittel für die Menschheit"));
            v.Add(new Talk(63, "Habe ich den Geist eines Evangeliumsverkündigers?"));
            v.Add(new Talk(64, "Liebe ich das Vergnügen oder Gott?"));
            v.Add(new Talk(65, "Frieden fördern in einer Welt voller Wut"));
            v.Add(new Talk(66, "Kann ich bei der Ernte mitarbeiten?"));
            v.Add(new Talk(67, "Über Gottes Wort und die Schöpfung intensiv nachdenken"));
            v.Add(new Talk(68, "Bin ich nachtragend oder vergebe ich?"));
            v.Add(new Talk(69, "Warum ist es wichtig, dass wir selbstlose Liebe zeigen?"));
            v.Add(new Talk(70, "Voll und ganz auf Jehova vertrauen", false)); //Dieser Vortrag sollte nicht mehr gehalten werden
            v.Add(new Talk(71, "Warum wir „wach bleiben“ müssen"));
            v.Add(new Talk(72, "Liebe – das Kennzeichen wahrer Christen"));
            v.Add(new Talk(73, "„Ein weises Herz bekommen“ – wie?"));
            v.Add(new Talk(74, "Jehovas Augen schauen auf uns"));
            v.Add(new Talk(75, "Erkenne ich Jehovas Souveränität in meinem Leben an?"));
            v.Add(new Talk(76, "Biblische Grundsätze - eine Hilfe bei heutigen Problemen?"));
            v.Add(new Talk(77, "„Seid immer gastfreundlich“"));
            v.Add(new Talk(78, "Jehova mit Freude dienen"));
            v.Add(new Talk(79, "Freundschaft mit Gott oder mit der Welt - wofür entscheide ich mich?"));
            v.Add(new Talk(80, "Wissenschaft oder Bibel – worauf stützt sich meine Hoffnung?"));
            v.Add(new Talk(81, "Wer ist befähigt, Gottes Diener zu sein?"));
            v.Add(new Talk(82, "Jehova und Christus - sind sie Teil einer Dreieinigkeit?"));
            v.Add(new Talk(83, "Die Zeit des Gerichts für die Religion"));
            v.Add(new Talk(84, "Dem entgehen, was dieser Welt bevorsteht"));
            v.Add(new Talk(85, "Eine gute Botschaft in einer gewalttätigen Welt"));
            v.Add(new Talk(86, "Welche Gebete erhört Gott?"));
            v.Add(new Talk(87, "Was für ein Verhältnis habe ich zu Gott?"));
            v.Add(new Talk(88, "Warum nach biblischen Maßstäben leben?"));
            v.Add(new Talk(89, "Den Durst nach Wahrheit stillen"));
            v.Add(new Talk(90, "Das wirkliche Leben ergreifen"));
            v.Add(new Talk(91, "Die Gegenwart des Messias und seine Herrschaft"));
            v.Add(new Talk(92, "Die Rolle der Religion im Weltgeschehen"));
            v.Add(new Talk(93, "Eingriffe Gottes - woran wirklich zu erkennen?"));
            v.Add(new Talk(94, "Die wahre Religion stillt die Bedürfnisse der menschlichen Gesellschaft"));
            v.Add(new Talk(95, "Was die Bibel über spiritistische Bräuche sagt"));
            v.Add(new Talk(96, "Welche Zukunft hat die Religion? "));
            v.Add(new Talk(97, "Sich in einer schlechten Welt nichts zuschulden kommen lassen"));
            v.Add(new Talk(98, "„Die Szene dieser Welt wechselt“"));
            v.Add(new Talk(99, "Warum man der Bibel vertrauen kann"));
            v.Add(new Talk(100, "Wahre Freundschaft mit Gott und den Mitmenschen"));
            v.Add(new Talk(101, "Jehova - der große Schöpfer"));
            v.Add(new Talk(102, "Den Prophezeiungen der Bibel Aufmerksamkeit schenken"));
            v.Add(new Talk(103, "Wie man im Dienst für Gott Freude finden kann"));
            v.Add(new Talk(104, "Als Eltern mit feuerfestem Material bauen"));
            v.Add(new Talk(105, "In allen unseren Prüfungen Trost finden"));
            v.Add(new Talk(106, "Die Zerstörung der Erde wird von Gott bestraft"));
            v.Add(new Talk(107, "In einer schlechten Welt ein gutes Gewissen behalten"));
            v.Add(new Talk(108, "Die Angst vor der Zukunft überwinden"));
            v.Add(new Talk(109, "Das Königreich Gottes ist nah"));
            v.Add(new Talk(110, "Gott steht in einer glücklichen Familie an erster Stelle"));
            v.Add(new Talk(111, "Was wird durch die Heilung der Völker erreicht?"));
            v.Add(new Talk(112, "In einer gesetzlosen Welt Liebe zeigen"));
            v.Add(new Talk(113, "Wie können Jugendliche glücklich und erfolgreich sein?"));
            v.Add(new Talk(114, "Für die Wunder der Schöpfung dankbar sein"));
            v.Add(new Talk(115, "Wie man Satans Fallen meidet"));
            v.Add(new Talk(116, "Bei seinem Umgang wählerisch sein"));
            v.Add(new Talk(117, "Wie man das Böse mit dem Guten besiegen kann"));
            v.Add(new Talk(118, "Jugendlichen gegenüber so eingestellt sein wie Jehova"));
            v.Add(new Talk(119, "Warum es gut ist, als Christ kein Teil der Welt zu sein"));
            v.Add(new Talk(120, "Warum man sich jetzt Gottes Herrschaft unterordnen sollte"));
            v.Add(new Talk(121, "Eine weltweite Bruderschaft in einer Zeit des Unheils bewahrt"));
            v.Add(new Talk(122, "Weltfrieden - woher zu erwarten?"));
            v.Add(new Talk(123, "Warum Christen anders sein müssen"));
            v.Add(new Talk(124, "Stammt die Bibel wirklich von Gott?"));
            v.Add(new Talk(125, "Warum die Menschheit ein Lösegeld benötigt"));
            v.Add(new Talk(126, "Wer kann gerettet werden?"));
            v.Add(new Talk(127, "Was geschieht, wenn wir sterben?"));
            v.Add(new Talk(128, "Ist die Hölle wirklich ein Ort feuriger Qual?"));
            v.Add(new Talk(129, "Ist die Dreieinigkeit eine biblische Lehre?"));
            v.Add(new Talk(130, "Die Erde wird für immer bestehen"));
            v.Add(new Talk(131, "Gibt es wirklich einen Teufel?"));
            v.Add(new Talk(132, "Die Auferstehung - der Sieg über den Tod"));
            v.Add(new Talk(133, "Der Ursprung des Menschen - ist es wichtig, was man glaubt?"));
            v.Add(new Talk(134, "Sollten Christen den Sabbat halten?"));
            v.Add(new Talk(135, "Die Heiligkeit von Leben und Blut"));
            v.Add(new Talk(136, "Wie denkt Gott über den Gebrauch von Bildern in der Anbetung?"));
            v.Add(new Talk(137, "Sind die in der Bibel berichteten Wunder wirklich geschehen?"));
            v.Add(new Talk(138, "Gutes Urteilsvermögen in einer verdorbenen Welt"));
            v.Add(new Talk(139, "Göttliche Weisheit in einer wissenschaftlich orientierten Welt"));
            v.Add(new Talk(140, "Jesus Christus - wer er wirklich ist"));
            v.Add(new Talk(141, "Das Seufzen der Menschheit - wann wird es enden?"));
            v.Add(new Talk(142, "Warum sollten wir bei Jehova Zuflucht suchen?"));
            v.Add(new Talk(143, "Auf den Gott allen Trostes vertrauen"));
            v.Add(new Talk(144, "Eine loyale Versammlung unter der Führung Christi"));
            v.Add(new Talk(145, "Wer ist wie Jehova, unser Gott?"));
            v.Add(new Talk(146, "Bildung zur Ehre Jehovas nutzen"));
            v.Add(new Talk(147, "Auf die rettende Macht Jehovas vertrauen"));
            v.Add(new Talk(148, "Das Leben so sehen, wie Gott es sieht"));
            v.Add(new Talk(149, "Unseren Weg mit Gott gehen"));
            v.Add(new Talk(150, "Ist die heutige Welt zum Untergang verurteilt?"));
            v.Add(new Talk(151, "Jehova ist seinem Volk „eine sichere Zuflucht“"));
            v.Add(new Talk(152, "Das wahre Armageddon – warum und wann?"));
            v.Add(new Talk(153, "Den „Ehrfurcht einflößenden Tag“ fest im Sinn behalten!"));
            v.Add(new Talk(154, "Die Menschenherrschaft - auf der Waage gewogen"));
            v.Add(new Talk(155, "Ist für Babylon die Stunde der Urteilsvollstreckung gekommen?"));
            v.Add(new Talk(156, "Der Gerichtstag – Grund zur Angst oder zur Hoffnung?"));
            v.Add(new Talk(157, "Wahre Christen lassen Gottes Lehren anziehend wirken"));
            v.Add(new Talk(158, "Seien wir mutig und vertrauen wir auf Jehova"));
            v.Add(new Talk(159, "In einer gefährlichen Welt Sicherheit finden"));
            v.Add(new Talk(160, "Die christliche Identität bewahren"));
            v.Add(new Talk(161, "Warum nahm Jesus Leid und Tod auf sich?"));
            v.Add(new Talk(162, "Befreiung aus einer finsteren Welt"));
            v.Add(new Talk(163, "Warum sollten wir Ehrfurcht vor dem wahren Gott haben?"));
            v.Add(new Talk(164, "Ist Gott noch Herr der Lage?"));
            v.Add(new Talk(165, "Wessen Wertvorstellungen teilen wir?"));
            v.Add(new Talk(166, "Mit Glauben und Mut in die Zukunft blicken"));
            v.Add(new Talk(167, "Vernünftig handeln in einer unvernünftigen Welt"));
            v.Add(new Talk(168, "Sicherheit in einer unruhigen Welt"));
            v.Add(new Talk(169, "Warum sich von der Bibel leiten lassen?"));
            v.Add(new Talk(170, "Wer eignet sich, die Menschheit zu regieren?"));
            v.Add(new Talk(171, "In Frieden leben – heute und für immer"));
            v.Add(new Talk(172, "In welchem Ruf stehe ich bei Gott?"));
            v.Add(new Talk(173, "Gibt es vom Standpunkt Gottes aus eine wahre Religion?"));
            v.Add(new Talk(174, "Gottes neue Welt - wer darf darin leben?"));
            v.Add(new Talk(175, "Was macht die Bibel glaubwürdig?"));
            v.Add(new Talk(176, "Echter Frieden und echte Sicherheit – wann?"));
            v.Add(new Talk(177, "Wo finden wir in schwierigen Zeiten Hilfe?"));
            v.Add(new Talk(178, "Den „Weg der Integrität“ gehen"));
            v.Add(new Talk(179, "Auf Gottes Königreich bauen - nicht auf Illusionen"));
            v.Add(new Talk(180, "Warum die Auferstehung für uns eine Realität sein sollte"));
            v.Add(new Talk(181, "Ist es später, als wir denken?"));
            v.Add(new Talk(182, "Was das Reich Gottes schon heute für uns tut"));
            v.Add(new Talk(183, "Den Blick von Wertlosem wegwenden"));
            v.Add(new Talk(184, "Ist mit dem Tod alles vorbei?"));
            v.Add(new Talk(185, "Was bewirkt die Wahrheit in unserem Leben?"));
            v.Add(new Talk(186, "Sich Gottes glücklichem Volk anschließen"));
            v.Add(new Talk(187, "Warum läßt ein liebevoller Gott das Böse zu?"));
            v.Add(new Talk(188, "Vertrauen wir voller Zuversicht auf Jehova?"));
            v.Add(new Talk(189, "Seinen Weg mit Gott zu gehen bringt Segen – jetzt und für immer"));
            v.Add(new Talk(190, "Vollkommenes Familienglück – ein Versprechen von Gott"));
            v.Add(new Talk(191, "Wie Liebe und Glaube die Welt besiegen"));
            v.Add(new Talk(192, "Bin ich auf dem Weg zum ewigen Leben?"));
            v.Add(new Talk(193, "In der „schweren Zeit“ gerettet werden"));
            v.Add(new Talk(194, "Wie göttliche Weisheit uns zugutekommt"));
            v.Add(new Talk(12322, "Echte Hoffnung - wo zu finden?"));
            v.Add(new Talk(-1, "Unbekannt", false));
            v.Add(new Talk(-24, "Was Gottes Herrschaft für uns bewirken kann", false));

            return v;
        }

        public static void DemoAktualisieren()
        {
            var yeardiff = DateTime.Today.Year - 2021; //in 2022 ist das ergebnis +1, also addYear(1)
            var kwdiff = yeardiff * 100;

            foreach (var m in Datamodels.DataContainer.MeinPlan)
            {
                m.Kw += kwdiff;

                //Sonderfall Anfragen, hier gibt es eine Liste von Daten
                if (m is Datamodels.Inquiry m1)
                {
                    m1.AnfrageDatum = m1.AnfrageDatum.AddYears(yeardiff);
                    for (int i = 0; i < m1.Kws.Count; i++)
                    {
                        m1.Kws[i] += kwdiff;
                    }
                }
            }

            foreach (var m in Datamodels.DataContainer.Abwesenheiten)
            {
                m.Kw += kwdiff;
            }

            foreach (var m in Datamodels.DataContainer.OffeneAnfragen)
            {
                m.Kw += kwdiff;
                for (int i = 0; i < m.Kws.Count; i++)
                {
                    m.Kws[i] += kwdiff;
                }
            }

            foreach (var m in Datamodels.DataContainer.ExternerPlan)
            {
                m.Kw += kwdiff;
            }

            foreach (var m in Datamodels.DataContainer.Absagen)
            {
                m.Kw += kwdiff;
            }

            foreach (var m in Datamodels.DataContainer.Aktivitäten)
            {
                m.Datum = m.Datum.AddYears(yeardiff);
                m.KalenderKw += kwdiff;
            }
            foreach (var m in Datamodels.DataContainer.AufgabenPersonKalender)
            {
                m.Kw += kwdiff;
            }
        }
    }
}