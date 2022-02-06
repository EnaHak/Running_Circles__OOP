using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    //apstraktna
    public abstract class Igra1: Sprite
    {
        protected double _brzina;
        public double Brzina
        {
            get { return _brzina;; }
            set { _brzina = value; }
        }

        public Igra1(string s, int x, int y) : base(s, x, y)
        {
            this._brzina =1;
        }
    }

    //klasa za krugove 
    public class Circle : Igra1
    {  
        public Circle(string s, int x, int y) : base(s, x, y)
        {
            this.Brzina = 1;
        }
     
        public event EventHandler teleport;

        public override int Y
        {
            get
            {
                return base.Y;
            }
            set
            {
                if (base.Y >= 685)
                {
                    base.Y = -275;
                    teleport(this, null);
                }
                else
                {
                    base.Y =value;
                }
            }
        }
    }
    
    //klasa za andela
    public class Angel : Igra1
    {   
        private double kut;
        public double Kut
        {
            get { return kut; }
            set { kut = value; }
        }

        private int novcici;
        public int Novcici
        {
            get { return novcici; }
            set { novcici = value; }
        }

        private int _zivoti;
        public int Zivoti
        {
            get { return _zivoti; }
            set
            {
                if(value ==0)
                {
                    throw new ArgumentException();
                }
                else
                {
                    _zivoti = value;
                }
            }
        }

        public Angel(string s, int x,int y) : base(s, x, y)
        {
            this.Brzina = 1;
            this.Novcici = 0;
            this.Zivoti = 3;
        }      
    }

    //klasa za vraga
    public class Devil : Igra1
    {      
        private double kut;
        public double Kut
        {
            get { return kut; }
            set { kut = value; }
        }
        
        public Devil(string s, int x,int y) : base(s, x, y)
        {
            this.Brzina = 1;
        }
    }

    //klasa za novcice
    public class Coin : Igra1
    {      
        private double kut;
        public double Kut
        {
            get { return kut; }
            set { kut = value; }
        }

        public bool Skupljen;

        public Coin(string s, int x, int y) : base(s, x, y)
        {
            this.Brzina = 1;
        }
    }
}
