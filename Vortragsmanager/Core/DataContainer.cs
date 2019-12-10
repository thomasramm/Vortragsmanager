using System;
using Vortragsmanager.Models;
using OfficeOpenXml;
using System.IO;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using Vortragsmanager.Views;
using System.Globalization;

namespace Vortragsmanager.Core
{
    public static class DataContainer
    {
        private static FileInfo file;
        private static int displayedYear = DateTime.Now.Year;

        public static CultureInfo German { get; } = new CultureInfo("de-DE");

        public static bool IsInitialized { get; set; }

        public static ObservableCollection<Talk> Vorträge { get; } = new ObservableCollection<Talk>();

        public static Conregation MeineVersammlung { get; set; }

        public static ObservableCollection<Conregation> Versammlungen { get; } = new ObservableCollection<Conregation>();

        public static ObservableCollection<Speaker> Redner { get; } = new ObservableCollection<Speaker>();

        public static ObservableCollection<Invitation> MeinPlan { get; } = new ObservableCollection<Invitation>();

        public static ObservableCollection<Outside> ExternerPlan { get; } = new ObservableCollection<Outside>();

        public static void ReadData(string Filename)
        {
            file = new FileInfo(Filename);

            ReadTalks();
            ReadConregations();
            MeineVersammlung = FindConregation("Hofgeismar");
            Versammlungen.Add(new Conregation() { Kreis = -1, Name = "Unbekannt" });
            ReadSpeakers();
            ReadInvitations();
            ReadExternalInvitations();
            UpdateTalkDate();
        }

        public static void UpdateTalkDate()
        {
            foreach(var m in MeinPlan)
            {
                if (m.Vortrag is null)
                    continue;
                if (m.Datum > m.Vortrag.zuletztGehalten || m.Vortrag.zuletztGehalten == null)
                    m.Vortrag.zuletztGehalten = m.Datum;
            }
        }

        private static void ReadTalks()
        {
            Vorträge.Clear();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Vortrag"];
                var row = 2;
                while (true)
                {
                    var nr = worksheet.Cells[row, 1].Value;
                    var thema = worksheet.Cells[row, 2].Value;

                    if (nr == null)
                        break;

                    Vorträge.Add(new Talk(int.Parse(nr.ToString(), German), thema.ToString()));

                    row++;
                }

            } // the using statement automatically calls Dispose() which closes the package.
        }

        private static void ReadConregations()
        {
            Versammlungen.Clear();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Koordinatoren"];
                var row = 2;
                while (true)
                {
                    var kreis = worksheet.Cells[row, 1].Value;
                    var vers = worksheet.Cells[row, 2].Value;
                    var saal = worksheet.Cells[row, 3].Value;
                    var zeit2019 = worksheet.Cells[row, 4].Value;
                    var zeit2020 = worksheet.Cells[row, 5].Value;
                    var koord = worksheet.Cells[row, 6].Value;
                    var tel = worksheet.Cells[row, 7].Value;
                    var handy = worksheet.Cells[row, 8].Value;
                    var mail = worksheet.Cells[row, 9].Value;
                    var jw = worksheet.Cells[row, 10].Value;

                    if (kreis == null)
                        break;

                    var v = new Conregation
                    {
                        Kreis = int.Parse(kreis.ToString(), German),
                        Name = vers.ToString(),
                        Koordinator = koord.ToString(),
                        KoordinatorTelefon = tel?.ToString(),
                        KoordinatorMobil = handy?.ToString(),
                        KoordinatorMail = mail?.ToString(),
                        KoordinatorJw = jw?.ToString(),
                    };

                    // Anschrift
                    var adr = saal.ToString().Split(Environment.NewLine.ToCharArray());
                    v.Anschrift1 = adr[0];
                    v.Anschrift2 = adr[1];
                    v.Telefon = adr[2];

                    // Zusammenkunftszeiten
                    var z19 = zeit2019.ToString();
                    var z20 = zeit2020.ToString();
                    if (z20 == "liegt nicht vor")
                        z20 = z19;
                    v.SetZusammenkunftszeit(2019, z19);
                    if (z20 != z19)
                        v.SetZusammenkunftszeit(2020, z19);

                    Versammlungen.Add(v);

                    row++;
                }

            } // the using statement automatically calls Dispose() which closes the package.
        }

        private static void ReadSpeakers()
        {
            Redner.Clear();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Redner"];
                var row = 2;
                while (true)
                {
                    var vers = worksheet.Cells[row, 1].Value;
                    var name = worksheet.Cells[row, 2].Value;
                    var vort = worksheet.Cells[row, 3].Value;

                    if (vers == null)
                        break;

                    var s = new Speaker
                    {
                        Name = name.ToString()
                    };

                    //Versammlung
                    var meineVersammlung = FindConregation(vers.ToString());
                    s.Versammlung = meineVersammlung;

                    //Vorträge
                    var meineVotrgäge = vort.ToString().Split(new[] { ';', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var v in meineVotrgäge)
                    {
                        var nr = int.Parse(v, German);
                        var t = FindTalk(nr);
                        s.Vorträge.Add(t);
                    }

                    Redner.Add(s);

                    row++;
                }

            } // the using statement automatically calls Dispose() which closes the package.
        }

        private static void ReadInvitations()
        {
            MeinPlan.Clear();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Eingeladene Redner"];
                var row = 2;
                while (true)
                {
                    var datum = worksheet.Cells[row, 1].Value;
                    var redner = worksheet.Cells[row, 2].Value;
                    var vortrag = worksheet.Cells[row, 3].Value;
                    var versammlung = worksheet.Cells[row, 4].Value;
                    var kommentar = worksheet.Cells[row, 5].Value;


                    if (datum == null)
                        break;

                    if (redner == null)
                    {
                        row++;
                        continue;
                    }

                    var i = new Invitation
                    {
                        Datum = DateTime.Parse(datum.ToString(), German)
                    };

                    //Versammlung
                    var v1 = versammlung?.ToString() ?? "Unbekannt";
                    var v = FindConregation(v1);
                    var r = FindSpeaker(redner.ToString(), v);
                    i.Ältester = r;

                    //Vortrag
                    var vn = int.Parse(vortrag.ToString(), German);
                    var t = FindTalk(vn);
                    i.Vortrag = t;

                    MeinPlan.Add(i);

                    row++;
                }

            } // the using statement automatically calls Dispose() which closes the package.
        }

        private static void ReadExternalInvitations()
        {
            ExternerPlan.Clear();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Eigene Redner auswärts"];
                var row = 2;
                while (true)
                {
                    var datum = worksheet.Cells[row, 1].Value;
                    var redner = worksheet.Cells[row, 2].Value;
                    var vortrag = worksheet.Cells[row, 3].Value;
                    var versammlung = worksheet.Cells[row, 4].Value;
                    var kommentar = worksheet.Cells[row, 5].Value;


                    if (datum == null)
                        break;

                    if (redner == null)
                    {
                        row++;
                        continue;
                    }

                    var i = new Outside
                    {
                        Datum = DateTime.Parse(datum.ToString(), German)
                    };

                    //Gast-Versammlung
                    var v1 = versammlung?.ToString() ?? "Unbekannt";
                    if (v1 == "Urlaub")
                        i.Reason = OutsideReason.NotAvailable;
                    var v = FindConregation(v1);
                    i.Versammlung = v;

                    //Redner
                    var r = FindSpeaker(redner.ToString(), MeineVersammlung);
                    if (r == null)
                    {
                        row++;
                        continue;
                    }

                    i.Ältester = r;

                    //Vortrag
                    var vn = int.Parse(vortrag.ToString(), German);
                    var t = FindTalk(vn);
                    i.Vortrag = t;

                    ExternerPlan.Add(i);

                    row++;
                }

            } // the using statement automatically calls Dispose() which closes the package.
        }
        
        public static Conregation FindConregation(string name)
        {
            foreach (var c in Versammlungen)
            {
                if (c.Name == name)
                    return c;
            }

            return null;
        }

        private static Talk FindTalk(int Nummer)
        {
            foreach (var t in Vorträge)
            {
                if (t.Nummer == Nummer)
                    return t;
            }

            return null;
        }

        private static Speaker FindSpeaker(string name, Conregation versammlung)
        {
            foreach (var s in Redner)
            {
                if (s.Name == name && s.Versammlung == versammlung)
                    return s;
            }

            return null;
        }

        public static int DisplayedYear 
        {
            get => displayedYear;
            set 
            { 
                displayedYear = value;
                Messenger.Default.Send(Messages.DisplayYearChanged);
            }
        }
    }
}
