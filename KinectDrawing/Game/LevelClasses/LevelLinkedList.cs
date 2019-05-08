using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDrawing.Game.LevelClasses
{
    class Level1LinkedList
    {
        private LevelNode head; // the first node
        private LevelNode tail; //the last node in the list
        public Level1LinkedList(LevelNode levelNode)
        {
            this.head = new LetterLevelNode();
            this.head.le = letter;
            this.head.letter = letter;
            currentNode = head;
        }
       
        public LevelNode getHead()
        {
            return head;
        }
        public void printAllNodes()
        {
            LetterLevelNode current = head;
            while (current != null)
            {
                Console.WriteLine(current.letter);
                current = current.next;
            }
        }

        public void AddToLast(char letter)
        {
            LetterLevelNode toAdd = new LetterLevelNode();
            toAdd.letter = letter;
            currentNode.next = toAdd;
            currentNode = currentNode.next;
        }
    }
}
