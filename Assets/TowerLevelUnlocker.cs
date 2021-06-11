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
            TowerControl.main.towers[s].unlockedLevels[levelToUnlock - 1] = true;
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
}
