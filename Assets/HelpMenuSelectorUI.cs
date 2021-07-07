using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMenuSelectorUI : MonoBehaviour
{
    public string helpName;
    public GameObject selected;
    public GameObject content;

    public void SetSelected(bool selected)
	{
        this.selected.SetActive(selected);
        content.SetActive(selected);
	}
}
