using UnityEngine;
using System.Collections;

public class Rotary : Mobile
{
    [Header("Rotary Motion Attributes")]
    [SerializeField] private Quaternion rotation1;
    [SerializeField] private Quaternion rotation2;
    [SerializeField] private Quaternion rotationBegin;
    [SerializeField] private Quaternion rotationEnd;
    [SerializeField] private float openRotation = 90.0f;

    public float OpenRotation
    {
        get { return openRotation; }
        set { openRotation = value; }
    }

    public override void Setup()
    {
        base.Setup();

        MotionType = MotionTypes.Rotary;

        rotation1 = InteractiveTransform.rotation;
    }

    public override void Interact()
    {
        base.Interact();
        InteractiveMove(MotionRotary());
    }

    private IEnumerator MotionRotary()
    {
        if (InteractiveTransform.eulerAngles == rotation1.eulerAngles)
        {
            Transform characterTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            Vector3 characterPositionHorizontal = new Vector3(characterTransform.position.x, 0, characterTransform.position.z);
            Vector3 doorPositionHorizontal = new Vector3(InteractiveTransform.position.x, 0, InteractiveTransform.position.z);
            Vector3 doorToCharacterHorizontal = characterPositionHorizontal - doorPositionHorizontal;
            float dot = Vector3.Dot(InteractiveTransform.forward, doorToCharacterHorizontal);

            if (dot < 0) rotation2 = Quaternion.Euler(InteractiveTransform.eulerAngles.x, InteractiveTransform.eulerAngles.y + openRotation, InteractiveTransform.eulerAngles.z);
            else rotation2 = Quaternion.Euler(InteractiveTransform.eulerAngles.x, InteractiveTransform.eulerAngles.y - openRotation, InteractiveTransform.eulerAngles.z);
        }

        rotationBegin = rotationBegin == rotation1 ? rotation2 : rotation1;
        rotationEnd = rotationBegin == rotation1 ? rotation2 : rotation1;

        if (InteractiveTransform.rotation == rotation1) InteractivePortal.open = true;

        while (InteractiveTransform.eulerAngles != rotationEnd.eulerAngles)
        {
            InteractiveRigidbody.MoveRotation(Quaternion.Lerp(rotationBegin, rotationEnd, TimePassed / TimeToMove));
            TimePassed += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        if (InteractiveTransform.rotation == rotation1) InteractivePortal.open = false;

        yield break;
    }
}
