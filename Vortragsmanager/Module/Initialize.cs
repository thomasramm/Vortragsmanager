using System;
using System.Collections.Generic;
using Vortragsmanager.Datamodels;
using Vortragsmanager.Windows;

namespace Vortragsmanager.Module
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
            var v = new List<Talk>(200)
            {
                new Talk(1, "Wie gut kenne ich Gott?"),
                new Talk(2, "Die „letzten Tage“ - Wer wird sie überleben?"),
                new Talk(3, "Mit Jehovas vereinter Organisation in Richtung Ewigkeit"),
                new Talk(4, "Sichtbare Belege für die Existenz Gottes"),
                new Talk(5, "Wie kann man als Familie glücklich sein?"),
                new Talk(6, "Die Sintflut – nicht nur eine Geschichte"),
                new Talk(7, "Sich den „Vater tiefen Mitgefühls“ zum Vorbild nehmen"),
                new Talk(8, "Für Gott und nicht für sich selbst leben"),
                new Talk(9, "Gottes Wort hören und danach leben"),
                new Talk(10, "Bei allem, was wir tun, ehrlich sein"),
                new Talk(11, "„Kein Teil der Welt“ sein – so wie Christus"),
                new Talk(12, "Autorität – ist es Gott wichtig, wie wir darüber denken?"),
                new Talk(13, "Gottes Ansicht über Sexualität und Ehe"),
                new Talk(14, "Durch Sauberkeit und Reinheit Jehova ehren"),
                new Talk(15, "Wie wir „allen Gutes tun“"),
                new Talk(16, "Wie man sein Verhältnis zu Gott vertieft"),
                new Talk(17, "Gott mit allem ehren, was wir haben"),
                new Talk(18, "Ist Jehova meine „Festung“?"),
                new Talk(19, "Wie kann man erfahren, was in Zukunft geschieht?"),
                new Talk(20, "Ist für Gott die Zeit gekommen, die Welt zu regieren?"),
                new Talk(21, "Das Vorrecht schätzen, zu Gottes Königreich zu gehören"),
                new Talk(22, "Ziehen wir vollen Nutzen aus allem, wofür Jehova sorgt?"),
                new Talk(23, "Unser Leben hat einen Sinn"),
                new Talk(24, "„Eine besonders kostbare Perle“ – habe ich sie gefunden?"),
                new Talk(25, "Dem Geist der Welt widerstehen"),
                new Talk(26, "Bin ich Gott wichtig?"), //Dieser Vortrag sollte nicht mehr gehalten werden -> Neue Disposition
                new Talk(27, "Ein guter Start in die Ehe"),
                new Talk(28, "In der Ehe Liebe und Respekt zeigen"),
                new Talk(29, "Elternsein – eine verantwortungsvolle, aber lohnende Aufgabe"),
                new Talk(30, "Die Kommunikation in der Familie verbessern – wie?"),
                new Talk(31, "Brauche ich Gott in meinem Leben?"),
                new Talk(32, "Wie man mit den Sorgen des Lebens fertigwird"),
                new Talk(33, "Wird Ungerechtigkeit jemals enden?"),
                new Talk(34, "Werde ich das Zeichen zum Überleben bekommen?", false), //Dieser Vortrag sollte nicht mehr gehalten werden
                new Talk(35, "Können wir ewig leben? Wenn ja, wie?"),
                new Talk(36, "Ist mit dem jetzigen Leben alles vorbei?"),
                new Talk(37, "Was bringt es, sich von Gott leiten zu lassen?"),
                new Talk(38, "Wie kann man das Ende der Welt überleben?"),
                new Talk(39, "Jesus Christus hat die Welt besiegt – wie und wann?"),
                new Talk(40, "Was die nahe Zukunft bringt"), //Dieser Vortrag sollte nicht mehr gehalten werden -> Neue Disposition
                new Talk(41, "„Bleibt stehen und seht, wie Jehova euch rettet“"),
                new Talk(42, "Wie wirkt sich Gottes Königreich auf unser Leben aus?"),
                new Talk(43, "Tue ich, was Gott von mir erwartet?"),
                new Talk(44, "Was bringen uns die Lehren Jesu?"),
                new Talk(45, "Den „Weg zum Leben“ gehen"),
                new Talk(46, "Bleiben wir zuversichtlich bis zum Ende"),
                new Talk(47, "Glaubt an die gute Botschaft", false), //Dieser Vortrag sollte nicht mehr gehalten werden
                new Talk(48, "Als Christ Loyalität beweisen"),
                new Talk(49, "Eine gereinigte Erde – wer wird darauf leben?"),
                new Talk(50, "Wie man gute Entscheidungen trifft"),
                new Talk(51, "Verändert die Wahrheit mein Leben?"),
                new Talk(52, "Wer ist mein Gott?"),
                new Talk(53, "Denke ich so wie Gott?"),
                new Talk(54, "Wie man den Glauben an Gott und seine Versprechen stärkt"),
                new Talk(55, "Wie kann man sich einen guten Namen bei Gott machen?"),
                new Talk(56, "Wessen Führung kann man vertrauen?"),
                new Talk(57, "Unter Verfolgung standhaft bleiben"),
                new Talk(58, "Woran erkennt man echte Christen?"),
                new Talk(59, "Man erntet, was man sät"),
                new Talk(60, "Was gibt meinem Leben Sinn?"),
                new Talk(61, "Wessen Versprechen kann man vertrauen?"),
                new Talk(62, "Echte Hoffnung – wo zu finden?"),
                new Talk(63, "Habe ich den Geist eines Evangeliumsverkündigers?"),
                new Talk(64, "Liebe ich das Vergnügen oder Gott?"),
                new Talk(65, "Frieden fördern in einer Welt voller Wut"),
                new Talk(66, "Kann ich bei der Ernte mitarbeiten?"),
                new Talk(67, "Über Gottes Wort und die Schöpfung intensiv nachdenken"),
                new Talk(68, "Vergeben wir einander weiterhin großzügig"),
                new Talk(69, "Warum ist es wichtig, dass wir selbstlose Liebe zeigen?"),
                new Talk(70, "Voll und ganz auf Jehova vertrauen", false), //Dieser Vortrag sollte nicht mehr gehalten werden
                new Talk(71, "Warum wir „wach bleiben“ müssen"),
                new Talk(72, "Liebe – das Kennzeichen wahrer Christen"),
                new Talk(73, "„Ein weises Herz bekommen“ – wie?"),
                new Talk(74, "Jehovas Augen schauen auf uns"),
                new Talk(75, "Jehovas Herrschaft - unterstütze ich sie?"),
                new Talk(76, "Biblische Grundsätze - eine Hilfe bei heutigen Problemen?"),
                new Talk(77, "„Seid immer gastfreundlich“"),
                new Talk(78, "Jehova zu dienen bringt Freude"),
                new Talk(79, "Für welche Freundschaft werde ich mich entscheiden?"),
                new Talk(80, "Wissenschaft oder Bibel – worauf sollte man seine Hoffnung setzen?"),
                new Talk(81, "Kann ich ein Bibellehrer sein?"),
                new Talk(82, "Jehova und Christus - sind sie Teil einer Dreieinigkeit?", false),
                new Talk(83, "Die Zeit des Gerichts für die Religion"),
                new Talk(84, "Dem entgehen, was dieser Welt bevorsteht"),
                new Talk(85, "Eine gute Botschaft in einer gewalttätigen Welt"),
                new Talk(86, "Welche Gebete erhört Gott?"),
                new Talk(87, "Was für ein Verhältnis habe ich zu Gott?"),
                new Talk(88, "Warum nach biblischen Maßstäben leben?"),
                new Talk(89, "Den Durst nach Wahrheit stillen"),
                new Talk(90, "Das wirkliche Leben ergreifen"),
                new Talk(91, "Die Gegenwart des Messias und seine Herrschaft"),
                new Talk(92, "Die Rolle der Religion im Weltgeschehen"),
                new Talk(93, "Naturkatastrophen – werden sie jemals enden?"),
                new Talk(94, "Die wahre Religion stillt die Bedürfnisse der menschlichen Gesellschaft"),
                new Talk(95, "Spiritismus – warum gefährlich?"),
                new Talk(96, "Welche Zukunft hat die Religion? "),
                new Talk(97, "Sich in einer schlechten Welt nichts zuschulden kommen lassen"),
                new Talk(98, "„Die Szene dieser Welt wechselt“"),
                new Talk(99, "Warum man der Bibel vertrauen kann"),
                new Talk(100, "Wie kann ich starke und dauerhafte Freundschaften aufbauen?"),
                new Talk(101, "Jehova – der „große Schöpfer“"),
                new Talk(102, "Den Prophezeiungen der Bibel Aufmerksamkeit schenken"),
                new Talk(103, "Wie man im Dienst für Gott Freude finden kann"),
                new Talk(104, "Als Eltern mit feuerfestem Material bauen"),
                new Talk(105, "In allen unseren Prüfungen Trost finden"),
                new Talk(106, "Die Zerstörung der Erde wird von Gott bestraft"),
                new Talk(107, "In einer schlechten Welt ein gutes Gewissen behalten"),
                new Talk(108, "Die Angst vor der Zukunft überwinden"),
                new Talk(109, "Das Königreich Gottes ist nah"),
                new Talk(110, "Gott steht in einer glücklichen Familie an erster Stelle"),
                new Talk(111, "Was wird durch die Heilung der Völker erreicht?"),
                new Talk(112, "In einer gesetzlosen Welt Liebe zeigen"),
                new Talk(113, "Wie können Jugendliche glücklich und erfolgreich sein?"),
                new Talk(114, "Für die Wunder der Schöpfung dankbar sein"),
                new Talk(115, "Wie man Satans Fallen meidet"),
                new Talk(116, "Bei seinem Umgang wählerisch sein"),
                new Talk(117, "Wie man das Böse mit dem Guten besiegen kann"),
                new Talk(118, "Jugendlichen gegenüber so eingestellt sein wie Jehova"),
                new Talk(119, "Warum es gut ist, als Christ kein Teil der Welt zu sein"),
                new Talk(120, "Warum man sich jetzt Gottes Herrschaft unterordnen sollte"),
                new Talk(121, "Ein geeintes Volk wird gerettet"),
                new Talk(122, "Weltfrieden - woher zu erwarten?"),
                new Talk(123, "Warum Christen anders sein müssen"),
                new Talk(124, "Stammt die Bibel wirklich von Gott?"),
                new Talk(125, "Warum die Menschheit ein Lösegeld benötigt"),
                new Talk(126, "Wer kann gerettet werden?"),
                new Talk(127, "Was geschieht, wenn wir sterben?"),
                new Talk(128, "Ist die Hölle wirklich ein Ort feuriger Qual?"),
                new Talk(129, "Ist die Dreieinigkeit eine biblische Lehre?"),
                new Talk(130, "Die Erde wird für immer bestehen"),
                new Talk(131, "Gibt es wirklich einen Teufel?"),
                new Talk(132, "Die Auferstehung - der Sieg über den Tod"),
                new Talk(133, "Der Ursprung des Menschen - ist es wichtig, was man glaubt?"),
                new Talk(134, "Sollten Christen den Sabbat halten?"),
                new Talk(135, "Die Heiligkeit von Leben und Blut"),
                new Talk(136, "Wie denkt Gott über den Gebrauch von Bildern in der Anbetung?"),
                new Talk(137, "Sind die in der Bibel berichteten Wunder wirklich geschehen?"),
                new Talk(138, "Gutes Urteilsvermögen in einer verdorbenen Welt"),
                new Talk(139, "Göttliche Weisheit in einer wissenschaftlich orientierten Welt"),
                new Talk(140, "Jesus Christus - wer er wirklich ist"),
                new Talk(141, "Das Seufzen der Menschheit - wann wird es enden?"),
                new Talk(142, "Warum sollten wir bei Jehova Zuflucht suchen?"),
                new Talk(143, "Auf den Gott allen Trostes vertrauen"),
                new Talk(144, "Eine loyale Versammlung unter der Führung Christi"),
                new Talk(145, "Wer ist wie Jehova, unser Gott?"),
                new Talk(146, "Bildung zur Ehre Jehovas nutzen"),
                new Talk(147, "Auf die rettende Macht Jehovas vertrauen"),
                new Talk(148, "Das Leben so sehen, wie Gott es sieht"),
                new Talk(149, "Unseren Weg mit Gott gehen"),
                new Talk(150, "Ist die heutige Welt zum Untergang verurteilt?"),
                new Talk(151, "Jehova ist seinem Volk „eine sichere Zuflucht“"),
                new Talk(152, "Das wahre Armageddon – warum und wann?"),
                new Talk(153, "Den „Ehrfurcht einflößenden Tag“ fest im Sinn behalten!"),
                new Talk(154, "Die Menschenherrschaft - auf der Waage gewogen"),
                new Talk(155, "Ist für Babylon die Stunde der Urteilsvollstreckung gekommen?"),
                new Talk(156, "Der Gerichtstag – Grund zur Angst oder zur Hoffnung?"),
                new Talk(157, "Wahre Christen lassen Gottes Lehren anziehend wirken"),
                new Talk(158, "Seien wir mutig und vertrauen wir auf Jehova"),
                new Talk(159, "In einer gefährlichen Welt Sicherheit finden"),
                new Talk(160, "Die christliche Identität bewahren"),
                new Talk(161, "Warum nahm Jesus Leid und Tod auf sich?"),
                new Talk(162, "Befreiung aus einer finsteren Welt"),
                new Talk(163, "Warum sollten wir Ehrfurcht vor dem wahren Gott haben?"),
                new Talk(164, "Ist Gott noch Herr der Lage?"),
                new Talk(165, "Wessen Wertvorstellungen teilen wir?"),
                new Talk(166, "Mit Glauben und Mut in die Zukunft blicken"),
                new Talk(167, "Vernünftig handeln in einer unvernünftigen Welt"),
                new Talk(168, "Sicherheit in einer unruhigen Welt"),
                new Talk(169, "Warum sich von der Bibel leiten lassen?"),
                new Talk(170, "Wer eignet sich, die Menschheit zu regieren?"),
                new Talk(171, "In Frieden leben – heute und für immer"),
                new Talk(172, "In welchem Ruf stehe ich bei Gott?"),
                new Talk(173, "Gibt es vom Standpunkt Gottes aus eine wahre Religion?"),
                new Talk(174, "Gottes neue Welt - wer darf darin leben?"),
                new Talk(175, "Was macht die Bibel glaubwürdig?"),
                new Talk(176, "Echter Frieden und echte Sicherheit – wann?"),
                new Talk(177, "Wo finden wir in schwierigen Zeiten Hilfe?"),
                new Talk(178, "Den „Weg der Integrität“ gehen"),
                new Talk(179, "Auf Gottes Königreich bauen - nicht auf Illusionen"),
                new Talk(180, "Warum die Auferstehung für uns eine Realität sein sollte"),
                new Talk(181, "Ist es später, als wir denken?"),
                new Talk(182, "Was das Reich Gottes schon heute für uns tut"),
                new Talk(183, "Den Blick von Wertlosem wegwenden"),
                new Talk(184, "Ist mit dem Tod alles vorbei?"),
                new Talk(185, "Was bewirkt die Wahrheit in unserem Leben?"),
                new Talk(186, "Sich Gottes glücklichem Volk anschließen"),
                new Talk(187, "Warum läßt ein liebevoller Gott das Böse zu?"),
                new Talk(188, "Vertrauen wir voller Zuversicht auf Jehova?"),
                new Talk(189, "Seinen Weg mit Gott zu gehen bringt Segen – jetzt und für immer"),
                new Talk(190, "Vollkommenes Familienglück – ein Versprechen von Gott"),
                new Talk(191, "Wie Liebe und Glaube die Welt besiegen"),
                new Talk(192, "Bin ich auf dem Weg zum ewigen Leben?"),
                new Talk(193, "In der „schweren Zeit“ gerettet werden"),
                new Talk(194, "Wie göttliche Weisheit uns zugutekommt"),
                new Talk(12322, "Echte Hoffnung - wo zu finden?"),
                new Talk(12323, "Wir können zuversichtlich in die Zukunft schauen!"),
                new Talk(-1, "Unbekannt", false),
                new Talk(-24, "Was Gottes Herrschaft für uns bewirken kann", false)
            };

            return v;
        }

        public static void DemoAktualisieren()
        {
            var yeardiff = DateTime.Today.Year - 2021; //in 2022 ist das ergebnis +1, also addYear(1)
            var kwdiff = yeardiff * 100;

            foreach (var m in DataContainer.MeinPlan)
            {
                m.Kw += kwdiff;

                //Sonderfall Anfragen, hier gibt es eine Liste von Daten
                if (m is Inquiry m1)
                {
                    m1.AnfrageDatum = m1.AnfrageDatum.AddYears(yeardiff);
                    for (int i = 0; i < m1.Kws.Count; i++)
                    {
                        m1.Kws[i] += kwdiff;
                    }
                }
            }

            foreach (var m in DataContainer.Abwesenheiten)
            {
                m.Kw += kwdiff;
            }

            foreach (var m in DataContainer.OffeneAnfragen)
            {
                m.Kw += kwdiff;
                for (int i = 0; i < m.Kws.Count; i++)
                {
                    m.Kws[i] += kwdiff;
                }
            }

            foreach (var m in DataContainer.ExternerPlan)
            {
                m.Kw += kwdiff;
            }

            foreach (var m in DataContainer.Absagen)
            {
                m.Kw += kwdiff;
            }

            foreach (var m in DataContainer.Aktivitäten)
            {
                m.Datum = m.Datum.AddYears(yeardiff);
                m.KalenderKw += kwdiff;
            }
            foreach (var m in DataContainer.AufgabenPersonKalender)
            {
                m.Kw += kwdiff;
            }
        }
    }
}