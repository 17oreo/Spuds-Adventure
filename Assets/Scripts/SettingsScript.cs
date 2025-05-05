using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsScript : MonoBehaviour
{

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private TMP_Text masterVolumePercentageText;
    [SerializeField] private Slider splatVolumeSlider;
    [SerializeField] private TMP_Text splatVolumePercentageText;

    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TMP_Text musicVolumePercentageText;

    // Define the min and max dB range
    private float minVolumeDb = -80f;
    private float maxVolumeDb = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Load saved values or default to 1.0
        float masterValue = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicValue = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float splatValue = PlayerPrefs.GetFloat("SplatVolume", 1f);

        // Set sliders and apply volumes
        masterVolumeSlider.value = masterValue;
        SetMasterVolume(masterValue);

        musicVolumeSlider.value = musicValue;
        SetMusicVolume(musicValue);

        splatVolumeSlider.value = splatValue;
        SetSplatVolume(splatValue);

        // Add listeners AFTER setting initial values to avoid triggering twice
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        
        if (splatVolumeSlider != null)
            splatVolumeSlider.onValueChanged.AddListener(SetSplatVolume);
    }


    public void SetMasterVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("MasterVolume", dB);

        PlayerPrefs.SetFloat("MasterVolume", sliderValue);

        UpdateMasterVolumeText(sliderValue);
    }

    public void SetSplatVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("Splat Volume", dB);

        PlayerPrefs.SetFloat("SplatVolume", sliderValue);

        UpdateSplatVolumeText(sliderValue);
    }

    public void SetMusicVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("Music Volume", dB);

       PlayerPrefs.SetFloat("MusicVolume", sliderValue);

        UpdateMusicVolumeText(sliderValue);
    }

    private void UpdateMasterVolumeText(float sliderValue)
    {
        int percent = Mathf.RoundToInt(sliderValue * 100f); 
        masterVolumePercentageText.text = percent + "%";
    }

    private void UpdateSplatVolumeText(float sliderValue)
    {
        int percent = Mathf.RoundToInt(sliderValue * 100f); 
        splatVolumePercentageText.text = percent + "%";
    }
    private void UpdateMusicVolumeText(float sliderValue)
    {
        int percent = Mathf.RoundToInt(sliderValue * 100f); 
        musicVolumePercentageText.text = percent + "%";
    }
}
