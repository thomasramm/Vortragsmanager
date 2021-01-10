using Vortragsmanager.Datamodels;
using Vortragsmanager.Views;
using System.Linq;

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
            DataContainer.TalkClear();
            DataContainer.TalkAdd(1, "Wie gut kennst du Gott?");
            DataContainer.TalkAdd(2, "Die letzten Tage: Wer wird sie überleben?");
            DataContainer.TalkAdd(3, "Mit Jehovas vereinter Organisation weiter Richtung Ewigkeit");
            DataContainer.TalkAdd(4, "Beweise für die Existenz Gottes wahrnehmen");
            DataContainer.TalkAdd(5, "Als Familie glücklich sein");
            DataContainer.TalkAdd(6, "Die Sintflut - mehr als eine Geschichte");
            DataContainer.TalkAdd(7, "Den \"Vater inniger Erbarmungen\" nachahmen");
            DataContainer.TalkAdd(8, "Für Gott und nicht für sich selbst leben");
            DataContainer.TalkAdd(9, "Auf Gottes Wort hören und danach handeln");
            DataContainer.TalkAdd(10, "In all unserem Handeln ehrlich sein");
            DataContainer.TalkAdd(11, "Nachahmer Christi sind \"kein Teil der Welt\"");
            DataContainer.TalkAdd(12, "Autorität – Ist es Gott wichtig wie du darüber denkst?");
            DataContainer.TalkAdd(13, "Gottes Standpunkt zu Sexualität und Ehe");
            DataContainer.TalkAdd(14, "Ein reines Volk ehrt Jehova");
            DataContainer.TalkAdd(15, "\"Gegenüber allen das Gute wirken\"");
            DataContainer.TalkAdd(16, "Vertiefe weiterhin dein Verhältnis zu Gott ");
            DataContainer.TalkAdd(17, "Gott verherrlichen mit allem, was wir haben");
            DataContainer.TalkAdd(18, "Mache Jehova zu deiner Festung");
            DataContainer.TalkAdd(19, "Wie kann man erfahren, was in Zukunft geschieht?");
            DataContainer.TalkAdd(20, "Ist für Gott die Zeit gekommen, die Welt zu regieren?");
            DataContainer.TalkAdd(21, "Das Vorrecht schätzen, zu Gottes Königreich zu gehören");
            DataContainer.TalkAdd(22, "Ziehen wir vollen Nutzen aus allem, wofür Jehova sorgt?");
            DataContainer.TalkAdd(23, "Unser Leben hat einen Sinn");
            DataContainer.TalkAdd(-24, "Was Gottes Herrschaft für uns bewirken kann", false);
            DataContainer.TalkAdd(24, "„Eine besonders kostbare Perle“ – habe ich sie gefunden?");
            DataContainer.TalkAdd(27, "Ein guter Anfang für die Ehe");
            DataContainer.TalkAdd(28, "In der Ehe Respekt und Liebe bekunden");
            DataContainer.TalkAdd(29, "Eltern sein - eine dankbare, aber verantwortungsvolle Aufgabe");
            DataContainer.TalkAdd(30, "Gedankenaustausch - in der Familie und mit Gott");
            DataContainer.TalkAdd(31, "Brauche ich Gott in meinem Leben? ");
            DataContainer.TalkAdd(32, "Wie man mit den Sorgen des Lebens fertig wird");
            DataContainer.TalkAdd(33, "Was verbirgt sich hinter dem Geist der Rebellion?");
            DataContainer.TalkAdd(35, "Kannst du und wirst du ewig leben?");
            DataContainer.TalkAdd(36, "Ist mit dem jetzigen Leben alles vorbei?");
            DataContainer.TalkAdd(37, "Bringen Gottes Wege uns wirklich weiter?");
            DataContainer.TalkAdd(38, "Weise handeln, während das Ende herannaht");
            DataContainer.TalkAdd(39, "Blicke dem göttlichen Sieg mit Zuversicht entgegen!");
            DataContainer.TalkAdd(41, "\"Bleibt stehen und seht die Rettung Jehovas\"");
            DataContainer.TalkAdd(42, "Wie wirkt sich Gottes Königreich auf uns aus?");
            DataContainer.TalkAdd(43, "Tust du, was Gott von dir verlangt?");
            DataContainer.TalkAdd(44, "Sucht fortgesetzt Gottes Königreich");
            DataContainer.TalkAdd(45, "Gehe den Weg, der zu ewigem Leben führt");
            DataContainer.TalkAdd(46, "Halte standhaft bis zum Ende an deiner Zuversicht fest");
            DataContainer.TalkAdd(48, "Beweise deine Loyalität als Christ");
            DataContainer.TalkAdd(49, "Eine gereinigte Erde - wirst du sie erleben?");
            DataContainer.TalkAdd(50, "Wie wirst du dich entscheiden?");
            DataContainer.TalkAdd(51, "Verändert die Wahrheit dein Leben?");
            DataContainer.TalkAdd(52, "Wer ist dein Gott?");
            DataContainer.TalkAdd(53, "Stimmst du in deinem Denken mit Gott überein?");
            DataContainer.TalkAdd(54, "Stärke deinen Glauben an den Schöpfer des Menschen");
            DataContainer.TalkAdd(55, "Was für einen Namen machst du dir bei Gott?");
            DataContainer.TalkAdd(56, "Wessen Führung kannst du vertrauen?");
            DataContainer.TalkAdd(57, "Unter Verfolgung standhalten");
            DataContainer.TalkAdd(58, "Was macht Christen zu wahren Christen?");
            DataContainer.TalkAdd(59, "Du wirst ernten, was du säst");
            DataContainer.TalkAdd(60, "Wie sinnvoll ist dein Leben?");
            DataContainer.TalkAdd(61, "Auf wessen Versprechungen vertraust du?");
            DataContainer.TalkAdd(62, "Das einzige Heilmittel für die kranke Menschheit");
            DataContainer.TalkAdd(63, "Hast du den Geist eines Evangeliumsverkündigers?");
            DataContainer.TalkAdd(64, "Liebst du Vergnügungen mehr als Gott?");
            DataContainer.TalkAdd(65, "Frieden fördern in einer Welt voller Wut");
            DataContainer.TalkAdd(66, "Dient als Sklaven für den Herrn der Ernte");
            DataContainer.TalkAdd(67, "Nimm dir Zeit, über geistige Dinge nachzusinnen");
            DataContainer.TalkAdd(68, "Hegst du Groll oder vergibst du?");
            DataContainer.TalkAdd(69, "Den Geist der Selbstaufopferung beleben");
            DataContainer.TalkAdd(70, "Mache Jehova zu deiner Zuversicht");
            DataContainer.TalkAdd(71, "Wie man geistig wach bleibt");
            DataContainer.TalkAdd(72, "Liebe - das Kennzeichen der wahren Christenversammlung");
            DataContainer.TalkAdd(73, "Ein \"Herz der Weisheit\" erwerben");
            DataContainer.TalkAdd(74, "Jehovas Augen sind auf uns gerichtet");
            DataContainer.TalkAdd(75, "Erkennst du Jehovas Souveränität in deinem eigenen Leben an?");
            DataContainer.TalkAdd(76, "Biblische Grundsätze - eine Hilfe bei heutigen Problemen?");
            DataContainer.TalkAdd(77, "Folgt dem Weg der Gastfreundschaft ");
            DataContainer.TalkAdd(78, "Diene Jehova mit einem freudigem Herzen");
            DataContainer.TalkAdd(79, "Freundschaft mit Gott oder mit der Welt - wofür entscheidest du dich?");
            DataContainer.TalkAdd(80, "Stützt sich deine Hoffnung auf die Wissenschaft oder auf die Bibel?");
            DataContainer.TalkAdd(81, "Wer ist befähigt, Gottes Diener zu sein?");
            DataContainer.TalkAdd(82, "Jehova und Christus - sind sie Teil einer Dreieinigkeit?");
            DataContainer.TalkAdd(83, "Die Gerichtszeit für die Religion");
            DataContainer.TalkAdd(84, "Wirst du dem Geschick dieser Welt entgehen?");
            DataContainer.TalkAdd(85, "Eine gute Botschaft in einer gewalttätigen Welt");
            DataContainer.TalkAdd(86, "Gebete, die von Gott erhört werden");
            DataContainer.TalkAdd(87, "Welches Verhältnis hast du zu Gott?");
            DataContainer.TalkAdd(88, "Warum nach biblischen Maßstäben leben?");
            DataContainer.TalkAdd(89, "Kommt, die ihr nach der Wahrheit dürstet!");
            DataContainer.TalkAdd(90, "Ergreife das wirkliche Leben! ");
            DataContainer.TalkAdd(91, "Die Gegenwart des Messias und seine Herrschaft");
            DataContainer.TalkAdd(92, "Die Rolle der Religion im Weltgeschehen");
            DataContainer.TalkAdd(93, "Eingriffe Gottes - woran wirklich zu erkennen?");
            DataContainer.TalkAdd(94, "Die wahre Religion stillt die Bedürfnisse der menschlichen Gesellschaft");
            DataContainer.TalkAdd(95, "Was die Bibel über spiritistische Bräuche sagt");
            DataContainer.TalkAdd(96, "Welche Zukunft hat die Religion? ");
            DataContainer.TalkAdd(97, "Inmitten einer verkehrten Generation untadelig bleiben");
            DataContainer.TalkAdd(98, "\"Die Szene dieser Welt wechselt\"");
            DataContainer.TalkAdd(99, "Warum man der Bibel vertrauen kann");
            DataContainer.TalkAdd(100, "Wahre Freundschaft mit Gott und dem Nächsten");
            DataContainer.TalkAdd(101, "Jehova - der große Schöpfer");
            DataContainer.TalkAdd(102, "Dem prophetischen Wort Aufmerksamkeit schenken");
            DataContainer.TalkAdd(103, "Wie man im Dienst für Gott Freude finden kann");
            DataContainer.TalkAdd(104, "Ihr Eltern, baut ihr mit feuerbeständigem Material?");
            DataContainer.TalkAdd(105, "In all unseren Drangsalen Trost finden");
            DataContainer.TalkAdd(106, "Die Zerstörung der Erde wird von Gott bestraft");
            DataContainer.TalkAdd(107, "In einer sündigen Welt ein gutes Gewissen bewahren");
            DataContainer.TalkAdd(108, "Die Angst vor der Zukunft überwinden");
            DataContainer.TalkAdd(109, "Das Königreich Gottes ist nahe");
            DataContainer.TalkAdd(110, "Gott steht in einer glücklichen Familie an erster Stelle");
            DataContainer.TalkAdd(111, "Was wird durch die Heilung der Nationen erreicht?");
            DataContainer.TalkAdd(112, "Wie man in einer gesetzlosen Welt Liebe bekundet");
            DataContainer.TalkAdd(113, "Wie können Jugendliche die heutige Krisensituation meistern?");
            DataContainer.TalkAdd(114, "Die Wunder der Schöpfung Gottes würdigen");
            DataContainer.TalkAdd(115, "Wie man Satans Schlingen meidet");
            DataContainer.TalkAdd(116, "Sei wählerisch in deinem Umgang");
            DataContainer.TalkAdd(117, "Wie man das Böse mit dem Guten besiegen kann");
            DataContainer.TalkAdd(118, "Jugendlichen gegenüber so eingestellt sein wie Jehova");
            DataContainer.TalkAdd(119, "Von welchem Nutzen es für Christen ist, sich von der Welt getrennt zu halten");
            DataContainer.TalkAdd(120, "Warum sich jetzt Gottes Herrschaft unterwerfen");
            DataContainer.TalkAdd(121, "Eine weltweite Bruderschaft in einer Zeit des Unheils bewahrt");
            DataContainer.TalkAdd(122, "Weltfrieden - woher zu erwarten?");
            DataContainer.TalkAdd(123, "Warum Christen anders sein müssen");
            DataContainer.TalkAdd(124, "Worauf sich unser Vertrauen in die göttliche Urheberschaft der Bibel stützt");
            DataContainer.TalkAdd(125, "Warum die Menschheit ein Lösegeld benötigt");
            DataContainer.TalkAdd(126, "Wer kann gerettet werden?");
            DataContainer.TalkAdd(127, "Was geschieht, wenn wir sterben?");
            DataContainer.TalkAdd(128, "Ist die Hölle wirklich ein Ort feuriger Qual?");
            DataContainer.TalkAdd(129, "Ist die Dreieinigkeit eine biblische Lehre?");
            DataContainer.TalkAdd(130, "Die Erde wird für immer bestehenbleiben");
            DataContainer.TalkAdd(131, "Gibt es wirklich einen Teufel?");
            DataContainer.TalkAdd(132, "Die Auferstehung - der Sieg über den Tod");
            DataContainer.TalkAdd(133, "Der Ursprung des Menschen - ist es wichtig, was man glaubt?");
            DataContainer.TalkAdd(134, "Sollten Christen den Sabbat halten?");
            DataContainer.TalkAdd(135, "Die Heiligkeit von Leben und Blut");
            DataContainer.TalkAdd(136, "Ist der Gebrauch von Bildnissen in der Anbetung Gott wohlgefällig?");
            DataContainer.TalkAdd(137, "Sind die in der Bibel berichteten Wunder wirklich geschehen?");
            DataContainer.TalkAdd(138, "Mit gesundem Sinn leben in einer verdorbenen Welt");
            DataContainer.TalkAdd(139, "Göttliche Weisheit in einer wissenschaftlich orientierten Welt");
            DataContainer.TalkAdd(140, "Jesus Christus - wer er wirklich ist");
            DataContainer.TalkAdd(141, "Das Seufzen der Menschheit - wann wird es enden?");
            DataContainer.TalkAdd(142, "Warum sollten wir bei Jehova Zuflucht suchen?");
            DataContainer.TalkAdd(143, "Auf den Gott allen Trostes vertrauen");
            DataContainer.TalkAdd(144, "Eine loyale Versammlung unter der Führung Christi");
            DataContainer.TalkAdd(145, "Wer ist wie Jehova, unser Gott?");
            DataContainer.TalkAdd(146, "Bildung zum Lobpreis Jehovas nutzen");
            DataContainer.TalkAdd(147, "Auf die rettende Macht Jehovas vertrauen");
            DataContainer.TalkAdd(148, "Teilen wir Gottes Ansicht über das Leben?");
            DataContainer.TalkAdd(149, "Wandeln wir mit Gott?");
            DataContainer.TalkAdd(150, "Ist die heutige Welt zum Untergang verurteilt? ");
            DataContainer.TalkAdd(151, "Jehova ist seinem Volk \"eine sichere Höhe\"");
            DataContainer.TalkAdd(152, "Das wahre Harmagedon - Warum und wann?");
            DataContainer.TalkAdd(153, "Den \"furchteinflößenden Tag\" fest im Sinn behalten");
            DataContainer.TalkAdd(154, "Die Menschenherrschaft - auf der Waage gewogen");
            DataContainer.TalkAdd(155, "Ist die Stunde des Gerichts für Babylon gekommen?");
            DataContainer.TalkAdd(156, "Der Gerichtstag - ein Anlass zur Furcht oder zur Hoffnung?");
            DataContainer.TalkAdd(157, "Wahre Christen lassen Gottes Lehren anziehend wirken");
            DataContainer.TalkAdd(158, "Sei mutig und vertraue auf Jehova ");
            DataContainer.TalkAdd(159, "In einer gefährlichen Welt Sicherheit finden");
            DataContainer.TalkAdd(160, "Die christliche Identität bewahren");
            DataContainer.TalkAdd(161, "Warum nahm Jesus Leid und Tod auf sich?");
            DataContainer.TalkAdd(162, "Befreiung aus einer finsteren Welt");
            DataContainer.TalkAdd(163, "Warum sollten wir den wahren Gott fürchten?");
            DataContainer.TalkAdd(164, "Ist Gott noch Herr der Lage?");
            DataContainer.TalkAdd(165, "Wessen Wertvorstellungen teilen wir?");
            DataContainer.TalkAdd(166, "Mit Glauben und Mut in die Zukunft blicken");
            DataContainer.TalkAdd(167, "Vernünftig handeln in einer unvernünftigen Welt");
            DataContainer.TalkAdd(168, "Sicherheit in einer unruhigen Welt");
            DataContainer.TalkAdd(169, "Warum sich von der Bibel leiten lassen?");
            DataContainer.TalkAdd(170, "Wer eignet sich, die Menschheit zu regieren?");
            DataContainer.TalkAdd(171, "Wir können schon heute in Frieden leben - und für alle Zeit!");
            DataContainer.TalkAdd(172, "In welchem Ruf stehen wir bei Gott?");
            DataContainer.TalkAdd(173, "Gibt es vom Standpunkt Gottes aus eine wahre Religion?");
            DataContainer.TalkAdd(174, "Gottes neue Welt - wer darf darin leben?");
            DataContainer.TalkAdd(175, "Was kennzeichnet die Bibel als glaubwürdig?");
            DataContainer.TalkAdd(176, "Wann wird es echten Frieden und echte Sicherheit geben?");
            DataContainer.TalkAdd(177, "Wo finden wir in schwierigen Zeiten Hilfe?");
            DataContainer.TalkAdd(178, "Ein gottergebenes Leben führen");
            DataContainer.TalkAdd(179, "Auf Gottes Königreich bauen - nicht auf Illusionen");
            DataContainer.TalkAdd(180, "Warum die Auferstehung für uns eine Realität sein sollte");
            DataContainer.TalkAdd(181, "Ist es später, als wir denken?");
            DataContainer.TalkAdd(182, "Was das Reich Gottes schon heute für uns tut");
            DataContainer.TalkAdd(183, "Den Blick von wertlosen Dingen abwenden");
            DataContainer.TalkAdd(184, "Ist mit dem Tod alles vorbei?");
            DataContainer.TalkAdd(185, "Was bewirkt die Wahrheit in unserem Leben?");
            DataContainer.TalkAdd(186, "Schließe dich Gottes glücklichem Volk an!");
            DataContainer.TalkAdd(187, "Warum läßt ein liebevoller Gott das Böse zu?");
            DataContainer.TalkAdd(188, "Vertrauen wir voller Zuversicht auf Jehova?");
            DataContainer.TalkAdd(189, "Mit Gott zu wandeln bringt Segen - jetzt und für immer");
            DataContainer.TalkAdd(190, "Vollkommenes Familienglück ist verheißen");
            DataContainer.TalkAdd(191, "Wie Liebe und Glaube die Welt besiegen");
            DataContainer.TalkAdd(192, "Bist du auf dem Weg zum ewigen Leben?");
            DataContainer.TalkAdd(193, "Befreiung aus der Weltbedrängnis");
            DataContainer.TalkAdd(194, "Wie göttliche Weisheit uns nützt");
            DataContainer.TalkAdd(-1, "Unbekannt", false);
        }

        public static void Update()
        {
            Log.Info(nameof(Update));

            if (DataContainer.Version < 3)
            {
                DataContainer.TalkAdd(56, "Wessen Führung kannst du vertrauen?");
                DataContainer.TalkAdd(-1, "Unbekannt", false);
            }

            if (DataContainer.Version < 11)
            {
                // => IoSqlite.UpdateDatabase(); -- Bestehender Vortrag 24 auf -24 geändert
                DataContainer.TalkAdd(24, "„Eine besonders kostbare Perle“ – habe ich sie gefunden?");
            }

            //auf aktuellste Version setzen
            DataContainer.Version = Helper.CurrentVersion;
        }
    }
}