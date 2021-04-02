using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [Range(0, 1f)]public float lerpSpeed;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //void FixedUpdate()
    //{
    //    if(target != null) transform.position = Vector3.Lerp(transform.position, target.position, lerpSpeed);
    //}
	void LateUpdate()
	{
		if (target != null) transform.position = Vector3.Lerp(transform.position, target.position, lerpSpeed);
	}
}
