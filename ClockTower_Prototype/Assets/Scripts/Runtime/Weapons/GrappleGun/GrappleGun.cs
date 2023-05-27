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
    [SerializeField] private Transform hookHitTransform;
    [SerializeField] private LayerMask hookIgnore;
    [SerializeField] private float timeToMoveMax = 0.375f;
    [SerializeField] private float timeToMoveMin = 0.125f;
    [SerializeField] private float timeToMove;
    [SerializeField] private float timePassedMove = 0.0f;
    [SerializeField] private float timeToRotate = 1.0f;
    [SerializeField] private float timePassedRotate = 0.0f;
    [SerializeField] private float ropeLengthMax = 100;
    [SerializeField] private float ropeLength;

    [Header("Character Attributes")]
    [SerializeField] private Vector3 grappleDirection;
    [SerializeField] private float grappleSpeedMax = 85.0f;
    [SerializeField] private float grappleSpeedMin = 15.0f;
    [SerializeField] private float grappleSpeed;
    [SerializeField] private bool grappled = false;

    public delegate void GrappleInteractable(Transform transform);
    public static event GrappleInteractable OnGrappleInteractable;

    private enum HookPosition : ushort { hookOrigin, hookTarget };

    protected override void OnEnable() => base.OnEnable();

    protected override void OnDisable() => base.OnDisable();

    private void Start()
    {
        grappleGunTransform = gameObject.transform;

        hookOriginTransform = grappleGunTransform.GetChild(0).GetChild(0).GetComponent<Transform>();
        hookTransform = grappleGunTransform.GetChild(0).GetChild(1).GetComponent<Transform>();
        lineRenderer = hookOriginTransform.GetComponent<LineRenderer>();

        characterBodyTransform = grappleGunTransform.root.GetChild(1).GetComponent<Transform>();
        characterMovement = grappleGunTransform.root.GetComponent<CharacterMovement>();

        hookIgnore = LayerMask.GetMask("Player");
    }

    protected override void ShootPrimary() { if (grappled == false) StartCoroutine(Grapple()); }

    private IEnumerator Grapple()
    {
        RaycastHit hookHit;

        grappled = Physics.Raycast(CharacterCameraTransform.position, CharacterCameraTransform.forward, out hookHit, ropeLengthMax, ~hookIgnore, QueryTriggerInteraction.Ignore);

        if (grappled == true)
        {
            hookTransform.SetParent(null);

            hookHitTransform = new GameObject("Hook Hit").transform;
            hookHitTransform.position = hookHit.point;
            hookHitTransform.SetParent(hookHit.transform);

            StartCoroutine(GrappleDraw());
            StartCoroutine(GrappleRotate());
            StartCoroutine(GrappleMove(HookPosition.hookTarget));

            while (hookTransform.position != hookHitTransform.position && hookHit.transform != null) yield return null;

            if (hookHit.transform != null)
            {
                if (hookHit.transform.gameObject.layer == LayerMask.NameToLayer("Interactable"))
                {
                    Vector3 grappleDirectionHorizontal;
                    float forceImpulse;
                    float angularDrag;
                    float drag;

                    drag = hookHit.rigidbody.drag;
                    angularDrag = hookHit.rigidbody.angularDrag;
                    forceImpulse = Mathf.Lerp(2.5f, 15.0f, ropeLength / ropeLengthMax);

                    hookHit.rigidbody.drag = 0.5f;
                    hookHit.rigidbody.angularDrag = 0.0f;

                    hookHit.rigidbody.AddForce(Vector3.up * forceImpulse, ForceMode.Impulse);

                    ropeLength = Vector3.Distance(hookOriginTransform.position, hookHitTransform.position);
                    grappleSpeed = grappleSpeedMax;

                    while (hookHit.transform != null)
                    {
                        if (Vector3.Distance(CharacterCameraTransform.position, hookHitTransform.position) > 5)
                        {
                            hookTransform.position = hookHitTransform.position;

                            grappleDirection = CharacterCameraTransform.position - hookHitTransform.position;

                            grappleDirectionHorizontal = new Vector3(grappleDirection.x, 0, grappleDirection.z);

                            if (Vector3.Dot(characterBodyTransform.forward, grappleDirectionHorizontal) > 0) break;

                            if (characterBodyTransform.position.y + 5 < hookHitTransform.position.y) grappleDirection -= characterBodyTransform.up;

                            if (Vector3.Angle(characterBodyTransform.right, -grappleDirectionHorizontal) > 90) grappleDirection += characterBodyTransform.right;
                            else if (Vector3.Angle(characterBodyTransform.right, -grappleDirectionHorizontal) < 90) grappleDirection -= characterBodyTransform.right;

                            grappleDirection.Normalize();

                            hookHit.rigidbody.AddForce(grappleDirection * grappleSpeed, ForceMode.Force);
                        }
                        else
                        {
                            hookHit.rigidbody.angularDrag = angularDrag;
                            hookHit.rigidbody.drag = drag;

                            if (Vector3.Distance(CharacterCameraTransform.position, hookHitTransform.position) < 5)
                            {
                                if (OnGrappleInteractable != null) OnGrappleInteractable(hookHit.transform);
                            }

                            break;
                        }

                        yield return new WaitForFixedUpdate();
                    }
                }
                else
                {
                    characterMovement.CheckGrapple = true;

                    grappleDirection = (hookHitTransform.position - characterBodyTransform.position).normalized;

                    while (Vector3.Distance(hookOriginTransform.position, hookHitTransform.position) > 5)
                    {
                        ropeLength = Vector3.Distance(hookOriginTransform.position, hookHit.point);
                        grappleSpeed = Mathf.Lerp(grappleSpeedMax, grappleSpeedMin, ropeLength / ropeLengthMax);

                        characterMovement.CharacterVelocity = grappleDirection * grappleSpeed;

                        yield return null;
                    }
                }
            }

            StartCoroutine(GrappleMove(HookPosition.hookOrigin));
            StartCoroutine(GrappleEnd());
        }

        yield break;
    }

    private IEnumerator GrappleMove(HookPosition hookPosition)
    {
        Transform transform;

        timeToMove = Mathf.Lerp(timeToMoveMin, timeToMoveMax, ropeLength / ropeLengthMax);

        if (hookPosition == HookPosition.hookTarget) transform = hookHitTransform;
        else transform = hookTransform;

        while (timePassedMove < timeToMove)
        {
            if (hookPosition == HookPosition.hookTarget) hookTransform.position = Vector3.Lerp(hookOriginTransform.position, transform.position, timePassedMove / timeToMove);
            else hookTransform.position = Vector3.Lerp(transform.position, hookOriginTransform.position, timePassedMove / timeToMove);

            timePassedMove += Time.deltaTime;

            yield return null;
        }

        if (hookPosition == HookPosition.hookTarget) hookTransform.position = hookHitTransform.position;
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
        if (hookHitTransform != null) Destroy(hookHitTransform.gameObject);

        hookTransform.SetParent(grappleGunTransform);
        grappleDirection = Vector3.zero;
        grappled = false;

        characterMovement.CheckGrapple = false;

        yield break;
    }
}
