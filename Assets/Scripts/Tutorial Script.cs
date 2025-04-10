using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI tutorialText;

    public void StartTutorialRoutine()
    {
        
        StartCoroutine(TutorialRoutine());
    }

    IEnumerator TutorialRoutine()
    {
        tutorialText.text = "Use the LEFT and RIGHT ARROW keys to move";
        //wait until the user clicks the left or right key
        while (!Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.RightArrow))
        {
            yield return null;
        }

        tutorialText.text = "Good Job!";
        yield return new WaitForSeconds(1f);

        tutorialText.text = "Click Z to Jump";
        while (!Input.GetKeyDown(KeyCode.Z))
        {
            yield return null;
        }

        tutorialText.text = "Good Job!";
        yield return new WaitForSeconds(1f);

        tutorialText.text = "Click the DOWN ARROW key to duck";
        while (!Input.GetKeyDown(KeyCode.DownArrow))
        {
            yield return null;
        }

        tutorialText.text = "Good Job!";
        yield return new WaitForSeconds(1f);

        tutorialText.text = "Click LEFT SHIFT to dash";
        while (!Input.GetKeyDown(KeyCode.LeftShift))
        {
            yield return null;
        }

        tutorialText.text = "Good Job!";
        yield return new WaitForSeconds(1f);

        tutorialText.text = "Click X to shoot";
        while (!Input.GetKeyDown(KeyCode.X))
        {
            yield return null;
        }
        
        tutorialText.text = "Good Job!";
        yield return new WaitForSeconds(1f);

        tutorialText.text = "Click ENTER to continue to the main menu";
        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }
        GameManager.Instance.SetState(GameState.MainMenu);
        
    }
}
