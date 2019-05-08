using KinectDrawing.Game.LevelClasses;
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
            //level1 - letters

            
            //createing levels
            LevelNode level1Letter = new LevelNode('A');
            LevelNode level2Letter = new LevelNode('B');
            LevelNode level3Letter = new LevelNode('C');
            LevelNode level4Letter = new LevelNode('D');
            LevelNode level5Letter = new LevelNode('E');
            LevelNode level6Letter = new LevelNode('F');
            LevelNode level7Letter = new LevelNode('G');
            LevelNode level8Letter = new LevelNode('H');

            LevelsLinkedList levelsLinkedList = new LevelsLinkedList(level1Letter);
            levelsLinkedList.AddToLast(level2Letter);
            levelsLinkedList.AddToLast(level3Letter);
            levelsLinkedList.AddToLast(level4Letter);
            levelsLinkedList.AddToLast(level5Letter);
            levelsLinkedList.AddToLast(level6Letter);
            levelsLinkedList.AddToLast(level7Letter);
            levelsLinkedList.AddToLast(level8Letter);
       
            //Level2 - words

            //same for words
            return levelsLinkedList.getHead();
        }
    }
}
