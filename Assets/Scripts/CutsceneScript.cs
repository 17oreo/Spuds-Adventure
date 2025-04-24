using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CutsceneScript : MonoBehaviour
{
    [SerializeField] private GameObject firstCutscene;
    [SerializeField] private GameObject lastCutscene;

    [SerializeField] private GameObject[] frames;
    [SerializeField] private String[] firstTexts;
    [SerializeField] private TextMeshProUGUI cutsceneText;

    private Coroutine myCoroutine;
    private bool cutSceneStarted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState == GameState.Cutscene && !cutSceneStarted)
        {
            if (!GameManager.Instance.gameWon)
            {
                startFirstScene();
                cutSceneStarted = true;
            }
            else 
            {
                startEndScene();
                cutSceneStarted = true;
            }
        }
    }

    public void startFirstScene()
    {
        if (myCoroutine == null)
        {
            StartCoroutine(firstCutSceneRoutine());
        }
        
    }

    IEnumerator firstCutSceneRoutine()
    {
        for (int i = 0; i < 4; i++)
        {
            frames[i].SetActive(true);
            
            cutsceneText.text = firstTexts[i];
            yield return new WaitForSeconds(4f);
            cutsceneText.text = "";
            frames[i].SetActive(false);
        }
        GameManager.Instance.cutScene1Played = true;
        GameManager.Instance.SetState(GameState.MainMenu);
    }

    public void startEndScene()
    {

    }
}
