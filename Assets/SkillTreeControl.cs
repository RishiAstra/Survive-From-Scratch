using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillTreeControl : MonoBehaviour
{
	public static List<SkillTreeButton> skillButtons = new List<SkillTreeButton>();
	public static int selectedSkillButton;

	public Abilities targetA;
	public StatScript targetS;
	public TextMeshProUGUI skillPointsLeftText;
	public Menu skillMenu;

	// Start is called before the first frame update
	void Start()
	{
		//RefreshUI();
	}

	public void TryToBuySkill()
	{
		SkillTreeButton t = skillButtons[selectedSkillButton];
		if (!HasSkill(t.prerequisite))
		{
			return;
		}
		int pointsSpent = UpdateSkills();
		int pointsRemaining = targetS.GetSkillPointTotal() - pointsSpent;
		if(pointsRemaining > t.cost && (t.s.levelable || !HasSkill(t.s)))
		{
			AddSkill(t.s);

			RefreshUI();
			print("Got skill: " + t.s.name);
		}

		print("tried to buy skill: " + t.s.name + ", " + pointsRemaining + ", " + pointsSpent);

	}

	void RefreshUI()
	{
		int p = UpdateSkills();
		skillPointsLeftText.text = "Skill points: " + (targetS.GetSkillPointTotal() - p);
	}

	void AddSkill(Skill s)
	{
		if (s is UsableSkill)
		{
			UsableSkill temp = s as UsableSkill;
			if (!targetA.skills.Contains(temp))
			{
				targetA.skills.Add(temp);
				targetA.skillLvls.Add(1);
			}
			else if (s.levelable)
			{
				int index = targetA.skills.IndexOf(temp);
				targetA.skillLvls[index]++;
			}
		}
		else if (s is StatSkill)
		{
			StatSkill temp = s as StatSkill;
			if (!targetS.statSkills.Contains(temp))
			{
				targetS.statSkills.Add(temp);
				targetS.skillLvls.Add(1);
			}
			else if (s.levelable)
			{
				int index = targetS.statSkills.IndexOf(temp);
				targetS.skillLvls[index]++;
			}
		}
	}

	int UpdateSkills()
	{
		int skillPointsSpent = 0;
		targetA = GameControl.main.myAbilities;
		targetS = targetA.myStat;

		foreach (SkillTreeButton s in skillButtons)
		{
			bool has = HasSkill(s.s);

			s.hasThisSkill = has;
			if (has) skillPointsSpent += s.cost;

		}
		return skillPointsSpent;
	}

	private bool HasSkill(Skill s)
	{
		bool has = false;
		if (s == null) return true;

		if (s is UsableSkill)
		{
			has = targetA.skills.Contains(s as UsableSkill);
		}
		else if (s is StatSkill)
		{
			has = targetS.statSkills.Contains(s as StatSkill);
		}

		return has;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F2))
		{
			skillMenu.ToggleMenu();
			RefreshUI();
		}
	}
}
