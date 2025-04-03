using System.Collections;
using UnityEngine;

public class UI_Script : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject mainCanvas;

    public bool gameStart = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        startCanvas.SetActive(true);
        mainCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        gameStart = true;
        mainCanvas.SetActive(true);
        startCanvas.SetActive(false);
    }


    public void WinScreen()
    {
        winPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        // Stop play mode if in the editor
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
