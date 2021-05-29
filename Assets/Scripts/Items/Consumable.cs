/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using bobStuff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
	private const float small = 0.00001f;
	public Equip me;
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(1) && Cursor.lockState == CursorLockMode.Locked)
		{
			Item i = me.bob.inv.items[me.bob.invSel];
			ItemType t = GameControl.itemTypes[i.id];

			//print(me.bob.inventory[me.bob.invSel].amount >= 1);
			if (i.amount >= 1)
			{
				Stat s = GameControl.main.myAbilities.myStat.stat;
				Stat m = GameControl.main.myAbilities.myStat.maxStat;
				bool canUse = false;
				//if this will provide some benefit from eating, allow eating
				canUse = canUse || (s.hp <  m.hp  && t.consumeRestore.stat.hp > small);
				canUse = canUse || (s.mp <  m.mp  && t.consumeRestore.stat.mp  > small);
				canUse = canUse || (s.eng < m.eng && t.consumeRestore.stat.eng > small);
				canUse = canUse || (s.mor < m.mor && t.consumeRestore.stat.mor > small);
				canUse = canUse || (s.atk < m.atk && t.consumeRestore.stat.atk > small);//TODO: probably unused

				//must be stackable or there is no statrestore active
				canUse = canUse && (t.consumeRestore.stackable || GameControl.main.myAbilities.myStat.statRestores.Count == 0);

				if (canUse)
				{
					me.bob.RemoveItem(me.bob.invSel, 1);
					GameControl.main.myAbilities.myStat.AddStatRestore(t.consumeRestore);
					//Stat f = new Stat
					//{
					//	hp  = s.hp  + t.consumeRestore.stat.hp ,
					//	mp  = s.mp  + t.consumeRestore.stat.mp ,
					//	eng = s.eng + t.consumeRestore.stat.eng,
					//	mor = s.mor + t.consumeRestore.stat.mor,
					//	atk = s.atk + t.consumeRestore.stat.atk,
					//};

					//if (f.hp  > m.hp ) f.hp  = m.hp ;
					//if (f.mp  > m.mp ) f.mp  = m.mp ;
					//if (f.eng > m.eng) f.eng = m.eng;
					//if (f.mor > m.mor) f.mor = m.mor;
					//if (f.atk > m.atk) f.atk = m.atk;

					//GameControl.main.myAbilities.myStat.stat = f;

				}
			}
		}
	}
}
