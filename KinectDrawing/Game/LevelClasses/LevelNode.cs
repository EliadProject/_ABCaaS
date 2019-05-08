using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KinectDrawing.Game.LevelClasses
{
    class LevelNode
    {
        public int sectionLevel;
        public PngBitmapEncoder encoder = new PngBitmapEncoder();
        public char[] letters;
        //for section one - letters
        public LevelNode(char letter)
        {
            this.sectionLevel = 1;
            this.letters = new char[1];
            this.letters[0] = letter;
        }
        //for section 2 - words
        public LevelNode(char[] letters)
        {
            this.sectionLevel = 2;
            //worst case word
            this.letters = new char[10];
            this.letters = letters;
        }

        //for section 3 - words & images
        public LevelNode(char[] letters, PngBitmapEncoder encoder)
        {
            this.sectionLevel = 3;
            this.letters = new char[10];
            this.letters = letters;
            this.encoder = encoder;
        }

        public LevelNode next;
        public LevelNode GetNode()
        {
            return this;
        }

    }
    
}

