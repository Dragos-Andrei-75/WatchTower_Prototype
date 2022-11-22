using UnityEngine;
using TMPro;

public class Clock : MonoBehaviour
{
    private TextMeshProUGUI clockText;

    private int hours = 0;
    private int minutes = 0;
    private int seconds = 0;
    private double miliseconds = 0.0f;
    private string stringHours;
    private string stringMinutes;
    private string stringSeconds;

    private void Start()
    {
        clockText = gameObject.GetComponent<TextMeshProUGUI>();
    }

    private void FixedUpdate()
    {
        miliseconds += Mathf.Round(Time.fixedDeltaTime * 1000) / 1000;

        if (miliseconds >= 1.0f || seconds == 60 || minutes == 60 || hours == 24) DisplayTime();

        if (miliseconds >= 1.0f)
        {
            miliseconds = 0;
            seconds++;
        }

        if (seconds == 60)
        {
            seconds = 0;
            minutes++;
        }

        if (minutes == 60)
        {
            minutes = 0;
            hours++;
        }

        if (hours == 24)
        {
            hours = 0;
        }
    }

    private void DisplayTime()
    {
        stringHours = hours < 10 ? "0" + hours.ToString() : hours.ToString();
        stringMinutes = minutes < 10 ? "0" + minutes.ToString() : minutes.ToString();
        stringSeconds = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();

        clockText.text = stringHours + ":" + stringMinutes + ":" + stringSeconds;
    }
}
