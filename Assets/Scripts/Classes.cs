using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace bobStuff
{

	/// <summary>
	/// Item Catagorys. Usefull when deciding if you can equip something, place, ect.
	/// </summary>
	[System.Flags]
	public enum ItemToolType
	{
		/// <summary>
		/// Normal
		/// </summary>
		Normal = 1 << 0,
	};

	//TODO: maybe this should be used
	//public struct Tag
	//{
	//	ItemTag tag;//e.g. string is a tag that grass might have
	//	int value;//e.g. grass would have a low value, maybe 10 vs. the 100 of good string, because it is a bad string
	//}

	///// <summary>
	///// Item Catagorys. Usefull when deciding if you can equip something, place, ect.
	///// </summary>
	//[System.Flags]
	//public enum ItemTag
	//{
	//	/// <summary>
	//	/// Item for inventory only. Has no physical appearence.
	//	/// </summary>
	//	Item					= 1 << 0,
	//	/// <summary>
	//	/// Can be placed.
	//	/// </summary>
	//	Placeable				= 1 << 1,
	//	/// <summary>
	//	/// Can be Equipped on head.
	//	/// </summary>
	//	HeadEquip				= 1 << 2,
	//	/// <summary>
	//	/// Can be Equipped on chest.
	//	/// </summary>
	//	ChestEquip				= 1 << 3,
	//	/// <summary>
	//	/// Can be Equipped on right hand.
	//	/// </summary>
	//	HandREquip				= 1 << 4,
	//	/// <summary>
	//	/// Can be Equipped on left hand.
	//	/// </summary>
	//	HandLEquip				= 1 << 5,
	//	/// <summary>
	//	/// Can be Equipped on pant/legs.
	//	/// </summary>
	//	PantEquip				= 1 << 6,
	//	/// <summary>
	//	/// Can be Equipped on feet.
	//	/// </summary>
	//	FootEquip				= 1 << 7,

	//	All = ~0
	//};

	[System.Serializable]
	public struct Item
	{
		public int id;
		public int amount;
		public float strength;
		public float currentStrength;

		//public Item()
		//{
		//	id = 0;
		//	amount = 0;
		//	strength = 0;
		//	currentStrength = 0;
		//}

		public Item(int id, int amount, float duriation, float currentDuriation)
		{
			this.id = id;
			this.amount = amount;
			this.strength = duriation;
			this.currentStrength = currentDuriation;
		}
		public override string ToString()
		{
			return "Item Id: " + id + ", amount: " + amount;
		}
	}

	//public static class JsonHelper
	//{
	//	public static T[] FromJson<T>(string json)
	//	{
	//		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
	//		return wrapper.Items;
	//	}

	//	public static string ToJson<T>(T[] array)
	//	{
	//		Wrapper<T> wrapper = new Wrapper<T>();
	//		wrapper.Items = array;
	//		return JsonUtility.ToJson(wrapper);
	//	}

	//	public static string ToJson<T>(T[] array, bool prettyPrint)
	//	{
	//		Wrapper<T> wrapper = new Wrapper<T>();
	//		wrapper.Items = array;
	//		return JsonUtility.ToJson(wrapper, prettyPrint);
	//	}

	//	[System.Serializable]
	//	private class Wrapper<T>
	//	{
	//		public T[] Items;
	//	}
	//}

	[System.Serializable]
	public struct ItemType
	{
		public string name;
		public Sprite icon;//icon to display in inventory
						   //public ItemTag Cat;
		public List<int> tags;
		public GameObject prefab;
		public GameObject equipPrefab;
		public float strength;

		public ItemType(string name, Sprite icon, GameObject prefab, GameObject equipPrefab, float strength, List<int> tags)//ItemToolType type,
		{
			this.name = name;
			this.icon = icon;
			this.prefab = prefab;
			this.equipPrefab = equipPrefab;
			//this.type = type;
			this.strength = strength;
			this.tags = tags;
		}

		public static bool Same(ItemType a, ItemType b)
		{
			if (a.name != b.name)					return false;
			if (a.icon != b.icon)					return false;
			if (a.prefab != b.prefab)				return false;
			if (a.equipPrefab != b.equipPrefab)		return false;
			if (a.strength != b.strength)			return false;
			if (a.tags == null || b.tags == null)	return false;
			if (!a.tags.SequenceEqual(b.tags))		return false;
			return true;

		}
	}

	[System.Serializable]
	public class inventoryStuff
	{
		public float invButtonSize = 50;
		public Texture2D invBackground;
		public float buttonsPerInvRow;
		//public int inventorySize;
		public Color selectedColor;
		public Color normalColor;
	}

}
