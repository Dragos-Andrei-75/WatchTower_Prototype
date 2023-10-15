using UnityEngine;

public class InputCharacterSingleton<T> : MonoBehaviour where T : Component
{
    private static T instance;

    private static InputCharacter inputCharacter;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject;

                gameObject = new GameObject();
                gameObject.isStatic = true;
                gameObject.hideFlags = HideFlags.HideAndDontSave;

                instance = gameObject.AddComponent<T>();
            }

            return instance;
        }
    }

    public static InputCharacter Character
    {
        get
        {
            if (inputCharacter == null) inputCharacter = new InputCharacter();

            return inputCharacter;
        }
    }

    protected virtual void OnEnable()
    {
        Pause.OnPauseResume -= OnEnable;
        Pause.OnPauseResume += OnDisable;
    }

    protected virtual void OnDisable()
    {
        Pause.OnPauseResume += OnEnable;
        Pause.OnPauseResume -= OnDisable;
    }

    protected void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
