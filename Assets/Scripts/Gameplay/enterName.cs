using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class enterName : MonoBehaviour {
	public InputField nameHere;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Return)) {
			startGame ();
		}
	}

	public void startGame(){
		//PhotonNetwork.playerName = nameHere.text;
		SceneManager.LoadScene (1);
	}
}
