using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Vortragsmanager.Datamodels;

namespace Vortragsmanager.Core
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

                DataContainer.UpdateTalkDate();
                DataContainer.IsInitialized = true;

                //Falls es jetzt noch weitere Updates gibt die am Container durchgeführt
                //werden müssen, ist hier ein guter Ort...
                Initialize.Update();

                db.Close();
            }
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
                    FileInfo fi = new FileInfo(file);
                    newfile = Path.Combine(fi.DirectoryName, fi.Name.Replace(fi.Extension, "") + i + fi.Extension);
                    i++;
                }
            }
            File.Move(tempFile, newfile);

            if (createBackup)
            {
                var fi = new FileInfo(newfile);
                var backup = fi.DirectoryName + "\\" + fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length) + $"_{DateTime.Now:yyyy-MM-dd-HH-mm}" + fi.Extension;
                File.Copy(newfile, backup, true);
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
            Log.Info(nameof(CreateEmptyDatabase), "connection");

            var cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Parameter (
                Name TEXT,
                Wert TEXT)", db);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Conregation (
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
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Conregation_Zusammenkunftszeiten (
                    IdConregation INTEGER,
                    Jahr INTEGER,
                    Zeit TEXT)", db);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Invitation (
                IdAltester INTEGER,
                IdVortrag INTEGER,
                IdConregation INTEGER,
                Datum INTEGER,
                Status INTEGER,
                LetzteAktion TEXT,
                Kommentar TEXT)", db);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Events (
                Typ INTEGER,
                Name TEXT,
                Thema TEXT,
                VORTRAGENDER TEXT,
                Datum INTEGER,
                IdVortrag INTEGER)", db);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Outside (
                IdSpeaker INTEGER,
                IdConregation INTEGER,
                Datum INTEGER,
                Reason INTEGER,
                IdTalk INTEGER)", db);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Speaker (
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
                JwMail TEXT)", db);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Speaker_Vortrag (
                IdSpeaker INTEGER,
                IdTalk INTEGER,
                IdSong1 INTEGER,
                IdSong2 INTEGER)", db);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Talks (
                Nummer INTEGER,
                Thema TEXT,
                Gultig INTEGER,
                ZuletztGehalten INTEGER)", db);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Templates (
                Id INTEGER,
                Inhalt STRING)", db);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Inquiry (
                Id INTEGER,
                IdConregation INTEGER,
                Status INTEGER,
                AnfrageDatum INTEGER,
                Kommentar STRING,
                Mailtext STRING)", db);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Inquiry_Dates (
                IdInquiry INTEGER,
                Datum INTEGER)", db);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Inquiry_SpeakerTalk (
                IdInquiry INTEGER,
                IdSpeaker INTEGER,
                IdTalk INTEGER)", db);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Cancelation (
                Datum INTEGER,
                IdSpeaker INTEGER,
                IdLastStatus INTEGER)", db);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Activity (
                    Id INTEGER,
                    Datum INTEGER,
                    VersammlungId INTEGER,
                    Type INTEGER,
                    Objekt TEXT,
                    Kommentar TEXT)", db);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        #region READ

        private static void UpdateDatabase(SQLiteConnection db)
        {
            if (DataContainer.Version < 2)
            {
                var cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Cancelation (
                    Datum INTEGER,
                    IdSpeaker INTEGER,
                    IdLastStatus INTEGER)", db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            if (DataContainer.Version < 3)
            {
                var cmd = new SQLiteCommand(@"ALTER TABLE Speaker ADD Einladen INTEGER;", db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            if (DataContainer.Version < 4)
            {
                var cmd = new SQLiteCommand(@"ALTER TABLE Inquiry ADD Mailtext STRING;", db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            //Version 5 hat nur C# Updates an den Daten

            if (DataContainer.Version < 6)
            {
                var cmd = new SQLiteCommand(@"ALTER TABLE Speaker_Vortrag ADD IdSong1 INTEGER;", db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                cmd = new SQLiteCommand(@"ALTER TABLE Speaker_Vortrag ADD IdSong2 INTEGER;", db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            if (DataContainer.Version < 7)
            {
                var cmd = new SQLiteCommand(@"ALTER TABLE Conregation ADD Zoom TEXT;", db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                cmd = new SQLiteCommand(@"ALTER TABLE Speaker ADD JwMail TEXT;", db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                cmd = new SQLiteCommand(@"ALTER TABLE Events ADD IdVortrag INTEGER;", db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                cmd = new SQLiteCommand(@"DELETE FROM Templates;", db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            if (DataContainer.Version < 8)
            {
                var cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Activity (
                    Id INTEGER,
                    Datum INTEGER,
                    VersammlungId INTEGER,
                    Type INTEGER,
                    Objekt TEXT,
                    Kommentar TEXT)", db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
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
                            DataContainer.Version = int.Parse(value, Helper.German);
                            break;

                        case "IsInitialized":
                            DataContainer.IsInitialized = bool.Parse(value);
                            break;

                        case "MeineVersammlung":
                            //Die VErsammmlungen müssen bereits eingelsen sein, hier muss eine Objektzuordnung statt finden
                            //deshalb nochmaliger Aufruf unter ReadVersammlungen
                            break;

                        case "DisplayedYear":
                            Helper.DisplayedYear = int.Parse(value, Helper.German);
                            break;

                        default:
                            break;
                    }
                }

                rdr.Close();
            }
        }

        /// <summary>
        /// Liest außerdem Parameter "MeineVersammlung.Id"
        /// </summary>
        /// <param name="file">Kompletter Pfad zur Datenbank</param>
        private static void ReadVersammlungen(SQLiteConnection db)
        {
            Log.Info(nameof(ReadVersammlungen));
            DataContainer.Versammlungen.Clear();

            var vers = int.Parse(ReadParameter(Parameter.MeineVersammlung, db), Helper.German);
            using (var cmd1 = new SQLiteCommand("SELECT Id, Kreis, Name, Anschrift1, Anschrift2, Anreise, Entfernung, Telefon, Koordinator, KoordinatorTelefon, KoordinatorMobil, KoordinatorMail, KoordinatorJw, Zoom FROM Conregation", db))
            using (var cmd2 = new SQLiteCommand("SELECT Jahr, Zeit FROM Conregation_Zusammenkunftszeiten WHERE IdConregation = @Id", db))
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
                        var zeit = rdr2.GetString(1);
                        c.SetZusammenkunftszeit(jahr, zeit);
                    }
                    rdr2.Close();

                    if (c.Id == vers)
                        DataContainer.MeineVersammlung = c;
                }

                rdr.Close();
            }
        }

        private static void ReadVorträge(SQLiteConnection db)
        {
            Log.Info(nameof(ReadVorträge));
            DataContainer.Vorträge.Clear();

            using (var cmd = new SQLiteCommand("SELECT Nummer, Thema, Gultig, ZuletztGehalten FROM Talks", db))
            {
                SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var nr = rdr.GetInt32(0);
                    var th = rdr.GetString(1);
                    var t = new Talk(nr, th)
                    {
                        Gültig = rdr.GetBoolean(2),
                        ZuletztGehalten = rdr.IsDBNull(3) ? (DateTime?)null : rdr.GetDateTime(3)
                    };
                    DataContainer.Vorträge.Add(t);
                }

                rdr.Close();
            }
        }

        /// <summary>
        /// Benötigt: Versammlungen
        /// Benötigt: Vorträge
        /// </summary>
        /// <param name="file">Kompletter Pfad zur Datenbank</param>
        private static void ReadRedner(SQLiteConnection db)
        {
            Log.Info(nameof(ReadRedner));
            DataContainer.Redner.Clear();

            using (var cmd = new SQLiteCommand("SELECT Id, Name, IdConregation, Mail, Telefon, Mobil, Altester, Aktiv, InfoPrivate, InfoPublic, Einladen, JwMail FROM Speaker", db))
            using (var cmd2 = new SQLiteCommand("SELECT IdTalk, IdSong1, IdSong2 FROM Speaker_Vortrag WHERE IdSpeaker = @IdSpeaker", db))
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
                        var vortrag = DataContainer.Vorträge.First(x => x.Nummer == id);
                        r.Vorträge.Add(new TalkSong(vortrag, song1, song2));
                    }
                    rdr2.Close();
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
        /// <param name="file">Kompletter Pfad zur Datenbank</param>
        private static void ReadMeinPlan(SQLiteConnection db)
        {
            Log.Info(nameof(ReadMeinPlan));
            DataContainer.MeinPlan.Clear();

            using (var cmd = new SQLiteCommand("SELECT IdAltester, IdVortrag, IdConregation, Datum, Status, LetzteAktion, Kommentar FROM Invitation", db))
            {
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var i = new Invitation();
                    var IdAltester = rdr.IsDBNull(0) ? (int?)null : rdr.GetInt32(0);
                    var IdVortrag = rdr.IsDBNull(1) ? (int?)null : rdr.GetInt32(1);
                    var IdConregation = rdr.IsDBNull(2) ? (int?)null : rdr.GetInt32(2);
                    i.Datum = rdr.GetDateTime(3);
                    i.Status = (EventStatus)rdr.GetInt32(4);
                    i.LetzteAktion = rdr.GetDateTime(5);
                    i.Kommentar = rdr.IsDBNull(6) ? null : rdr.GetString(6);

                    if (!(IdAltester is null))
                        i.Ältester = DataContainer.Redner.First(x => x.Id == IdAltester);
                    if (!(IdVortrag is null))
                    {
                        i.Vortrag = i.Ältester.Vorträge.FirstOrDefault(x => x.Vortrag.Nummer == IdVortrag);
                        if (!(i.Vortrag is null))
                            i.Vortrag = new TalkSong(DataContainer.TalkFind((int)IdVortrag), -1, -1);
                    }
                    if (!(IdConregation is null))
                        i.AnfrageVersammlung = DataContainer.Versammlungen.First(x => x.Id == IdConregation);

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
        /// <param name="file"></param>
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
                    var IdSpeaker = rdr.GetInt32(0);
                    var IdConregation = rdr.IsDBNull(1) ? (int?)null : rdr.GetInt32(1);
                    o.Datum = rdr.GetDateTime(2);
                    o.Reason = (OutsideReason)rdr.GetInt32(3);
                    var IdTalk = rdr.GetInt32(4);

                    o.Ältester = DataContainer.Redner.First(x => x.Id == IdSpeaker);
                    if (IdConregation != null)
                        o.Versammlung = DataContainer.Versammlungen.First(x => x.Id == IdConregation);
                    o.Vortrag = o.Ältester.Vorträge.FirstOrDefault(x => x.Vortrag.Nummer == IdTalk);
                    if (o.Vortrag is null)
                        o.Vortrag = new TalkSong(DataContainer.TalkFind(IdTalk));

                    DataContainer.ExternerPlan.Add(o);
                }

                rdr.Close();
            }
        }

        private static string ReadParameter(Parameter parameter, SQLiteConnection db)
        {
            Log.Info(nameof(ReadParameter), $"parameter={parameter}");
            using (var cmd = new SQLiteCommand($"SELECT Wert FROM Parameter WHERE Name = @Name", db))
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
                        Datum = rdr.GetDateTime(4),
                        Vortrag = rdr.IsDBNull(5) ? null : new TalkSong(DataContainer.TalkFind(rdr.GetInt32(5)))
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
                        var datum = rdr2.GetDateTime(0);
                        v.Wochen.Add(datum);
                    }
                    rdr2.Close();

                    cmd3.Parameters[0].Value = id;
                    SQLiteDataReader rdr3 = cmd3.ExecuteReader();
                    while (rdr3.Read())
                    {
                        var idSpeaker = rdr3.GetInt32(0);
                        var idTalk = rdr3.GetInt32(1);

                        var s = DataContainer.Redner.First(x => x.Id == idSpeaker);
                        var t = DataContainer.Vorträge.First(x => x.Nummer == idTalk);
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
            using (var cmd = new SQLiteCommand("SELECT Datum, IdSpeaker, IdLastStatus FROM Cancelation WHERE Datum >= date('now')", db))
            {
                SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var IdAltester = rdr.GetInt32(1);
                    var Altester = DataContainer.Redner.FirstOrDefault(x => x.Id == IdAltester);

                    if (Altester == null)
                        continue;

                    var v = new Cancelation
                    {
                        Datum = rdr.GetDateTime(0),
                        Ältester = Altester,
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
            using (var cmd = new SQLiteCommand("SELECT Id, Datum, VersammlungId, Type, Objekt, Kommentar FROM Activity", db))
            {
                SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var versId = rdr.GetInt32(2);
                    var vers = DataContainer.Versammlungen.FirstOrDefault(x => x.Id == versId);
                    if (vers == null)
                        vers = DataContainer.ConregationGetUnknown();

                    var v = new Activity
                    {
                        Id = rdr.GetInt32(0),
                        Datum = rdr.GetDateTime(1),
                        Versammlung = vers,
                        Typ = (ActivityType)rdr.GetInt32(3),
                        Objekt = rdr.GetString(4),
                        Kommentar = rdr.GetString(5),
                    };

                    DataContainer.Aktivitäten.Add(v);
                }

                rdr.Close();
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

            SQLiteCommand zusammenkunfzszeitenInsertCommand = new SQLiteCommand("INSERT INTO Conregation_Zusammenkunftszeiten(IdConregation, Jahr, Zeit) " +
                "VALUES (@Id, @Jahr, @Zeit)", db);

            zusammenkunfzszeitenInsertCommand.Parameters.Add("@Id", System.Data.DbType.Int32);
            zusammenkunfzszeitenInsertCommand.Parameters.Add("@Jahr", System.Data.DbType.Int32);
            zusammenkunfzszeitenInsertCommand.Parameters.Add("@Zeit", System.Data.DbType.String);

            foreach (var vers in DataContainer.Versammlungen)
            {
                foreach (var j in vers.Zusammenkunftszeiten)
                {
                    zusammenkunfzszeitenInsertCommand.Parameters[0].Value = vers.Id;
                    zusammenkunfzszeitenInsertCommand.Parameters[1].Value = j.Key;
                    zusammenkunfzszeitenInsertCommand.Parameters[2].Value = j.Value;
                    zusammenkunfzszeitenInsertCommand.ExecuteNonQuery();
                }
            }
            zusammenkunfzszeitenInsertCommand.Dispose();
        }

        private static void SaveMeinPlan(SQLiteConnection db)
        {
            Log.Info(nameof(SaveMeinPlan));
            var cmd = new SQLiteCommand("INSERT INTO Invitation(" +
                "IdAltester, IdVortrag, IdConregation, Datum, Status, LetzteAktion, Kommentar) " +
                "VALUES (@IdAltester, @IdVortrag, @IdConregation, @Datum, @Status, @LetzteAktion, @Kommentar)", db);

            cmd.Parameters.Add("@IdAltester", System.Data.DbType.Int32);
            cmd.Parameters.Add("@IdVortrag", System.Data.DbType.Int32);
            cmd.Parameters.Add("@IdConregation", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Datum", System.Data.DbType.Date);
            cmd.Parameters.Add("@Status", System.Data.DbType.Int32);
            cmd.Parameters.Add("@LetzteAktion", System.Data.DbType.Date);
            cmd.Parameters.Add("@Kommentar", System.Data.DbType.String);

            foreach (var er in DataContainer.MeinPlan.Where(x => x.Status != EventStatus.Ereignis))
            {
                var con = er as Invitation;
                cmd.Parameters[0].Value = con.Ältester?.Id;
                cmd.Parameters[1].Value = con.Vortrag?.Vortrag.Nummer;
                cmd.Parameters[2].Value = con.Ältester?.Versammlung?.Id ?? con.AnfrageVersammlung?.Id;
                cmd.Parameters[3].Value = con.Datum;
                cmd.Parameters[4].Value = (int)con.Status;
                cmd.Parameters[5].Value = con.LetzteAktion;
                cmd.Parameters[6].Value = con.Kommentar;
                cmd.ExecuteNonQuery();
            }

            cmd.Dispose();

            cmd = new SQLiteCommand("INSERT INTO Events(Typ, Name, Thema, Vortragender, Datum, IdVortrag)" +
                "VALUES (@Typ, @Name, @Thema, @Vortragender, @Datum, @IdVortrag)", db);

            cmd.Parameters.Add("@Typ", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Name", System.Data.DbType.String);
            cmd.Parameters.Add("@Thema", System.Data.DbType.String);
            cmd.Parameters.Add("@Vortragender", System.Data.DbType.String);
            cmd.Parameters.Add("@Datum", System.Data.DbType.Date);
            cmd.Parameters.Add("@IdVortrag", System.Data.DbType.Int32);

            foreach (var er in DataContainer.MeinPlan.Where(x => x.Status == EventStatus.Ereignis))
            {
                var evt = er as SpecialEvent;
                cmd.Parameters[0].Value = (int)evt.Typ;
                cmd.Parameters[1].Value = evt.Name;
                cmd.Parameters[2].Value = evt.Thema;
                cmd.Parameters[3].Value = evt.Vortragender;
                cmd.Parameters[4].Value = evt.Datum;
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
            cmd2.Parameters.Add("@Datum", System.Data.DbType.Date);

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
                foreach (var d in con.Wochen)
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
            cmd.Parameters.Add("@Datum", System.Data.DbType.Date);
            cmd.Parameters.Add("@Reason", System.Data.DbType.Int32);
            cmd.Parameters.Add("@IdTalk", System.Data.DbType.Int32);

            foreach (var plan in DataContainer.ExternerPlan)
            {
                cmd.Parameters[0].Value = plan.Ältester.Id;
                cmd.Parameters[1].Value = plan.Versammlung?.Id;
                cmd.Parameters[2].Value = plan.Datum;
                cmd.Parameters[3].Value = (int)plan.Reason;
                cmd.Parameters[4].Value = plan.Vortrag.Vortrag.Nummer;
                cmd.ExecuteNonQuery();
            }

            cmd.Dispose();
        }

        private static void SaveRedner(SQLiteConnection db)
        {
            Log.Info(nameof(SaveRedner));
            var cmd = new SQLiteCommand("INSERT INTO Speaker(Id, Name, IdConregation, Mail, Telefon, Mobil, Altester, Aktiv, InfoPrivate, InfoPublic, Einladen, JwMail) " +
                "VALUES (@Id, @Name, @IdConregation, @Mail, @Telefon, @Mobil, @Altester, @Aktiv, @InfoPrivate, @InfoPublic, @Einladen, @JwMail)", db);

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
            cmd.Parameters.Add("@ZuletztGehalten", System.Data.DbType.Date);

            foreach (var vort in DataContainer.Vorträge)
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
            cmd.Parameters[1].Value = Helper.DisplayedYear;
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

            cmd.Parameters.Add("@Datum", System.Data.DbType.Date);
            cmd.Parameters.Add("@IdSpeaker", System.Data.DbType.Int32);
            cmd.Parameters.Add("@IdLastStatus", System.Data.DbType.Int32);

            foreach (var absage in DataContainer.Absagen)
            {
                cmd.Parameters[0].Value = absage.Datum;
                cmd.Parameters[1].Value = absage.Ältester.Id;
                cmd.Parameters[2].Value = (int)absage.LetzterStatus;
                cmd.ExecuteNonQuery();
            }

            cmd.Dispose();
        }

        private static void SaveActivity(SQLiteConnection db)
        {
            var cmd = new SQLiteCommand("INSERT INTO Activity(Id, Datum, VersammlungId, Type, Objekt, Kommentar) " +
    "VALUES (@Id, @Datum, @VersammlungId, @Typ, @Objekt, @Kommentar)", db);

            cmd.Parameters.Add("@Id", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Datum", System.Data.DbType.Date);
            cmd.Parameters.Add("@VersammlungId", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Typ", System.Data.DbType.Int32);
            cmd.Parameters.Add("@Objekt", System.Data.DbType.String);
            cmd.Parameters.Add("@Kommentar", System.Data.DbType.String);

            foreach (var a in DataContainer.Aktivitäten)
            {
                cmd.Parameters[0].Value = a.Id;
                cmd.Parameters[1].Value = a.Datum;
                cmd.Parameters[2].Value = a.Versammlung.Id;
                cmd.Parameters[3].Value = (int)a.Typ;
                cmd.Parameters[4].Value = a.Objekt;
                cmd.Parameters[5].Value = a.Kommentar;

                cmd.ExecuteNonQuery();
            }

            cmd.Dispose();
        }

        #endregion SAVE

        public enum Parameter
        {
            Version,
            IsInitialized,
            MeineVersammlung,
            DisplayedYear,
        }
    }
}