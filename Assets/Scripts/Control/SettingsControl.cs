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
using System.IO;
using Newtonsoft.Json;
using System;

public class SettingsControl : MonoBehaviour
{

	public Volume mainPostProcessingVolume;
	[Space(10)]
	public TMP_InputField nameField;
	public TMP_Dropdown settingsPresetDropdown;//for selecting my quality presets, which include a Unity quality preset as one of the settings
	[Space(10)]
	public TMP_Dropdown primarySettingsPresetDropdown;//for selecting Unity's quality settings preset
	[Space(10)]
	public Toggle bloomEnableToggle;
	public Slider bloomIntensitySlider;
	public TMP_InputField bloomIntensityField;
	public Slider grassDensitySlider;
	public TMP_InputField grassDensityField;
	//public Slider bloomThresholdSlider;
	//public Slider bloomThresholdSlider;
	public UserQualitySettings settings;
	public int settingsIndex;
	public List<UserQualitySettings> settingsPresets;
	public List<UserQualitySettings> defaultSettingsPresets;

	private UnityEngine.Rendering.Universal.Bloom bloom;

	//TODO: add settings presets of the type UserQualitySettings in a list that user can choose from and save
	//TODO: if there is no settings, use default (must make default)

	// Start is called before the first frame update
	void Start()
	{

		mainPostProcessingVolume.profile.TryGet(out bloom);
		SceneManager.sceneLoaded += OnSceneLoaded;


		settingsPresetDropdown.onValueChanged.AddListener((int i) => { SetQualityPreset(i); });

		primarySettingsPresetDropdown.options = new List<TMP_Dropdown.OptionData>();
		string[] names = QualitySettings.names;
		for (int i = 0; i < names.Length; i++)
		{
			primarySettingsPresetDropdown.options.Add(new TMP_Dropdown.OptionData(names[i]));
		}

		primarySettingsPresetDropdown.onValueChanged.AddListener(OnunityQualityPresetSelected);


		nameField.onValueChanged.AddListener((string text) => settings.name = text);

		bloomEnableToggle.onValueChanged.AddListener((bool value) => { settings.bloomEnabled = value; });
		bloomIntensitySlider.onValueChanged.AddListener((float value) => { settings.bloomIntensity = value; bloomIntensityField.text = value.ToString("F2"); });
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

		grassDensitySlider.onValueChanged.AddListener((float value) => { settings.grassDensity = value; grassDensityField.text = value.ToString("F2"); });
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



		LoadSettings();
		UpdateQualitySettingsDisplay();
	}

	void UpdateQualitySettingsDisplay()
	{
		nameField.text = settings.name;
		primarySettingsPresetDropdown.value = settings.qualitySelected;

		bloomEnableToggle.isOn = settings.bloomEnabled;
		bloomIntensitySlider.value = settings.bloomIntensity;
		bloomIntensityField.text = settings.bloomIntensity.ToString("F2");
		grassDensitySlider.value = settings.grassDensity;
		grassDensityField.text = settings.grassDensity.ToString("F2");

	}

	private void SetQualityPreset(int i)
	{
		settings = new UserQualitySettings(settingsPresets[i]);
		settingsIndex = i;
		UpdateQualitySettingsDisplay();
	}

	public void LoadSettings()
	{
		//read the custom presets
		if (Directory.Exists(GetSettingsPresetSaveDirectory()))
		{
			settingsPresets = new List<UserQualitySettings>();
			settingsIndex = int.Parse(File.ReadAllText(GetSettingsSaveDirectory() + "last.txt"));

			foreach (string s in Directory.GetFiles(GetSettingsPresetSaveDirectory()))
			{
				settingsPresets.Add(JsonConvert.DeserializeObject<UserQualitySettings>(File.ReadAllText(s)));
			}
		}
		else
		{
			ResetAllSettings();
		}

		SetQualityPreset(settingsIndex);
		ApplySettings();
	}

	public void RefreshQualityPresetsDropdown()
	{
		settingsPresetDropdown.ClearOptions();
		foreach(UserQualitySettings s in settingsPresets)
		{
			settingsPresetDropdown.options.Add(new TMP_Dropdown.OptionData(s.name));
		}
		settingsPresetDropdown.value = settingsIndex;
	}

	public void ResetAllSettings()
	{
		settingsPresets = new List<UserQualitySettings>();
		foreach (UserQualitySettings s in defaultSettingsPresets)
		{
			settingsPresets.Add(s);
		}
	}

	public void AddQualityPreset()
	{
		settingsPresets.Insert(settingsIndex, new UserQualitySettings(settings));
		SetQualityPreset(settingsIndex);
		RefreshQualityPresetsDropdown();
		UpdateQualitySettingsDisplay();
	}

	public void RemoveQualityPreset()
	{
		if(settingsPresets.Count > 1)
		{
			settingsPresets.RemoveAt(settingsIndex);
			if(settingsIndex == 0)
			{
				settingsIndex = 0;
			}
			else
			{
				settingsIndex--;
			}

			SetQualityPreset(settingsIndex);
			RefreshQualityPresetsDropdown();
			UpdateQualitySettingsDisplay();
		}
		
	}

	public void SaveSettings()
	{
		ApplySettings();
		if (Directory.Exists(GetSettingsPresetSaveDirectory()))
		{
			Directory.Delete(GetSettingsPresetSaveDirectory(), true);
		}
		Directory.CreateDirectory(GetSettingsPresetSaveDirectory());
		foreach(UserQualitySettings s in settingsPresets)
		{
			File.WriteAllText(GetSettingsPresetSaveDirectory() + s.name, JsonConvert.SerializeObject(s, Formatting.Indented));
		}

		File.WriteAllText(GetSettingsSaveDirectory() + "last.txt", settingsIndex.ToString());
	}

	public void ApplySettings()
	{
		//apply the settings
		SetUnityQualityPreset(settings.qualitySelected);
		bloom.active = settings.bloomEnabled;
		bloom.intensity.value = settings.bloomIntensity;
		UpdateGrassDensity(settings.grassDensity);

		//apply to preset list
		settingsPresets[settingsIndex] = new UserQualitySettings(settings);
		RefreshQualityPresetsDropdown();
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

	void OnunityQualityPresetSelected(int index)
	{
		settings.qualitySelected = index;
	}

	void SetUnityQualityPreset(int index)
	{
		QualitySettings.SetQualityLevel(index);
		GraphicsSettings.renderPipelineAsset = QualitySettings.GetRenderPipelineAssetAt(index);
	}

	public string GetSettingsSaveDirectory()
	{
		return GameControl.saveDirectory + "/Settings/";
	}

	public string GetSettingsPresetSaveDirectory()
	{
		return GetSettingsSaveDirectory() + "Presets/";
	}

	// Update is called once per frame
	void Update()
	{
		//if(Input.)
	}

	private void OnApplicationQuit()
	{
		SaveSettings();
	}
}

[System.Serializable]
public class UserQualitySettings
{
	public string name;
	public int qualitySelected;
	public bool bloomEnabled;
	public float bloomIntensity;
	public float grassDensity;

	public UserQualitySettings()
	{
		this.name = "Quality Preset";
		this.qualitySelected = 0;
		this.bloomEnabled = false;
		this.bloomIntensity = 0;
		this.grassDensity = 0;
	}

	/// <summary>
	/// clones an existing UserQualitySettings
	/// </summary>
	/// <param name="o">the original to clone</param>
	public UserQualitySettings(UserQualitySettings o)
	{
		this.name = o.name;
		this.qualitySelected = o.qualitySelected;
		this.bloomEnabled = o.bloomEnabled;
		this.bloomIntensity = o.bloomIntensity;
		this.grassDensity = o.grassDensity;
	}
}