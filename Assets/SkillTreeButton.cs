using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillTreeButton : MonoBehaviour
{
    public Skill s;
    public SkillTreeButton prerequisiteButton;
    public bool hasThisSkill;
    public int cost = 1;
    public int index;
    public GameObject boughtTint;

    public Skill prerequisite;
    // Start is called before the first frame update
    void Awake()
    {
        if(prerequisiteButton != null) prerequisite = prerequisiteButton.s;
        GetComponentInChildren<TextMeshProUGUI>().text = s.name;
        index = SkillTreeControl.skillButtons.Count;
        if(!SkillTreeControl.skillButtons.Contains(this)) SkillTreeControl.skillButtons.Add(this);
    }

    public void ClickSkill()
	{
        SkillTreeControl.selectedSkillButton = index;
	}

    // Update is called once per frame
    void Update()
    {
        boughtTint.SetActive(hasThisSkill);
    }
}
