using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class InitializePlayerPrefs : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    private void Start()
    {
        // Méthode statique pour charger les paramètres au démarrage du jeu

        // Apply resolution settings
        if (PlayerPrefs.HasKey("resolutionWidth") && PlayerPrefs.HasKey("resolutionHeight"))
        {
            int width = PlayerPrefs.GetInt("resolutionWidth");
            int height = PlayerPrefs.GetInt("resolutionHeight");
            bool isFullScreen = PlayerPrefs.GetInt("isFullScreen") == 1;
            Screen.SetResolution(width, height, isFullScreen);
        }

        // Apply volume settings
        if (audioMixer != null)
        {
            float volumeMaster = Mathf.Clamp(PlayerPrefs.GetFloat("masterVolume", 1f), 0.0001f, 1f);
            float volumeMusic = Mathf.Clamp(PlayerPrefs.GetFloat("musicVolume", 1f), 0.0001f, 1f);
            float volumeSfx = Mathf.Clamp(PlayerPrefs.GetFloat("sfxVolume", 1f), 0.0001f, 1f);

            audioMixer.SetFloat("master", Mathf.Log10(volumeMaster) * 20);
            audioMixer.SetFloat("music", Mathf.Log10(volumeMusic) * 20);
            audioMixer.SetFloat("sfx", Mathf.Log10(volumeSfx) * 20);
        }
    }
}
