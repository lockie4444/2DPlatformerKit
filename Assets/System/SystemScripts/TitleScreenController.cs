using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

public class TitleScreenController : MonoBehaviour
{
    public Animator animator;
    public float timeToWait = 2f;
    private bool startPressed;

    private void Update()
    {
        if (startPressed) return;
#if ENABLE_INPUT_SYSTEM
        // Includes keyboard keys, mouse buttons, touch presses, and gamepad buttons.
        foreach (var device in InputSystem.devices)
        {
            foreach (var control in device.allControls)
            {
                if (control is ButtonControl button && button.wasPressedThisFrame)
                {
                    StartGamePressed();
                    return;
                }
            }
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.anyKeyDown) StartGamePressed();
#endif
    }

    // Also used by the title scene's existing PlayerInput event.
    public void StartGamePressed()
    {
        if (startPressed) return;
        startPressed = true;
        StartCoroutine(StartGameEnumerator());
    }

    private IEnumerator StartGameEnumerator()
    {
        if (animator != null) animator.SetTrigger("begin");
        yield return new WaitForSecondsRealtime(Mathf.Max(0f, timeToWait));
        SceneManager.LoadScene("FrozenKingdom");
    }
}