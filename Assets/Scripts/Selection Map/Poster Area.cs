using System.Collections;
using UnityEngine;

public class PosterArea : MonoBehaviour
{
    [SerializeField] private GameObject text;
    [SerializeField] private GameObject selectionPoster;

    private bool isInTriggerBounds = false;

    private Coroutine coroutineRef;

    void Start()
    {
        selectionPoster.SetActive(false);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInTriggerBounds = true;
            text.SetActive(true);
            if (coroutineRef == null)
            {
                coroutineRef = StartCoroutine(selectionRoutine());
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            text.SetActive(false);
            StopCoroutine(coroutineRef);
            coroutineRef = null;
            isInTriggerBounds = false;
            selectionPoster.SetActive(false);
        }
    }


    IEnumerator selectionRoutine()
    {
        while (isInTriggerBounds)
        {

            while (!Input.GetKeyDown(KeyCode.V))
            {
                yield return null;
            }

            selectionPoster.SetActive(true);

            yield return null;

            while (!Input.GetKeyDown(KeyCode.V))
            {
                yield return null;
            }
            selectionPoster.SetActive(false);

            yield return null;
        }
    }


}
