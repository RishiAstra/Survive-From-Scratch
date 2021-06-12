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
    public UserQualitySettings settings;

    private UnityEngine.Rendering.Universal.Bloom bloom;

    //TODO: add settings presets of the type UserQualitySettings in a list that user can choose from and save
    //TODO: if there is no settings, use default (must make default)

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

        bloomEnableToggle.isOn = settings.bloomEnabled;
        bloomEnableToggle.onValueChanged.AddListener((bool value) => { settings.bloomEnabled = value; });
        bloomIntensitySlider.value = settings.bloomIntensity;
        bloomIntensitySlider.onValueChanged.AddListener((float value) => { settings.bloomIntensity = value; bloomIntensityField.text = value.ToString("F2"); });
        bloomIntensityField.text = settings.bloomIntensity.ToString("F2");
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

        grassDensitySlider.value = settings.grassDensity;
        grassDensitySlider.onValueChanged.AddListener((float value) => { settings.grassDensity = value; grassDensityField.text = value.ToString("F2"); });
        grassDensityField.text = settings.grassDensity.ToString("F2");
        grassDensityField.onValueChanged.AddListener((string text) =>
        {
            float value;
            if (float.TryParse(text, out value))
            {
                value = Mathf.Clamp(value, grassDensitySlider.minValue, grassDensitySlider.maxValue);
                grassDensitySlider.value = value;
                settings.grassDensity = value;
            }
        }
        );
    }

    public void ApplySettings()
	{
        SetQualityPreset(settings.qualitySelected);
        bloom.active = settings.bloomEnabled;
        bloom.intensity.value = settings.bloomIntensity;
        UpdateGrassDensity(settings.grassDensity);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
        ApplySettings();
		//UpdateGrassDensity();
	}

	private void UpdateGrassDensity(float value)
	{
		VegetationStudioManager m = FindObjectOfType<VegetationStudioManager>();
        if(m != null)
		{
            foreach (VegetationSystem v in m.VegetationSystemList)
		    {
			    v.SetGrassDensity(value);
                v.SetPlantDensity(value);
		    }
		}
		
	}

	void OnQualityPresetSelected(int index)
	{
        settings.qualitySelected = index;
	}

    void SetQualityPreset(int index)
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


public class UserQualitySettings
{
    public int qualitySelected;
    public bool bloomEnabled;
    public float bloomIntensity;
    public float grassDensity;
}