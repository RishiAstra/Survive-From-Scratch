using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public static int openMenuCount;

    public bool pauseOnActive = true;
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.activeSelf) ActivateMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleMenu()
    {
        if (gameObject.activeSelf)
        {
            DeactivateMenu();
        }
        else
        {
            ActivateMenu();
        }
    }

    public void ActivateMenu()
	{
        openMenuCount++;
        gameObject.SetActive(true);
        if (pauseOnActive)
        {
            TimeControl.main.SetTimeScale(0, "menu");
        }
        GameControl.main.TryUnlockCursor();
    }

    public void DeactivateMenu()
	{
        openMenuCount--;
        gameObject.SetActive(false);
        if (pauseOnActive)
        {
            TimeControl.main.RemoveTimeScale("menu");
        }
        GameControl.main.TryLockCursor();
    }

}
