using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnDestroy()
	{
		if (GetComponent<badguy>().hp > 0) return;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
}
