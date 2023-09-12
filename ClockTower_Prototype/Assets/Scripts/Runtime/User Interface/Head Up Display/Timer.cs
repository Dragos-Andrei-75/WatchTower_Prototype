using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    //UI
    private TextMeshProUGUI timerText;

    //Timer Attributes
    private string stringHours;
    private string stringMinutes;
    private string stringSeconds;
    private float milliseconds = 0;
    private int seconds = 0;
    private int minutes = 0;
    private int hours = 0;

    private int Seconds
    {
        get
        {
            return seconds;
        }
        set
        {
            seconds = value;

            TimerDisplay();
        }
    }

    private void Awake() => timerText = gameObject.GetComponent<TextMeshProUGUI>();

    private void OnEnable() => Pause.OnResume += TimerCount;

    private void OnDisable() => Pause.OnResume -= TimerCount;

    private void TimerDisplay()
    {
        stringHours = hours < 10 ? "0" + hours.ToString() : hours.ToString();
        stringMinutes = minutes < 10 ? "0" + minutes.ToString() : minutes.ToString();
        stringSeconds = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();

        timerText.text = stringHours + ":" + stringMinutes + ":" + stringSeconds;
    }

    private void TimerCount()
    {
        milliseconds += Time.deltaTime;

        if (milliseconds >= 1)
        {
            Seconds++;
            milliseconds = 0;
        }
        else if (seconds == 60)
        {
            minutes++;
            Seconds = 0;
        }
        else if (minutes == 60)
        {
            hours++;
            minutes = 0;
        }
    }
}
