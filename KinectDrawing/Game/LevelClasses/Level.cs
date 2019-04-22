using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDrawing.Game.LevelClasses
{
    abstract class Level
    {
        protected char letter;
        public char getLetter() {
            return letter;
        }
        abstract public String next();
    }
}
