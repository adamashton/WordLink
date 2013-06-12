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
	Console.WriteLine(string.Join(",", result));
}
```
Output
======
```
beer, deer, dee, die, dine, wine
```
