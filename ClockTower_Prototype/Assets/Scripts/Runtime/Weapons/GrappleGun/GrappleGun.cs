using UnityEngine;
using System.Collections;

public class GrappleGun : Weapon
{
    [Header("Grapple Gun Object and Component Referecnes")]
    [SerializeField] private Transform grappleGunTransform;

    [Header("Grapple Gun Child Object and Component References")]
    [SerializeField] private Transform hookOriginTransform;
    [SerializeField] private Transform hookTransform;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Character Object and Component References")]
    [SerializeField] private Transform characterBodyTransform;
    [SerializeField] private CharacterMovement characterMovement;

    [Header("Grapple Gun Attributes")]
    [SerializeField] private RaycastHit hookHit;
    [SerializeField] private LayerMask hookIgnore;
    [SerializeField] private float timeToMoveMax = 0.375f;
    [SerializeField] private float timeToMoveMin = 0.125f;
    [SerializeField] private float timeToMove;
    [SerializeField] private float timePassedMove = 0.0f;
    [SerializeField] private float timeToRotate = 1.0f;
    [SerializeField] private float timePassedRotate = 0.0f;
    [SerializeField] private float ropeLength;

    [Header("Character Attributes")]
    [SerializeField] private Vector3 grappleDirection;
    [SerializeField] private float grappleSpeedMax = 50.0f;
    [SerializeField] private float grappleSpeedMin = 25.0f;
    [SerializeField] private float grappleSpeed;
    [SerializeField] private bool grappled = false;

    public delegate IEnumerator GrappleInteractable(Transform transform);
    public static event GrappleInteractable OnGrappleInteractable;

    private enum HookPosition : ushort { hookOrigin, hookTarget };

    protected override void OnEnable() => base.OnEnable();

    protected override void OnDisable() => base.OnDisable();

    private void Start()
    {
        grappleGunTransform = gameObject.transform;

        hookOriginTransform = grappleGunTransform.GetChild(0).GetComponent<Transform>();
        hookTransform = grappleGunTransform.GetChild(1).GetComponent<Transform>();
        lineRenderer = hookOriginTransform.GetComponent<LineRenderer>();

        characterBodyTransform = grappleGunTransform.root.GetChild(1).GetComponent<Transform>();
        characterMovement = grappleGunTransform.root.GetComponent<CharacterMovement>();

        hookIgnore = LayerMask.GetMask("Player");
    }

    protected override void Shoot()
    {
        if (grappled == false) StartCoroutine(Grapple());
    }

    private IEnumerator Grapple()
    {
        grappled = Physics.Raycast(CharacterCameraTransform.position, CharacterCameraTransform.forward, out hookHit, WeaponData.range, ~hookIgnore, QueryTriggerInteraction.Ignore);

        if (grappled == true)
        {
            hookTransform.SetParent(null);

            StartCoroutine(GrappleDraw());
            StartCoroutine(GrappleRotate());
            StartCoroutine(GrappleMove(HookPosition.hookTarget));

            while (hookTransform.position != hookHit.point) yield return null;

            if (hookHit.transform.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                GameObject hookContact;
                Vector3 grappleDirectionHorizontal;
                float forceImpulse;

                hookContact = new GameObject("Hook Contact");
                hookContact.transform.position = hookHit.point;
                hookContact.transform.SetParent(hookHit.transform);

                hookTransform.SetParent(hookContact.transform);

                ropeLength = Vector3.Distance(hookOriginTransform.position, hookContact.transform.position);
                grappleSpeed = grappleSpeedMax;

                forceImpulse = Mathf.Lerp(2.5f, 17.5f, ropeLength / WeaponData.range);

                hookHit.rigidbody.AddForce(Vector3.up * forceImpulse, ForceMode.Impulse);

                while (Vector3.Distance(CharacterCameraTransform.position, hookContact.transform.position) > 5)
                {
                    grappleDirection = CharacterCameraTransform.position - hookContact.transform.position;

                    grappleDirectionHorizontal = new Vector3(grappleDirection.x, 0, grappleDirection.z);

                    if (Vector3.Dot(characterBodyTransform.forward, grappleDirectionHorizontal) > 0) break;

                    if (characterBodyTransform.position.y + 5 < hookHit.transform.position.y) grappleDirection -= characterBodyTransform.up;

                    if (Vector3.Angle(characterBodyTransform.right, -grappleDirectionHorizontal) > 90) grappleDirection += characterBodyTransform.right;
                    else if (Vector3.Angle(characterBodyTransform.right, -grappleDirectionHorizontal) < 90) grappleDirection -= characterBodyTransform.right;

                    grappleDirection.Normalize();

                    hookHit.rigidbody.AddForce(grappleDirection * grappleSpeed, ForceMode.Force);

                    yield return null;
                }

                if(Vector3.Distance(CharacterCameraTransform.position, hookContact.transform.position) < 5)
                {
                    if (OnGrappleInteractable != null) StartCoroutine(OnGrappleInteractable(hookHit.transform));
                }

                Destroy(hookContact);
            }
            else
            {
                grappleDirection = (hookHit.point - characterBodyTransform.position).normalized;

                while (Vector3.Distance(hookOriginTransform.position, hookHit.point) > 5)
                {
                    ropeLength = Vector3.Distance(hookOriginTransform.position, hookHit.point);
                    grappleSpeed = Mathf.Lerp(grappleSpeedMax, grappleSpeedMin, ropeLength / WeaponData.range);

                    characterMovement.CharacterVelocity = grappleDirection * grappleSpeed;

                    yield return null;
                }
            }

            StartCoroutine(GrappleMove(HookPosition.hookOrigin));
            StartCoroutine(GrappleEnd());
        }

        yield break;
    }

    private IEnumerator GrappleMove(HookPosition hookPosition)
    {
        Vector3 staticPoint;

        timeToMove = Mathf.Lerp(timeToMoveMin, timeToMoveMax, ropeLength / WeaponData.range);

        if (hookPosition == HookPosition.hookTarget) staticPoint = hookHit.point;
        else staticPoint = hookTransform.position;

        while (timePassedMove < timeToMove)
        {
            if (hookPosition == HookPosition.hookTarget) hookTransform.position = Vector3.Lerp(hookOriginTransform.position, staticPoint, timePassedMove / timeToMove);
            else if (hookPosition == HookPosition.hookOrigin) hookTransform.position = Vector3.Lerp(staticPoint, hookOriginTransform.position, timePassedMove / timeToMove);

            timePassedMove += Time.deltaTime;

            yield return null;
        }

        if (hookPosition == HookPosition.hookTarget) hookTransform.position = hookHit.point;
        else hookTransform.position = hookOriginTransform.position;

        timePassedMove = 0;
        timeToMove = 0;

        yield break;
    }

    private IEnumerator GrappleRotate()
    {
        while (Vector3.Distance(hookOriginTransform.position, hookTransform.position) < 3.75f) yield return null;

        while (grappled == true || hookTransform.position != hookOriginTransform.position)
        {
            grappleGunTransform.LookAt(hookTransform.position);
            yield return null;
        }

        while (grappleGunTransform.localEulerAngles != Vector3.zero)
        {
            grappleGunTransform.localRotation = Quaternion.Lerp(grappleGunTransform.localRotation, Quaternion.Euler(Vector3.zero), timePassedRotate / timeToRotate);
            timePassedRotate += Time.deltaTime;

            yield return null;
        }

        grappleGunTransform.localEulerAngles = Vector3.zero;

        timePassedRotate = 0;

        yield break;
    }

    private IEnumerator GrappleDraw()
    {
        lineRenderer.positionCount = 2;

        while (grappled == true || hookTransform.position != hookOriginTransform.position)
        {
            lineRenderer.SetPosition(0, hookOriginTransform.position);
            lineRenderer.SetPosition(1, hookTransform.position);

            yield return null;
        }

        lineRenderer.positionCount = 0;

        yield break;
    }

    private IEnumerator GrappleEnd()
    {
        hookTransform.SetParent(grappleGunTransform);
        grappleDirection = Vector3.zero;
        grappled = false;

        yield break;
    }
}
