using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public static int openMenuCount;

    public bool pauseOnActive = true;

	private bool initialized;
    // Start is called before the first frame update
    void Awake()
    {
        if (!initialized && gameObject.activeSelf)
		{
			ActivateMenu();
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleMenu()
    {
        if (gameObject.activeSelf)
        {
            TryDeactivateMenu();
        }
        else
        {
            TryActivateMenu();
        }
    }

    public void TryActivateMenu()
	{
		if (!gameObject.activeSelf)
		{
			ActivateMenu();
		}

	}

	private void ActivateMenu()
	{
		initialized = true;
		openMenuCount++;
		gameObject.SetActive(true);
		if (pauseOnActive)
		{
			TimeControl.main.SetTimeScale(0, "menu");
		}
		GameControl.main.TryUnlockCursor();
		//print("activated menu: " + gameObject.name + ", " + openMenuCount);
	}

	public void TryDeactivateMenu()
	{
		if (gameObject.activeSelf)
		{
			DeactivateMenu();
		}
	}

	private void DeactivateMenu()
	{
		openMenuCount--;
		gameObject.SetActive(false);
		if (pauseOnActive)
		{
			TimeControl.main.RemoveTimeScale("menu");
		}
		GameControl.main.TryLockCursor();
		print("deactivated menu: " + gameObject.name + ", " + openMenuCount);
	}
}
