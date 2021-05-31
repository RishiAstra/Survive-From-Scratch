/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeButton : MonoBehaviour
{
	public const float selectedScale = 1.1f;

    public Skill s;
    public SkillTreeButton prerequisiteButton;
    public bool hasThisSkill;
    public int cost = 1;
    public int index;
    public GameObject boughtTint;
    public GameObject selectedTint;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI descriptionText;
	public Image mainImage;

    public Skill prerequisite;

	private int plvl = -1;
    // Start is called before the first frame update
    void Awake()
	{
		if (prerequisiteButton != null) prerequisite = prerequisiteButton.s;
		UpdateSkillNameText();
		index = SkillTreeControl.skillButtons.Count;
		if (!SkillTreeControl.skillButtons.Contains(this)) SkillTreeControl.skillButtons.Add(this);
	}
	//immediately update when enabled
	private void OnEnable()
	{
		Update();
	}

	private void UpdateSkillNameText()
	{
		TextMeshProUGUI temp = GetComponentInChildren<TextMeshProUGUI>();
		if(temp != null && s != null) temp.text = s.name;
	}

	private void OnValidate()
	{
		UpdateSkillNameText();
	}

	public void ClickSkill()
	{
        SkillTreeControl.selectedSkillButton = index;
		UpdateSkillDescriptionUIAsThis();
	}

    // Update is called once per frame
    void Update()
	{
		boughtTint.SetActive(hasThisSkill && !s.levelable);
		bool isSelected = SkillTreeControl.skillButtons[SkillTreeControl.selectedSkillButton] == this;
		selectedTint.SetActive(isSelected);
		SkillTreeControl.main.HasSkill(s, out int lvl);
		levelText.text = lvl == 0 ? "" : "Level " + lvl;

		transform.localScale = Vector3.one * (isSelected ? selectedScale : 1);
		Color c = mainImage.color;
		c.a = isSelected ? 0.95f : 0.8f;
		mainImage.color = c;

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
				description = ss.mods.ToString(lvl + 1);
			}
		}
		else if (s is UsableSkill)
		{
			UsableSkill ss = s as UsableSkill;
			StringBuilder sb = new StringBuilder();
			sb.Append(ss.description);
			//description = ss.description;
			bool shp =  !Mathf.Approximately(0, ss.cost.hp);
			bool smp =  !Mathf.Approximately(0, ss.cost.mp);
			bool seng = !Mathf.Approximately(0, ss.cost.eng);
			bool smor = !Mathf.Approximately(0, ss.cost.mor);

			//if this restores nothing, return blank string
			if (!shp && !smp && !seng && !smor) 
			{ 
			
			}
			else
			{
				sb.Append("\nCost: \n");
				if (shp ) sb.Append("<#" + ColorUtility.ToHtmlStringRGB(GameControl.main.hpColor ) + ">" + ss.cost.hp + "HP</color>\n");
				if (smp ) sb.Append("<#" + ColorUtility.ToHtmlStringRGB(GameControl.main.mpColor ) + ">" + ss.cost.mp + "MP</color>\n");
				if (seng) sb.Append("<#" + ColorUtility.ToHtmlStringRGB(GameControl.main.engColor) + ">" + ss.cost.eng + "ENG</color>\n");
				if (smor) sb.Append("<#" + ColorUtility.ToHtmlStringRGB(GameControl.main.morColor) + ">" + ss.cost.mor + "MOR</color>\n");
			}
			description = sb.ToString();
		}
		return description;
	}
}
