using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OTTER
{
    /// <summary>
    /// -
    /// </summary>
    public partial class BGL : Form
    {
        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {                
                foreach (Sprite sprite in allSprites)
                {                    
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            Init();
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.Transparent);
          
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 18);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Gold, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;            
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion

        //za ispis
        Sprite btnStart, btnEasy, btnHard, devil1, andeo2, andeo3, novcic, circle2, circle4;

        Angel andeo;
        Devil[] vragovi= new Devil[4];        
        Circle[] krugovi = new Circle[4];
     
        Coin[,] novcici = new Coin[4,5];
        int brojac = 0;
        double radius;
        int[] lijevodesno=new int[4];
        Random rng = new Random();
        bool IGRA, IZBORNIK;
        int makismalno=0, rezultat;
        private void SetupGame()
        {
            SetStageTitle("Anđeli");            
            setBackgroundPicture("backgrounds\\trava.jpg");
            setPictureLayout("stretch");

            btnStart = new Sprite("sprites\\PLAY.png", 0, 0);
            Game.AddSprite(btnStart);
            btnEasy = new Sprite("sprites\\EASY.png", 2000, 0);
            btnEasy.SetSize(85);
            Game.AddSprite(btnEasy);
            btnHard = new Sprite("sprites\\HARD.png", 2000, 0);
            btnHard.SetSize(85);
            Game.AddSprite(btnHard);
            btnStart.X = GameOptions.RightEdge/2 - btnStart.Width / 2;
            btnStart.Y = GameOptions.DownEdge/2 - btnStart.Heigth / 2;
            //NIZOVI KRUZNICA
            krugovi[0] = new Circle("sprites\\circle.png", 350, 445);
            krugovi[0].SetSize(39);
            Game.AddSprite(krugovi[0]);
            krugovi[1] = new Circle("sprites\\circle.png", 350, 206);
            krugovi[1].SetSize(39);
            Game.AddSprite(krugovi[1]);
            krugovi[2] = new Circle("sprites\\circle.png", 350, -32);
            krugovi[2].SetSize(39);
            Game.AddSprite(krugovi[2]);
            krugovi[3] = new Circle("sprites\\circle.png", 350, -274);
            krugovi[3].SetSize(39);
            Game.AddSprite(krugovi[3]);
            //NOVCICI
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    novcici[i,j]=new Coin("sprites\\coin.png", 0, -0);
                    novcici[i, j].SetSize(50);
                    novcici[i, j].Skupljen = false;
                    Game.AddSprite(novcici[i,j]);
                }
            }

            //ISPIS
            novcic = new Sprite("sprites\\coin.png", 8, 0);
            novcic.SetSize(40);
            Game.AddSprite(novcic);

            andeo2 = new Sprite("sprites\\Picture2.png", 0, 50);
            andeo2.SetSize(60);
            Game.AddSprite(andeo2);

            andeo3 = new Sprite("sprites\\andeoispis.png", 200, 220);
            andeo3.SetSize(60);
            Game.AddSprite(andeo3);

            devil1 = new Sprite("sprites\\devilispis.png", 777, 217);
            devil1.SetSize(40);
            Game.AddSprite(devil1);

            circle2 = new Sprite("sprites\\circle.png", 115, 295);
            circle2.SetSize(40);
            Game.AddSprite(circle2);

            circle4 = new Sprite("sprites\\circle.png", 660, 295);
            circle4.SetSize(40);
            Game.AddSprite(circle4);


            //ANDEO
            andeo = new Angel("sprites\\Picture2.png", 0, 0);
            andeo.SetSize(80);
            Game.AddSprite(andeo);
            andeo.Kut = 0;

            //VRAG
            for (int i = 0; i < vragovi.Length; i++)
            {
                vragovi[i] = new Devil("sprites\\devilll.png", 0, 0);
                vragovi[i].SetSize(8);
                if (rng.Next(0, 2) == 0) lijevodesno[i] = 1;
                else lijevodesno[i] = -1;
                vragovi[i].Brzina = 0.02;
                Game.AddSprite(vragovi[i]);
            }
            vragovi[0].SetVisible(false);
            
            radius = (krugovi[1].Width / 2) -40;
            foreach (Circle item in krugovi)
            {
                item.teleport+= new EventHandler(Postavicoinvrag);
            }
            Sakrijlikove();
            IZBORNIK = true;
            Game.StartScript(Izbornik);
        }

        private int Izbornik()
        {
            while (IZBORNIK)
            {
                
                IGRA = false;
                ISPIS = "";
                andeo2.SetVisible(false);
                circle2.X = 115;
                circle2.Y = 295;
                andeo3.X = 200;
                andeo3.Y = 220;
                circle4.X = 660;
                circle4.Y = 295;
                devil1.X = 777;
                devil1.Y = 217;
            
                if (sensing.MouseDown)
                {
                    if (btnStart.Clicked(sensing.Mouse))
                    {
                        btnStart.X = 2000;
                        btnEasy.X = GameOptions.RightEdge / 2 - btnEasy.Width / 2-5;
                        btnEasy.Y = GameOptions.DownEdge / 2 - btnEasy.Heigth+1;
                        btnHard.X = GameOptions.RightEdge / 2 - btnHard.Width / 2;
                        btnHard.Y = GameOptions.DownEdge / 2;
                       
                        Wait(0.5);
                    }
                    else if (btnEasy.Clicked(sensing.Mouse))
                    {
                        IZBORNIK = false;
                        IGRA = true;
                        ShowButtonsAndLabels("");
                        circle2.SetVisible(false);
                        circle4.SetVisible(false);
                        andeo3.SetVisible(false);
                        devil1.SetVisible(false);
                        btnHard.X = 2000;
                        btnEasy.X = 2000;
                        setBackgroundPicture("backgrounds\\sky.jpg");
                        setPictureLayout("stretch");
                        andeo2.SetVisible(true);
                        novcic.SetVisible(true);
                        GameOptions.PocetnePozicije(krugovi, novcici, vragovi, andeo, ref lijevodesno, 1);
                        Game.StartScript(KretanjeAndeo);
                        Game.StartScript(KretanjeKrugova);
                        Game.StartScript(KretanjeVragova);
                        Game.StartScript(KretanjeNovcica);
                        Game.StartScript(UzeoNovcic);
                        Game.StartScript(VragUdarac);
                        Game.StartScript(IspaoSEkrana);
                       
                        Wait(0.1);
                    }
                    else if (btnHard.Clicked(sensing.Mouse))
                    {
                        IZBORNIK = false;
                        IGRA = true;
                        circle2.SetVisible(false);
                        circle4.SetVisible(false);
                        andeo3.SetVisible(false);
                        devil1.SetVisible(false);
                        ShowButtonsAndLabels("");
                        btnHard.X = 2000;
                        btnEasy.X = 2000;
                        setBackgroundPicture("backgrounds\\sky.jpg");
                        setPictureLayout("stretch");
                        andeo2.SetVisible(true);
                        novcic.SetVisible(true);
                        GameOptions.PocetnePozicije(krugovi, novcici, vragovi, andeo, ref lijevodesno, 3);
                        Game.StartScript(KretanjeAndeo);
                        Game.StartScript(KretanjeKrugova);
                        Game.StartScript(KretanjeVragova);
                        Game.StartScript(KretanjeNovcica);
                        Game.StartScript(UzeoNovcic);
                        Game.StartScript(VragUdarac);
                        Game.StartScript(IspaoSEkrana);
                      
                        Wait(0.1);
                    }
                }
                Wait(0.01);
            }
            return 0;
        }
        
        private void Postavicoinvrag(Object sender, EventArgs e)
        {
            for (int i = 0; i < krugovi.Length; i++)
            {
                if (sender == krugovi[i])
                {
                    if (rng.Next(0, 2) == 0) lijevodesno[i] = 1;
                    else lijevodesno[i] = -1;
                    vragovi[i].Kut =rng.Next(0, 9);
                    vragovi[i].Brzina += 0.003;
                    
                    for (int j = 0; j < 5; j++)
                    {
                        novcici[i, j].Skupljen = false;
                        novcici[i, j].SetVisible(true);
                        novcici[i, j].Kut = rng.Next(0, 9);
                    }
                }
            }
            
        }
        private void Sakrijlikove()
        {
            andeo.Zivoti = 3;
            brojac = 0;
            andeo.Novcici = 0;
            andeo.Kut = 0;
            andeo.SetVisible(false);
            andeo2.SetVisible(false);
            novcic.SetVisible(false);
            for (int i = 0; i < 4; i++)
            {
                krugovi[i].SetVisible(false);
                vragovi[i].SetVisible(false);
                for (int j = 0; j < 5; j++)
                {
                    novcici[i, j].SetVisible(false);
                }
            }
            vragovi[0].X = 2000;
        }
        private int KretanjeAndeo()
        {
            while (IGRA)
            {
                ISPIS = "        " + andeo.Novcici + "\n\n        "+ andeo.Zivoti;
                
                andeo.Y = (krugovi[brojac].Y + krugovi[brojac].Heigth / 2) + (int)Math.Round(Math.Sin(andeo.Kut) * radius) -andeo.Width/2;
                andeo.X = (krugovi[brojac].X + krugovi[brojac].Width / 2) + (int)Math.Round(Math.Cos(andeo.Kut) * radius) - andeo.Heigth / 2;
                if (sensing.KeyPressed(Keys.Left))
                {
                    andeo.Kut += 0.03;
                    Wait(0.01);
                }
                else if (sensing.KeyPressed(Keys.Right))
                {
                    andeo.Kut -= 0.03;
                    Wait(0.01);
                }
                else if (sensing.KeyPressed(Keys.Up))
                {
                    if (Math.Abs(andeo.Y - krugovi[brojac].Y) < 10)
                    {
                        Wait(0.2);
                        brojac++;
                        andeo.Kut = 7.92;
                        if (brojac > 3)
                        {
                            brojac = 0;
                        }
                    }         
                }
                Wait(0.01);                                       
            }
            return 0;
        }
    
        private int KretanjeKrugova()
        {
            while (IGRA)
            {
                for (int i = 0; i < krugovi.Length; i++)
                {
                    krugovi[i].Y += (int)krugovi[i].Brzina;
                }
                    Wait(0.1);
            }
            return 0;
        }

        private int KretanjeVragova()
        {
            while (IGRA)
            {
                GameOptions.Kretanje(krugovi, vragovi, andeo, lijevodesno, radius);
                Wait(0.04);
            }
            return 0;
        }
        
        private int KretanjeNovcica()
        {
            while (IGRA)
            {
                GameOptions.Kretanje(krugovi, novcici, andeo, lijevodesno, radius);
                Wait(0.04);
            }
            return 0;
        }

        private int UzeoNovcic()
       {
        while (IGRA)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                        if (andeo.TouchingSprite(novcici[i, j]) && !novcici[i, j].Skupljen)
                        {
                            novcici[i, j].Skupljen = true;
                            novcici[i, j].SetVisible(false);
                            novcici[i, j].X = 2000;
                            andeo.Novcici++;
                        }

                    }
            }
            Wait(0.1);
        }
        return 0;
    }
        private int VragUdarac()
        {
            while (IGRA)
            {
                for (int i = 0; i < vragovi.Length; i++)
                {
                    if (andeo.TouchingSprite(vragovi[i]))
                    {
                        try
                        {
                            andeo.Zivoti--;
                            andeo.SetVisible(false);
                            Wait(0.2);
                            andeo.SetVisible(true);
                            Wait(0.2);
                            andeo.SetVisible(false);
                            Wait(0.2);
                            andeo.SetVisible(true);
                            Wait(0.2);
                            andeo.SetVisible(false);
                            Wait(0.2);
                            andeo.SetVisible(true);
                            Wait(0.2);
                        }
                        catch (ArgumentException)
                        {
                            IGRA = false;
                            IZBORNIK = true;
                            Wait(1);
                            if (andeo.Novcici > makismalno) makismalno = andeo.Novcici;
                            ShowButtonsAndLabels("Uhvatili su vas vragovi\n  Broj novcica: "+ andeo.Novcici+"\nNajbolji rezultat: "+makismalno);
                            Sakrijlikove();
                            btnStart.X = GameOptions.RightEdge / 2 - btnStart.Width / 2;
                            btnStart.Y = GameOptions.DownEdge / 2 - btnStart.Heigth / 2;
                            setBackgroundPicture("backgrounds\\trava.jpg");
                            setPictureLayout("stretch");

                            circle2.SetVisible(true);
                            circle4.SetVisible(true);
                            andeo2.SetVisible(true);
                            andeo3.SetVisible(true);
                            devil1.SetVisible(true);
                           
                            Game.StartScript(Izbornik);
                        }
                        Wait(3);
                    }
                }
               
                Wait(0.04);

            }
            return 0;

        }

        private int IspaoSEkrana()
        {
            while (IGRA)
            {
                if (andeo.Y > 700 || andeo.Y < GameOptions.UpEdge)
                {
                    IGRA = false;
                    IZBORNIK = true;
                    Wait(1);
                    if (andeo.Novcici > makismalno) makismalno = andeo.Novcici;
                    ShowButtonsAndLabels("Ispali ste s ekrana\n  Broj novcica: " + andeo.Novcici + "\nNajbolji rezultat: " + makismalno);
                    andeo.Y = GameOptions.DownEdge / 2;
                    Sakrijlikove();
                    btnStart.X = GameOptions.RightEdge / 2 - btnStart.Width / 2;
                    btnStart.Y = GameOptions.DownEdge / 2 - btnStart.Heigth / 2;
                    setBackgroundPicture("backgrounds\\trava.jpg");
                    setPictureLayout("stretch");
                    circle2.SetVisible(true);
                    circle4.SetVisible(true);
                    andeo2.SetVisible(true);
                    andeo3.SetVisible(true);
                    devil1.SetVisible(true);
                 
                    Game.StartScript(Izbornik);
                }
                Wait(0.5);
            }
            return 0;
        }

        delegate void ShowButtonsAndLabels_delegat(string text);
        private void ShowButtonsAndLabels(string text)
        {
          
            if (this.label1.InvokeRequired)
            {
                //ako proces koji nije stvorio kontrole mijenja kontrole, onda stvaramo novu:
                ShowButtonsAndLabels_delegat d = new ShowButtonsAndLabels_delegat(ShowButtonsAndLabels);
                this.Invoke(d, new object[] { text });
            }
            else if (text == "")
            {
                label1.Visible = false;
            }
            else
            {
                //ako proces koji je stvorio kontrole mijenja kontrole:
                label1.Visible = true;
                this.label1.Text = text;
            }
        }






        /* ------------ GAME CODE END ------------ */


    }
}
