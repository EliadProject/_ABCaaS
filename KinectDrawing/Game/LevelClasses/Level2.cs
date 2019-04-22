using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDrawing.Game.LevelClasses
{
    class Level2 : Level
    {
        public Level2()
        {
            this.letter = 'C';
        }
        public override string next()
        {
            return "level3";
        }

    }
}
