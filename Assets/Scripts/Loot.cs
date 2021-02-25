using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bobStuff;

//TODO: save stuff in saveentity.cs
[RequireComponent(typeof(Abilities))]
public class Loot : MonoBehaviour
{
    public List<LootItem> loots;

    private bool looted;
    private Abilities a;
    // Start is called before the first frame update
    void Start()
    {
        a = GetComponent<Abilities>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!looted && a.dead)
		{
            GenerateLoot();
            looted = true;
		}
    }

	private void GenerateLoot()
	{
        Rigidbody myRig = GetComponent<Rigidbody>();
		foreach(LootItem i in loots)
		{
            //chance of spawning
            if(Random.value <= i.chance)
			{
                //amount to make (exclusive max so add 1)
                int amount = Random.Range(i.minAmount, i.maxAmount + 1);
                //spawn that amount
                for(int j = 0; j < amount; j++){
					Instantiate(GameControl.itemTypes[i.item.id].prefab, transform.position, transform.rotation);
				}
			}
		}
	}
}

//TODO: warning: ignores amount in item, uses min/maxamount
[System.Serializable]
public struct LootItem
{
    public int minAmount;//if drops, min/max amount
    public int maxAmount;
    public float chance;//the chance that it will drop at all
    public Item item;//the item
}
