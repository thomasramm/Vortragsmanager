using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vortragsmanager.Core
{
    public static class IoSqlite
    {
        public static void SaveContainer(string file)
        {
            SQLiteConnection.CreateFile($"{file}");
            SaveParameter(file);
            SaveVorträge(file);
            SaveVersammlungen(file);
            SaveRedner(file);
            SaveMeinPlan(file);
            SaveExternerPlan(file);
        }
        private static void SaveVersammlungen(string file)
        {
            using (SQLiteConnection db = new SQLiteConnection($"Data Source = {file}; Version = 3;"))
            {
                db.Open();

                var cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Conregation (
                    Id INTEGER, 
                    Kreis INTEGER, 
                    Name TEXT, 
                    Anschrift1 TEXT, 
                    Anschrift2 TEXT, 
                    Anreise TEXT, 
                    Telefon TEXT, 
                    Koordinator TEXT, 
                    KoordinatorTelefon TEXT, 
                    KoordinatorMobil TEXT, 
                    KoordinatorMail TEXT, 
                    KoordinatorJw TEXT)", db);
                    cmd.ExecuteNonQuery();
                cmd.Dispose();

                SQLiteCommand conregationInsertCommand = new SQLiteCommand("INSERT INTO Conregation(Id, Kreis, Name, Anschrift1, Anschrift2, Anreise, Telefon, Koordinator, KoordinatorTelefon, KoordinatorMobil, KoordinatorMail, KoordinatorJw) " +
                    "VALUES (@Id, @Kreis, @Name, @Anschrift1, @Anschrift2, @Anreise, @Telefon, @Koordinator, @KoordinatorTelefon, @KoordinatorMobil, @KoordinatorMail, @KoordinatorJw)", db);

                conregationInsertCommand.Parameters.Add("@Id", System.Data.DbType.Int32);
                conregationInsertCommand.Parameters.Add("@Kreis", System.Data.DbType.Int32);
                conregationInsertCommand.Parameters.Add("@Name", System.Data.DbType.String);
                conregationInsertCommand.Parameters.Add("@Anschrift1", System.Data.DbType.String);
                conregationInsertCommand.Parameters.Add("@Anschrift2", System.Data.DbType.String);
                conregationInsertCommand.Parameters.Add("@Anreise", System.Data.DbType.String);
                conregationInsertCommand.Parameters.Add("@Telefon", System.Data.DbType.String);
                conregationInsertCommand.Parameters.Add("@Koordinator", System.Data.DbType.String);
                conregationInsertCommand.Parameters.Add("@KoordinatorTelefon", System.Data.DbType.String);
                conregationInsertCommand.Parameters.Add("@KoordinatorMobil", System.Data.DbType.String);
                conregationInsertCommand.Parameters.Add("@KoordinatorMail", System.Data.DbType.String);
                conregationInsertCommand.Parameters.Add("@KoordinatorJw", System.Data.DbType.String);

                foreach (var vers in DataContainer.Versammlungen)
                {
                    conregationInsertCommand.Parameters[0].Value = vers.Id;
                    conregationInsertCommand.Parameters[1].Value = vers.Kreis;
                    conregationInsertCommand.Parameters[2].Value = vers.Name;
                    conregationInsertCommand.Parameters[3].Value = vers.Anschrift1;
                    conregationInsertCommand.Parameters[4].Value = vers.Anschrift2;
                    conregationInsertCommand.Parameters[5].Value = vers.Anreise;
                    conregationInsertCommand.Parameters[6].Value = vers.Telefon;
                    conregationInsertCommand.Parameters[7].Value = vers.Koordinator;
                    conregationInsertCommand.Parameters[8].Value = vers.KoordinatorTelefon;
                    conregationInsertCommand.Parameters[9].Value = vers.KoordinatorMobil;
                    conregationInsertCommand.Parameters[10].Value = vers.KoordinatorMail;
                    conregationInsertCommand.Parameters[11].Value = vers.KoordinatorJw;
                    conregationInsertCommand.ExecuteNonQuery();

                }

                cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Conregation_Zusammenkunftszeiten (
                    IdConregation INTEGER, 
                    Jahr INTEGER, 
                    Zeit TEXT)", db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

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

                conregationInsertCommand.Dispose();
                zusammenkunfzszeitenInsertCommand.Dispose();
                db.Close();
            }
        }

        private static void SaveMeinPlan(string file)
        {
            using (SQLiteConnection db = new SQLiteConnection($"Data Source = {file}; Version = 3;"))
            {
                db.Open();

                var cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Invitation (
                    IdAltester INTEGER, 
                    IdVortrag INTEGER, 
                    IdConregation INTEGER,
                    Datum INTEGER,
                    Status INTEGER,
                    LetzteAktion TEXT,
                    Kommentar TEXT)", db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                cmd = new SQLiteCommand("INSERT INTO Invitation(" +
                    "IdAltester, IdVortrag, IdConregation, Datum, Status, LetzteAktion, Kommentar) " +
                    "VALUES (@IdAltester, @IdVortrag, @IdConregation, @Datum, @Status, @LetzteAktion, @Kommentar)", db);

                cmd.Parameters.Add("@IdAltester", System.Data.DbType.Int32);
                cmd.Parameters.Add("@IdVortrag", System.Data.DbType.Int32);
                cmd.Parameters.Add("@IdConregation", System.Data.DbType.Int32);
                cmd.Parameters.Add("@Datum", System.Data.DbType.Date);
                cmd.Parameters.Add("@Status", System.Data.DbType.Int32);
                cmd.Parameters.Add("@LetzteAktion", System.Data.DbType.Date);
                cmd.Parameters.Add("@Kommentar", System.Data.DbType.String);

                foreach (var con in DataContainer.MeinPlan)
                {
                    cmd.Parameters[0].Value = con.Ältester?.Id;
                    cmd.Parameters[1].Value = con.Vortrag?.Nummer;
                    cmd.Parameters[2].Value = con.Ältester?.Versammlung?.Id;
                    cmd.Parameters[3].Value = con.Datum;
                    cmd.Parameters[4].Value = (int)con.Status;
                    cmd.Parameters[5].Value = con.LetzteAktion;
                    cmd.Parameters[6].Value = con.Kommentar;
                    cmd.ExecuteNonQuery();
                }

                cmd.Dispose();
                db.Close();
            }

        }

        private static void SaveExternerPlan(string file)
        {
            using (SQLiteConnection db = new SQLiteConnection($"Data Source = {file}; Version = 3;"))
            {
                db.Open();

                var cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Outside (
                    IdSpeaker INTEGER, 
                    IdConregation INTEGER, 
                    Datum INTEGER,
                    Reason INTEGER,
                    IdTalk INTEGER)", db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                cmd = new SQLiteCommand("INSERT INTO Outside(IdSpeaker, IdConregation, Datum, Reason, IdTalk) " +
                    "VALUES (@IdSpeaker, @IdConregation, @Datum, @Reason, @IdTalk)", db);

                cmd.Parameters.Add("@IdSpeaker", System.Data.DbType.Int32);
                cmd.Parameters.Add("@IdConregation", System.Data.DbType.Int32);
                cmd.Parameters.Add("@Datum", System.Data.DbType.Date);
                cmd.Parameters.Add("@Reason", System.Data.DbType.Int32);
                cmd.Parameters.Add("@IdTalk", System.Data.DbType.Int32);

                foreach (var plan in DataContainer.ExternerPlan)
                {
                    cmd.Parameters[0].Value = plan.Ältester.Id;
                    cmd.Parameters[1].Value = plan.Versammlung.Id;
                    cmd.Parameters[2].Value = plan.Datum;
                    cmd.Parameters[3].Value = (int)plan.Reason;
                    cmd.Parameters[4].Value = plan.Vortrag.Nummer;
                    cmd.ExecuteNonQuery();
                }

                cmd.Dispose();
                db.Close();
            }

        }

        private static void SaveRedner(string file)
        {
            using (SQLiteConnection db = new SQLiteConnection($"Data Source = {file}; Version = 3;"))
            {
                db.Open();

                var cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Speaker (
                    Id INTEGER, 
                    Name TEXT, 
                    IdConregation INTEGER,
                    Mail TEXT,
                    Telefon TEXT,
                    Mobil TEXT,
                    Altester INTEGER,
                    Aktiv INTEGER,
                    InfoPrivate TEXT,
                    InfoPublic TEXT)", db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                cmd = new SQLiteCommand("INSERT INTO Speaker(Id, Name, IdConregation, Mail, Telefon, Mobil, Altester, Aktiv, InfoPrivate, InfoPublic) " +
                    "VALUES (@Id, @Name, @IdConregation, @Mail, @Telefon, @Mobil, @Altester, @Aktiv, @InfoPrivate, @InfoPublic)", db);

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
                    cmd.ExecuteNonQuery();
                }
                cmd.Dispose();

                cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Speaker_Vortrag (
                    IdSpeaker INTEGER, 
                    IdTalk INTEGER)", db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                cmd = new SQLiteCommand("INSERT INTO Speaker_Vortrag(IdSpeaker, IdTalk) " +
                    "VALUES (@IdSpeaker, @IdTalk)", db);

                cmd.Parameters.Add("@IdSpeaker", System.Data.DbType.Int32);
                cmd.Parameters.Add("@IdTalk", System.Data.DbType.Int32);

                foreach (var red in DataContainer.Redner)
                {
                    cmd.Parameters[0].Value = red.Id;
                    foreach (var t in red.Vorträge)
                    {
                        cmd.Parameters[1].Value = t.Nummer;
                        cmd.ExecuteNonQuery();
                    }                  
                }

                cmd.Dispose();
                db.Close();
            }

        }

        private static void SaveVorträge(string file)
        {
            using (SQLiteConnection db = new SQLiteConnection($"Data Source = {file}; Version = 3;"))
            {
                db.Open();

                var cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Talks (
                    Nummer INTEGER, 
                    Thema TEXT, 
                    Gultig INTEGER,
                    ZuletztGehalten INTEGER)", db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                cmd = new SQLiteCommand("INSERT INTO Talks(Nummer, Thema, Gultig, ZuletztGehalten)" +
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
                    cmd.Parameters[3].Value = vort.zuletztGehalten;
                    cmd.ExecuteNonQuery();
                }

                cmd.Dispose();
                db.Close();
            }

        }
        
        private static void SaveParameter(string file)
        {
            using (SQLiteConnection db = new SQLiteConnection($"Data Source = {file}; Version = 3;"))
            {
                db.Open();

                var cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Parameter (
                    Name TEXT, 
                    Wert TEXT)", db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                cmd = new SQLiteCommand("INSERT INTO Parameter(Name, Wert)" +
                    "VALUES (@Name, @Wert)", db);

                cmd.Parameters.Add("@Name", System.Data.DbType.String);
                cmd.Parameters.Add("@Wert", System.Data.DbType.String);

                cmd.Parameters[0].Value = "Version";
                cmd.Parameters[1].Value = 1;
                cmd.ExecuteNonQuery();

                cmd.Parameters[0].Value = "IsInitialized";
                cmd.Parameters[1].Value = DataContainer.IsInitialized;
                cmd.ExecuteNonQuery();

                cmd.Parameters[0].Value = "MeineVersammlung";
                cmd.Parameters[1].Value = DataContainer.MeineVersammlung.Id;
                cmd.ExecuteNonQuery();

                cmd.Parameters[0].Value = "DisplayedYear";
                cmd.Parameters[1].Value = DataContainer.DisplayedYear;
                cmd.ExecuteNonQuery();

                cmd.Dispose();
                db.Close();
            }

        }
    }
}
