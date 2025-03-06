using System.Collections;
using UnityEngine;

public class UI_Script : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WinScreen()
    {
        winPanel.SetActive(true);
    }
}
