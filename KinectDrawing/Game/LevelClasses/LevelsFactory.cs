using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDrawing.Game.LevelClasses
{
    static class LevelsFactory
    {
        static public Level getLevel(String level)
        {
            switch (level)
            {
                case "level1":
                    return new Level2();
                case "level2":
                    return new Level2();
                default:
                    return null;
            }

        }
    }
}
