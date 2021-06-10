﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControl : MonoBehaviour
{
    public static InputControl main;

    public static KeyCode interactKeyCode = KeyCode.F;
    private static bool interactPressed;

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
        if (result) interactPressed = false;

        return result;
	}

    public static bool InteractKeyHeld()
	{
        return Input.GetKey(interactKeyCode);
	}

    // Update is called once per frame
    void Update()
    {
        //reset interact
        interactPressed = Input.GetKeyDown(interactKeyCode);
    }
}