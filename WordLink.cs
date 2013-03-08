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

        /// <summary>Constructs a WordLinker class to find paths between 2 words.</summary>
        /// <param name="wordlistFilename">The full file path to the wordlist to use.</param>
        /// <param name="changeableWordSize">Can the words grow and shrink as you attempt to get to the target word?</param>
        public WordLink(string wordlistFilename, bool changeableWordSize)
        {
            this.changeableWordSize = changeableWordSize;
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

            while (q.Count > 0)
            {
                Stack<string> currentWalk = q.Dequeue();
                string currentWord = currentWalk.Peek();

                foreach (string nextWord in GetAdjacentNodes(currentWord))
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
                        return new List<string>(newWalk.Reverse());
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

        /// <summary>Can the word size grow/shrink by 1 character?</summary>
        private bool changeableWordSize;

        /// <summary>Returns all the valid words that are 1 edge away from the word given.</summary>
        private IEnumerable<string> GetAdjacentNodes(string word)
        {
            List<string> result = null;
            if (_oneStepMemoization.TryGetValue(word, out result))
            {
                return result;
            }

            result = Onestep(word);

            if (this.changeableWordSize)
            {
                result.AddRange(OneStepExtraCharacter(word));
                result.AddRange(OneStepLessCharacter(word));
            }

            _oneStepMemoization.TryAdd(word, result);
            return result;
        }

        /// <summary>Returns words that are 1 char away from the word given.</summary>
        private List<string> Onestep(string word)
        {
            List<string> result;
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
            return result;
        }

        /// <summary>Returns all valid words where 1 character has been removed.</summary>
        private IEnumerable<string> OneStepLessCharacter(string word)
        {
            List<string> result = new List<string>();
            char[] wordArr = word.ToCharArray();
            char[] possibleWordArr = new char[wordArr.Length - 1];

            for (int i = 0; i < wordArr.Length; i++)
            {
                for (int j = 0; j < possibleWordArr.Length; j++)
                {
                    if (j != i)
                    {
                        possibleWordArr[j] = wordArr[j];
                    }
                }

                string possibleWord = new string(possibleWordArr);
                if (WordList.Contains(possibleWord))
                {
                    result.Add(possibleWord);
                }
            }
            return result;
        }

        /// <summary>Returns all valid words where 1 character has been added.</summary>
        private IEnumerable<string> OneStepExtraCharacter(string word)
        {
            List<string> result = new List<string>();

            char[] wordArr = word.ToCharArray();
            char[] possibleWordArr = new char[wordArr.Length + 1];

            for (int i = 0; i < wordArr.Length; i++)
            {
                int offset = 0;
                for (int j = 0; j < possibleWordArr.Length; j++)
                {
                    if (j != i)
                    {
                        possibleWordArr[j] = wordArr[j - offset];
                    }
                    else
                    {
                        possibleWordArr[j] = '_'; // this is the extra character position
                        offset = 1;
                    }
                }

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
