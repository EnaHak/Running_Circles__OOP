using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    /// <summary>
    /// Game options
    /// </summary>
    static class GameOptions
    {
        public static int SpriteHeight = 100;
        public static int SpriteWidth = 100;
        public static int Speed = 5;

        public static int LeftEdge = 0;
        public static int RightEdge = 1100;
        public static int UpEdge = 0;
        public static int DownEdge = 690;

        public static void PocetnePozicije(Circle[] krugovi, Coin[,] novcici, Devil[] vragovi,Angel angeo,  ref int[] lijevodesno, int brzina)
        {
            angeo.SetVisible(true);
            krugovi[0].Y= 445;
            krugovi[1].Y =206;
            krugovi[2].Y = -32;
            krugovi[3].Y= -274;
            Random random = new Random();
            for (int i = 0; i < krugovi.Length; i++)
            {
                krugovi[i].SetVisible(true);
                vragovi[i].SetVisible(true);
                krugovi[i].Brzina = brzina;
                if (random.Next(0, 2) == 0) lijevodesno[i] = 1;
                else lijevodesno[i] = -1;
                if (i != 0) vragovi[i].Kut = random.Next(0, 9);
                for (int j = 0; j < 5; j++)
                {
                    novcici[i, j].Skupljen = false;
                    novcici[i, j].SetVisible(true);
                    novcici[i, j].Kut = random.Next(0, 9);
                }
            }
        }

        public static void Kretanje(Circle[] krugovi, Devil[] vragovi, Angel andeo, int[] lijevodesno, double radius)
        {
            for (int i = 1; i < vragovi.Length; i++)
            {
                vragovi[i].Kut += vragovi[i].Brzina * lijevodesno[i];
                vragovi[i].X = (krugovi[i].X + krugovi[i].Width / 2) + (int)Math.Round(Math.Cos(vragovi[i].Kut) * radius) - vragovi[i].Heigth / 2 - 20;
                vragovi[i].Y = (krugovi[i].Y + krugovi[i].Heigth / 2) + (int)Math.Round(Math.Sin(vragovi[i].Kut) * radius) - vragovi[i].Width / 2 + 20;
            }
            if (andeo.Novcici > 9)
            {
                vragovi[0].SetVisible(true);
                vragovi[0].Kut += vragovi[0].Brzina * lijevodesno[0];
                vragovi[0].X = (krugovi[0].X + krugovi[0].Width / 2) + (int)Math.Round(Math.Cos(vragovi[0].Kut) * radius) - vragovi[0].Heigth / 2 - 20;
                vragovi[0].Y = (krugovi[0].Y + krugovi[0].Heigth / 2) + (int)Math.Round(Math.Sin(vragovi[0].Kut) * radius) - vragovi[0].Width / 2 + 20;
            }
        }

        public static void Kretanje(Circle[] krugovi, Coin[,] novcici, Angel andeo, int[] lijevodesno, double radius)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (!novcici[i, j].Skupljen)
                    {
                        novcici[i, j].X = (krugovi[i].X + krugovi[i].Width / 2) + (int)Math.Round(Math.Cos(novcici[i, j].Kut) * radius) - novcici[i, j].Heigth / 2;
                        novcici[i, j].Y = (krugovi[i].Y + krugovi[i].Heigth / 2) + (int)Math.Round(Math.Sin(novcici[i, j].Kut) * radius) - novcici[i, j].Width / 2;
                    }
                }

            }
        }
    }
}
