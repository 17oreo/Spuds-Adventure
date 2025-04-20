using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsScript : MonoBehaviour
{

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumePercentageText;

    // Define the min and max dB range
    private float minVolumeDb = -80f;
    private float maxVolumeDb = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        volumeSlider.value = 0.5f;
        SetVolume(0.5f);
    }

    public void SetVolume(float sliderValue)
    {
        float dB = Mathf.Lerp(minVolumeDb, maxVolumeDb, sliderValue);
        audioMixer.SetFloat("MasterVolume", dB);

        PlayerPrefs.SetFloat("MasterVolume", sliderValue);

        UpdateVolumeText(sliderValue);
    }

    private void UpdateVolumeText(float sliderValue)
    {
        int percent = Mathf.RoundToInt(sliderValue * 100f); 
        volumePercentageText.text = percent + "%";
    }
}
