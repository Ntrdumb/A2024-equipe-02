using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle fullScreenToggle;



    
    private void Start()
    {
        resolutions = Screen.resolutions;

        // Préparer les options de résolution
        List<string> options = new List<string>();
        int currentResolutionIndex = PlayerPrefs.HasKey("resolutionIndex") ? PlayerPrefs.GetInt("resolutionIndex") : 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            // Trouver l'index correspondant à la résolution de l'écran si aucune préférence n'est enregistrée
            if (!PlayerPrefs.HasKey("resolutionIndex") &&
                resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        // Mettre à jour le dropdown
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        //Si le joueur a des playerprefs
        if (PlayerPrefs.HasKey("musicVolume") || PlayerPrefs.HasKey("sfxVolume") || PlayerPrefs.HasKey("masterVolume") 
            || PlayerPrefs.HasKey("isFullScreen"))
        {
            LoadPlayerPref();
        }
        else {
            SetVolume();
            SetMusicVolume();
            SetSFfxVolume();
            SetResolution(currentResolutionIndex); 
        }
    }

    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetVolume()
    {
        float volume = masterSlider.value;
        audioMixer.SetFloat("master", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("masterVolume", volume);
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("music", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }
    
    public void SetSFfxVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("sfx", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }
    
    public void SetFullScreen (bool isFullScreen)
    {
        fullScreenToggle.isOn = isFullScreen;
        Screen.fullScreen = isFullScreen;
    }

    public void LoadPlayerPref()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        SetVolume();        
        SetMusicVolume();
        SetSFfxVolume();
        
        // Charger la résolution et le mode plein écran
        int width = PlayerPrefs.GetInt("resolutionWidth");
        int height = PlayerPrefs.GetInt("resolutionHeight");
        Screen.SetResolution(width, height, Screen.fullScreen);

        bool isFullScreen = PlayerPrefs.GetInt("isFullScreen") == 1;
        SetFullScreen(isFullScreen);
    }

}
