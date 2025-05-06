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
    [SerializeField] private String[] endTexts;
    [SerializeField] private TextMeshProUGUI firstCutsceneText;
    [SerializeField] private TextMeshProUGUI endCutsceneText;
    [SerializeField] private float time = 5f;

    private Coroutine firstCoroutine;
    private Coroutine endCoroutine;
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
        }
        if (Input.GetKeyDown(KeyCode.Return) && !GameManager.Instance.cutScene1Played)
        {
            GameManager.Instance.cutScene1Played = true;
            GameManager.Instance.SelectBoss();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && !GameManager.Instance.cutScene2Played)
        {
            GameManager.Instance.cutScene2Played = true;
            GameManager.Instance.MainMenu();
        }
    }

    public void startFirstScene()
    {
        if (firstCoroutine == null)
        {
            StartCoroutine(firstCutSceneRoutine());
        }
    }

    public void startEndScene()
    {
        if (endCoroutine == null)
        {
            StartCoroutine(endCutSceneRoutine());
        }
    }

    IEnumerator firstCutSceneRoutine()
    {
        for (int i = 0; i < 4; i++)
        {
            frames[i].SetActive(true);
            
            firstCutsceneText.text = firstTexts[i];
            if (i == 1)
            {
                yield return new WaitForSeconds(time+1f);
            }
            else
            {
                yield return new WaitForSeconds(time);
            }
            
            firstCutsceneText.text = "";
            frames[i].SetActive(false);
        }
        GameManager.Instance.cutScene1Played = true;
        GameManager.Instance.SelectBoss();

        cutSceneStarted = false;
    }

    IEnumerator endCutSceneRoutine()
    {
        for (int i = 4; i < 8; i++)
        {
            frames[i].SetActive(true);
            
            endCutsceneText.text = endTexts[i-4];
            if (i == 1)
            {
                yield return new WaitForSeconds(time+1f);
            }
            else
            {
                yield return new WaitForSeconds(time);
            }
            
            endCutsceneText.text = "";
            frames[i].SetActive(false);
        }
        GameManager.Instance.cutScene2Played = true;
        GameManager.Instance.MainMenu();

        cutSceneStarted = false;
    }
}
