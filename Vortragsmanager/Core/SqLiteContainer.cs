using System;
using System.Collections.Generic;
using System.IO;

namespace Vortragsmanager.Core
{
    public static class SqLiteContainer
    {
        private static FileInfo file;

        //public static void ReadData(string dateiname)
        //{
        //    file = new FileInfo(dateiname);
        //}

        public static void SaveDatabase(string dateiname)
        {
            file = new FileInfo(dateiname);
        }



        //private static void SaveInvitation()
        //{ }

        //private static void SaveOutside()
        //{ }

        //private static void SaveSpeaker()
        //{ }

        //private static void SaveTalk()
        //{ }



        //public static void CreateDatabase(string dateiname)
        //{
        //    using (SqliteConnection db = new SqliteConnection($"Filename={dateiname}"))
        //    {
        //        db.Open();

        //        new SqliteCommand("CREATE TABLE IF NOT EXISTS MyTable (Primary_Key INTEGER PRIMARY KEY, ext_Entry NVARCHAR(2048) NULL)", db).ExecuteNonQuery();
        //    }
        //}

        //public static void AddData(string inputText)
        //{
        //    using (SqliteConnection db =
        //      new SqliteConnection($"Filename={file.FullName}"))
        //    {
        //        db.Open();

        //        SqliteCommand insertCommand = new SqliteCommand();
        //        insertCommand.Connection = db;

        //        // Use parameterized query to prevent SQL injection attacks
        //        insertCommand.CommandText = "INSERT INTO MyTable VALUES (NULL, @Entry);";
        //        insertCommand.Parameters.AddWithValue("@Entry", inputText);

        //        insertCommand.ExecuteReader();

        //        db.Close();
        //    }

        //}

        //public static List<String> GetData()
        //{
        //    List<String> entries = new List<string>();

        //    using (SqliteConnection db =
        //       new SqliteConnection($"Filename={file.FullName}"))
        //    {
        //        db.Open();

        //        SqliteCommand selectCommand = new SqliteCommand("SELECT Text_Entry from MyTable", db);

        //        SqliteDataReader query = selectCommand.ExecuteReader();

        //        while (query.Read())
        //        {
        //            entries.Add(query.GetString(0));
        //        }

        //        db.Close();
        //    }

        //    return entries;
        //}
    }
}