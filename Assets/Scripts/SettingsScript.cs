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
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }
        if (splatVolumeSlider != null)
        {
            splatVolumeSlider.onValueChanged.AddListener(SetSplatVolume);
        }
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        masterVolumeSlider.value = 0.5f;
        SetMasterVolume(0.5f);

        musicVolumeSlider.value = 0.5f;
        SetMusicVolume(0.5f);

        splatVolumeSlider.value = 0.5f;
        SetSplatVolume(0.5f);
    }

    public void SetMasterVolume(float sliderValue)
    {
        float dB = Mathf.Lerp(minVolumeDb, maxVolumeDb, sliderValue);
        audioMixer.SetFloat("MasterVolume", dB);

        PlayerPrefs.SetFloat("MasterVolume", sliderValue);

        UpdateMasterVolumeText(sliderValue);
    }

    public void SetSplatVolume(float sliderValue)
    {
        float dB = Mathf.Lerp(minVolumeDb, maxVolumeDb, sliderValue);
        audioMixer.SetFloat("Splat Volume", dB);

        PlayerPrefs.SetFloat("MasterVolume", sliderValue);

        UpdateSplatVolumeText(sliderValue);
    }

    public void SetMusicVolume(float sliderValue)
    {
        float dB = Mathf.Lerp(minVolumeDb, maxVolumeDb, sliderValue);
        audioMixer.SetFloat("Music Volume", dB);

        PlayerPrefs.SetFloat("MasterVolume", sliderValue);

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
