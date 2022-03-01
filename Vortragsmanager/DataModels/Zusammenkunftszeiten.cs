using System.Collections.Generic;
using System.Linq;
using Vortragsmanager.Enums;
using Vortragsmanager.Module;

namespace Vortragsmanager.DataModels
{
    public class Zusammenkunftszeiten
    {
        public List<Zusammenkunftszeit> Items { get; } = new List<Zusammenkunftszeit>();

        public Zusammenkunftszeit Get(int jahr)
        {
            Log.Info(nameof(Get), jahr);
            
            //Das aktuelle Jahr abfragen
            var myItem = Items.FirstOrDefault(x => x.Jahr == jahr);
            if (myItem != null)
                return myItem;

            //den letzen Eintrag vor meinem Jahr abfragen
            myItem = Items.Where(x => x.Jahr <= jahr).OrderByDescending(y => y.Jahr).FirstOrDefault();
            if (myItem != null)
                return myItem;

            //das jüngste Jahr nach meinem Eintrag abfragen
            myItem = Items.OrderBy(y => y.Jahr).FirstOrDefault();
            if (myItem != null)
                return myItem;

            //Es gibt kein Jahr, neues Jahr erstellen
            myItem = new Zusammenkunftszeit(jahr);
            Items.Add(myItem);
            return myItem;
        }

        public Zusammenkunftszeit Set(int jahr, Wochentag tag, string zeit)
        {
            Log.Info(nameof(Set), $"jahr={jahr}, Tag={tag}, Zeit={zeit}");
            var myItem = Items.FirstOrDefault(x => x.Jahr == jahr);
            if (myItem == null) 
            {
                myItem = new Zusammenkunftszeit(jahr, tag, zeit);
                Items.Add(myItem);
            }
            else
            {
                myItem.Tag = tag;
                myItem.Zeit = zeit;
            }
            return myItem;
        }

        public Zusammenkunftszeit Add(int jahr, Wochentag tag, string zeit)
        {
            return Set(jahr, tag, zeit);
        }

        public Zusammenkunftszeit Add(int jahr, int tag, string zeit)
        {
            return Set(jahr, (Wochentag)tag, zeit);
        }

        public void Remove(Zusammenkunftszeit zeit)
        {
            Items.Remove(zeit);
        }

        public Zusammenkunftszeit GetLastItem()
        {
            return Items.OrderByDescending(x => x.Jahr).FirstOrDefault();
        }
    }
}