using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SeedFinder
{
    public class WordLink
    {
        /// <summary>List of words from a dictionary.</summary>
        public HashSet<string> WordList { get; private set; }

        /// <summary>a,b, ... z</summary>
        private List<char> az;

        public WordLink(string wordlistFilename)
        {
            LoadDictionary(wordlistFilename);
            LoadCharAZ();
        }

        /// <summary>Finds a link between 2 words changing 1 char at a time. NULL if no link found.</summary>
        public List<string> Find(string start, string target)
        {
            if (!WordList.Contains(start) || !WordList.Contains(target))
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

                    bool addNewWalk = true;
                    foreach (Stack<string> otherWalk in q)
                    {
                        if (otherWalk.Contains(nextWord))
                        {
                            // there is another walk, otherWalk, who's length is <= to this walk that gets to the same word.
                            // No need to pursue this walk.
                            addNewWalk = false;
                            break;
                        }
                    }

                    Stack<string> newWalk = new Stack<string>(currentWalk.Reverse());
                    newWalk.Push(nextWord);

                    if (nextWord == target)
                    {
                        // happy days
                        return new List<string>(newWalk);
                    }

                    if (addNewWalk)
                    {
                        q.Enqueue(newWalk);
                    }
                }
            }

            return null;
        }

        /// <summary>A dictionary of params->result for the method OneStep.</summary>
        private ConcurrentDictionary<string, List<string>> _oneStepMemoization = new ConcurrentDictionary<string, List<string>>();

        /// <summary>Returns words that are 1 char away from the word given.</summary>
        /// <remarks>Brute force atm :/</remarks>
        private IEnumerable<string> OneStep(string word)
        {
            List<string> result = null;
            if (_oneStepMemoization.TryGetValue(word, out result))
            {
                return result;
            }

            result = new List<string>();
            char[] wordArr = word.ToCharArray();
            char[] possibleWordArr = new char[wordArr.Length];

            for (int i = 0; i < wordArr.Length; i++)
            {
                wordArr.CopyTo(possibleWordArr, 0);

                foreach (char c in az)
                {
                    possibleWordArr[i] = c;
                    string possibleWord = new string(possibleWordArr);
                    if (WordList.Contains(possibleWord))
                    {
                        result.Add(possibleWord);
                    }
                }
            }

            _oneStepMemoization.TryAdd(word, result);
            return result;
        }

        private void LoadDictionary(string location)
        {
            WordList = new HashSet<string>();
            foreach (string word in System.IO.File.ReadAllLines(location))
            {
                WordList.Add(word);
            }
        }

        private void LoadCharAZ()
        {
            az = new List<char>();
            for (int i = 'a'; i <= 'z'; i++)
            {
                az.Add((char)i);
            }
        }
    }
}
