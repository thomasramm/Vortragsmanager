using System.Collections.Generic;
using System.Linq;
using Vortragsmanager.Enums;

namespace Vortragsmanager.Core.DataHelper
{
    public class Zusammenkunftszeiten
    {
        public List<Zusammenkunftszeit> Items { get; } = new List<Zusammenkunftszeit>();

        public Zusammenkunftszeit Get(int Jahr)
        {
            Log.Info(nameof(Get), Jahr);
            
            //Das aktuelle Jahr abfragen
            var myItem = Items.FirstOrDefault(x => x.Jahr == Jahr);
            if (myItem != null)
                return myItem;

            //den letzen Eintrag vor meinem Jahr abfragen
            myItem = Items.Where(x => x.Jahr <= Jahr).OrderByDescending(y => y.Jahr).FirstOrDefault();
            if (myItem != null)
                return myItem;

            //das jüngste Jahr nach meinem Eintrag abfragen
            myItem = Items.OrderBy(y => y.Jahr).FirstOrDefault();
            if (myItem != null)
                return myItem;

            //Es gibt kein Jahr, neues Jahr erstellen
            myItem = new Zusammenkunftszeit(Jahr);
            Items.Add(myItem);
            return myItem;
        }

        public Zusammenkunftszeit Set(int Jahr, Wochentag Tag, string Zeit)
        {
            Log.Info(nameof(Set), $"jahr={Jahr}, Tag={Tag}, Zeit={Zeit}");
            var myItem = Items.FirstOrDefault(x => x.Jahr == Jahr);
            if (myItem == null) 
            {
                myItem = new Zusammenkunftszeit(Jahr, Tag, Zeit);
                Items.Add(myItem);
            }
            else
            {
                myItem.Tag = Tag;
                myItem.Zeit = Zeit;
            }
            return myItem;
        }

        public Zusammenkunftszeit Add(int Jahr, Wochentag Tag, string Zeit)
        {
            return Set(Jahr, Tag, Zeit);
        }

        public Zusammenkunftszeit Add(int Jahr, int Tag, string Zeit)
        {
            return Set(Jahr, (Wochentag)Tag, Zeit);
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