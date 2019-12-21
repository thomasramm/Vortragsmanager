using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Vortragsmanager.Core
{
    internal class IoExcel
    {
        private static FileInfo file;

        public static void ReadContainer(string filename)
        {
            file = new FileInfo(filename);

            ReadTalks();
            ReadConregations();
            DataContainer.MeineVersammlung = DataContainer.FindConregation("Hofgeismar");
            DataContainer.Versammlungen.Add(new Models.Conregation() { Kreis = -1, Name = "Unbekannt" });
            ReadSpeakers();
            ReadInvitations();
            ReadExternalInvitations();
            UpdateTalkDate();
        }

        public static void UpdateTalkDate()
        {
            foreach (var evt in DataContainer.MeinPlan.Where(x => x.Status != Models.InvitationStatus.Ereignis))
            {
                var m = (evt as Models.Invitation);
                if (m.Vortrag is null)
                    continue;
                if (m.Datum > m.Vortrag.zuletztGehalten || m.Vortrag.zuletztGehalten == null)
                    m.Vortrag.zuletztGehalten = m.Datum;
            }
        }

        private static void ReadTalks()
        {
            DataContainer.Vorträge.Clear();
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

                    DataContainer.Vorträge.Add(new Models.Talk(int.Parse(nr.ToString(), DataContainer.German), thema.ToString()));

                    row++;
                }
            } // the using statement automatically calls Dispose() which closes the package.
        }

        private static void ReadConregations()
        {
            DataContainer.Versammlungen.Clear();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Koordinatoren"];
                var row = 2;
                var id = 1;
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

                    var v = new Models.Conregation
                    {
                        Id = id,
                        Kreis = int.Parse(kreis.ToString(), DataContainer.German),
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

                    DataContainer.Versammlungen.Add(v);

                    row++;
                    id++;
                }
            } // the using statement automatically calls Dispose() which closes the package.
        }

        private static void ReadSpeakers()
        {
            DataContainer.Redner.Clear();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Redner"];
                var row = 2;
                var id = 1;
                while (true)
                {
                    var vers = worksheet.Cells[row, 1].Value;
                    var name = worksheet.Cells[row, 2].Value;
                    var vort = worksheet.Cells[row, 3].Value;

                    if (vers == null)
                        break;

                    var s = new Models.Speaker
                    {
                        Name = name.ToString(),
                        Id = id,
                    };

                    //Versammlung
                    var rednerVersammlung = DataContainer.FindOrAddConregation(vers.ToString());
                    s.Versammlung = rednerVersammlung;

                    //Vorträge
                    var meineVotrgäge = vort.ToString().Split(new[] { ';', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var v in meineVotrgäge)
                    {
                        var nr = int.Parse(v, DataContainer.German);
                        var t = DataContainer.FindTalk(nr);
                        s.Vorträge.Add(t);
                    }

                    DataContainer.Redner.Add(s);

                    row++;
                    id++;
                }
            } // the using statement automatically calls Dispose() which closes the package.
        }

        private static void ReadInvitations()
        {
            DataContainer.MeinPlan.Clear();
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

                    var i = new Models.Invitation
                    {
                        Datum = DateTime.Parse(datum.ToString(), DataContainer.German)
                    };

                    //Versammlung
                    var v1 = versammlung?.ToString() ?? "Unbekannt";
                    var v = DataContainer.FindOrAddConregation(v1);
                    var r = DataContainer.FindOrAddSpeaker(redner.ToString(), v);
                    i.Ältester = r;

                    //Vortrag
                    var vn = int.Parse(vortrag.ToString(), DataContainer.German);
                    var t = DataContainer.FindTalk(vn);
                    i.Vortrag = t;

                    DataContainer.MeinPlan.Add(i);

                    row++;
                }
            } // the using statement automatically calls Dispose() which closes the package.
        }

        private static void ReadExternalInvitations()
        {
            DataContainer.ExternerPlan.Clear();
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

                    var i = new Models.Outside
                    {
                        Datum = DateTime.Parse(datum.ToString(), DataContainer.German)
                    };

                    //Gast-Versammlung
                    var v1 = versammlung?.ToString() ?? "Unbekannt";
                    if (v1 == "Urlaub")
                        i.Reason = Models.OutsideReason.NotAvailable;
                    var v = DataContainer.FindConregation(v1);
                    i.Versammlung = v;

                    //Redner
                    var r = DataContainer.FindOrAddSpeaker(redner.ToString(), DataContainer.MeineVersammlung);
                    if (r == null)
                    {
                        row++;
                        continue;
                    }

                    i.Ältester = r;

                    //Vortrag
                    var vn = int.Parse(vortrag.ToString(), DataContainer.German);
                    var t = DataContainer.FindTalk(vn);
                    i.Vortrag = t;

                    DataContainer.ExternerPlan.Add(i);

                    row++;
                }
            } // the using statement automatically calls Dispose() which closes the package.
        }

        internal static class Vplanung
        {
            public static int Kreis { get; set; } = -1;

            public static List<Models.Conregation> Conregations = new List<Models.Conregation>();

            public static void ImportKoordinatoren(string filename)
            {
                file = new FileInfo(filename);
                Conregations = new List<Models.Conregation>();

                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (ExcelPackage package = new ExcelPackage(fs))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var pos = worksheet.Name.LastIndexOf(' ');
                    var kreisString = worksheet.Name.Substring(pos + 1, worksheet.Name.Length - pos);
                    Kreis = int.Parse(kreisString, DataContainer.German);                 

                    var row = 2;
                    var id = DataContainer.Versammlungen?.Max(x => x.Id) + 1 ?? 1;
                    while (true)
                    {
                        var vers = worksheet.Cells[row, 1].Value;
                        var saal = worksheet.Cells[row, 2].Value;
                        var zeit2019 = worksheet.Cells[row, 3].Value;
                        var zeit2020 = worksheet.Cells[row, 4].Value;
                        var koord = worksheet.Cells[row, 5].Value;
                        var tel = worksheet.Cells[row, 6].Value;
                        var handy = worksheet.Cells[row, 7].Value;
                        var mail = worksheet.Cells[row, 8].Value;
                        var jw = worksheet.Cells[row, 9].Value;

                        if (vers is null)
                            break;

                        if (vers.ToString().Substring(0,23) == "Hinweis zum Datenschutz")
                            break;

                        var v = new Models.Conregation
                        {
                            Id = id,
                            Kreis = Kreis,
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

                        Conregations.Add(v);

                        row++;
                        id++;
                    }

                }
            }

            public static List<Models.IEvent> MeinPlan { get; } = new List<Models.IEvent>();

            public static List<Models.Outside> ExternerPlan { get; } = new List<Models.Outside>();

            public static void ImportEigenePlanungen(string filename)
            {
                file = new FileInfo(filename);

                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (ExcelPackage package = new ExcelPackage(fs))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                    var row = 2;
                    while (true)
                    {
                        var datum = worksheet.Cells[row, 1].Value;
                        var redner = worksheet.Cells[row, 2].Value;
                        var vortrag = worksheet.Cells[row, 3].Value;
                        var thema = worksheet.Cells[row, 4].Value;
                        var versammlung = worksheet.Cells[row, 5].Value;
                        var rednerTelefon = worksheet.Cells[row, 6].Value?.ToString();
                        var rednerHandy = worksheet.Cells[row, 7].Value?.ToString();
                        row++;

                        if (datum.ToString() == "Datum")
                            continue;

                        if (datum == null) //Die Liste ist zu Ende
                            break;

                        if (redner == null && vortrag == null) //Keine Planung eingetragen
                            continue;

                        if (redner == null) //Special Event eintragen
                        {
                            var se = new Models.SpecialEvent();
                            se.Datum = DateTime.Parse(datum.ToString(), DataContainer.German);
                            var typ = thema.ToString();
                            se.Typ = Models.EventTyp.Sonstiges;
                            if (typ.Contains("Weltzentrale"))
                            {
                                se.Typ = Models.EventTyp.Streaming;
                                se.Name = typ;
                             }
                            else if (typ.Contains("Kreiskongress"))
                            {
                                se.Typ = Models.EventTyp.Kreiskongress;
                            }
                            else if (typ.Contains("Sondervortrag"))
                            {
                                se.Typ = Models.EventTyp.Streaming;
                                se.Name = typ;
                            }
                            else if (typ.Contains("Regionaler Kongress"))
                            {
                                se.Typ = Models.EventTyp.RegionalerKongress;
                            }
                            MeinPlan.Add(se);
                            continue;
                        }

                        var i = new Models.Invitation
                        {
                            Datum = DateTime.Parse(datum.ToString(), DataContainer.German)
                        };

                        //Versammlung
                        var v1 = versammlung?.ToString() ?? "Unbekannt";

                        if (v1 == "Kreisaufseher")
                        {
                            var se = new Models.SpecialEvent();
                            se.Datum = i.Datum;
                            se.Typ = Models.EventTyp.Dienstwoche;
                            se.Vortragender = redner?.ToString() ?? "Kreisaufseher";
                            se.Thema = thema?.ToString();
                            continue;
                        }

                        var v = DataContainer.FindOrAddConregation(v1);
                        var r = DataContainer.FindOrAddSpeaker(redner.ToString(), v);
                        i.Ältester = r;

                        if (string.IsNullOrEmpty(i.Ältester.Telefon) && !string.IsNullOrEmpty(rednerTelefon))
                            i.Ältester.Telefon = rednerTelefon;
                        if (string.IsNullOrEmpty(i.Ältester.Mobil) && !string.IsNullOrEmpty(rednerHandy))
                            i.Ältester.Mobil = rednerHandy;

                        //Vortrag
                        var vn = int.Parse(vortrag.ToString(), DataContainer.German);
                        var t = DataContainer.FindTalk(vn);
                        i.Vortrag = t;

                        MeinPlan.Add(i);
                    }
                }
            }

            public static void ImportRednerPlanungen(string filename)
            {
                file = new FileInfo(filename);
                Conregations = new List<Models.Conregation>();

                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (ExcelPackage package = new ExcelPackage(fs))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var pos = worksheet.Name.LastIndexOf(' ');
                    var kreisString = worksheet.Name.Substring(pos + 1, worksheet.Name.Length - pos);
                    Kreis = int.Parse(kreisString, DataContainer.German);

                    var row = 2;
                    var id = DataContainer.Versammlungen?.Max(x => x.Id) + 1 ?? 1;
                    while (true)
                    {
                        var datum = worksheet.Cells[row, 1].Value;
                        var redner = worksheet.Cells[row, 2].Value;
                        var vortrag = worksheet.Cells[row, 3].Value;
                        var thema = worksheet.Cells[row, 4].Value;
                        var versammlung = worksheet.Cells[row, 5].Value;
                        row++;

                        if (datum.ToString() == "Datum")
                            continue;

                        if (datum == null)
                            break;

                        if (redner == null)
                            continue;

                        var i = new Models.Outside
                        {
                            Datum = DateTime.Parse(datum.ToString(), DataContainer.German)
                        };

                        //Redner
                        var r = DataContainer.FindOrAddSpeaker(redner.ToString(), DataContainer.MeineVersammlung);
                        if (r == null)
                            continue;

                        i.Ältester = r;

                        //Gast-Versammlung
                        var v1 = versammlung?.ToString() ?? "Unbekannt";
                        if (v1 == "Urlaub")
                            i.Reason = Models.OutsideReason.NotAvailable;
                        var v = DataContainer.FindConregation(v1);
                        i.Versammlung = v;

                        //Vortrag
                        var vn = int.Parse(vortrag.ToString(), DataContainer.German);
                        var t = DataContainer.FindTalk(vn);
                        i.Vortrag = t;

                        ExternerPlan.Add(i);
                    }
                }
            }
        }
    }
 }