using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutUIAtPosition : MonoBehaviour
{

    public GameObject toPutOnUI;
    public GameObject myUIThing;

    // Start is called before the first frame update
    void Start()
    {
        myUIThing = Instantiate(toPutOnUI, GameControl.main.UIPositionMatchersParent);
    }

	// Update is called once per frame

	private void LateUpdate()
	{
		Vector3 pos = GameControl.mainCamera.WorldToScreenPoint(transform.position);
        myUIThing.SetActive(pos.z >= 0);
		myUIThing.transform.position = pos;
    }
}
