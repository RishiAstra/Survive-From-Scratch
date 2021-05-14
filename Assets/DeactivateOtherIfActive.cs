using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateOtherIfActive : MonoBehaviour
{
    //static lists to keep track of what is being deactivated and by how many scripts
    public static List<GameObject> deactivated;
    public static List<int> deactivatedCount;//how many times this gameobject is deactivated

    public List<GameObject> toDeactivate;
    // Start is called before the first frame update
    void Awake()
    {
        if (deactivated == null) deactivated = new List<GameObject>();
        if (deactivatedCount == null) deactivatedCount = new List<int>();
    }
	private void OnEnable()
	{
        for(int i = 0; i < toDeactivate.Count; i++)
		{
            GameObject g = toDeactivate[i];
            //find index of this in the static list, or add it
            int ind = deactivated.IndexOf(g);
            if(ind == -1)
			{
                ind = deactivated.Count;
                deactivated.Add(g);
                g.SetActive(false);
                deactivatedCount.Add(0);
			}

            deactivatedCount[ind]++;

		}
	}

	private void OnDisable()
	{
        for (int i = 0; i < toDeactivate.Count; i++)
        {
            GameObject g = toDeactivate[i];
            //find index of this in the static list, or add it
            int ind = deactivated.IndexOf(g);
            if (ind == -1)
            {
                Debug.LogWarning("Tried to reactivate gameobject but it wasn't deactivated");
			}
			else
			{
                deactivatedCount[ind]--;
                if(deactivatedCount[ind] <= 0)
				{
                    deactivated[ind].SetActive(true);
                    deactivated.RemoveAt(ind);
                    deactivatedCount.RemoveAt(ind);
				}
            }
        }
    }

	// Update is called once per frame
	void Update()
    {
        
    }
}
