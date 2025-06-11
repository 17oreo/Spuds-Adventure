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

        //jump
        tutorialText.text = "Click Z to Jump";
        while (!Input.GetButtonDown("Jump"))
        {
            yield return null;
        }

        tutorialText.text = "Good Job!";
        yield return new WaitForSeconds(1f);

        //duck
        tutorialText.text = "Click the DOWN ARROW key to duck";
        while (!Input.GetKeyDown(KeyCode.DownArrow))
        {
            yield return null;
        }

        tutorialText.text = "Good Job!";
        yield return new WaitForSeconds(1f);

        //dash
        tutorialText.text = "Click LEFT SHIFT to dash";
        while (!Input.GetKeyDown(KeyCode.LeftShift))
        {
            yield return null;
        }

        tutorialText.text = "Good Job!";
        yield return new WaitForSeconds(1f);

        //shooting
        tutorialText.text = "Click X to shoot";
        while (!Input.GetKeyDown(KeyCode.X))
        {
            yield return null;
        }
        
        tutorialText.text = "Good Job!";

        yield return new WaitForSeconds(1f);



        //shoot up while standing still
        tutorialText.text = "Hold UP ARROW to aim straight up while standing still, then press X to shoot";

        bool hasShotUpWhileStanding = false;

        while (!hasShotUpWhileStanding)
        {
            bool isAimingUp = Input.GetKey(KeyCode.UpArrow);
            bool isStandingStill = Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.1f;

            if (isAimingUp && isStandingStill && Input.GetKeyDown(KeyCode.X))
            {
                hasShotUpWhileStanding = true;
            }

            yield return null;
        }
        tutorialText.text = "Good Job!";



        yield return new WaitForSeconds(1f);


        //shoot while aiming up and moving
        tutorialText.text = "Now move and aim up, then shoot!";
        bool hasShotUpWhileMoving = false;

        while (!hasShotUpWhileMoving)
        {
            bool isAimingUp = Input.GetKey(KeyCode.UpArrow);
            bool isMoving = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f;

            if (isAimingUp && isMoving && Input.GetKeyDown(KeyCode.X))
            {
                hasShotUpWhileMoving = true;
            }

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
