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
            Log.Info(nameof(LoadTalks));
            TalkList.Clear();
            TalkList.Add(1, "Wie gut kennst du Gott?");
            TalkList.Add(2, "Die letzten Tage: Wer wird sie überleben?");
            TalkList.Add(3, "Mit Jehovas vereinter Organisation weiter Richtung Ewigkeit");
            TalkList.Add(4, "Beweise für die Existenz Gottes wahrnehmen");
            TalkList.Add(5, "Als Familie glücklich sein");
            TalkList.Add(6, "Die Sintflut - mehr als eine Geschichte");
            TalkList.Add(7, "Den \"Vater inniger Erbarmungen\" nachahmen");
            TalkList.Add(8, "Für Gott und nicht für sich selbst leben");
            TalkList.Add(9, "Auf Gottes Wort hören und danach handeln");
            TalkList.Add(10, "In all unserem Handeln ehrlich sein");
            TalkList.Add(11, "Nachahmer Christi sind \"kein Teil der Welt\"");
            TalkList.Add(12, "Autorität – Ist es Gott wichtig wie du darüber denkst?");
            TalkList.Add(13, "Gottes Standpunkt zu Sexualität und Ehe");
            TalkList.Add(14, "Ein reines Volk ehrt Jehova");
            TalkList.Add(15, "\"Gegenüber allen das Gute wirken\"");
            TalkList.Add(16, "Vertiefe weiterhin dein Verhältnis zu Gott ");
            TalkList.Add(17, "Gott verherrlichen mit allem, was wir haben");
            TalkList.Add(18, "Mache Jehova zu deiner Festung");
            TalkList.Add(19, "Wie kann man erfahren, was in Zukunft geschieht?");
            TalkList.Add(20, "Ist für Gott die Zeit gekommen, die Welt zu regieren?");
            TalkList.Add(21, "Das Vorrecht schätzen, zu Gottes Königreich zu gehören");
            TalkList.Add(22, "Ziehen wir vollen Nutzen aus allem, wofür Jehova sorgt?");
            TalkList.Add(23, "Unser Leben hat einen Sinn");
            TalkList.Add(-24, "Was Gottes Herrschaft für uns bewirken kann", false);
            TalkList.Add(24, "„Eine besonders kostbare Perle“ – habe ich sie gefunden?");
            TalkList.Add(27, "Ein guter Anfang für die Ehe");
            TalkList.Add(28, "In der Ehe Respekt und Liebe bekunden");
            TalkList.Add(29, "Eltern sein - eine dankbare, aber verantwortungsvolle Aufgabe");
            TalkList.Add(30, "Gedankenaustausch - in der Familie und mit Gott");
            TalkList.Add(31, "Brauche ich Gott in meinem Leben? ");
            TalkList.Add(32, "Wie man mit den Sorgen des Lebens fertig wird");
            TalkList.Add(33, "Was verbirgt sich hinter dem Geist der Rebellion?");
            TalkList.Add(35, "Kannst du und wirst du ewig leben?");
            TalkList.Add(36, "Ist mit dem jetzigen Leben alles vorbei?");
            TalkList.Add(37, "Bringen Gottes Wege uns wirklich weiter?");
            TalkList.Add(38, "Weise handeln, während das Ende herannaht");
            TalkList.Add(39, "Blicke dem göttlichen Sieg mit Zuversicht entgegen!");
            TalkList.Add(41, "\"Bleibt stehen und seht die Rettung Jehovas\"");
            TalkList.Add(42, "Wie wirkt sich Gottes Königreich auf uns aus?");
            TalkList.Add(43, "Tust du, was Gott von dir verlangt?");
            TalkList.Add(44, "Sucht fortgesetzt Gottes Königreich");
            TalkList.Add(45, "Gehe den Weg, der zu ewigem Leben führt");
            TalkList.Add(46, "Halte standhaft bis zum Ende an deiner Zuversicht fest");
            TalkList.Add(48, "Beweise deine Loyalität als Christ");
            TalkList.Add(49, "Eine gereinigte Erde - wirst du sie erleben?");
            TalkList.Add(50, "Wie wirst du dich entscheiden?");
            TalkList.Add(51, "Verändert die Wahrheit dein Leben?");
            TalkList.Add(52, "Wer ist dein Gott?");
            TalkList.Add(53, "Stimmst du in deinem Denken mit Gott überein?");
            TalkList.Add(54, "Stärke deinen Glauben an den Schöpfer des Menschen");
            TalkList.Add(55, "Was für einen Namen machst du dir bei Gott?");
            TalkList.Add(56, "Wessen Führung kannst du vertrauen?");
            TalkList.Add(57, "Unter Verfolgung standhalten");
            TalkList.Add(58, "Was macht Christen zu wahren Christen?");
            TalkList.Add(59, "Du wirst ernten, was du säst");
            TalkList.Add(60, "Wie sinnvoll ist dein Leben?");
            TalkList.Add(61, "Auf wessen Versprechungen vertraust du?");
            TalkList.Add(62, "Das einzige Heilmittel für die kranke Menschheit");
            TalkList.Add(63, "Hast du den Geist eines Evangeliumsverkündigers?");
            TalkList.Add(64, "Liebst du Vergnügungen mehr als Gott?");
            TalkList.Add(65, "Frieden fördern in einer Welt voller Wut");
            TalkList.Add(66, "Dient als Sklaven für den Herrn der Ernte");
            TalkList.Add(67, "Nimm dir Zeit, über geistige Dinge nachzusinnen");
            TalkList.Add(68, "Hegst du Groll oder vergibst du?");
            TalkList.Add(69, "Den Geist der Selbstaufopferung beleben");
            TalkList.Add(70, "Voll und ganz auf Jehova vertrauen", false);
            TalkList.Add(71, "Wie man geistig wach bleibt");
            TalkList.Add(72, "Liebe - das Kennzeichen der wahren Christenversammlung");
            TalkList.Add(73, "Ein \"Herz der Weisheit\" erwerben");
            TalkList.Add(74, "Jehovas Augen sind auf uns gerichtet");
            TalkList.Add(75, "Erkennst du Jehovas Souveränität in deinem eigenen Leben an?");
            TalkList.Add(76, "Biblische Grundsätze - eine Hilfe bei heutigen Problemen?");
            TalkList.Add(77, "Folgt dem Weg der Gastfreundschaft ");
            TalkList.Add(78, "Diene Jehova mit einem freudigem Herzen");
            TalkList.Add(79, "Freundschaft mit Gott oder mit der Welt - wofür entscheidest du dich?");
            TalkList.Add(80, "Stützt sich deine Hoffnung auf die Wissenschaft oder auf die Bibel?");
            TalkList.Add(81, "Wer ist befähigt, Gottes Diener zu sein?");
            TalkList.Add(82, "Jehova und Christus - sind sie Teil einer Dreieinigkeit?");
            TalkList.Add(83, "Die Gerichtszeit für die Religion");
            TalkList.Add(84, "Wirst du dem Geschick dieser Welt entgehen?");
            TalkList.Add(85, "Eine gute Botschaft in einer gewalttätigen Welt");
            TalkList.Add(86, "Gebete, die von Gott erhört werden");
            TalkList.Add(87, "Welches Verhältnis hast du zu Gott?");
            TalkList.Add(88, "Warum nach biblischen Maßstäben leben?");
            TalkList.Add(89, "Kommt, die ihr nach der Wahrheit dürstet!");
            TalkList.Add(90, "Ergreife das wirkliche Leben! ");
            TalkList.Add(91, "Die Gegenwart des Messias und seine Herrschaft");
            TalkList.Add(92, "Die Rolle der Religion im Weltgeschehen");
            TalkList.Add(93, "Eingriffe Gottes - woran wirklich zu erkennen?");
            TalkList.Add(94, "Die wahre Religion stillt die Bedürfnisse der menschlichen Gesellschaft");
            TalkList.Add(95, "Was die Bibel über spiritistische Bräuche sagt");
            TalkList.Add(96, "Welche Zukunft hat die Religion? ");
            TalkList.Add(97, "Inmitten einer verkehrten Generation untadelig bleiben");
            TalkList.Add(98, "\"Die Szene dieser Welt wechselt\"");
            TalkList.Add(99, "Warum man der Bibel vertrauen kann");
            TalkList.Add(100, "Wahre Freundschaft mit Gott und dem Nächsten");
            TalkList.Add(101, "Jehova - der große Schöpfer");
            TalkList.Add(102, "Dem prophetischen Wort Aufmerksamkeit schenken");
            TalkList.Add(103, "Wie man im Dienst für Gott Freude finden kann");
            TalkList.Add(104, "Ihr Eltern, baut ihr mit feuerbeständigem Material?");
            TalkList.Add(105, "In all unseren Drangsalen Trost finden");
            TalkList.Add(106, "Die Zerstörung der Erde wird von Gott bestraft");
            TalkList.Add(107, "In einer sündigen Welt ein gutes Gewissen bewahren");
            TalkList.Add(108, "Die Angst vor der Zukunft überwinden");
            TalkList.Add(109, "Das Königreich Gottes ist nahe");
            TalkList.Add(110, "Gott steht in einer glücklichen Familie an erster Stelle");
            TalkList.Add(111, "Was wird durch die Heilung der Nationen erreicht?");
            TalkList.Add(112, "Wie man in einer gesetzlosen Welt Liebe bekundet");
            TalkList.Add(113, "Wie können Jugendliche die heutige Krisensituation meistern?");
            TalkList.Add(114, "Die Wunder der Schöpfung Gottes würdigen");
            TalkList.Add(115, "Wie man Satans Schlingen meidet");
            TalkList.Add(116, "Sei wählerisch in deinem Umgang");
            TalkList.Add(117, "Wie man das Böse mit dem Guten besiegen kann");
            TalkList.Add(118, "Jugendlichen gegenüber so eingestellt sein wie Jehova");
            TalkList.Add(119, "Von welchem Nutzen es für Christen ist, sich von der Welt getrennt zu halten");
            TalkList.Add(120, "Warum sich jetzt Gottes Herrschaft unterwerfen");
            TalkList.Add(121, "Eine weltweite Bruderschaft in einer Zeit des Unheils bewahrt");
            TalkList.Add(122, "Weltfrieden - woher zu erwarten?");
            TalkList.Add(123, "Warum Christen anders sein müssen");
            TalkList.Add(124, "Worauf sich unser Vertrauen in die göttliche Urheberschaft der Bibel stützt");
            TalkList.Add(125, "Warum die Menschheit ein Lösegeld benötigt");
            TalkList.Add(126, "Wer kann gerettet werden?");
            TalkList.Add(127, "Was geschieht, wenn wir sterben?");
            TalkList.Add(128, "Ist die Hölle wirklich ein Ort feuriger Qual?");
            TalkList.Add(129, "Ist die Dreieinigkeit eine biblische Lehre?");
            TalkList.Add(130, "Die Erde wird für immer bestehenbleiben");
            TalkList.Add(131, "Gibt es wirklich einen Teufel?");
            TalkList.Add(132, "Die Auferstehung - der Sieg über den Tod");
            TalkList.Add(133, "Der Ursprung des Menschen - ist es wichtig, was man glaubt?");
            TalkList.Add(134, "Sollten Christen den Sabbat halten?");
            TalkList.Add(135, "Die Heiligkeit von Leben und Blut");
            TalkList.Add(136, "Ist der Gebrauch von Bildnissen in der Anbetung Gott wohlgefällig?");
            TalkList.Add(137, "Sind die in der Bibel berichteten Wunder wirklich geschehen?");
            TalkList.Add(138, "Mit gesundem Sinn leben in einer verdorbenen Welt");
            TalkList.Add(139, "Göttliche Weisheit in einer wissenschaftlich orientierten Welt");
            TalkList.Add(140, "Jesus Christus - wer er wirklich ist");
            TalkList.Add(141, "Das Seufzen der Menschheit - wann wird es enden?");
            TalkList.Add(142, "Warum sollten wir bei Jehova Zuflucht suchen?");
            TalkList.Add(143, "Auf den Gott allen Trostes vertrauen");
            TalkList.Add(144, "Eine loyale Versammlung unter der Führung Christi");
            TalkList.Add(145, "Wer ist wie Jehova, unser Gott?");
            TalkList.Add(146, "Bildung zum Lobpreis Jehovas nutzen");
            TalkList.Add(147, "Auf die rettende Macht Jehovas vertrauen");
            TalkList.Add(148, "Teilen wir Gottes Ansicht über das Leben?");
            TalkList.Add(149, "Wandeln wir mit Gott?");
            TalkList.Add(150, "Ist die heutige Welt zum Untergang verurteilt? ");
            TalkList.Add(151, "Jehova ist seinem Volk \"eine sichere Höhe\"");
            TalkList.Add(152, "Das wahre Harmagedon - Warum und wann?");
            TalkList.Add(153, "Den \"furchteinflößenden Tag\" fest im Sinn behalten");
            TalkList.Add(154, "Die Menschenherrschaft - auf der Waage gewogen");
            TalkList.Add(155, "Ist die Stunde des Gerichts für Babylon gekommen?");
            TalkList.Add(156, "Der Gerichtstag - ein Anlass zur Furcht oder zur Hoffnung?");
            TalkList.Add(157, "Wahre Christen lassen Gottes Lehren anziehend wirken");
            TalkList.Add(158, "Sei mutig und vertraue auf Jehova ");
            TalkList.Add(159, "In einer gefährlichen Welt Sicherheit finden");
            TalkList.Add(160, "Die christliche Identität bewahren");
            TalkList.Add(161, "Warum nahm Jesus Leid und Tod auf sich?");
            TalkList.Add(162, "Befreiung aus einer finsteren Welt");
            TalkList.Add(163, "Warum sollten wir den wahren Gott fürchten?");
            TalkList.Add(164, "Ist Gott noch Herr der Lage?");
            TalkList.Add(165, "Wessen Wertvorstellungen teilen wir?");
            TalkList.Add(166, "Mit Glauben und Mut in die Zukunft blicken");
            TalkList.Add(167, "Vernünftig handeln in einer unvernünftigen Welt");
            TalkList.Add(168, "Sicherheit in einer unruhigen Welt");
            TalkList.Add(169, "Warum sich von der Bibel leiten lassen?");
            TalkList.Add(170, "Wer eignet sich, die Menschheit zu regieren?");
            TalkList.Add(171, "Wir können schon heute in Frieden leben - und für alle Zeit!");
            TalkList.Add(172, "In welchem Ruf stehen wir bei Gott?");
            TalkList.Add(173, "Gibt es vom Standpunkt Gottes aus eine wahre Religion?");
            TalkList.Add(174, "Gottes neue Welt - wer darf darin leben?");
            TalkList.Add(175, "Was kennzeichnet die Bibel als glaubwürdig?");
            TalkList.Add(176, "Wann wird es echten Frieden und echte Sicherheit geben?");
            TalkList.Add(177, "Wo finden wir in schwierigen Zeiten Hilfe?");
            TalkList.Add(178, "Ein gottergebenes Leben führen");
            TalkList.Add(179, "Auf Gottes Königreich bauen - nicht auf Illusionen");
            TalkList.Add(180, "Warum die Auferstehung für uns eine Realität sein sollte");
            TalkList.Add(181, "Ist es später, als wir denken?");
            TalkList.Add(182, "Was das Reich Gottes schon heute für uns tut");
            TalkList.Add(183, "Den Blick von wertlosen Dingen abwenden");
            TalkList.Add(184, "Ist mit dem Tod alles vorbei?");
            TalkList.Add(185, "Was bewirkt die Wahrheit in unserem Leben?");
            TalkList.Add(186, "Schließe dich Gottes glücklichem Volk an!");
            TalkList.Add(187, "Warum läßt ein liebevoller Gott das Böse zu?");
            TalkList.Add(188, "Vertrauen wir voller Zuversicht auf Jehova?");
            TalkList.Add(189, "Mit Gott zu wandeln bringt Segen - jetzt und für immer");
            TalkList.Add(190, "Vollkommenes Familienglück ist verheißen");
            TalkList.Add(191, "Wie Liebe und Glaube die Welt besiegen");
            TalkList.Add(192, "Bist du auf dem Weg zum ewigen Leben?");
            TalkList.Add(193, "Befreiung aus der Weltbedrängnis");
            TalkList.Add(194, "Wie göttliche Weisheit uns nützt");
            TalkList.Add(-1, "Unbekannt", false);
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

            //auf aktuellste Version setzen
            DataContainer.Version = Helper.CurrentVersion;
        }
    }
}