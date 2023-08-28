using UnityEngine;
using TMPro;

public class FrameRate : MonoBehaviour
{
    //UI
    private TextMeshProUGUI fpsText;

    //FrameRate Attributes
    private float time = 0.0f;
    private float deltaTime = 0.0f;
    private float pollingTime = 0.1f;
    private int frameRate = 0;

    private void Awake() => fpsText = gameObject.GetComponent<TextMeshProUGUI>();

    private void OnEnable() => Pause.OnResume += CalculateFPS;

    private void OnDisable() => Pause.OnResume -= CalculateFPS;

    private void CalculateFPS()
    {
        time += Time.deltaTime;
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * pollingTime;

        if (time >= pollingTime)
        {
            frameRate = Mathf.RoundToInt(1 / deltaTime);
            time = 0;
        }

        fpsText.text = frameRate.ToString() + " FPS";
    }
}
