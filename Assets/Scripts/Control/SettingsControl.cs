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

    public Volume mainPostProcessingVolume;
    [Space(10)]
    public TMP_Dropdown presetDropdown;
    [Space(10)]
    public Toggle bloomEnableToggle;
    public Slider bloomIntensitySlider;
    public TMP_InputField bloomIntensityField;
    //public Slider bloomThresholdSlider;
    //public Slider bloomThresholdSlider;

    private UnityEngine.Rendering.Universal.Bloom bloom;
    // Start is called before the first frame update
    void Start()
    {
        mainPostProcessingVolume.profile.TryGet(out bloom);


        presetDropdown.options = new List<TMP_Dropdown.OptionData>();
		string[] names = QualitySettings.names;
		for (int i = 0; i < names.Length; i++)
		{
			presetDropdown.options.Add(new TMP_Dropdown.OptionData(names[i]));
		}

        presetDropdown.onValueChanged.AddListener(OnQualityPresetSelected);

        OnQualityPresetSelected(0);//load 1st quality

        bloomEnableToggle.onValueChanged.AddListener((bool value) => { bloom.active = value; });
        bloomIntensitySlider.onValueChanged.AddListener((float value) => { bloom.intensity.value = value; bloomIntensityField.text = value.ToString("F2"); });
        bloomIntensityField.onValueChanged.AddListener((string text) => 
            {
                float value;
                if(float.TryParse(text, out value))
				{
                    value = Mathf.Clamp(value, bloomIntensitySlider.minValue, bloomIntensitySlider.maxValue);
                    bloom.intensity.value = value;
                    bloomIntensitySlider.value = value;
				}
            }
        );
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
