WordLink
========

Attempts to link two words by changing one char at a time on each word to obtain another word.

Usage
=====
```
WordLink wl = new WordLink(@"wordlist.txt");
List<string> result = wl.Find("beer", "good");
if (result != null)
{
	String sep = string.Empty;
	foreach (string word in result)
	{
		Console.Write(sep);
		sep = ", ";
		Console.Write(word);
	}
	Console.WriteLine();
}
```
Output
======
``beer, boer, boor, boon, goon, good``
