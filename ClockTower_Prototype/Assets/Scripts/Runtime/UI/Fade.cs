using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour
{
    //Canvas
    private CanvasGroup canvasGroup;

    //Fade Attributes
    [SerializeField] private float speedFade = 10.0f;

    private void Awake() => canvasGroup = gameObject.transform.GetComponent<CanvasGroup>();

    private void OnEnable()
    {
        Pause.OnPauseCoroutine += FadeOut;
        Pause.OnResumeCoroutine += FadeIn;
    }

    private void OnDisable()
    {
        Pause.OnPauseCoroutine -= FadeOut;
        Pause.OnResumeCoroutine -= FadeIn;
    }

    private IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime * speedFade;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        yield break;
    }

    private IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime * speedFade;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        yield break;
    }
}
