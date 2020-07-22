using Vortragsmanager.Models;
using Vortragsmanager.Views;

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
            DataContainer.Vorträge.Clear();
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
            Log.Info(nameof(LoadTalks));
            DataContainer.Vorträge.Clear();
            var l = DataContainer.Vorträge;
            l.Add(new Talk(1, "Wie gut kennst du Gott?"));
            l.Add(new Talk(2, "Die letzten Tage: Wer wird sie überleben?"));
            l.Add(new Talk(3, "Mit Jehovas vereinter Organisation weiter Richtung Ewigkeit"));
            l.Add(new Talk(4, "Beweise für die Existenz Gottes wahrnehmen"));
            l.Add(new Talk(5, "Als Familie glücklich sein"));
            l.Add(new Talk(6, "Die Sintflut - mehr als eine Geschichte"));
            l.Add(new Talk(7, "Den \"Vater inniger Erbarmungen\" nachahmen"));
            l.Add(new Talk(8, "Für Gott und nicht für sich selbst leben"));
            l.Add(new Talk(9, "Auf Gottes Wort hören und danach handeln"));
            l.Add(new Talk(10, "In all unserem Handeln ehrlich sein"));
            l.Add(new Talk(11, "Nachahmer Christi sind \"kein Teil der Welt\""));
            l.Add(new Talk(12, "Autorität – Ist es Gott wichtig wie du darüber denkst?"));
            l.Add(new Talk(13, "Gottes Standpunkt zu Sexualität und Ehe"));
            l.Add(new Talk(14, "Ein reines Volk ehrt Jehova"));
            l.Add(new Talk(15, "\"Gegenüber allen das Gute wirken\""));
            l.Add(new Talk(16, "Vertiefe weiterhin dein Verhältnis zu Gott "));
            l.Add(new Talk(17, "Gott verherrlichen mit allem, was wir haben"));
            l.Add(new Talk(18, "Mache Jehova zu deiner Festung"));
            l.Add(new Talk(19, "Wie kann man erfahren, was in Zukunft geschieht?"));
            l.Add(new Talk(20, "Ist für Gott die Zeit gekommen, die Welt zu regieren?"));
            l.Add(new Talk(21, "Das Vorrecht schätzen, zu Gottes Königreich zu gehören"));
            l.Add(new Talk(22, "Ziehen wir vollen Nutzen aus allem, wofür Jehova sorgt?"));
            l.Add(new Talk(23, "Unser Leben hat einen Sinn"));
            l.Add(new Talk(24, "Was Gottes Herrschaft für uns bewirken kann"));
            l.Add(new Talk(27, "Ein guter Anfang für die Ehe"));
            l.Add(new Talk(28, "In der Ehe Respekt und Liebe bekunden"));
            l.Add(new Talk(29, "Eltern sein - eine dankbare, aber verantwortungsvolle Aufgabe"));
            l.Add(new Talk(30, "Gedankenaustausch - in der Familie und mit Gott"));
            l.Add(new Talk(31, "Brauche ich Gott in meinem Leben? "));
            l.Add(new Talk(32, "Wie man mit den Sorgen des Lebens fertig wird"));
            l.Add(new Talk(33, "Was verbirgt sich hinter dem Geist der Rebellion?"));
            l.Add(new Talk(35, "Kannst du und wirst du ewig leben?"));
            l.Add(new Talk(36, "Ist mit dem jetzigen Leben alles vorbei?"));
            l.Add(new Talk(37, "Bringen Gottes Wege uns wirklich weiter?"));
            l.Add(new Talk(38, "Weise handeln, während das Ende herannaht"));
            l.Add(new Talk(39, "Blicke dem göttlichen Sieg mit Zuversicht entgegen!"));
            l.Add(new Talk(41, "\"Bleibt stehen und seht die Rettung Jehovas\""));
            l.Add(new Talk(42, "Wie wirkt sich Gottes Königreich auf uns aus?"));
            l.Add(new Talk(43, "Tust du, was Gott von dir verlangt?"));
            l.Add(new Talk(44, "Sucht fortgesetzt Gottes Königreich"));
            l.Add(new Talk(45, "Gehe den Weg, der zu ewigem Leben führt"));
            l.Add(new Talk(46, "Halte standhaft bis zum Ende an deiner Zuversicht fest"));
            l.Add(new Talk(48, "Beweise deine Loyalität als Christ"));
            l.Add(new Talk(49, "Eine gereinigte Erde - wirst du sie erleben?"));
            l.Add(new Talk(50, "Wie wirst du dich entscheiden?"));
            l.Add(new Talk(51, "Verändert die Wahrheit dein Leben?"));
            l.Add(new Talk(52, "Wer ist dein Gott?"));
            l.Add(new Talk(53, "Stimmst du in deinem Denken mit Gott überein?"));
            l.Add(new Talk(54, "Stärke deinen Glauben an den Schöpfer des Menschen"));
            l.Add(new Talk(55, "Was für einen Namen machst du dir bei Gott?"));
            l.Add(new Talk(56, "Wessen Führung kannst du vertrauen?"));
            l.Add(new Talk(57, "Unter Verfolgung standhalten"));
            l.Add(new Talk(58, "Was macht Christen zu wahren Christen?"));
            l.Add(new Talk(59, "Du wirst ernten, was du säst"));
            l.Add(new Talk(60, "Wie sinnvoll ist dein Leben?"));
            l.Add(new Talk(61, "Auf wessen Versprechungen vertraust du?"));
            l.Add(new Talk(62, "Das einzige Heilmittel für die kranke Menschheit"));
            l.Add(new Talk(63, "Hast du den Geist eines Evangeliumsverkündigers?"));
            l.Add(new Talk(64, "Liebst du Vergnügungen mehr als Gott?"));
            l.Add(new Talk(65, "Frieden fördern in einer Welt voller Wut"));
            l.Add(new Talk(66, "Dient als Sklaven für den Herrn der Ernte"));
            l.Add(new Talk(67, "Nimm dir Zeit, über geistige Dinge nachzusinnen"));
            l.Add(new Talk(68, "Hegst du Groll oder vergibst du?"));
            l.Add(new Talk(69, "Den Geist der Selbstaufopferung beleben"));
            l.Add(new Talk(70, "Mache Jehova zu deiner Zuversicht"));
            l.Add(new Talk(71, "Wie man geistig wach bleibt"));
            l.Add(new Talk(72, "Liebe - das Kennzeichen der wahren Christenversammlung"));
            l.Add(new Talk(73, "Ein \"Herz der Weisheit\" erwerben"));
            l.Add(new Talk(74, "Jehovas Augen sind auf uns gerichtet"));
            l.Add(new Talk(75, "Erkennst du Jehovas Souveränität in deinem eigenen Leben an?"));
            l.Add(new Talk(76, "Biblische Grundsätze - eine Hilfe bei heutigen Problemen?"));
            l.Add(new Talk(77, "Folgt dem Weg der Gastfreundschaft "));
            l.Add(new Talk(78, "Diene Jehova mit einem freudigem Herzen"));
            l.Add(new Talk(79, "Freundschaft mit Gott oder mit der Welt - wofür entscheidest du dich?"));
            l.Add(new Talk(80, "Stützt sich deine Hoffnung auf die Wissenschaft oder auf die Bibel?"));
            l.Add(new Talk(81, "Wer ist befähigt, Gottes Diener zu sein?"));
            l.Add(new Talk(82, "Jehova und Christus - sind sie Teil einer Dreieinigkeit?"));
            l.Add(new Talk(83, "Die Gerichtszeit für die Religion"));
            l.Add(new Talk(84, "Wirst du dem Geschick dieser Welt entgehen?"));
            l.Add(new Talk(85, "Eine gute Botschaft in einer gewalttätigen Welt"));
            l.Add(new Talk(86, "Gebete, die von Gott erhört werden"));
            l.Add(new Talk(87, "Welches Verhältnis hast du zu Gott?"));
            l.Add(new Talk(88, "Warum nach biblischen Maßstäben leben?"));
            l.Add(new Talk(89, "Kommt, die ihr nach der Wahrheit dürstet!"));
            l.Add(new Talk(90, "Ergreife das wirkliche Leben! "));
            l.Add(new Talk(91, "Die Gegenwart des Messias und seine Herrschaft"));
            l.Add(new Talk(92, "Die Rolle der Religion im Weltgeschehen"));
            l.Add(new Talk(93, "Eingriffe Gottes - woran wirklich zu erkennen?"));
            l.Add(new Talk(94, "Die wahre Religion stillt die Bedürfnisse der menschlichen Gesellschaft"));
            l.Add(new Talk(95, "Was die Bibel über spiritistische Bräuche sagt"));
            l.Add(new Talk(96, "Welche Zukunft hat die Religion? "));
            l.Add(new Talk(97, "Inmitten einer verkehrten Generation untadelig bleiben"));
            l.Add(new Talk(98, "\"Die Szene dieser Welt wechselt\""));
            l.Add(new Talk(99, "Warum man der Bibel vertrauen kann"));
            l.Add(new Talk(100, "Wahre Freundschaft mit Gott und dem Nächsten"));
            l.Add(new Talk(101, "Jehova - der große Schöpfer"));
            l.Add(new Talk(102, "Dem prophetischen Wort Aufmerksamkeit schenken"));
            l.Add(new Talk(103, "Wie man im Dienst für Gott Freude finden kann"));
            l.Add(new Talk(104, "Ihr Eltern, baut ihr mit feuerbeständigem Material?"));
            l.Add(new Talk(105, "In all unseren Drangsalen Trost finden"));
            l.Add(new Talk(106, "Die Zerstörung der Erde wird von Gott bestraft"));
            l.Add(new Talk(107, "In einer sündigen Welt ein gutes Gewissen bewahren"));
            l.Add(new Talk(108, "Die Angst vor der Zukunft überwinden"));
            l.Add(new Talk(109, "Das Königreich Gottes ist nahe"));
            l.Add(new Talk(110, "Gott steht in einer glücklichen Familie an erster Stelle"));
            l.Add(new Talk(111, "Was wird durch die Heilung der Nationen erreicht?"));
            l.Add(new Talk(112, "Wie man in einer gesetzlosen Welt Liebe bekundet"));
            l.Add(new Talk(113, "Wie können Jugendliche die heutige Krisensituation meistern?"));
            l.Add(new Talk(114, "Die Wunder der Schöpfung Gottes würdigen"));
            l.Add(new Talk(115, "Wie man Satans Schlingen meidet"));
            l.Add(new Talk(116, "Sei wählerisch in deinem Umgang"));
            l.Add(new Talk(117, "Wie man das Böse mit dem Guten besiegen kann"));
            l.Add(new Talk(118, "Jugendlichen gegenüber so eingestellt sein wie Jehova"));
            l.Add(new Talk(119, "Von welchem Nutzen es für Christen ist, sich von der Welt getrennt zu halten"));
            l.Add(new Talk(120, "Warum sich jetzt Gottes Herrschaft unterwerfen"));
            l.Add(new Talk(121, "Eine weltweite Bruderschaft in einer Zeit des Unheils bewahrt"));
            l.Add(new Talk(122, "Weltfrieden - woher zu erwarten?"));
            l.Add(new Talk(123, "Warum Christen anders sein müssen"));
            l.Add(new Talk(124, "Worauf sich unser Vertrauen in die göttliche Urheberschaft der Bibel stützt"));
            l.Add(new Talk(125, "Warum die Menschheit ein Lösegeld benötigt"));
            l.Add(new Talk(126, "Wer kann gerettet werden?"));
            l.Add(new Talk(127, "Was geschieht, wenn wir sterben?"));
            l.Add(new Talk(128, "Ist die Hölle wirklich ein Ort feuriger Qual?"));
            l.Add(new Talk(129, "Ist die Dreieinigkeit eine biblische Lehre?"));
            l.Add(new Talk(130, "Die Erde wird für immer bestehenbleiben"));
            l.Add(new Talk(131, "Gibt es wirklich einen Teufel?"));
            l.Add(new Talk(132, "Die Auferstehung - der Sieg über den Tod"));
            l.Add(new Talk(133, "Der Ursprung des Menschen - ist es wichtig, was man glaubt?"));
            l.Add(new Talk(134, "Sollten Christen den Sabbat halten?"));
            l.Add(new Talk(135, "Die Heiligkeit von Leben und Blut"));
            l.Add(new Talk(136, "Ist der Gebrauch von Bildnissen in der Anbetung Gott wohlgefällig?"));
            l.Add(new Talk(137, "Sind die in der Bibel berichteten Wunder wirklich geschehen?"));
            l.Add(new Talk(138, "Mit gesundem Sinn leben in einer verdorbenen Welt"));
            l.Add(new Talk(139, "Göttliche Weisheit in einer wissenschaftlich orientierten Welt"));
            l.Add(new Talk(140, "Jesus Christus - wer er wirklich ist"));
            l.Add(new Talk(141, "Das Seufzen der Menschheit - wann wird es enden?"));
            l.Add(new Talk(142, "Warum sollten wir bei Jehova Zuflucht suchen?"));
            l.Add(new Talk(143, "Auf den Gott allen Trostes vertrauen"));
            l.Add(new Talk(144, "Eine loyale Versammlung unter der Führung Christi"));
            l.Add(new Talk(145, "Wer ist wie Jehova, unser Gott?"));
            l.Add(new Talk(146, "Bildung zum Lobpreis Jehovas nutzen"));
            l.Add(new Talk(147, "Auf die rettende Macht Jehovas vertrauen"));
            l.Add(new Talk(148, "Teilen wir Gottes Ansicht über das Leben?"));
            l.Add(new Talk(149, "Wandeln wir mit Gott?"));
            l.Add(new Talk(150, "Ist die heutige Welt zum Untergang verurteilt? "));
            l.Add(new Talk(151, "Jehova ist seinem Volk \"eine sichere Höhe\""));
            l.Add(new Talk(152, "Das wahre Harmagedon - Warum und wann?"));
            l.Add(new Talk(153, "Den \"furchteinflößenden Tag\" fest im Sinn behalten"));
            l.Add(new Talk(154, "Die Menschenherrschaft - auf der Waage gewogen"));
            l.Add(new Talk(155, "Ist die Stunde des Gerichts für Babylon gekommen?"));
            l.Add(new Talk(156, "Der Gerichtstag - ein Anlass zur Furcht oder zur Hoffnung?"));
            l.Add(new Talk(157, "Wahre Christen lassen Gottes Lehren anziehend wirken"));
            l.Add(new Talk(158, "Sei mutig und vertraue auf Jehova "));
            l.Add(new Talk(159, "In einer gefährlichen Welt Sicherheit finden"));
            l.Add(new Talk(160, "Die christliche Identität bewahren"));
            l.Add(new Talk(161, "Warum nahm Jesus Leid und Tod auf sich?"));
            l.Add(new Talk(162, "Befreiung aus einer finsteren Welt"));
            l.Add(new Talk(163, "Warum sollten wir den wahren Gott fürchten?"));
            l.Add(new Talk(164, "Ist Gott noch Herr der Lage?"));
            l.Add(new Talk(165, "Wessen Wertvorstellungen teilen wir?"));
            l.Add(new Talk(166, "Mit Glauben und Mut in die Zukunft blicken"));
            l.Add(new Talk(167, "Vernünftig handeln in einer unvernünftigen Welt"));
            l.Add(new Talk(168, "Sicherheit in einer unruhigen Welt"));
            l.Add(new Talk(169, "Warum sich von der Bibel leiten lassen?"));
            l.Add(new Talk(170, "Wer eignet sich, die Menschheit zu regieren?"));
            l.Add(new Talk(171, "Wir können schon heute in Frieden leben - und für alle Zeit!"));
            l.Add(new Talk(172, "In welchem Ruf stehen wir bei Gott?"));
            l.Add(new Talk(173, "Gibt es vom Standpunkt Gottes aus eine wahre Religion?"));
            l.Add(new Talk(174, "Gottes neue Welt - wer darf darin leben?"));
            l.Add(new Talk(175, "Was kennzeichnet die Bibel als glaubwürdig?"));
            l.Add(new Talk(176, "Wann wird es echten Frieden und echte Sicherheit geben?"));
            l.Add(new Talk(177, "Wo finden wir in schwierigen Zeiten Hilfe?"));
            l.Add(new Talk(178, "Ein gottergebenes Leben führen"));
            l.Add(new Talk(179, "Auf Gottes Königreich bauen - nicht auf Illusionen"));
            l.Add(new Talk(180, "Warum die Auferstehung für uns eine Realität sein sollte"));
            l.Add(new Talk(181, "Ist es später, als wir denken?"));
            l.Add(new Talk(182, "Was das Reich Gottes schon heute für uns tut"));
            l.Add(new Talk(183, "Den Blick von wertlosen Dingen abwenden"));
            l.Add(new Talk(184, "Ist mit dem Tod alles vorbei?"));
            l.Add(new Talk(185, "Was bewirkt die Wahrheit in unserem Leben?"));
            l.Add(new Talk(186, "Schließe dich Gottes glücklichem Volk an!"));
            l.Add(new Talk(187, "Warum läßt ein liebevoller Gott das Böse zu?"));
            l.Add(new Talk(188, "Vertrauen wir voller Zuversicht auf Jehova?"));
            l.Add(new Talk(189, "Mit Gott zu wandeln bringt Segen - jetzt und für immer"));
            l.Add(new Talk(190, "Vollkommenes Familienglück ist verheißen"));
            l.Add(new Talk(191, "Wie Liebe und Glaube die Welt besiegen"));
            l.Add(new Talk(192, "Bist du auf dem Weg zum ewigen Leben?"));
            l.Add(new Talk(193, "Befreiung aus der Weltbedrängnis"));
            l.Add(new Talk(194, "Wie göttliche Weisheit uns nützt"));
            l.Add(new Talk(-1, "Unbekannt") { Gültig = false });
        }

        public static void Update()
        {
            Log.Info(nameof(Update));

            if (DataContainer.Version < 3)
            {
                DataContainer.Vorträge.Add(new Talk(56, "Wessen Führung kannst du vertrauen?"));
                DataContainer.Vorträge.Add(new Talk(-1, "Unbekannt") { Gültig = false });
            }

            //auf aktuellste Version setzen
            DataContainer.Version = Helper.CurrentVersion;
        }
    }
}