using System;
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
        public LevelsLinkedList(char letter)
        {
            this.head = new LevelNode();
            this.head.letter = letter;
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
                Console.WriteLine(current.letter);
                current = current.next;
            }
        }

        public void AddToLast(char letter)
        {
            LevelNode toAdd = new LevelNode();
            toAdd.letter = letter;
            currentNode.next = toAdd;
            currentNode = currentNode.next;
        }
    }
}
