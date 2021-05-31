/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePanel : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        DialogueControl.main.UpdateDialogue();
    }
}
