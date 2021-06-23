using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(LootDropper))]
public class Chest : MonoBehaviour, IMouseHoverable
{
	public GameObject onHover;
	public GameObject lootableIndicator;
	public bool looted;
	public bool persistantOnlyLootOnce;
	public Animator anim;

	private LootDropper loot;
	private PersistantSaveID id;

	// Start is called before the first frame update
	void Start()
    {
		onHover.SetActive(false);
		lootableIndicator.SetActive(false);
		loot = GetComponent<LootDropper>();
		if (persistantOnlyLootOnce)
		{
			id = GetComponent<PersistantSaveID>();
			if (File.Exists(GetSavePath()))
			{
				looted = JsonConvert.DeserializeObject<bool>(File.ReadAllText(GetSavePath()));
				if(anim != null && looted)
				{
					anim.SetTrigger("Open_Immediate");
				}
			}
		}
	}

	private void OnDestroy()
	{
		if (persistantOnlyLootOnce)
		{
			Directory.CreateDirectory(GetChestPersistantSavePath());
			File.WriteAllText(GetSavePath(), JsonConvert.SerializeObject(looted));
		}
	}

	public string GetSavePath()
	{
		return GetChestPersistantSavePath() + id.id + ".json";
	}

	public static string GetChestPersistantSavePath()
	{
		return GameControl.saveDirectory + "Chests/";
	}

	public virtual bool IsUnlocked()
	{
		return true;
	}

	public void OnMouseHoverFromRaycast()
	{
		if (onHover != null) onHover.SetActive(true);
		if (CanBeLooted() && InputControl.InteractKeyDown())
		{
			loot.GenerateLoot();
			looted = true;
			if (anim != null)
			{
				anim.SetTrigger("Open");
			}
		}
	}

	public void OnMouseStopHoverFromRaycast()
	{
		if (onHover != null) onHover.SetActive(false);
	}



	// Update is called once per frame
	void Update()
	{
		//show if this chest can be looted (unlocked and hasn't been looted yet)
		lootableIndicator.SetActive(CanBeLooted());
	}

	private bool CanBeLooted()
	{
		return !looted && IsUnlocked();
	}
}
