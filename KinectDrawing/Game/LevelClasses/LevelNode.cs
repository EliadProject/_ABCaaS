using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDrawing.Game.LevelClasses
{
    class LevelNode
    {
        public LevelNode next;
        public char letter;
        public char getLetter()
        {
            return this.letter;
        }
    }
    
}

