using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vortragsmanager.Core
{
    class IoExcel
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
            foreach (var m in DataContainer.MeinPlan)
            {
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
                while (true)
                {
                    var vers = worksheet.Cells[row, 1].Value;
                    var name = worksheet.Cells[row, 2].Value;
                    var vort = worksheet.Cells[row, 3].Value;

                    if (vers == null)
                        break;

                    var s = new Models.Speaker
                    {
                        Name = name.ToString()
                    };

                    //Versammlung
                    var meineVersammlung = DataContainer.FindConregation(vers.ToString());
                    s.Versammlung = meineVersammlung;

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
                    var v = DataContainer.FindConregation(v1);
                    var r = DataContainer.FindSpeaker(redner.ToString(), v);
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
                    var r = DataContainer.FindSpeaker(redner.ToString(), DataContainer.MeineVersammlung);
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
    }
}
