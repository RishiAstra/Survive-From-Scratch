/********************************************************
* Copyright (c) 2021 Rishi A. Astra
* All rights reserved.
********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class SettingsControl : MonoBehaviour
{
    public TMP_Dropdown presetDropdown;
    // Start is called before the first frame update
    void Start()
    {
        presetDropdown.options = new List<TMP_Dropdown.OptionData>();
		string[] names = QualitySettings.names;
		for (int i = 0; i < names.Length; i++)
		{
			presetDropdown.options.Add(new TMP_Dropdown.OptionData(names[i]));
		}

        presetDropdown.onValueChanged.AddListener(OnQualityPresetSelected);
    }

    void OnQualityPresetSelected(int index)
	{
        QualitySettings.SetQualityLevel(index);
        GraphicsSettings.renderPipelineAsset = QualitySettings.GetRenderPipelineAssetAt(index);
	}

    // Update is called once per frame
    void Update()
    {
        //if(Input.)
    }
}
