/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillSelector : MonoBehaviour
{
    public int startIndex = 1;
    public float startAngle = 135f;
    public float angleStep = 90f;
    public float minDist = 128f;
    public GameObject[] hoverTints;
    public TextMeshProUGUI[] texts;
    public GameObject wheel;

    private bool showing;
    private int selected;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //update skill name texts
        for(int i = 0; i < texts.Length; i++)
		{
            if(GameControl.main.myAbilities != null && i < GameControl.main.myAbilities.skills.Count)
			{
                texts[i].text = GameControl.main.myAbilities.skills[i].name;
			}
			else
			{
                texts[i].text = "";
            }
		}

        //if you press down alt, start showing wheel
        if (GameControl.main.myAbilities != null && (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt)))
        {
            showing = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        //tried to use skill
        if(showing && (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt)))
		{
            showing = false;
            if (selected >= 0 && selected < GameControl.main.myAbilities.skills.Count)
            {
                GameControl.main.myAbilities.UseSkill(selected);
            }
            Cursor.lockState = CursorLockMode.Locked;
        }

        //if cursor is no longer locked or player doesn't exist, stop
        if (showing && (Cursor.lockState != CursorLockMode.Confined || GameControl.main.myAbilities == null)) {
            showing = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        //update 
        if(showing && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
        {
            //calculate position
            float scale = GameControl.main.mainCanvas.scaleFactor;
            Vector2 pos = Input.mousePosition;
            pos -= new Vector2(Screen.width, Screen.height) / 2f;
            pos /= scale;

            //calculate angle
            float a = -Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
            a -= startAngle;
            //get between 0 and 360
            a = Mathf.DeltaAngle(0, a);
            if (a < 0) a += 360;

            //calculate the index of the panel that the mouse would be over, starting at 0
            int s = Mathf.FloorToInt(a / angleStep);

            //if close enough and not out of range
            if(pos.magnitude > minDist && s >= 0 && s < GameControl.main.myAbilities.skills.Count)
			{
                if (selected >= 0 && selected < hoverTints.Length) hoverTints[selected].SetActive(false);
                selected = s;
                if (selected >= 0 && selected < hoverTints.Length) hoverTints[selected].SetActive(true);
            }
			else
			{
                if (selected >= 0 && selected < hoverTints.Length) hoverTints[selected].SetActive(false);
                selected = -1;
			}
		}
		else
		{
            //avoid stuck alt keys
            showing = false;
		}

        wheel.SetActive(showing);
    }
}
