﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerLevelUnlocker : MonoBehaviour
{
    public string towerName;
    public int levelToUnlock;
    public List<spawner> spawners;
    // Start is called before the first frame update
    void Start()
    {
        TowerControl.towerLevelUnlockers.Add(this);
    }

	private void OnDestroy()
	{
        TowerControl.towerLevelUnlockers.Remove(this);
	}

	void CheckIfUnlock()
	{
        bool ok = true;
        for(int i = 0; i < spawners.Count; i++)
		{
            spawner s = spawners[i];
            //if either the spawner isn't initialized or some enemies spawned by it remain
            if(!s.initialized || s.spawnedThese.Count != 0)
			{
                ok = false;
			}
		}

		if (ok)
		{
            int s = TowerControl.main.GetTowerIndex(towerName);
            int ind = levelToUnlock - 1;
            if(ind < TowerControl.main.towers[s].unlockedLevels.Count)
			{
                TowerControl.main.towers[s].unlockedLevels[ind] = true;
			}
		}
	}

    // Update is called once per frame
    void Update()
    {
        if(Time.frameCount % 10 == 0)
		{
            CheckIfUnlock();
		}
    }

	public int GetMonstersLeft()
	{
        int r = 0;
        foreach(spawner sp in spawners)
		{
            r += sp.spawnedThese.Count;
		}
        return r;
	}
}
