using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using bobStuff;

[ExecuteInEditMode]

public class TagScript : MonoBehaviour
{
	public static Dictionary<string, int> TagIntMap;
	public static Dictionary<int, string> TagStringMap;
	public static bool initialized = false;


	public List<int> tags;
    // Start is called before the first frame update
    public void Awake()
    {
		if (!initialized)
		{
			initialized = true;
			InitializeTagMap();
		}
	}

	public bool ContainsTag(int[] tag) {
		foreach(int i in tag)
		{
			if (tags.Contains(tag[i]))
			{
				return true;
			}
		}
		return false;
	}
	public bool ContainsTag(string[] tag)
	{
		foreach (string i in tag)
		{
			if (tags.Contains(TagIntMap[i]))
			{
				return true;
			}
		}
		return false;
	}
	public bool ContainsTag(string tag) { return ContainsTag(TagIntMap[tag]); }
	public bool ContainsTag(int tag)
	{
		return tags.Contains(tag);
	}

	private static void InitializeTagMap()
	{
		TextAsset ta = Resources.Load<TextAsset>("tags");
		string tagText = ta.text;
		string[] lines = tagText.Split(
			new[] { Environment.NewLine, "\r\n", "\r", "\n" },
			StringSplitOptions.None
		);
		TagIntMap = new Dictionary<string, int>();
		TagStringMap = new Dictionary<int, string>();
		int invalidLines = 0;
		foreach (string s in lines)
		{
			int index = s.IndexOf(':');
			if (index > -1)
			{
				int indexEnd = s.IndexOf('/');
				if (indexEnd == -1) indexEnd = s.Length;
				string temp = s.Substring(0, index);
				int tempi = int.Parse(s.Substring(index + 1, indexEnd - (index + 1)));
				TagIntMap.Add(temp, tempi);
				TagStringMap.Add(tempi, temp);
			}
			else
			{
				if (s.Length > 0 && s[0] != '/')
				{
					invalidLines++;
				}

			}

		}
		print("Initialized tags with " + invalidLines + " invalid tags");


	}


	public static int TagToId(string s, out bool succeed)
	{
		if (TagIntMap.ContainsKey(s))
		{
			succeed = true;
			return TagIntMap[s];
		}
		else
		{
			succeed = false;
			return -1;
		}
	}
	public static string IdTotag(int i, out bool succeed)
	{
		if (TagStringMap.ContainsKey(i))
		{
			succeed = true;
			return TagStringMap[i];
		}
		else
		{
			succeed = false;
			return "|error|";
		}
	}
}
