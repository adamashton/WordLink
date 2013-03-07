using System;
using System.Collections.Generic;
using System.Linq;

namespace WordLinker
{
    public class WordLink
    {
        /// <summary>List of words from a dictionary.</summary>
        private static HashSet<string> wordlist;

        /// <summary>a,b, ... z</summary>
        private static List<char> az;

        public WordLink(string wordlistFilename)
        {
            LoadDictionary(wordlistFilename);
            LoadCharAZ();
        }

        /// <summary>Finds a link between 2 words changing 1 char at a time. NULL if no link found.</summary>
        public List<string> Find(string start, string target)
        {
            if (!wordlist.Contains(start) || !wordlist.Contains(target))
                return null;

            // Breadth first search using a queue.
            // The stack is used to keep track of our walk from start to target
            Queue<Stack<string>> q = new Queue<Stack<string>>();

            Stack<string> firstStep = new Stack<string>();
            firstStep.Push(start);
            q.Enqueue(firstStep);

            Random r = new Random(); // randomizing the returned words should ensure a uniform run time.

            while (q.Count > 0)
            {
                Stack<string> currentWalk = q.Dequeue();
                string currentWord = currentWalk.Peek();

                List<string> nextSteps = OneStep(currentWord).OrderBy(x => r.Next()).ToList();
                foreach (string nextWord in nextSteps)
                {
                    if (currentWalk.Contains(nextWord))
                        continue; // already used this word, don't want to walk backwards

                    Stack<string> newWalk = new Stack<string>(currentWalk.Reverse());
                    newWalk.Push(nextWord);

                    if (nextWord == target)
                    {
                        // happy days
                        return new List<string>(newWalk);
                    }

                    q.Enqueue(newWalk);
                }
            }

            return null;
        }

        /// <summary>Returns words that are 1 char away from the word given.</summary>
        /// <remarks>Brute force atm :/</remarks>
        private static List<string> OneStep(string word)
        {
            List<string> result = new List<string>();
            char[] wordArr = word.ToCharArray();
            char[] possibleWordArr = new char[wordArr.Length];

            for (int i = 0; i < wordArr.Length; i++)
            {
                wordArr.CopyTo(possibleWordArr, 0);

                foreach (char c in az)
                {
                    possibleWordArr[i] = c;
                    string possibleWord = new string(possibleWordArr);
                    if (wordlist.Contains(possibleWord))
                    {
                        result.Add(possibleWord);
                    }
                }
            }
            return result;
        }

        private void LoadDictionary(string location)
        {
            wordlist = new HashSet<string>();
            foreach (string word in System.IO.File.ReadAllLines(location))
            {
                wordlist.Add(word);
            }
        }

        private static void LoadCharAZ()
        {
            az = new List<char>();
            for (int i = 'a'; i <= 'z'; i++)
            {
                az.Add((char)i);
            }
        }
    }
}
