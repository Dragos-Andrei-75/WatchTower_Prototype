using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    //UI References
    private UIFade uiFade;

    //Input
    private InputMenu inputMenu;
    private InputAction PauseResume;
    private static bool pause = false;

    public delegate void ActionPauseResume();
    public static event ActionPauseResume onPauseResume;

    private void Awake()
    {
        inputMenu = new InputMenu();

        PauseResume = inputMenu.MenuPause.PauseResume;
        PauseResume.started += _ => PauseResumeGame();
    }

    private void Start()
    {
        uiFade = gameObject.transform.root.GetComponent<UIFade>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        inputMenu.Enable();
        PauseResume.Enable();
    }

    private void OnDisable()
    {
        inputMenu.Disable();
        PauseResume.Disable();
    }

    private void PauseGame()
    {
        StartCoroutine(uiFade.FadeOut());

        Time.timeScale = 0;
        AudioListener.pause = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ResumeGame()
    {
        StartCoroutine(uiFade.FadeIn());

        Time.timeScale = 1;
        AudioListener.pause = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void PauseResumeGame()
    {
        onPauseResume();

        pause = !pause;

        if (pause == true) PauseGame();
        else ResumeGame();
    }
}
