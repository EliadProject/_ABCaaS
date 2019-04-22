using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDrawing.Game.LevelClasses
{
    class Level1 : Level
    {
        public Level1()
        {
            this.letter = 'B';
        }

        public override string next()
        {
            return "level2";
        }
    }
}
