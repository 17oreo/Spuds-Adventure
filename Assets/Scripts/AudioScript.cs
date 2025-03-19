using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class AudioScript : MonoBehaviour
{

    [SerializeField] GameObject audioOn;
    [SerializeField] GameObject audioMuted;
    private AudioSource backgroundMusic;
    private bool isMuted = false;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backgroundMusic = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.M))
        {
            //unmute
            if (isMuted)
            {
                backgroundMusic.mute = false;
                isMuted = false;
                audioOn.SetActive(true);
                audioMuted.SetActive(false);
            }
            //mute
            else
            {
                backgroundMusic.mute = true;
                isMuted = true;
                audioOn.SetActive(false);
                audioMuted.SetActive(true);
            }
        }
    }
}
