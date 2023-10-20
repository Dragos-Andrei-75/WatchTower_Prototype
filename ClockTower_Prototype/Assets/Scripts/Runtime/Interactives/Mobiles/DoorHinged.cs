using UnityEngine;
using System.Collections;

public class DoorHinged : Door
{
    [Header("Rotary Motion Attributes")]
    [SerializeField] private Quaternion rotation1;
    [SerializeField] private Quaternion rotation2;
    [SerializeField] private Quaternion rotationCurrent;
    [SerializeField] private Quaternion rotationTarget;
    [SerializeField] private float openRotation = 90.0f;

    public float OpenRotation
    {
        get { return openRotation; }
        set { openRotation = value; }
    }

    public override void Setup()
    {
        base.Setup();

        DoorType = DoorTypes.Rotary;

        rotation1 = MobileTransform.rotation;
    }

    public override void Interact()
    {
        base.Interact();
        MoveMobile(DoorHingedMove());
    }

    private IEnumerator DoorHingedMove()
    {
        if (MobileTransform.eulerAngles == rotation1.eulerAngles)
        {
            Transform characterTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            Vector3 characterPositionHorizontal = new Vector3(characterTransform.position.x, 0, characterTransform.position.z);
            Vector3 doorPositionHorizontal = new Vector3(MobileTransform.position.x, 0, MobileTransform.position.z);
            Vector3 doorToCharacterHorizontal = characterPositionHorizontal - doorPositionHorizontal;
            float dot = Vector3.Dot(MobileTransform.forward, doorToCharacterHorizontal);

            if (dot < 0) rotation2 = Quaternion.Euler(MobileTransform.eulerAngles.x, MobileTransform.eulerAngles.y + openRotation, MobileTransform.eulerAngles.z);
            else rotation2 = Quaternion.Euler(MobileTransform.eulerAngles.x, MobileTransform.eulerAngles.y - openRotation, MobileTransform.eulerAngles.z);
        }

        rotationCurrent = rotationCurrent == rotation1 ? rotation2 : rotation1;
        rotationTarget = rotationCurrent == rotation1 ? rotation2 : rotation1;

        if (MobileTransform.rotation == rotation1) InteractivePortal.open = true;

        while (MobileTransform.eulerAngles != rotationTarget.eulerAngles)
        {
            MobileRigidbody.MoveRotation(Quaternion.Lerp(rotationCurrent, rotationTarget, TimePassed / TimeToMove));
            TimePassed += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        if (MobileTransform.rotation == rotation1) InteractivePortal.open = false;

        yield break;
    }
}
