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
