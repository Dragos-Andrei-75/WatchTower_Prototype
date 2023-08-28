using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Pause : MonoBehaviour
{
    //Input
    private InputMenu inputMenu;
    private InputAction inputPauseResume;
    private static bool pause = false;

    public delegate void ActionPauseResume();
    public static event ActionPauseResume OnResume;
    public static event ActionPauseResume OnPauseResume;

    public delegate IEnumerator ActionPauseResumeCoroutine();
    public static event ActionPauseResumeCoroutine OnPauseCoroutine;
    public static event ActionPauseResumeCoroutine OnResumeCoroutine;

    private void Awake()
    {
        inputMenu = new InputMenu();

        inputPauseResume = inputMenu.MenuPause.PauseResume;
    }

    private void OnEnable()
    {
        inputMenu.Enable();
        inputPauseResume.Enable();

        inputPauseResume.started += PauseResumeGame;
    }

    private void OnDisable()
    {
        inputMenu.Disable();
        inputPauseResume.Disable();

        inputPauseResume.started -= PauseResumeGame;
    }

    private void Start() => StartCoroutine(UIStart());

    private void PauseGame()
    {
        Time.timeScale = 0;

        AudioListener.pause = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (OnPauseCoroutine != null) StartCoroutine(OnPauseCoroutine());
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;

        AudioListener.pause = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (OnResumeCoroutine != null) StartCoroutine(OnResumeCoroutine());

        StartCoroutine(UIUpdate());
    }

    private void PauseResumeGame(InputAction.CallbackContext contextPauseResume)
    {
        pause = !pause;

        if (pause == true) PauseGame();
        else ResumeGame();

        if (OnPauseResume != null) OnPauseResume();
    }

    private IEnumerator UIStart()
    {
        yield return new WaitForSeconds(5);

        ResumeGame();

        yield break;
    }

    private IEnumerator UIUpdate()
    {
        while (pause == false)
        {
            if (OnResume != null) OnResume();
            yield return null;
        }

        yield break;
    }
}
