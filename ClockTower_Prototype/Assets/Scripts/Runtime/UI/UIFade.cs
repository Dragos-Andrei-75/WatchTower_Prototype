using UnityEngine;
using System.Collections;

public class UIFade : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [SerializeField] private float speedFade = 10.0f;

    private void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }

    public IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime * speedFade;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        canvasGroup.alpha = 1;

        yield break;
    }

    public IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime * speedFade;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        canvasGroup.alpha = 0;

        yield break;
    }
}
