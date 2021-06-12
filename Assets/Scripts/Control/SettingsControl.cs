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
using UnityEngine.SceneManagement;
using AwesomeTechnologies.VegetationStudio;
using AwesomeTechnologies;

public class SettingsControl : MonoBehaviour
{

    public Volume mainPostProcessingVolume;
    [Space(10)]
    public TMP_Dropdown presetDropdown;
    [Space(10)]
    public Toggle bloomEnableToggle;
    public Slider bloomIntensitySlider;
    public TMP_InputField bloomIntensityField;
    public Slider grassDensitySlider;
    public TMP_InputField grassDensityField;
    //public Slider bloomThresholdSlider;
    //public Slider bloomThresholdSlider;

    private UnityEngine.Rendering.Universal.Bloom bloom;
    private 
    // Start is called before the first frame update
    void Start()
    {
        mainPostProcessingVolume.profile.TryGet(out bloom);
        SceneManager.sceneLoaded += OnSceneLoaded;

        presetDropdown.options = new List<TMP_Dropdown.OptionData>();
		string[] names = QualitySettings.names;
		for (int i = 0; i < names.Length; i++)
		{
			presetDropdown.options.Add(new TMP_Dropdown.OptionData(names[i]));
		}

        presetDropdown.onValueChanged.AddListener(OnQualityPresetSelected);

        OnQualityPresetSelected(0);//load 1st quality

        bloomEnableToggle.isOn = bloom.active;
        bloomEnableToggle.onValueChanged.AddListener((bool value) => { bloom.active = value; });
        bloomIntensitySlider.value = bloom.intensity.value;
        bloomIntensitySlider.onValueChanged.AddListener((float value) => { bloom.intensity.value = value; bloomIntensityField.text = value.ToString("F2"); });
        bloomIntensityField.text = bloom.intensity.value.ToString("F2");
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

        grassDensitySlider.onValueChanged.AddListener((float value) => { UpdateGrassDensity(); grassDensityField.text = value.ToString("F2"); });
        grassDensityField.onValueChanged.AddListener((string text) =>
        {
            float value;
            if (float.TryParse(text, out value))
            {
                value = Mathf.Clamp(value, grassDensitySlider.minValue, grassDensitySlider.maxValue);
                grassDensitySlider.value = value;
                UpdateGrassDensity();
            }
        }
        );
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		UpdateGrassDensity();
	}

	private void UpdateGrassDensity()
	{
		VegetationStudioManager m = FindObjectOfType<VegetationStudioManager>();
        if(m != null)
		{
            foreach (VegetationSystem v in m.VegetationSystemList)
		    {
			    v.SetGrassDensity(grassDensitySlider.value);
                v.SetPlantDensity(grassDensitySlider.value);
		    }
		}
		
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
