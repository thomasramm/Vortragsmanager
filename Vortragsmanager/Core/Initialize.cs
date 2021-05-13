using Vortragsmanager.Datamodels;
using Vortragsmanager.Views;
using System.Linq;
using System.Collections.Generic;

namespace Vortragsmanager.Core
{
    public static class Initialize
    {
        public static void Reset()
        {
            DataContainer.Version = Helper.CurrentVersion;
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
            v.Add(new Talk(1, "Wie gut kennst du Gott?") );
            v.Add(new Talk(1, "Wie gut kennst du Gott?"));
            v.Add(new Talk(2, "Die letzten Tage: Wer wird sie überleben?"));
            v.Add(new Talk(3, "Mit Jehovas vereinter Organisation weiter Richtung Ewigkeit"));
            v.Add(new Talk(4, "Beweise für die Existenz Gottes wahrnehmen"));
            v.Add(new Talk(5, "Als Familie glücklich sein"));
            v.Add(new Talk(6, "Die Sintflut - mehr als eine Geschichte"));
            v.Add(new Talk(7, "Den \"Vater inniger Erbarmungen\" nachahmen"));
            v.Add(new Talk(8, "Für Gott und nicht für sich selbst leben"));
            v.Add(new Talk(9, "Auf Gottes Wort hören und danach handeln"));
            v.Add(new Talk(10, "In all unserem Handeln ehrlich sein"));
            v.Add(new Talk(11, "Nachahmer Christi sind \"kein Teil der Welt\""));
            v.Add(new Talk(12, "Autorität – Ist es Gott wichtig wie du darüber denkst?"));
            v.Add(new Talk(13, "Gottes Standpunkt zu Sexualität und Ehe"));
            v.Add(new Talk(14, "Ein reines Volk ehrt Jehova"));
            v.Add(new Talk(15, "\"Gegenüber allen das Gute wirken\""));
            v.Add(new Talk(16, "Vertiefe weiterhin dein Verhältnis zu Gott "));
            v.Add(new Talk(17, "Gott verherrlichen mit allem, was wir haben"));
            v.Add(new Talk(18, "Mache Jehova zu deiner Festung"));
            v.Add(new Talk(19, "Wie kann man erfahren, was in Zukunft geschieht?"));
            v.Add(new Talk(20, "Ist für Gott die Zeit gekommen, die Welt zu regieren?"));
            v.Add(new Talk(21, "Das Vorrecht schätzen, zu Gottes Königreich zu gehören"));
            v.Add(new Talk(22, "Ziehen wir vollen Nutzen aus allem, wofür Jehova sorgt?"));
            v.Add(new Talk(23, "Unser Leben hat einen Sinn"));
            v.Add(new Talk(24, "„Eine besonders kostbare Perle“ – habe ich sie gefunden?"));
            v.Add(new Talk(27, "Ein guter Anfang für die Ehe"));
            v.Add(new Talk(28, "In der Ehe Respekt und Liebe bekunden"));
            v.Add(new Talk(29, "Eltern sein - eine dankbare, aber verantwortungsvolle Aufgabe"));
            v.Add(new Talk(30, "Gedankenaustausch - in der Familie und mit Gott"));
            v.Add(new Talk(31, "Brauche ich Gott in meinem Leben? "));
            v.Add(new Talk(32, "Wie man mit den Sorgen des Lebens fertig wird"));
            v.Add(new Talk(33, "Was verbirgt sich hinter dem Geist der Rebellion?"));
            v.Add(new Talk(35, "Kannst du und wirst du ewig leben?"));
            v.Add(new Talk(36, "Ist mit dem jetzigen Leben alles vorbei?"));
            v.Add(new Talk(37, "Bringen Gottes Wege uns wirklich weiter?"));
            v.Add(new Talk(38, "Weise handeln, während das Ende herannaht"));
            v.Add(new Talk(39, "Blicke dem göttlichen Sieg mit Zuversicht entgegen!"));
            v.Add(new Talk(41, "\"Bleibt stehen und seht die Rettung Jehovas\""));
            v.Add(new Talk(42, "Wie wirkt sich Gottes Königreich auf uns aus?"));
            v.Add(new Talk(43, "Tust du, was Gott von dir verlangt?"));
            v.Add(new Talk(44, "Sucht fortgesetzt Gottes Königreich"));
            v.Add(new Talk(45, "Gehe den Weg, der zu ewigem Leben führt"));
            v.Add(new Talk(46, "Halte standhaft bis zum Ende an deiner Zuversicht fest"));
            v.Add(new Talk(48, "Beweise deine Loyalität als Christ"));
            v.Add(new Talk(49, "Eine gereinigte Erde - wirst du sie erleben?"));
            v.Add(new Talk(50, "Wie wirst du dich entscheiden?"));
            v.Add(new Talk(51, "Verändert die Wahrheit dein Leben?"));
            v.Add(new Talk(52, "Wer ist dein Gott?"));
            v.Add(new Talk(53, "Stimmst du in deinem Denken mit Gott überein?"));
            v.Add(new Talk(54, "Stärke deinen Glauben an den Schöpfer des Menschen"));
            v.Add(new Talk(55, "Was für einen Namen machst du dir bei Gott?"));
            v.Add(new Talk(56, "Wessen Führung kannst du vertrauen?"));
            v.Add(new Talk(57, "Unter Verfolgung standhalten"));
            v.Add(new Talk(58, "Was macht Christen zu wahren Christen?"));
            v.Add(new Talk(59, "Du wirst ernten, was du säst"));
            v.Add(new Talk(60, "Wie sinnvoll ist dein Leben?"));
            v.Add(new Talk(61, "Auf wessen Versprechungen vertraust du?"));
            v.Add(new Talk(62, "Das einzige Heilmittel für die kranke Menschheit"));
            v.Add(new Talk(63, "Hast du den Geist eines Evangeliumsverkündigers?"));
            v.Add(new Talk(64, "Liebst du Vergnügungen mehr als Gott?"));
            v.Add(new Talk(65, "Frieden fördern in einer Welt voller Wut"));
            v.Add(new Talk(66, "Dient als Sklaven für den Herrn der Ernte"));
            v.Add(new Talk(67, "Nimm dir Zeit, über geistige Dinge nachzusinnen"));
            v.Add(new Talk(68, "Hegst du Groll oder vergibst du?"));
            v.Add(new Talk(69, "Den Geist der Selbstaufopferung beleben"));
            v.Add(new Talk(70, "Voll und ganz auf Jehova vertrauen", false));
            v.Add(new Talk(71, "Wie man geistig wach bleibt"));
            v.Add(new Talk(72, "Liebe - das Kennzeichen der wahren Christenversammlung"));
            v.Add(new Talk(73, "Ein \"Herz der Weisheit\" erwerben"));
            v.Add(new Talk(74, "Jehovas Augen sind auf uns gerichtet"));
            v.Add(new Talk(75, "Erkennst du Jehovas Souveränität in deinem eigenen Leben an?"));
            v.Add(new Talk(76, "Biblische Grundsätze - eine Hilfe bei heutigen Problemen?"));
            v.Add(new Talk(77, "Folgt dem Weg der Gastfreundschaft "));
            v.Add(new Talk(78, "Diene Jehova mit einem freudigem Herzen"));
            v.Add(new Talk(79, "Freundschaft mit Gott oder mit der Welt - wofür entscheidest du dich?"));
            v.Add(new Talk(80, "Stützt sich deine Hoffnung auf die Wissenschaft oder auf die Bibel?"));
            v.Add(new Talk(81, "Wer ist befähigt, Gottes Diener zu sein?"));
            v.Add(new Talk(82, "Jehova und Christus - sind sie Teil einer Dreieinigkeit?"));
            v.Add(new Talk(83, "Die Gerichtszeit für die Religion"));
            v.Add(new Talk(84, "Wirst du dem Geschick dieser Welt entgehen?"));
            v.Add(new Talk(85, "Eine gute Botschaft in einer gewalttätigen Welt"));
            v.Add(new Talk(86, "Gebete, die von Gott erhört werden"));
            v.Add(new Talk(87, "Welches Verhältnis hast du zu Gott?"));
            v.Add(new Talk(88, "Warum nach biblischen Maßstäben leben?"));
            v.Add(new Talk(89, "Kommt, die ihr nach der Wahrheit dürstet!"));
            v.Add(new Talk(90, "Ergreife das wirkliche Leben! "));
            v.Add(new Talk(91, "Die Gegenwart des Messias und seine Herrschaft"));
            v.Add(new Talk(92, "Die Rolle der Religion im Weltgeschehen"));
            v.Add(new Talk(93, "Eingriffe Gottes - woran wirklich zu erkennen?"));
            v.Add(new Talk(94, "Die wahre Religion stillt die Bedürfnisse der menschlichen Gesellschaft"));
            v.Add(new Talk(95, "Was die Bibel über spiritistische Bräuche sagt"));
            v.Add(new Talk(96, "Welche Zukunft hat die Religion? "));
            v.Add(new Talk(97, "Inmitten einer verkehrten Generation untadelig bleiben"));
            v.Add(new Talk(98, "\"Die Szene dieser Welt wechselt\""));
            v.Add(new Talk(99, "Warum man der Bibel vertrauen kann"));
            v.Add(new Talk(100, "Wahre Freundschaft mit Gott und dem Nächsten"));
            v.Add(new Talk(101, "Jehova - der große Schöpfer"));
            v.Add(new Talk(102, "Dem prophetischen Wort Aufmerksamkeit schenken"));
            v.Add(new Talk(103, "Wie man im Dienst für Gott Freude finden kann"));
            v.Add(new Talk(104, "Ihr Eltern, baut ihr mit feuerbeständigem Material?"));
            v.Add(new Talk(105, "In all unseren Drangsalen Trost finden"));
            v.Add(new Talk(106, "Die Zerstörung der Erde wird von Gott bestraft"));
            v.Add(new Talk(107, "In einer sündigen Welt ein gutes Gewissen bewahren"));
            v.Add(new Talk(108, "Die Angst vor der Zukunft überwinden"));
            v.Add(new Talk(109, "Das Königreich Gottes ist nahe"));
            v.Add(new Talk(110, "Gott steht in einer glücklichen Familie an erster Stelle"));
            v.Add(new Talk(111, "Was wird durch die Heilung der Nationen erreicht?"));
            v.Add(new Talk(112, "Wie man in einer gesetzlosen Welt Liebe bekundet"));
            v.Add(new Talk(113, "Wie können Jugendliche die heutige Krisensituation meistern?"));
            v.Add(new Talk(114, "Die Wunder der Schöpfung Gottes würdigen"));
            v.Add(new Talk(115, "Wie man Satans Schlingen meidet"));
            v.Add(new Talk(116, "Sei wählerisch in deinem Umgang"));
            v.Add(new Talk(117, "Wie man das Böse mit dem Guten besiegen kann"));
            v.Add(new Talk(118, "Jugendlichen gegenüber so eingestellt sein wie Jehova"));
            v.Add(new Talk(119, "Von welchem Nutzen es für Christen ist, sich von der Welt getrennt zu halten"));
            v.Add(new Talk(120, "Warum sich jetzt Gottes Herrschaft unterwerfen"));
            v.Add(new Talk(121, "Eine weltweite Bruderschaft in einer Zeit des Unheils bewahrt"));
            v.Add(new Talk(122, "Weltfrieden - woher zu erwarten?"));
            v.Add(new Talk(123, "Warum Christen anders sein müssen"));
            v.Add(new Talk(124, "Worauf sich unser Vertrauen in die göttliche Urheberschaft der Bibel stützt"));
            v.Add(new Talk(125, "Warum die Menschheit ein Lösegeld benötigt"));
            v.Add(new Talk(126, "Wer kann gerettet werden?"));
            v.Add(new Talk(127, "Was geschieht, wenn wir sterben?"));
            v.Add(new Talk(128, "Ist die Hölle wirklich ein Ort feuriger Qual?"));
            v.Add(new Talk(129, "Ist die Dreieinigkeit eine biblische Lehre?"));
            v.Add(new Talk(130, "Die Erde wird für immer bestehenbleiben"));
            v.Add(new Talk(131, "Gibt es wirklich einen Teufel?"));
            v.Add(new Talk(132, "Die Auferstehung - der Sieg über den Tod"));
            v.Add(new Talk(133, "Der Ursprung des Menschen - ist es wichtig, was man glaubt?"));
            v.Add(new Talk(134, "Sollten Christen den Sabbat halten?"));
            v.Add(new Talk(135, "Die Heiligkeit von Leben und Blut"));
            v.Add(new Talk(136, "Ist der Gebrauch von Bildnissen in der Anbetung Gott wohlgefällig?"));
            v.Add(new Talk(137, "Sind die in der Bibel berichteten Wunder wirklich geschehen?"));
            v.Add(new Talk(138, "Mit gesundem Sinn leben in einer verdorbenen Welt"));
            v.Add(new Talk(139, "Göttliche Weisheit in einer wissenschaftlich orientierten Welt"));
            v.Add(new Talk(140, "Jesus Christus - wer er wirklich ist"));
            v.Add(new Talk(141, "Das Seufzen der Menschheit - wann wird es enden?"));
            v.Add(new Talk(142, "Warum sollten wir bei Jehova Zuflucht suchen?"));
            v.Add(new Talk(143, "Auf den Gott allen Trostes vertrauen"));
            v.Add(new Talk(144, "Eine loyale Versammlung unter der Führung Christi"));
            v.Add(new Talk(145, "Wer ist wie Jehova, unser Gott?"));
            v.Add(new Talk(146, "Bildung zum Lobpreis Jehovas nutzen"));
            v.Add(new Talk(147, "Auf die rettende Macht Jehovas vertrauen"));
            v.Add(new Talk(148, "Teilen wir Gottes Ansicht über das Leben?"));
            v.Add(new Talk(149, "Wandeln wir mit Gott?"));
            v.Add(new Talk(150, "Ist die heutige Welt zum Untergang verurteilt? "));
            v.Add(new Talk(151, "Jehova ist seinem Volk \"eine sichere Höhe\""));
            v.Add(new Talk(152, "Das wahre Harmagedon - Warum und wann?"));
            v.Add(new Talk(153, "Den \"furchteinflößenden Tag\" fest im Sinn behalten"));
            v.Add(new Talk(154, "Die Menschenherrschaft - auf der Waage gewogen"));
            v.Add(new Talk(155, "Ist die Stunde des Gerichts für Babylon gekommen?"));
            v.Add(new Talk(156, "Der Gerichtstag - ein Anlass zur Furcht oder zur Hoffnung?"));
            v.Add(new Talk(157, "Wahre Christen lassen Gottes Lehren anziehend wirken"));
            v.Add(new Talk(158, "Sei mutig und vertraue auf Jehova "));
            v.Add(new Talk(159, "In einer gefährlichen Welt Sicherheit finden"));
            v.Add(new Talk(160, "Die christliche Identität bewahren"));
            v.Add(new Talk(161, "Warum nahm Jesus Leid und Tod auf sich?"));
            v.Add(new Talk(162, "Befreiung aus einer finsteren Welt"));
            v.Add(new Talk(163, "Warum sollten wir den wahren Gott fürchten?"));
            v.Add(new Talk(164, "Ist Gott noch Herr der Lage?"));
            v.Add(new Talk(165, "Wessen Wertvorstellungen teilen wir?"));
            v.Add(new Talk(166, "Mit Glauben und Mut in die Zukunft blicken"));
            v.Add(new Talk(167, "Vernünftig handeln in einer unvernünftigen Welt"));
            v.Add(new Talk(168, "Sicherheit in einer unruhigen Welt"));
            v.Add(new Talk(169, "Warum sich von der Bibel leiten lassen?"));
            v.Add(new Talk(170, "Wer eignet sich, die Menschheit zu regieren?"));
            v.Add(new Talk(171, "Wir können schon heute in Frieden leben - und für alle Zeit!"));
            v.Add(new Talk(172, "In welchem Ruf stehen wir bei Gott?"));
            v.Add(new Talk(173, "Gibt es vom Standpunkt Gottes aus eine wahre Religion?"));
            v.Add(new Talk(174, "Gottes neue Welt - wer darf darin leben?"));
            v.Add(new Talk(175, "Was kennzeichnet die Bibel als glaubwürdig?"));
            v.Add(new Talk(176, "Wann wird es echten Frieden und echte Sicherheit geben?"));
            v.Add(new Talk(177, "Wo finden wir in schwierigen Zeiten Hilfe?"));
            v.Add(new Talk(178, "Ein gottergebenes Leben führen"));
            v.Add(new Talk(179, "Auf Gottes Königreich bauen - nicht auf Illusionen"));
            v.Add(new Talk(180, "Warum die Auferstehung für uns eine Realität sein sollte"));
            v.Add(new Talk(181, "Ist es später, als wir denken?"));
            v.Add(new Talk(182, "Was das Reich Gottes schon heute für uns tut"));
            v.Add(new Talk(183, "Den Blick von wertlosen Dingen abwenden"));
            v.Add(new Talk(184, "Ist mit dem Tod alles vorbei?"));
            v.Add(new Talk(185, "Was bewirkt die Wahrheit in unserem Leben?"));
            v.Add(new Talk(186, "Schließe dich Gottes glücklichem Volk an!"));
            v.Add(new Talk(187, "Warum läßt ein liebevoller Gott das Böse zu?"));
            v.Add(new Talk(188, "Vertrauen wir voller Zuversicht auf Jehova?"));
            v.Add(new Talk(189, "Mit Gott zu wandeln bringt Segen - jetzt und für immer"));
            v.Add(new Talk(190, "Vollkommenes Familienglück ist verheißen"));
            v.Add(new Talk(191, "Wie Liebe und Glaube die Welt besiegen"));
            v.Add(new Talk(192, "Bist du auf dem Weg zum ewigen Leben?"));
            v.Add(new Talk(193, "Befreiung aus der Weltbedrängnis"));
            v.Add(new Talk(194, "Wie göttliche Weisheit uns nützt"));
            v.Add(new Talk(-1, "Unbekannt", false));
            v.Add(new Talk(-24, "Was Gottes Herrschaft für uns bewirken kann", false));

            return v;
        }

        public static void Update()
        {
            Log.Info(nameof(Update));

            if (DataContainer.Version < 3)
            {
                TalkList.Add(56, "Wessen Führung kannst du vertrauen?");
                TalkList.Add(-1, "Unbekannt", false);
            }

            if (DataContainer.Version < 11)
            {
                // => IoSqlite.UpdateDatabase(); -- Bestehender Vortrag 24 auf -24 geändert
                TalkList.Add(24, "„Eine besonders kostbare Perle“ – habe ich sie gefunden?");
            }

            if (DataContainer.Version < 15)
            {
                DataContainer.AufgabenPersonZuordnung.Add(new AufgabenZuordnung(-1) { PersonName = "Nicht Vorgesehen", IsVorsitz = true, IsLeser = true, Häufigkeit=1 });
            }
            //auf aktuellste Version setzen = 15
            DataContainer.Version = Helper.CurrentVersion;
        }
    }
}