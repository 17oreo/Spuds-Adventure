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
    [SerializeField] private float time = 5f;

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
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameManager.Instance.cutScene1Played = true;
            GameManager.Instance.SelectBoss();
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
            if (i == 1)
            {
                yield return new WaitForSeconds(time+1f);
            }
            else
            {
                yield return new WaitForSeconds(time);
            }
            
            cutsceneText.text = "";
            frames[i].SetActive(false);
        }
        GameManager.Instance.cutScene1Played = true;
        GameManager.Instance.SelectBoss();
    }

    public void startEndScene()
    {

    }
}
