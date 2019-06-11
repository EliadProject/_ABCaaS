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
            String[] directories = System.IO.Directory.GetDirectories(currentPath + "\\levels\\1");
            Array.Sort(directories);
            LevelsLinkedList levelsLinkedList = new LevelsLinkedList(new DirectoryInfo(directories[0]).Name);
            /*
            levelsLinkedList.AddToLast("C");
            levelsLinkedList.AddToLast("S");
            levelsLinkedList.AddToLast("CD");
            levelsLinkedList.AddToLast("DO");
            levelsLinkedList.AddToLast("DOLL");
            levelsLinkedList.AddToLast("DB");
            */
            for (int i = 0; i < directories.Length; i++)
            {
                var dir = new DirectoryInfo(directories[i]);
                var dirName = dir.Name;
                levelsLinkedList.AddToLast(dirName);
            }
            String[] directories2 = System.IO.Directory.GetDirectories(currentPath + "\\levels\\2");
            for (int i = 0; i < directories2.Length; i++)
            {
                var dir = new DirectoryInfo(directories2[i]);
                var dirName = dir.Name;
                levelsLinkedList.AddToLast(dirName);
            }
            String[] directories3 = System.IO.Directory.GetDirectories(currentPath + "\\levels\\3");
            for (int i = 0; i < directories3.Length; i++)
            {
                var dir = new DirectoryInfo(directories3[i]);
                var dirName = dir.Name;
                levelsLinkedList.AddToLast(dirName);
            }



            return levelsLinkedList.getHead().next;
        }
    }
}
