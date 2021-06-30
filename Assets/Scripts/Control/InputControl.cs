using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControl : MonoBehaviour
{
	public static InputControl main;

	public static KeyCode interactKeyCode = KeyCode.F;
	private static bool interactPressed;
	private static bool interactHeld;

	/* TODO: allow players to set their own controls (e.g. allow them to set an interactKeyCode, etc)
	 * consider something like this. It's inefficient because it looks a few hundred values, but it wouldn't be a problem to run it only when the player tries to change a control 
	 * foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
	 * {
	 *      if (Input.GetKey(kcode))
	 *      Debug.Log("KeyCode down: " + kcode);
	 * }
	 * 
	 * 
	 * 
	 */

	// Start is called before the first frame update
	void Awake()
	{

		if (main != null) Debug.LogError("two gameControls");
		main = this;
	}

	/// <summary>
	/// Get if interact key was pressed down
	/// This function only returns true once per time that the key is pressed, calling it will "use up" this press<para/>
	/// Be careful where you call this because it will prevent other code from detecting the interact key
	/// </summary>
	/// <returns>if interact key was pressed and not used</returns>
	public static bool InteractKeyDown()
	{

		if (main == null) Debug.LogError("No InputControl");

		bool result = interactPressed;
		//if interactPressed, then set it to false because it's used up
		if (result)
		{
			interactPressed = false;
			interactHeld = false;
		}

		return result;
	}

	/// <summary>
	/// Find out if the interact key is held down and unused. This does not use the key press.
	/// </summary>
	/// <returns></returns>
	public static bool InteractKeyHeld()
	{
		return interactHeld && Input.GetKey(interactKeyCode);
	}

	// Update is called once per frame
	void Update()
	{
		//reset interact
		interactPressed = Input.GetKeyDown(interactKeyCode);
		if (interactPressed)
		{
			interactHeld = true;
		}

		if(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
		{
			int keyPressed = GetIntKeyPressed();
			if(keyPressed >= 0 && keyPressed < GameControl.main.myParty.members.Count)
			{
				GameControl.main.SetControlledPartyMember(keyPressed);
			}
		}
		else
		{
			CheckHotBar();
		}

	}

	int GetIntKeyPressed()
	{
		if (Input.GetKeyDown("0")) return 9;
		for (int i = 1; i < 10; i++)
		{
			if (Input.GetKeyDown(i.ToString()))
			{
				return i - 1;
			}
		}

		return -1;
	}

	void CheckHotBar()
	{
		if(GameControl.main.playerControl != null)
		{
			int keyPressed = GetIntKeyPressed();
			if(keyPressed != -1)
			{
				GameControl.main.playerControl.SelectInv(keyPressed);
			}
			//if (Input.GetKeyDown("0")) GameControl.main.playerControl.SelectInv(9);
			//for (int i = 1; i < 10; i++)
			//{
			//	if (Input.GetKeyDown(i.ToString()))
			//	{
			//		GameControl.main.playerControl.SelectInv(i - 1);
			//		//print("select " + i);
			//	}
			//}

		}
		
	}
}
