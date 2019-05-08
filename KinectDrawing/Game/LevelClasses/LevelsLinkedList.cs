﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDrawing.Game.LevelClasses
{
    class LevelsLinkedList
    {
        private LevelNode head; // the first node
        private LevelNode currentNode; //the last node in the list
        public LevelsLinkedList(LevelNode levelNode)
        {
            this.head= levelNode;
            currentNode = head;
        }

        public LevelNode getHead()
        {
            return head;
        }
        public void printAllNodes()
        {
            LevelNode current = head;
            while (current != null)
            {
                Console.WriteLine(current.ToString());
                current = current.next;
            }
        }

        public void AddToLast(LevelNode levelNode)
        {
            currentNode.next = levelNode;
            currentNode = currentNode.next;
        }
    }
}
