using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Vortragsmanager.DataModels
{
    public class Speaker
    {
        public bool flagIsUpdateDoneByUser;

        public int Id { get; set; }

        string name;
        public string Name { get => name; set
            {
                if (name == value)
                {
                    return;
                }

                name = value;
                if (Versammlung != null)
                    ManualUpdateData();
            }
        }

        public Conregation Versammlung { get; set; }

        public List<TalkSong> Vorträge { get; } = new List<TalkSong>();

        string mail;
        public string Mail { get => mail; set
            {
                if (mail == value)
                {
                    return;
                }

                mail = value;
                ManualUpdateData();
            }
        }

        string jwMail;
        public string JwMail { get => jwMail; set
            {
                if (jwMail == value)
                {
                    return;
                }

                jwMail = value;
                ManualUpdateData();
            }
        }

        string telefon;
        public string Telefon { get => telefon; set
            {
                if (telefon == value)
                {
                    return;
                }

                telefon = value;
                ManualUpdateData();
            }
        }

        string mobil;
        public string Mobil { get => mobil; set
            {
                if (mobil == value)
                {
                    return;
                }

                mobil = value;
                ManualUpdateData();
            }
        }

        bool ältester = true;
        public bool Ältester { get => ältester; set
            {
                if (ältester == value)
                {
                    return;
                }

                ältester = value;
                ManualUpdateData();
            }
        }
        bool aktiv = true;
        public bool Aktiv { get => aktiv; set
            {
                if (aktiv == value)
                {
                    return;
                }

                aktiv = value;
                ManualUpdateData();
            }
        }
        bool einladen = true;
        public bool Einladen { get => einladen; set
            {
                if (einladen == value)
                {
                    return;
                }

                einladen = value;
                ManualUpdateData();
            }
        }
        int abstand = 4;
        public int Abstand { get => abstand; set
            {
                if (abstand == value)
                {
                    return;
                }

                abstand = value;
                ManualUpdateData();
            }
        }
        string infoPrivate;
        public string InfoPrivate { get => infoPrivate; set
            {
                if (infoPrivate == value)
                {
                    return;
                }

                infoPrivate = value;
                ManualUpdateData();
            }
        }

        string infoPublic;
        public string InfoPublic { get => infoPublic; set
            {
                if (infoPublic == value)
                {
                    return;
                }

                infoPublic = value;
                ManualUpdateData();
            }
        }

        public BitmapSource Foto { get; set; }

        public override string ToString() => $"{Name}";

        public void TalkRemove(TalkSong talk)
        {
            Vorträge.Remove(talk);
            ManualUpdateData();
        }

        public void TalkAdd(TalkSong talk)
        {
            Vorträge.Add(talk);
            ManualUpdateData();
        }

        private void ManualUpdateData()
        {
            if (!flagIsUpdateDoneByUser)
                return;

            if (Versammlung != null)
                Versammlung.Aktualisierung = DateTime.Now;
        }
    }
}