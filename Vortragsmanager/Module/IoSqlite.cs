using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using DevExpress.Mvvm;
using Vortragsmanager.Core;
using Vortragsmanager.Datamodels;
using Vortragsmanager.DataModels;
using Vortragsmanager.Enums;
using Vortragsmanager.Helper;
using Vortragsmanager.PageModels;

namespace Vortragsmanager.Module
{
    public static class IoSqlite
    {
        public static void ReadContainer(string file)
        {
            Log.Info(nameof(ReadContainer), file);
            using (SQLiteConnection db = new SQLiteConnection($"Data Source = {file}; Version = 3;"))
            {
                db.Open();

                ReadParameter(db);

                //Falls sich an der Datenstruktur was geändert hat, muss ich erst mal
                //die Struktur aktualisieren bevor ich etwas einlesen kann...
                UpdateDatabase(db);

                ReadVersammlungen(db);
                ReadVorträge(db);
                ReadRedner(db);
                ReadMeinPlan(db);
                ReadExternerPlan(db);
                ReadTemplates(db);
                ReadEvents(db);
                ReadAnfragen(db);
                ReadCancelation(db);
                ReadActivity(db);
                ReadAufgaben(db);
                ReadAbwesenheiten(db);

                TalkList.UpdateDate();
                DataContainer.IsInitialized = true;

                //Falls es jetzt noch weitere Updates gibt die am Container durchgeführt
                //werden müssen, ist hier ein guter Ort...
                Update.Process();

                db.Close();
            }

            Messenger.Default.Send(true, Messages.NewDatabaseOpened);
        }

        public static string SaveContainer(string file, bool createBackup)
        {
            Log.Info(nameof(SaveContainer), $"file={file}, createBackup={createBackup}");
            //Speichern der DB in einer tmp-Datei
            var tempFile = Path.GetTempFileName();
            SQLiteConnection.CreateFile($"{tempFile}");
            //using (SQLiteConnection db = new SQLiteConnection($"Data Source = {tempFile}; Version = 3;PRAGMA locking_mode = EXCLUSIVE;"))
            using (SQLiteConnection db = new SQLiteConnection($"Data Source = {tempFile}; Version = 3;"))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    CreateEmptyDatabase(db);
                    SaveParameter(db);
                    SaveVorträge(db);
                    SaveVersammlungen(db);
                    SaveRedner(db);
                    SaveMeinPlan(db);
                    SaveAnfragen(db);
                    SaveExternerPlan(db);
                    SaveTemplates(db);
                    SaveCancelation(db);
                    SaveActivity(db);
                    SaveAufgaben(db);
                    SaveAbwesenheiten(db);
                    transaction.Commit();
                }
                db.Close();
            }

            //letzte DB mit neuer tmp-Datei überschreiben,
            //bei einem Fehler neuen Dateinamen kreieren
            var newfile = file;
            try
            {
                File.Delete(file);
            }
            catch (Exception)
            {
                var i = 1;
                newfile = file;
                while (File.Exists(newfile))
                {
                    var fi = new FileInfo(file);
                    newfile = Path.Combine(fi.DirectoryName, fi.Name.Replace(fi.Extension, "") + i + fi.Extension);
                    i++;
                }
            }
            File.Move(tempFile, newfile);

            if (createBackup)
            {
                Backup.Add(newfile);
            }
            //Rückgabe des (neuen) Speichernamen
            return newfile;
        }

        public static void CreateEmptyDatabase(string file)
        {
            Log.Info(nameof(CreateEmptyDatabase), file);
            SQLiteConnection.CreateFile(file);
            using (SQLiteConnection db = new SQLiteConnection($"Data Source = {file}; Version = 3;"))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    CreateEmptyDatabase(db);
                    transaction.Commit();
                }
                db.Close();
            }
        }

        private static void CreateEmptyDatabase(SQLiteConnection db)
        {
            Log.Info(nameof(IoSqlite.CreateEmptyDatabase), "connection");

            ExecCommand(@"CREATE TABLE IF NOT EXISTS Parameter (
                Name TEXT,
                Wert TEXT)", db);
            
            ExecCommand(@"CREATE TABLE IF NOT EXISTS Conregation (
                Id INTEGER,
                Kreis INTEGER,
                Name TEXT,
                Anschrift1 TEXT,
                Anschrift2 TEXT,
                Anreise TEXT,
                Entfernung INTEGER,
                Telefon TEXT,
                Koordinator TEXT,
                KoordinatorTelefon TEXT,
                KoordinatorMobil TEXT,
                KoordinatorMail TEXT,
                KoordinatorJw TEXT,
                Zoom TEXT)", db);
            
            ExecCommand(@"CREATE TABLE IF NOT EXISTS Conregation_Zusammenkunftszeiten (
                    IdConregation INTEGER,
                    Jahr INTEGER,
                    Tag INTEGER,
                    Zeit TEXT)", db);
            
            ExecCommand(@"CREATE TABLE IF NOT EXISTS Invitation (
                IdAltester INTEGER,
                IdVortrag INTEGER,
                IdConregation INTEGER,
                Datum INTEGER,
                Status INTEGER,
                LetzteAktion TEXT,
                Kommentar TEXT,
                ErinnerungsmailGesendet INTEGER)", db);
            
            ExecCommand(@"CREATE TABLE IF NOT EXISTS Events (
                Typ INTEGER,
                Name TEXT,
                Thema TEXT,
                VORTRAGENDER TEXT,
                Datum INTEGER,
                IdVortrag INTEGER)", db);
            
            ExecCommand(@"CREATE TABLE IF NOT EXISTS Outside (
                IdSpeaker INTEGER,
                IdConregation INTEGER,
                Datum INTEGER,
                Reason INTEGER,
                IdTalk INTEGER)", db);
            
            ExecCommand(@"CREATE TABLE IF NOT EXISTS Speaker (
                Id INTEGER,
                Name TEXT,
                IdConregation INTEGER,
                Mail TEXT,
                Telefon TEXT,
                Mobil TEXT,
                Altester INTEGER,
                Aktiv INTEGER,
                InfoPrivate TEXT,
                InfoPublic TEXT,
                Einladen INTEGER,
                JwMail TEXT,
                Abstand INTEGER)", db);
            
            ExecCommand(@"CREATE TABLE IF NOT EXISTS Speaker_Vortrag (
                IdSpeaker INTEGER,
                IdTalk INTEGER,
                IdSong1 INTEGER,
                IdSong2 INTEGER)", db);
            
            ExecCommand(@"CREATE TABLE IF NOT EXISTS Talks (
                Nummer INTEGER,
                Thema TEXT,
                Gultig INTEGER,
                ZuletztGehalten INTEGER)", db);
            
            ExecCommand(@"CREATE TABLE IF NOT EXISTS Templates (
                Id INTEGER,
                Inhalt STRING)", db);
            
            ExecCommand(@"CREATE TABLE IF NOT EXISTS Inquiry (
                Id INTEGER,
                IdConregation INTEGER,
                Status INTEGER,
                AnfrageDatum INTEGER,
                Kommentar STRING,
                Mailtext STRING)", db);
            
            ExecCommand(@"CREATE TABLE IF NOT EXISTS Inquiry_Dates (
                IdInquiry INTEGER,
                Datum INTEGER)", db);
            
            ExecCommand(@"CREATE TABLE IF NOT EXISTS Inquiry_SpeakerTalk (
                IdInquiry INTEGER,
                IdSpeaker INTEGER,
                IdTalk INTEGER)", db);
            
            ExecCommand(@"CREATE TABLE IF NOT EXISTS Cancelation (
                Datum INTEGER,
                IdSpeaker INTEGER,
                IdLastStatus INTEGER)", db);
            
            ExecCommand(@"CREATE TABLE IF NOT EXISTS Activity (
                    Id INTEGER,
                    Datum INTEGER,
                    VersammlungId INTEGER,
                    RednerId INTEGER,
                    VortragId INGERER,
                    KalenderDatum INTEGER,
                    Type INTEGER,
                    Objekt TEXT,
                    Kommentar TEXT,
                    Mails TEXT)", db);
            
            ExecCommand(@"CREATE TABLE IF NOT EXISTS Aufgaben (
                    Id INTEGER,
                    PersonName TEXT,
                    IsVorsitz INTEGER,
                    IsLeser INTEGER,
                    SpeakerId INTEGER,
                    Rating INTEGER)", db);
            
            ExecCommand(@"CREATE TABLE IF NOT EXISTS Aufgaben_Kalender (
                    Datum INTEGER,
                    VorsitzId INTEGER,
                    LeserId INTEGER)", db);

            ExecCommand(@"CREATE TABLE IF NOT EXISTS Abwesenheiten (
                    PersonId INTEGER,
                    Datum INTEGER)", db);
        }

        #region READ

        private static void UpdateDatabase(SQLiteConnection db)
        {
            if (DataContainer.Version < 7)
            {
                UpdateCommand(DataContainer.Version, db, @"DELETE FROM Templates;");
            }

            //Updates aus alten Version werden nochmal wiederholt (kann ja nichts passieren, außer einem Log-Eintrag)
            if (DataContainer.Version < 8)
            {
                UpdateCommand(DataContainer.Version, db, @"CREATE TABLE IF NOT EXISTS Cancelation (Datum INTEGER, IdSpeaker INTEGER, IdLastStatus INTEGER)");
                UpdateCommand(DataContainer.Version, db, @"ALTER TABLE Speaker ADD Einladen INTEGER;");
                UpdateCommand(DataContainer.Version, db, @"ALTER TABLE Inquiry ADD Mailtext STRING;");
                UpdateCommand(DataContainer.Version, db, @"ALTER TABLE Speaker_Vortrag ADD IdSong1 INTEGER;");
                UpdateCommand(DataContainer.Version, db, @"ALTER TABLE Speaker_Vortrag ADD IdSong2 INTEGER;");
                UpdateCommand(DataContainer.Version, db, @"ALTER TABLE Conregation ADD Zoom TEXT;");
                UpdateCommand(DataContainer.Version, db, @"ALTER TABLE Speaker ADD JwMail TEXT;");
                UpdateCommand(DataContainer.Version, db, @"ALTER TABLE Events ADD IdVortrag INTEGER;");
                //Die eigentlichen v8 Updates
                UpdateCommand(DataContainer.Version, db, @"CREATE TABLE IF NOT EXISTS Activity (
                    Id INTEGER,
                    Datum INTEGER,
                    VersammlungId INTEGER,
                    RednerId INTEGER,
                    VortragId INGERER,
                    KalenderDatum INTEGER,
                    Type INTEGER,
                    Objekt TEXT,
                    Kommentar TEXT,
                    Mails TEXT)");
            }

            if (DataContainer.Version < 9)
            {
                UpdateCommand(DataContainer.Version, db, @"DROP TABLE IF EXISTS Aufgaben");
                UpdateCommand(DataContainer.Version, db, @"CREATE TABLE Aufgaben(Id INTEGER, PersonName TEXT, IsVorsitz INTEGER, IsLeser INTEGER, SpeakerId INTEGER, Rating INTEGER)");
                UpdateCommand(DataContainer.Version, db, @"CREATE TABLE IF NOT EXISTS Aufgaben_Kalender (Datum INTEGER, VorsitzId INTEGER, LeserId INTEGER)");
            }

            if (DataContainer.Version < 10)
            {
                UpdateCommand(DataContainer.Version, db, @"CREATE TABLE IF NOT EXISTS Abwesenheiten (PersonId INTEGER,Datum INTEGER)");
            }

            if (DataContainer.Version < 11)
            {
                UpdateCommand(DataContainer.Version, db, @"UPDATE Invitation SET IdVortrag = -24 WHERE IdVortrag = 24");
                UpdateCommand(DataContainer.Version, db, @"UPDATE Events SET IdVortrag = -24 WHERE IdVortrag = 24");
                UpdateCommand(DataContainer.Version, db, @"UPDATE Outside SET IdTalk = -24 WHERE IdTalk = 24");
                UpdateCommand(DataContainer.Version, db, @"UPDATE Speaker_Vortrag SET IdTalk = -24 WHERE IdTalk = 24");
                UpdateCommand(DataContainer.Version, db, @"UPDATE Inquiry_SpeakerTalk SET IdTalk = -24 WHERE IdTalk = 24");
                UpdateCommand(DataContainer.Version, db, @"UPDATE Activity SET VortragId = -24 WHERE VortragId = 24");
                UpdateCommand(DataContainer.Version, db, @"UPDATE Talks SET Nummer = -24, Gultig = 0 WHERE Nummer = 24");
            }

            if (DataContainer.Version < 12)
            {

                UpdateV12DateColumn(db, "Invitation", "Datum");
                UpdateV12DateColumn(db, "Talks", "ZuletztGehalten");
                UpdateV12DateColumn(db, "Outside", "Datum");
                UpdateV12DateColumn(db, "Events", "Datum");
                UpdateV12DateColumn(db, "Inquiry_Dates", "Datum");
                UpdateV12DateColumn(db, "Cancelation", "Datum");
                UpdateV12DateColumn(db, "Activity", "KalenderDatum");
                UpdateV12DateColumn(db, "Aufgaben_Kalender", "Datum");
                UpdateV12DateColumn(db, "Abwesenheiten", "Datum");
            }

            if (DataContainer.Version < 13)
            {
                UpdateCommand(DataContainer.Version, db, @"ALTER TABLE Conregation_Zusammenkunftszeiten ADD Tag INTEGER;");
                UpdateCommand(DataContainer.Version, db, @"UPDATE Conregation_Zusammenkunftszeiten SET Tag = 1,Zeit = TRIM(REPLACE(Zeit,'Montag','')) WHERE ZEIT LIKE '%Montag%';");
                UpdateCommand(DataContainer.Version, db, @"UPDATE Conregation_Zusammenkunftszeiten SET Tag = 2,Zeit = TRIM(REPLACE(Zeit,'Dienstag','')) WHERE ZEIT LIKE '%Dienstag%';");
                UpdateCommand(DataContainer.Version, db, @"UPDATE Conregation_Zusammenkunftszeiten SET Tag = 3,Zeit = TRIM(REPLACE(Zeit,'Mittwoch','')) WHERE ZEIT LIKE '%Mittwoch%';");
                UpdateCommand(DataContainer.Version, db, @"UPDATE Conregation_Zusammenkunftszeiten SET Tag = 4,Zeit = TRIM(REPLACE(Zeit,'Donnerstag','')) WHERE ZEIT LIKE '%Donnerstag%';");
                UpdateCommand(DataContainer.Version, db, @"UPDATE Conregation_Zusammenkunftszeiten SET Tag = 5,Zeit = TRIM(REPLACE(Zeit,'Freitag','')) WHERE ZEIT LIKE '%Freitag%';");
                UpdateCommand(DataContainer.Version, db, @"UPDATE Conregation_Zusammenkunftszeiten SET Tag = 6,Zeit = TRIM(REPLACE(Zeit,'Samstag','')) WHERE ZEIT LIKE '%Samstag%';");
                UpdateCommand(DataContainer.Version, db, @"UPDATE Conregation_Zusammenkunftszeiten SET Tag = 0,Zeit = TRIM(REPLACE(Zeit,'Sonntag','')) WHERE Tag IS NULL;");
            }

            if (DataContainer.Version < 14)
            {
                UpdateCommand(DataContainer.Version, db, @"UPDATE Talkes SET Gultig = 0, Thema = 'Voll und ganz auf Jehova vertrauen' WHERE Nummer = 70");
            }

            if (DataContainer.Version < 17)
            {
                UpdateCommand(DataContainer.Version, db, @"ALTER TABLE Invitation ADD ErinnerungsmailGesendet INTEGER");
            }

            if (DataContainer.Version < 21)
            {
                UpdateCommand(DataContainer.Version, db, @"ALTER TABLE Speaker ADD Abstand INTEGER");
                UpdateCommand(DataContainer.Version, db, @"UPDATE Speaker SET Abstand = 4");
            }
            //Aktuelle Version = 17, siehe auch Helper.CurrentVersion und Initialize.Update
        }

        private static void UpdateV12DateColumn(SQLiteConnection db, string tablename, string columname)
        {
#pragma warning disable CA2100 // SQL-Abfragen auf Sicherheitsrisiken überprüfen
            UpdateCommand(DataContainer.Version, db, $"ALTER TABLE {tablename} RENAME COLUMN {columname} TO {columname}OLDDATE");
            UpdateCommand(DataContainer.Version, db, $"ALTER TABLE {tablename} ADD COLUMN {columname} INTEGER");
            UpdateCommand(DataContainer.Version, db, $"UPDATE {tablename} SET {columname} = -1");
            List<DateTime> dates = new List<DateTime>();
            using (var cmd = new SQLiteCommand($"SELECT DISTINCT {columname}OLDDATE FROM {tablename}", db))
            {
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                        dates.Add(rdr.GetDateTime(0));
                }
            }

            using (var cmd = new SQLiteCommand($"UPDATE {tablename} SET {columname} = @Kw WHERE {columname}OLDDATE = @Datum", db))
            {
                cmd.Parameters.Add("@Datum", System.Data.DbType.Date);
                cmd.Parameters.Add("@Kw", System.Data.DbType.Int32);
                foreach (var dat in dates)
                {
                    cmd.Parameters[0].Value = dat;
                    cmd.Parameters[1].Value = DateCalcuation.CalculateWeek(dat);
                    cmd.ExecuteNonQuery();
                }
                cmd.Parameters[0].Value = null;
                cmd.Parameters[1].Value = -1;
                cmd.ExecuteNonQuery();
            }

            UpdateCommand(DataContainer.Version, db, $"UPDATE {tablename} SET {columname} = -1 WHERE {columname} = 101");
#pragma warning restore CA2100 // SQL-Abfragen auf Sicherheitsrisiken überprüfen
        }

        private static void UpdateCommand(int version, SQLiteConnection db, string command)
        {
            try
            {
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
                var cmd = new SQLiteCommand(command, db);
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error($"Update der Datenbank von Version {version} fehlgeschlagen.", ex.Message);
            }
        }

        private static void ExecCommand(string command, SQLiteConnection db)
        {
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            var cmd = new SQLiteCommand(command, db);
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        private static void ReadParameter(SQLiteConnection db)
        {
            Log.Info(nameof(ReadParameter));
            using (var cmd = new SQLiteCommand("SELECT Name, Wert FROM Parameter", db))
            {
                SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var key = rdr.GetString(0);
                    var value = rdr.GetString(1);

                    switch (key)
                    {
                        case "Version":
                            DataContainer.Version = int.Parse(value, Helper.Helper.German);
                            break;

                        case "IsInitialized":
                            DataContainer.IsInitialized = bool.Parse(value);
                            break;

                        case "MeineVersammlung":
                            //Die VErsammmlungen müssen bereits eingelsen sein, hier muss eine Objektzuordnung statt finden
                            //deshalb nochmaliger Aufruf unter ReadVersammlungen
                            break;

                        case "DisplayedYear":
                            Helper.Helper.DisplayedYear = int.Parse(value, Helper.Helper.German);
                            break;

                        case "Wochentag":
                            DateCalcuation.Wochentag = (Wochentag)int.Parse(value, Helper.Helper.German);
                            break;
                    }
                }

                rdr.Close();
            }
        }

        /// <summary>
        /// Liest außerdem Parameter "MeineVersammlung.Id"
        /// </summary>
        /// <param name="db">ConnectionString der lokalen Datenbankdatei</param>
        private static void ReadVersammlungen(SQLiteConnection db)
        {
            Log.Info(nameof(ReadVersammlungen));
            DataContainer.Versammlungen.Clear();

            var vers = int.Parse(ReadParameter(Parameter.MeineVersammlung, db), Helper.Helper.German);
            using (var cmd1 = new SQLiteCommand("SELECT Id, Kreis, Name, Anschrift1, Anschrift2, Anreise, Entfernung, Telefon, Koordinator, KoordinatorTelefon, KoordinatorMobil, KoordinatorMail, KoordinatorJw, Zoom FROM Conregation", db))
            using (var cmd2 = new SQLiteCommand("SELECT Jahr, Tag, Zeit FROM Conregation_Zusammenkunftszeiten WHERE IdConregation = @Id", db))
            {
                cmd2.Parameters.Add("@Id", System.Data.DbType.Int32);

                SQLiteDataReader rdr = cmd1.ExecuteReader();

                while (rdr.Read())
                {
                    var c = new Conregation
                    {
                        Id = rdr.GetInt32(0),
                        Kreis = rdr.GetInt32(1),
                        Name = rdr.GetString(2),
                        Anschrift1 = rdr.IsDBNull(3) ? null : rdr.GetString(3),
                        Anschrift2 = rdr.IsDBNull(4) ? null : rdr.GetString(4),
                        Anreise = rdr.IsDBNull(5) ? null : rdr.GetString(5),
                        Entfernung = rdr.IsDBNull(6) ? 0 : rdr.GetInt32(6),
                        Telefon = rdr.IsDBNull(7) ? null : rdr.GetString(7),
                        Koordinator = rdr.IsDBNull(8) ? null : rdr.GetString(8),
                        KoordinatorTelefon = rdr.IsDBNull(9) ? null : rdr.GetString(9),
                        KoordinatorMobil = rdr.IsDBNull(10) ? null : rdr.GetString(10),
                        KoordinatorMail = rdr.IsDBNull(11) ? null : rdr.GetString(11),
                        KoordinatorJw = rdr.IsDBNull(12) ? null : rdr.GetString(12),
                        Zoom = rdr.IsDBNull(13) ? null : rdr.GetString(13),
                    };
                    DataContainer.Versammlungen.Add(c);

                    //Vorträge zuordnen
                    cmd2.Parameters[0].Value = c.Id;
                    SQLiteDataReader rdr2 = cmd2.ExecuteReader();
                    while (rdr2.Read())
                    {
                        var jahr = rdr2.GetInt32(0);
                        var tag = rdr2.GetInt32(1);
                        var zeit = rdr2.GetString(2);
                        c.Zeit.Add(jahr, tag, zeit);
                    }
                    rdr2.Close();

                    if (c.Id == vers)
                    {
                        DataContainer.MeineVersammlung = c;
                    }
                }

                rdr.Close();
            }
        }

        private static void ReadVorträge(SQLiteConnection db)
        {
            Log.Info(nameof(ReadVorträge));
            TalkList.Clear();

            using (var cmd = new SQLiteCommand("SELECT Nummer, Thema, Gultig, ZuletztGehalten FROM Talks", db))
            {
                SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var nr = rdr.GetInt32(0);
                    var th = rdr.GetString(1);
                    var gültig = rdr.GetBoolean(2);
                    var datum = rdr.GetInt32(3);
                    var t = new Talk(nr, th, gültig, datum);
                    TalkList.Add(t);
                }

                rdr.Close();
            }
        }

        /// <summary>
        /// Benötigt: Versammlungen
        /// Benötigt: Vorträge
        /// </summary>
        /// <param name="db">ConnectionString der lokalen Datenbankdatei</param>
        private static void ReadRedner(SQLiteConnection db)
        {
            Log.Info(nameof(ReadRedner));
            DataContainer.Redner.Clear();

            using (var cmd = new SQLiteCommand("SELECT Id, Name, IdConregation, Mail, Telefon, Mobil, Altester, Aktiv, InfoPrivate, InfoPublic, Einladen, JwMail, Abstand FROM Speaker", db))
            using (var cmd2 = new SQLiteCommand("SELECT IdTalk, IdSong1, IdSong2 FROM Speaker_Vortrag WHERE IdSpeaker = @IdSpeaker ORDER BY IdTalk", db))
            {
                cmd2.Parameters.Add("@IdSpeaker", System.Data.DbType.Int32);
                SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var r = new Speaker
                    {
                        Id = rdr.GetInt32(0),
                        Name = rdr.GetString(1),
                        Mail = rdr.IsDBNull(3) ? null : rdr.GetString(3),
                        Telefon = rdr.IsDBNull(4) ? null : rdr.GetString(4),
                        Mobil = rdr.IsDBNull(5) ? null : rdr.GetString(5),
                        Ältester = rdr.GetBoolean(6),
                        Aktiv = rdr.GetBoolean(7),
                        InfoPrivate = rdr.IsDBNull(8) ? null : rdr.GetString(8),
                        InfoPublic = rdr.IsDBNull(9) ? null : rdr.GetString(9),
                        Einladen = rdr.IsDBNull(10) || rdr.GetBoolean(10),
                        JwMail = rdr.IsDBNull(11) ? null : rdr.GetString(11),
                        Abstand = rdr.GetInt32(12)
                    };
                    var idConregation = rdr.IsDBNull(2) ? 0 : rdr.GetInt32(2); //Id 0 = Versammlung "unbekannt"

                    //Versammlung zuordnen
                    r.Versammlung = DataContainer.Versammlungen.First(x => x.Id == idConregation);

                    //Vorträge zuordnen
                    cmd2.Parameters[0].Value = r.Id;
                    SQLiteDataReader rdr2 = cmd2.ExecuteReader();
                    while (rdr2.Read())
                    {
                        var id = rdr2.GetInt32(0);
                        var song1 = rdr2.IsDBNull(1) ? (int?)null : rdr2.GetInt32(1);
                        var song2 = rdr2.IsDBNull(2) ? (int?)null : rdr2.GetInt32(2);
                        var vortrag = TalkList.Find(id);
                        r.Vorträge.Add(new TalkSong(vortrag, song1, song2));
                    }
                    rdr2.Close();
                    r.Vorträge.Sort();
                    DataContainer.Redner.Add(r);
                }

                rdr.Close();
            }
        }

        /// <summary>
        /// Benötigt: Versammlungen
        /// Benötigt: Redner
        /// Benötigt: Vorträge
        /// </summary>
        /// <param name="db">ConnectionString der lokalen Datenbankdatei</param>
        private static void ReadMeinPlan(SQLiteConnection db)
        {
            Log.Info(nameof(ReadMeinPlan));
            DataContainer.MeinPlan.Clear();

            using (var cmd = new SQLiteCommand("SELECT IdAltester, IdVortrag, IdConregation, Datum, Status, LetzteAktion, Kommentar, ErinnerungsmailGesendet FROM Invitation", db))
            {
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var i = new Invitation();
                    var idAltester = rdr.IsDBNull(0) ? (int?)null : rdr.GetInt32(0);
                    var idVortrag = rdr.IsDBNull(1) ? (int?)null : rdr.GetInt32(1);
                    var idConregation = rdr.IsDBNull(2) ? (int?)null : rdr.GetInt32(2);
                    i.Kw = rdr.GetInt32(3);
                    i.Status = (EventStatus)rdr.GetInt32(4);
                    i.LetzteAktion = rdr.GetDateTime(5);
                    i.Kommentar = rdr.IsDBNull(6) ? null : rdr.GetString(6);
                    i.ErinnerungsMailGesendet = !rdr.IsDBNull(7) && rdr.GetBoolean(7);

                    if (!(idAltester is null))
                        i.Ältester = DataContainer.Redner.First(x => x.Id == idAltester);
                    if (!(idVortrag is null))
                    {
                        i.Vortrag = i.Ältester.Vorträge.FirstOrDefault(x => x.Vortrag.Nummer == idVortrag);
                        if (!(i.Vortrag is null))
                            i.Vortrag = new TalkSong(TalkList.Find((int)idVortrag), -1, -1);
                    }
                    if (!(idConregation is null))
                        i.AnfrageVersammlung = DataContainer.Versammlungen.First(x => x.Id == idConregation);

                    DataContainer.MeinPlan.Add(i);
                }

                rdr.Close();
            }
        }

        /// <summary>
        /// Benötigt: Versammlungen
        /// Benötigt: Vorträge
        /// Benötigt: Redner
        /// </summary>
        /// <param name="db">ConnectionString der lokalen Datenbankdatei</param>
        private static void ReadExternerPlan(SQLiteConnection db)
        {
            Log.Info(nameof(ReadExternerPlan));
            DataContainer.ExternerPlan.Clear();
            using (var cmd = new SQLiteCommand("SELECT IdSpeaker, IdConregation, Datum, Reason, IdTalk FROM Outside", db))
            {
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var o = new Outside();
                    var idSpeaker = rdr.GetInt32(0);
                    var idConregation = rdr.IsDBNull(1) ? (int?)null : rdr.GetInt32(1);
                    o.Kw = rdr.GetInt32(2);
                    o.Reason = (OutsideReason)rdr.GetInt32(3);
                    var idTalk = rdr.GetInt32(4);

                    o.Ältester = DataContainer.Redner.First(x => x.Id == idSpeaker);
                    if (idConregation != null)
                        o.Versammlung = DataContainer.Versammlungen.First(x => x.Id == idConregation);
                    o.Vortrag = o.Ältester.Vorträge.FirstOrDefault(x => x.Vortrag.Nummer == idTalk) 
                                ?? new TalkSong(TalkList.Find(idTalk));

                    DataContainer.ExternerPlan.Add(o);
                }

                rdr.Close();
            }
        }

        private static string ReadParameter(Parameter parameter, SQLiteConnection db)
        {
            Log.Info(nameof(ReadParameter), $"parameter={parameter}");
            using (var cmd = new SQLiteCommand("SELECT Wert FROM Parameter WHERE Name = @Name", db))
            {
                cmd.Parameters.AddWithValue("@Name", parameter.ToString());
                var rdr = cmd.ExecuteScalar();
                var result = rdr.ToString();
                return result;
            }
        }

        private static void ReadTemplates(SQLiteConnection db)
        {
            Log.Info(nameof(ReadTemplates));
            Templates.Load();

            using (var cmd = new SQLiteCommand("SELECT Id, Inhalt FROM Templates", db))
            {
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var id = rdr.GetInt32(0);
                    var name = (Templates.TemplateName)id;
                    Templates.Vorlagen[name].Inhalt = rdr.GetString(1);
                    Templates.Vorlagen[name].BenutzerdefinierterInhalt = true;
                }

                rdr.Close();
            }
        }

        /// <summary>
        /// Vorher muß ReadMeinPlan ausgeführt werden, da dort das Zielobjekt geleert wird.
        /// </summary>
        /// <param name="db">Datenbank.</param>
        private static void ReadEvents(SQLiteConnection db)
        {
            Log.Info(nameof(ReadEvents));
            using (var cmd = new SQLiteCommand("SELECT Typ, Name, Thema, Vortragender, Datum, IdVortrag FROM Events", db))
            {
                SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var v = new SpecialEvent
                    {
                        Typ = (SpecialEventTyp)rdr.GetInt32(0),
                        Name = rdr.IsDBNull(1) ? null : rdr.GetString(1),
                        Thema = rdr.IsDBNull(2) ? null : rdr.GetString(2),
                        Vortragender = rdr.IsDBNull(3) ? null : rdr.GetString(3),
                        Kw = rdr.GetInt32(4),
                        Vortrag = rdr.IsDBNull(5) ? null : new TalkSong(TalkList.Find(rdr.GetInt32(5)))
                    };

                    DataContainer.MeinPlan.Add(v);
                }

                rdr.Close();
            }
        }

        private static void ReadAnfragen(SQLiteConnection db)
        {
            Log.Info(nameof(ReadAnfragen));
            DataContainer.OffeneAnfragen.Clear();

            using (var cmd1 = new SQLiteCommand("SELECT Id, IdConregation, Status, AnfrageDatum, Kommentar, Mailtext FROM Inquiry", db))
            using (var cmd2 = new SQLiteCommand("SELECT Datum FROM Inquiry_Dates WHERE IdInquiry = @Id", db))
            using (var cmd3 = new SQLiteCommand("SELECT IdSpeaker, IdTalk FROM Inquiry_SpeakerTalk WHERE IdInquiry = @Id", db))
            {
                cmd2.Parameters.Add("@Id", System.Data.DbType.Int32);
                cmd3.Parameters.Add("@Id", System.Data.DbType.Int32);

                SQLiteDataReader rdr1 = cmd1.ExecuteReader();
                while (rdr1.Read())
                {
                    var id = rdr1.GetInt32(0);
                    var idConregation = rdr1.GetInt32(1);
                    var status = rdr1.GetInt32(2);
                    var anfrage = rdr1.GetDateTime(3);
                    var kommentar = rdr1.GetString(4);
                    var mailtext = rdr1.IsDBNull(5) ? "" : rdr1.GetString(5);

                    var v = new Inquiry
                    {
                        Id = id,
                        Versammlung = DataContainer.Versammlungen.First(x => x.Id == idConregation),
                        //Status = (EventStatus)status,
                        AnfrageDatum = anfrage,
                        Kommentar = kommentar,
                        Mailtext = mailtext,
                    };

                    cmd2.Parameters[0].Value = id;
                    SQLiteDataReader rdr2 = cmd2.ExecuteReader();
                    while (rdr2.Read())
                    {
                        var datum = rdr2.GetInt32(0);
                        v.Kws.Add(datum);
                    }
                    rdr2.Close();

                    cmd3.Parameters[0].Value = id;
                    SQLiteDataReader rdr3 = cmd3.ExecuteReader();
                    while (rdr3.Read())
                    {
                        var idSpeaker = rdr3.GetInt32(0);
                        var idTalk = rdr3.GetInt32(1);

                        var s = DataContainer.Redner.First(x => x.Id == idSpeaker);
                        var t = TalkList.Find(idTalk);
                        v.RednerVortrag.Add(s, t);
                    }
                    rdr3.Close();

                    DataContainer.OffeneAnfragen.Add(v);
                }

                rdr1.Close();
            }
        }

        private static void ReadCancelation(SQLiteConnection db)
        {
            DataContainer.Absagen.Clear();
            var aktuelleKw = DateCalcuation.CalculateWeek(DateTime.Today);
#pragma warning disable CA2100 // SQL-Abfragen auf Sicherheitsrisiken überprüfen
            using (var cmd = new SQLiteCommand("SELECT Datum, IdSpeaker, IdLastStatus FROM Cancelation WHERE Datum >= " + aktuelleKw, db))
#pragma warning restore CA2100 // SQL-Abfragen auf Sicherheitsrisiken überprüfen
            {
                SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var idAltester = rdr.GetInt32(1);
                    var altester = DataContainer.Redner.FirstOrDefault(x => x.Id == idAltester);

                    if (altester == null)
                        continue;

                    var v = new Cancelation
                    {
                        Kw = rdr.GetInt32(0),
                        Ältester = altester,
                        LetzterStatus = (EventStatus)rdr.GetInt32(2)
                    };

                    DataContainer.Absagen.Add(v);
                }

                rdr.Close();
            }
        }

        private static void ReadActivity(SQLiteConnection db)
        {
            DataContainer.Aktivitäten.Clear();

            using (var cmd = new SQLiteCommand("SELECT Id, Datum, VersammlungId, RednerId, VortragId, KalenderDatum, Type, Objekt, Kommentar, Mails FROM Activity", db))
            {
                SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var versId = rdr.GetInt32(2);
                    var vers = DataContainer.Versammlungen.FirstOrDefault(x => x.Id == versId) 
                               ?? DataContainer.ConregationGetUnknown();

                    var rednId = rdr.IsDBNull(3) ? -1 : rdr.GetInt32(3);
                    var redn = DataContainer.Redner.FirstOrDefault(x => x.Id == rednId) 
                               ?? DataContainer.SpeakerGetUnknown();

                    var vortrId = rdr.IsDBNull(4) ? -1 : rdr.GetInt32(4);
                    var vortr = TalkList.Find(vortrId);

                    var v = new ActivityLog.ActivityItemViewModel
                    {
                        Id = rdr.GetInt32(0),
                        Datum = rdr.GetDateTime(1),
                        Versammlung = vers,
                        Redner = redn,
                        Vortrag = vortr,
                        KalenderKw = rdr.GetInt32(5),
                        Typ = (ActivityTypes)rdr.GetInt32(6),
                        Objekt = rdr.IsDBNull(7) ? null : rdr.GetString(7),
                        Kommentar = rdr.IsDBNull(8) ? null : rdr.GetString(8),
                        Mails = rdr.IsDBNull(9) ? null : rdr.GetString(9)
                    };

                    DataContainer.Aktivitäten.Add(v);
                }

                rdr.Close();
            }
        }

        private static void ReadAufgaben(SQLiteConnection db)
        {
            DataContainer.AufgabenPersonZuordnung.Clear();

            using (var cmd = new SQLiteCommand("SELECT Id, PersonName, IsVorsitz, IsLeser, SpeakerId, Rating FROM Aufgaben", db))
            {
                SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var id = rdr.GetInt32(0);
                    var name = rdr.IsDBNull(1) ? null : rdr.GetString(1);
                    var isVorsitz = rdr.GetBoolean(2);
                    var isLeser = rdr.GetBoolean(3);
                    var speakerId = rdr.IsDBNull(4) ? -1 : rdr.GetInt32(4);
                    var speaker = DataContainer.Redner.FirstOrDefault(x => x.Id == speakerId);
                    var rating = rdr.IsDBNull(5) ? 3 : rdr.GetInt32(5);

                    var erg = new AufgabenZuordnung(id)
                    {
                        PersonName = name,
                        IsVorsitz = isVorsitz,
                        IsLeser = isLeser,
                        VerknüpftePerson = speaker,
                        Häufigkeit = rating
                    };

                    DataContainer.AufgabenPersonZuordnung.Add(erg);
                }

                rdr.Close();
            }

            DataContainer.AufgabenPersonKalender.Clear();
            using (var cmd = new SQLiteCommand("SELECT Datum, VorsitzId, LeserId FROM Aufgaben_Kalender", db))
            {
                SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var datum = rdr.GetInt32(0);
                    var vorsitz = rdr.IsDBNull(1) ? null : DataContainer.AufgabenPersonZuordnung.FirstOrDefault(x => x.Id == rdr.GetInt32(1));
                    var leser = rdr.IsDBNull(2) ? null : DataContainer.AufgabenPersonZuordnung.FirstOrDefault(x => x.Id == rdr.GetInt32(2));
                    DataContainer.AufgabenPersonKalender.Add(new AufgabenKalender(datum, vorsitz, leser));
                }

                rdr.Close();
            }
        }

        private static void ReadAbwesenheiten(SQLiteConnection db)
        {
            DataContainer.Abwesenheiten.Clear();

            using (var cmd = new SQLiteCommand("SELECT PersonId, Datum FROM Abwesenheiten", db))
            {
                SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var rednId = rdr.GetInt32(0);
                    var datum = rdr.GetInt32(1);

                    var redn = DataContainer.Redner.FirstOrDefault(x => x.Id == rednId);
                    var year = datum / 100;
                    if (redn == null || year < DateTime.Today.Year)
                    {
                        //Alte Abwesenheiten (Vorjahre) werden nicht mehr eingelesen und damit gelöscht.
                        continue;
                    }
                    DataContainer.Abwesenheiten.Add(new Busy(redn, datum));
                }
            }
        }

        #endregion READ

        #region SAVE

        private static void SaveVersammlungen(SQLiteConnection db)
        {
            Log.Info(nameof(SaveVersammlungen));
            SQLiteCommand conregationInsertCommand = new SQLiteCommand("INSERT INTO Conregation(Id, Kreis, Name, Anschrift1, Anschrift2, Anreise, Entfernung, Telefon, Koordinator, KoordinatorTelefon, KoordinatorMobil, KoordinatorMail, KoordinatorJw, Zoom) " +
                "VALUES (@Id, @Kreis, @Name, @Anschrift1, @Anschrift2, @Anreise, @Entfernung, @Telefon, @Koordinator, @KoordinatorTelefon, @KoordinatorMobil, @KoordinatorMail, @KoordinatorJw, @Zoom)", db);

            conregationInsertCommand.Parameters.Add("@Id", System.Data.DbType.Int32);
            conregationInsertCommand.Parameters.Add("@Kreis", System.Data.DbType.Int32);
            conregationInsertCommand.Parameters.Add("@Name", System.Data.DbType.String);
            conregationInsertCommand.Parameters.Add("@Anschrift1", System.Data.DbType.String);
            conregationInsertCommand.Parameters.Add("@Anschrift2", System.Data.DbType.String);
            conregationInsertCommand.Parameters.Add("@Anreise", System.Data.DbType.String);
            conregationInsertCommand.Parameters.Add("@Entfernung", System.Data.DbType.Int32);
            conregationInsertCommand.Parameters.Add("@Telefon", System.Data.DbType.String);
            conregationInsertCommand.Parameters.Add("@Koordinator", System.Data.DbType.String);
            conregationInsertCommand.Parameters.Add("@KoordinatorTelefon", System.Data.DbType.String);
            conregationInsertCommand.Parameters.Add("@KoordinatorMobil", System.Data.DbType.String);
            conregationInsertCommand.Parameters.Add("@KoordinatorMail", System.Data.DbType.String);
            conregationInsertCommand.Parameters.Add("@KoordinatorJw", System.Data.DbType.String);
            conregationInsertCommand.Parameters.Add("@Zoom", System.Data.DbType.String);

            foreach (var vers in DataContainer.Versammlungen)
            {
                conregationInsertCommand.Parameters[0].Value = vers.Id;
                conregationInsertCommand.Parameters[1].Value = vers.Kreis;
                conregationInsertCommand.Parameters[2].Value = vers.Name;
                conregationInsertCommand.Parameters[3].Value = vers.Anschrift1;
                conregationInsertCommand.Parameters[4].Value = vers.Anschrift2;
                conregationInsertCommand.Parameters[5].Value = vers.Anreise;
                conregationInsertCommand.Parameters[6].Value = vers.Entfernung;
                conregationInsertCommand.Parameters[7].Value = vers.Telefon;
                conregationInsertCommand.Parameters[8].Value = vers.Koordinator;
                conregationInsertCommand.Parameters[9].Value = vers.KoordinatorTelefon;
                conregationInsertCommand.Parameters[10].Value = vers.KoordinatorMobil;
                conregationInsertCommand.Parameters[11].Value = vers.KoordinatorMail;
                conregationInsertCommand.Parameters[12].Value = vers.KoordinatorJw;
                conregationInsertCommand.Parameters[13].Value = vers.Zoom;
                conregationInsertCommand.ExecuteNonQuery();
            }
            conregationInsertCommand.Dispose();

            SQLiteCommand zusammenkunfzszeitenInsertCommand = new SQLiteCommand("INSERT INTO Conregation_Zusammenkunftszeiten(IdConregation, Jahr, Tag, Zeit) " +
                "VALUES (@Id, @Jahr, @Tag, @Zeit)", db);

            zusammenkunfzszeitenInsertCommand.Parameters.Add("@Id", System.Data.DbType.Int32);
            zusammenkunfzszeitenInsertCommand.Parameters.Add("@Jahr", System.Data.DbType.Int32);
            zusammenkunfzszeitenInsertCommand.Parameters.Add("@Tag", System.Data.DbType.Int32);
            zusammenkunfzszeitenInsertCommand.Parameters.Add("@Zeit", System.Data.DbType.String);

            foreach (var vers in DataContainer.Versammlungen)
            {
                foreach (var j in vers.Zeit.Items)
                {
                    zusammenkunfzszeitenInsertCommand.Parameters[0].Value = vers.Id;
                    zusammenkunfzszeitenInsertCommand.Parameters[1].Value = j.Jahr;
                    zusammenkunfzszeitenInsertCommand.Parameters[2].Value = (int)j.Tag;
                    zusammenkunfzszeitenInsertCommand.Parameters[3].Value = j.Zeit;
                    zusammenkunfzszeitenInsertCommand.ExecuteNonQuery();
                }
            }
            zusammenkunfzszeitenInsertCommand.Dispose();
        }

        private static void SaveMeinPlan(SQLiteConnection db)
        {
            Log.Info(nameof(SaveMeinPlan));
            var cmd = new SQLiteCommand("INSERT INTO Invitation(" +
                "IdAltester, IdVortrag, IdConregation, Datum, Status, LetzteAktion, Kommentar, ErinnerungsmailGesendet) " +
                "VALUES (@IdAltester, @IdVortrag, @IdConregation, @Datum, @Status, @LetzteAktion, @Kommentar, @ErinnerungsmailGesendet)", db);

            cmd.Parameters.Add("@IdAltester", System.Data.DbType.Int32);
            cmd.Parameters.Add("@IdVortrag", System.Data.DbType.Int32);
            cmd.Parameters.Add("@IdConregation", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Datum", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Status", System.Data.DbType.Int32);
            cmd.Parameters.Add("@LetzteAktion", System.Data.DbType.Date);
            cmd.Parameters.Add("@Kommentar", System.Data.DbType.String);
            cmd.Parameters.Add("@ErinnerungsmailGesendet", System.Data.DbType.Boolean);

            foreach (var er in DataContainer.MeinPlan.Where(x => x.Status != EventStatus.Ereignis))
            {
                var con = er as Invitation;
                cmd.Parameters[0].Value = con.Ältester?.Id;
                cmd.Parameters[1].Value = con.Vortrag?.Vortrag.Nummer;
                cmd.Parameters[2].Value = con.Ältester?.Versammlung?.Id ?? con.AnfrageVersammlung?.Id;
                cmd.Parameters[3].Value = con.Kw;
                cmd.Parameters[4].Value = (int)con.Status;
                cmd.Parameters[5].Value = con.LetzteAktion;
                cmd.Parameters[6].Value = con.Kommentar;
                cmd.Parameters[7].Value = con.ErinnerungsMailGesendet;
                cmd.ExecuteNonQuery();
            }

            cmd.Dispose();

            cmd = new SQLiteCommand("INSERT INTO Events(Typ, Name, Thema, Vortragender, Datum, IdVortrag)" +
                "VALUES (@Typ, @Name, @Thema, @Vortragender, @Datum, @IdVortrag)", db);

            cmd.Parameters.Add("@Typ", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Name", System.Data.DbType.String);
            cmd.Parameters.Add("@Thema", System.Data.DbType.String);
            cmd.Parameters.Add("@Vortragender", System.Data.DbType.String);
            cmd.Parameters.Add("@Datum", System.Data.DbType.Int32);
            cmd.Parameters.Add("@IdVortrag", System.Data.DbType.Int32);

            foreach (var er in DataContainer.MeinPlan.Where(x => x.Status == EventStatus.Ereignis))
            {
                var evt = er as SpecialEvent;
                cmd.Parameters[0].Value = (int)evt.Typ;
                cmd.Parameters[1].Value = evt.Name;
                cmd.Parameters[2].Value = evt.Thema;
                cmd.Parameters[3].Value = evt.Vortragender;
                cmd.Parameters[4].Value = evt.Kw;
                cmd.Parameters[5].Value = evt.Vortrag?.Vortrag?.Nummer;
                cmd.ExecuteNonQuery();
            }

            cmd.Dispose();
        }

        private static void SaveAnfragen(SQLiteConnection db)
        {
            Log.Info(nameof(SaveAnfragen));
            var cmd1 = new SQLiteCommand("INSERT INTO Inquiry(Id, IdConregation, Status, AnfrageDatum, Kommentar, Mailtext) VALUES (@Id, @IdConregation, @Status, @AnfrageDatum, @Kommentar, @Mailtext)", db);
            var cmd2 = new SQLiteCommand("INSERT INTO Inquiry_Dates(IdInquiry, Datum) VALUES (@IdInquiry, @Datum)", db);
            var cmd3 = new SQLiteCommand("INSERT INTO Inquiry_SpeakerTalk(IdInquiry, IdSpeaker, IdTalk) VALUES (@IdInquiry, @IdSpeaker, @IdTalk)", db);

            cmd1.Parameters.Add("@Id", System.Data.DbType.Int32);
            cmd1.Parameters.Add("@IdConregation", System.Data.DbType.Int32);
            cmd1.Parameters.Add("@Status", System.Data.DbType.Int32);
            cmd1.Parameters.Add("@AnfrageDatum", System.Data.DbType.Date);
            cmd1.Parameters.Add("@Kommentar", System.Data.DbType.String);
            cmd1.Parameters.Add("@Mailtext", System.Data.DbType.String);

            cmd2.Parameters.Add("@IdInquiry", System.Data.DbType.Int32);
            cmd2.Parameters.Add("@Datum", System.Data.DbType.Int32);

            cmd3.Parameters.Add("@IdInquiry", System.Data.DbType.Int32);
            cmd3.Parameters.Add("@IdSpeaker", System.Data.DbType.Int32);
            cmd3.Parameters.Add("@IdTalk", System.Data.DbType.Int32);

            foreach (var con in DataContainer.OffeneAnfragen)
            {
                cmd1.Parameters[0].Value = con.Id;
                cmd1.Parameters[1].Value = con.Versammlung.Id;
                cmd1.Parameters[2].Value = (int)con.Status;
                cmd1.Parameters[3].Value = con.AnfrageDatum;
                cmd1.Parameters[4].Value = con.Kommentar;
                cmd1.Parameters[5].Value = con.Mailtext;
                cmd1.ExecuteNonQuery();

                cmd2.Parameters[0].Value = con.Id;
                foreach (var d in con.Kws)
                {
                    cmd2.Parameters[1].Value = d;
                    cmd2.ExecuteNonQuery();
                }

                cmd3.Parameters[0].Value = con.Id;
                foreach (var person in con.RednerVortrag)
                {
                    cmd3.Parameters[1].Value = person.Key.Id;
                    cmd3.Parameters[2].Value = person.Value.Nummer;
                    cmd3.ExecuteNonQuery();
                }
            }

            cmd1.Dispose();
            cmd2.Dispose();
            cmd3.Dispose();
        }

        private static void SaveExternerPlan(SQLiteConnection db)
        {
            Log.Info(nameof(SaveExternerPlan));
            var cmd = new SQLiteCommand("INSERT INTO Outside(IdSpeaker, IdConregation, Datum, Reason, IdTalk) " +
                "VALUES (@IdSpeaker, @IdConregation, @Datum, @Reason, @IdTalk)", db);

            cmd.Parameters.Add("@IdSpeaker", System.Data.DbType.Int32);
            cmd.Parameters.Add("@IdConregation", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Datum", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Reason", System.Data.DbType.Int32);
            cmd.Parameters.Add("@IdTalk", System.Data.DbType.Int32);

            foreach (var plan in DataContainer.ExternerPlan)
            {
                cmd.Parameters[0].Value = plan.Ältester.Id;
                cmd.Parameters[1].Value = plan.Versammlung?.Id;
                cmd.Parameters[2].Value = plan.Kw;
                cmd.Parameters[3].Value = (int)plan.Reason;
                cmd.Parameters[4].Value = plan.Vortrag.Vortrag.Nummer;
                cmd.ExecuteNonQuery();
            }

            cmd.Dispose();
        }

        private static void SaveRedner(SQLiteConnection db)
        {
            Log.Info(nameof(SaveRedner));
            var cmd = new SQLiteCommand("INSERT INTO Speaker(Id, Name, IdConregation, Mail, Telefon, Mobil, Altester, Aktiv, InfoPrivate, InfoPublic, Einladen, JwMail, Abstand) " +
                "VALUES (@Id, @Name, @IdConregation, @Mail, @Telefon, @Mobil, @Altester, @Aktiv, @InfoPrivate, @InfoPublic, @Einladen, @JwMail, @Abstand)", db);

            cmd.Parameters.Add("@Id", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Name", System.Data.DbType.String);
            cmd.Parameters.Add("@IdConregation", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Mail", System.Data.DbType.String);
            cmd.Parameters.Add("@Telefon", System.Data.DbType.String);
            cmd.Parameters.Add("@Mobil", System.Data.DbType.String);
            cmd.Parameters.Add("@Altester", System.Data.DbType.Boolean);
            cmd.Parameters.Add("@Aktiv", System.Data.DbType.Boolean);
            cmd.Parameters.Add("@InfoPrivate", System.Data.DbType.String);
            cmd.Parameters.Add("@InfoPublic", System.Data.DbType.String);
            cmd.Parameters.Add("@Einladen", System.Data.DbType.Boolean);
            cmd.Parameters.Add("@JwMail", System.Data.DbType.String);
            cmd.Parameters.Add("@Abstand", System.Data.DbType.Int32);

            foreach (var red in DataContainer.Redner)
            {
                cmd.Parameters[0].Value = red.Id;
                cmd.Parameters[1].Value = red.Name;
                cmd.Parameters[2].Value = red.Versammlung?.Id;
                cmd.Parameters[3].Value = red.Mail;
                cmd.Parameters[4].Value = red.Telefon;
                cmd.Parameters[5].Value = red.Mobil;
                cmd.Parameters[6].Value = red.Ältester;
                cmd.Parameters[7].Value = red.Aktiv;
                cmd.Parameters[8].Value = red.InfoPrivate;
                cmd.Parameters[9].Value = red.InfoPublic;
                cmd.Parameters[10].Value = red.Einladen;
                cmd.Parameters[11].Value = red.JwMail;
                cmd.Parameters[12].Value = red.Abstand;
                cmd.ExecuteNonQuery();
            }
            cmd.Dispose();

            cmd = new SQLiteCommand("INSERT INTO Speaker_Vortrag(IdSpeaker, IdTalk, IdSong1, IdSong2) " +
                "VALUES (@IdSpeaker, @IdTalk, @IdSong1, @IdSong2)", db);

            cmd.Parameters.Add("@IdSpeaker", System.Data.DbType.Int32);
            cmd.Parameters.Add("@IdTalk", System.Data.DbType.Int32);
            cmd.Parameters.Add("@IdSong1", System.Data.DbType.Int32);
            cmd.Parameters.Add("@IdSong2", System.Data.DbType.Int32);

            foreach (var red in DataContainer.Redner)
            {
                cmd.Parameters[0].Value = red.Id;
                foreach (var t in red.Vorträge)
                {
                    if (t is null)
                        continue;
                    cmd.Parameters[1].Value = t.Vortrag.Nummer;
                    cmd.Parameters[2].Value = t.Lied;
                    cmd.Parameters[3].Value = t.LiedErsatz;
                    cmd.ExecuteNonQuery();
                }
            }

            cmd.Dispose();
        }

        private static void SaveVorträge(SQLiteConnection db)
        {
            Log.Info(nameof(SaveVorträge));
            var cmd = new SQLiteCommand("INSERT INTO Talks(Nummer, Thema, Gultig, ZuletztGehalten)" +
                "VALUES (@Nummer, @Thema, @Gultig, @ZuletztGehalten)", db);

            cmd.Parameters.Add("@Nummer", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Thema", System.Data.DbType.String);
            cmd.Parameters.Add("@Gultig", System.Data.DbType.Boolean);
            cmd.Parameters.Add("@ZuletztGehalten", System.Data.DbType.Int32);

            foreach (var vort in TalkList.Get())
            {
                cmd.Parameters[0].Value = vort.Nummer;
                cmd.Parameters[1].Value = vort.Thema;
                cmd.Parameters[2].Value = vort.Gültig;
                cmd.Parameters[3].Value = vort.ZuletztGehalten;
                cmd.ExecuteNonQuery();
            }

            cmd.Dispose();
        }

        private static void SaveParameter(SQLiteConnection db)
        {
            Log.Info(nameof(SaveParameter));
            var cmd = new SQLiteCommand("INSERT INTO Parameter(Name, Wert)" +
                "VALUES (@Name, @Wert)", db);

            cmd.Parameters.Add("@Name", System.Data.DbType.String);
            cmd.Parameters.Add("@Wert", System.Data.DbType.String);

            cmd.Parameters[0].Value = "Version";
            cmd.Parameters[1].Value = DataContainer.Version;
            cmd.ExecuteNonQuery();

            cmd.Parameters[0].Value = "IsInitialized";
            cmd.Parameters[1].Value = DataContainer.IsInitialized;
            cmd.ExecuteNonQuery();

            cmd.Parameters[0].Value = "MeineVersammlung";
            cmd.Parameters[1].Value = DataContainer.MeineVersammlung?.Id;
            cmd.ExecuteNonQuery();

            cmd.Parameters[0].Value = "DisplayedYear";
            cmd.Parameters[1].Value = Helper.Helper.DisplayedYear;
            cmd.ExecuteNonQuery();

            cmd.Parameters[0].Value = "Wochentag";
            cmd.Parameters[1].Value = (int)DateCalcuation.Wochentag;
            cmd.ExecuteNonQuery();

            cmd.Dispose();
        }

        private static void SaveTemplates(SQLiteConnection db)
        {
            Log.Info(nameof(SaveTemplates));
            var cmd1 = new SQLiteCommand("INSERT INTO Templates(Id, Inhalt) VALUES (@Id, @Inhalt)", db);
            cmd1.Parameters.Add("@Id", System.Data.DbType.Int32);
            cmd1.Parameters.Add("@Inhalt", System.Data.DbType.String);

            foreach (var t in Templates.Vorlagen.Where(x => x.Value.BenutzerdefinierterInhalt))
            {
                cmd1.Parameters[0].Value = (int)t.Value.Name;
                cmd1.Parameters[1].Value = t.Value.Inhalt;
                cmd1.ExecuteNonQuery();
            }

            cmd1.Dispose();
        }

        private static void SaveCancelation(SQLiteConnection db)
        {
            var cmd = new SQLiteCommand("INSERT INTO Cancelation(Datum, IdSpeaker, IdLastStatus) " +
    "VALUES (@Datum, @IdSpeaker, @IdLastStatus)", db);

            cmd.Parameters.Add("@Datum", System.Data.DbType.Int32);
            cmd.Parameters.Add("@IdSpeaker", System.Data.DbType.Int32);
            cmd.Parameters.Add("@IdLastStatus", System.Data.DbType.Int32);

            foreach (var absage in DataContainer.Absagen)
            {
                cmd.Parameters[0].Value = absage.Kw;
                cmd.Parameters[1].Value = absage.Ältester.Id;
                cmd.Parameters[2].Value = (int)absage.LetzterStatus;
                cmd.ExecuteNonQuery();
            }

            cmd.Dispose();
        }

        private static void SaveActivity(SQLiteConnection db)
        {
            var cmd = new SQLiteCommand("INSERT INTO Activity(Id, Datum, VersammlungId, RednerId, VortragId, KalenderDatum, Type, Objekt, Kommentar, Mails) " +
    "VALUES (@Id, @Datum, @VersammlungId, @RednerId, @VortragId, @KalenderDatum, @Typ, @Objekt, @Kommentar, @Mails)", db);

            cmd.Parameters.Add("@Id", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Datum", System.Data.DbType.Date);
            cmd.Parameters.Add("@VersammlungId", System.Data.DbType.Int32);
            cmd.Parameters.Add("@RednerId", System.Data.DbType.Int32);
            cmd.Parameters.Add("@VortragId", System.Data.DbType.Int32);
            cmd.Parameters.Add("@KalenderDatum", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Typ", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Objekt", System.Data.DbType.String);
            cmd.Parameters.Add("@Kommentar", System.Data.DbType.String);
            cmd.Parameters.Add("@Mails", System.Data.DbType.String);

            foreach (var a in DataContainer.Aktivitäten)
            {
                cmd.Parameters[0].Value = a.Id;
                cmd.Parameters[1].Value = a.Datum;
                cmd.Parameters[2].Value = a.Versammlung.Id;
                cmd.Parameters[3].Value = a.Redner?.Id;
                cmd.Parameters[4].Value = a.Vortrag?.Nummer;
                cmd.Parameters[5].Value = a.KalenderKw;
                cmd.Parameters[6].Value = (int)a.Typ;
                cmd.Parameters[7].Value = a.Objekt;
                cmd.Parameters[8].Value = a.Kommentar;
                cmd.Parameters[9].Value = a.Mails;

                cmd.ExecuteNonQuery();
            }

            cmd.Dispose();
        }

        private static void SaveAufgaben(SQLiteConnection db)
        {
            // new SQLiteCommand("SELECT Id, PersonName, IsVorsitz, IsLeser, SpeakerId FROM Aufgaben", db))
            var cmd = new SQLiteCommand("INSERT INTO Aufgaben(Id, PersonName, IsVorsitz, IsLeser, SpeakerId, Rating) " +
                                        "VALUES (@Id, @Name, @IsVorsitz, @IsLeser, @SpeakerId, @Oft)", db);

            cmd.Parameters.Add("@Id", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Name", System.Data.DbType.String);
            cmd.Parameters.Add("@IsVorsitz", System.Data.DbType.Boolean);
            cmd.Parameters.Add("@IsLeser", System.Data.DbType.Boolean);
            cmd.Parameters.Add("@SpeakerId", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Oft", System.Data.DbType.Int32);

            foreach (var a in DataContainer.AufgabenPersonZuordnung)
            {
                cmd.Parameters[0].Value = a.Id;
                cmd.Parameters[1].Value = (a.VerknüpftePerson == null) ? a.PersonName : a.VerknüpftePerson.Name;
                cmd.Parameters[2].Value = a.IsVorsitz;
                cmd.Parameters[3].Value = a.IsLeser;
                cmd.Parameters[4].Value = a.VerknüpftePerson?.Id;
                cmd.Parameters[5].Value = a.Häufigkeit;

                cmd.ExecuteNonQuery();
            }

            cmd.Dispose();

            cmd = new SQLiteCommand("INSERT INTO Aufgaben_Kalender(Datum, VorsitzId, LeserId) VALUES (@Datum, @IdVorsitz, @IdLeser)", db);

            cmd.Parameters.Add("@Datum", System.Data.DbType.Int32);
            cmd.Parameters.Add("@IdVorsitz", System.Data.DbType.Int32);
            cmd.Parameters.Add("@IdLeser", System.Data.DbType.Int32);

            foreach (var a in DataContainer.AufgabenPersonKalender.Where(x => x.Vorsitz != null || x.Leser != null))
            {
                cmd.Parameters[0].Value = a.Kw;
                cmd.Parameters[1].Value = a.Vorsitz?.Id;
                cmd.Parameters[2].Value = a.Leser?.Id;

                cmd.ExecuteNonQuery();
            }

            cmd.Dispose();
        }

        private static void SaveAbwesenheiten(SQLiteConnection db)
        {
            var cmd = new SQLiteCommand("INSERT INTO Abwesenheiten(PersonId, Datum) VALUES (@PersonId, @Datum)", db);
            cmd.Parameters.Add("@PersonId", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Datum", System.Data.DbType.Int32);

            foreach (var a in DataContainer.Abwesenheiten)
            {
                if (a is null)
                    continue;
                cmd.Parameters[0].Value = a.Redner.Id;
                cmd.Parameters[1].Value = a.Kw;
                cmd.ExecuteNonQuery();
            }

            cmd.Dispose();
        }

        #endregion SAVE
    }
}