using KinectDrawing.Game.LevelClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDrawing.Game
{
    static class GameFlow
    {
        static public LevelNode createGameFlow()
        {

            //first section - gets letter from the model
            String currentPath = System.IO.Directory.GetCurrentDirectory();
            String[] directories = System.IO.Directory.GetDirectories(currentPath + "/categories");
            Array.Sort(directories);
            LevelsLinkedList levelsLinkedList = new LevelsLinkedList(new DirectoryInfo(directories[0]).Name);
            levelsLinkedList.AddToLast("DO");
            levelsLinkedList.AddToLast("DOLL");
            for (int i = 1; i < directories.Length; i++)
            {
                var dir = new DirectoryInfo(directories[i]);
                var dirName = dir.Name;
                levelsLinkedList.AddToLast(dirName);
            }

            levelsLinkedList.AddToLast("DOG");
            levelsLinkedList.AddToLast("CAT");
            levelsLinkedList.AddToLast("THE");


            return levelsLinkedList.getHead().next;
        }
    }
}
