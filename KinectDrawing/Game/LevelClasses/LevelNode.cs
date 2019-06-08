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
        public string letter;
        public string getLetter()
        {
            return this.letter;
        }
    }
    
}

