﻿using KinectDrawing.Game.LevelClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDrawing.Game
{
    static class GameFlow
    {
        static public LevelNode createGameFlow()
        {
            LevelsLinkedList levelsLinkedList = new LevelsLinkedList('B');
            levelsLinkedList.AddToLast('C');
            levelsLinkedList.AddToLast('L');
            levelsLinkedList.AddToLast('S');
            levelsLinkedList.AddToLast('B');
            levelsLinkedList.AddToLast('D');

            return levelsLinkedList.getHead();
        }
    }
}