using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public static Minimap main;

    public List<Transform> arrowedPositions;
    public List<Transform> arrows;

    [Range(0, 1)]
    public float arrowNormalizedDistance;
    public GameObject arrowPref;
    public RectTransform parent;
    public RectTransform map;

    // Start is called before the first frame update
    void Awake()
    {
        if (main != null) Debug.LogError("Two Minimap");
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < arrowedPositions.Count; i++)
		{
            if (arrowedPositions[i] == null)
			{
                arrowedPositions.RemoveAt(i);
                Destroy(arrows[i]);
                arrows.RemoveAt(i);
                i--;//since arrow position was removed, go back 1 index
                continue;
			}

            Transform target = arrowedPositions[i];
			Transform arrow = arrows[i];

            Vector3 rpos = target.position - GameControl.main.myAbilities.transform.position;
            rpos.y = 0;
            float dist = rpos.magnitude;
            float mapSize = MapCamera.main.cam.orthographicSize;
            //if far enough that the map can't show it
            if (dist > mapSize * arrowNormalizedDistance)
			{
                arrow.gameObject.SetActive(true);
                //position = normalized position relative to map size times the width of the map on the screen
                //arrow.localPosition = rpos / mapSize * map.sizeDelta.x;
                Vector3 temp = rpos.normalized;
                //transform it from world coordinates to screen
                //temp.x = -temp.x;
                temp.y = temp.z;
                temp.z = 0;
                arrow.localPosition = temp * map.sizeDelta.x / 2;
                arrow.up = temp;

            }
            else
			{
                arrow.gameObject.SetActive(false);
            }
		}
    }

    public void AddArrowedPosition(Transform t)
	{
        arrowedPositions.Add(t);
        arrows.Add(Instantiate(arrowPref, parent).transform);
	}
}
