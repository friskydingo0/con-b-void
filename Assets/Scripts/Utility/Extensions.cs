using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
	public static T RandomSelect<T>(this List<T> list)
	{
		int selection = Random.Range(0, list.Count);
		return list[selection];
	}
}
