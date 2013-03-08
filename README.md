WordLink
========

Attempts to link two words by changing one char at a time on each word to obtain another word.

Usage
=====
```
WordLink wl = new WordLink(@"wordlist.txt", true);
List<string> result = wl.Find("beer", "wine");
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
```
beer, deer, dee, die, dine, wine
```
