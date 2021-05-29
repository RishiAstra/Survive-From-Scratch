using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillTreeButton : MonoBehaviour
{
    public Skill s;
    public SkillTreeButton prerequisiteButton;
    public bool hasThisSkill;
    public int cost = 1;
    public int index;
    public GameObject boughtTint;
    public GameObject selectedTint;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI descriptionText;

    public Skill prerequisite;

	private int plvl;
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
		UpdateSkillDescriptionUIAsThis();
	}

    // Update is called once per frame
    void Update()
	{
		boughtTint.SetActive(hasThisSkill);
		bool isSelected = SkillTreeControl.skillButtons[SkillTreeControl.selectedSkillButton] == this;
		selectedTint.SetActive(isSelected);
		SkillTreeControl.main.HasSkill(s, out int lvl);
		levelText.text = lvl == 0 ? "" : "Level " + lvl;

		if (isSelected && lvl != plvl)
		{
			UpdateSkillDescriptionUIAsThis();
			plvl = lvl;
		}

		//update description
		//UpdateDescription(lvl);
	}

	private void UpdateSkillDescriptionUIAsThis()
	{
		SkillTreeControl.main.skillDescriptionText.text = GetDescription();
		SkillTreeControl.main.skillTitleText.text = s.name;
	}

	public string GetDescription()
	{
		string description = "unknown skill";
		SkillTreeControl.main.HasSkill(s, out int lvl);

		if (s is StatSkill)
		{
			StatSkill ss = s as StatSkill;
			if (lvl >= 1 && ss.levelable)
			{
				description = ss.mods.GetUpgradeString(lvl, lvl + 1);
			}
			else
			{
				description = ss.mods.ToString(lvl);
			}
		}
		else if (s is UsableSkill)
		{
			description = (s as UsableSkill).description;
		}
		return description;
	}
}
