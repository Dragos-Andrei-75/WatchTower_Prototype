using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
    [Header("Character Object and Component References")]
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Transform characterCameraTransform;
    [SerializeField] private Transform characterBodyTransform;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private CharacterLook characterLook;

    [Header("Physics")]
    [SerializeField] private float gravity = -25.0f;
    [SerializeField] private float friction = 10.0f;

    [Header("Character States")]
    [SerializeField] private Transform sphereTransform;
    [SerializeField] private LayerMask layerCharacter;
    [SerializeField] private LayerMask layerSurface;
    [SerializeField] private LayerMask layerInteractable;
    [SerializeField] private Vector3 cameraStand;
    [SerializeField] private Vector3 cameraCrouch;
    [SerializeField] private Vector3 cameraTarget;
    [SerializeField] private Vector3 centerStand;
    [SerializeField] private Vector3 centerCrouch;
    [SerializeField] private Vector3 centerTarget;
    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private float heightTarget;
    [SerializeField] private bool checkSurface = false;
    [SerializeField] private bool checkSlip = false;
    [SerializeField] private bool checkFall = false;
    [SerializeField] private bool checkJump = false;
    [SerializeField] private bool checkCrouch = false;
    [SerializeField] private bool checkVault = false;
    [SerializeField] private bool checkSlide = false;
    [SerializeField] private bool checkWallRun = false;
    [SerializeField] private bool checkClimb = false;
    [SerializeField] private bool checkSwim = false;
    [SerializeField] private bool checkDash = false;
    [SerializeField] private bool checkCharge = false;
    [SerializeField] private bool checkGrapple = false;
    [SerializeField] private bool checkCarry = false;

    [Header("Character Attributes")]
    [SerializeField] private Vector3 move;
    [SerializeField] private Vector3 characterVelocity;
    [SerializeField] private float speedMove = 0.0f;
    [SerializeField] private float speedRun = 25.0f;
    [SerializeField] private float speedWalk = 12.5f;
    [SerializeField] private float speedAir = 10.0f;
    [SerializeField] private float speedCrouch = 12.5f;
    [SerializeField] private float speedVault = 7.5f;
    [SerializeField] private float speedSlide = 75.0f;
    [SerializeField] private float speedClimb = 7.5f;
    [SerializeField] private float speedSwim = 12.5f;
    [SerializeField] private float speedSink = 5.0f;
    [SerializeField] private float speedDash = 50.0f;
    [SerializeField] private float acceleration = 12.5f;
    [SerializeField] private float gaitRate = 2.5f;
    [SerializeField] private float airControl = 0.75f;
    [SerializeField] private float jumpHeight = 1.25f;
    [SerializeField] private float reactionTimeAir = 0.5f;
    [SerializeField] private float reactionTimeJump = 0.25f;
    [SerializeField] private float heightStand = 1.85f;
    [SerializeField] private float heightCrouch = 1.0f;
    [SerializeField] private float headTilt = 0.0f;
    [SerializeField] private float headTiltAngle = 12.5f;
    [SerializeField] private float timeCrouch = 0.125f;
    [SerializeField] private float timeWallRun = 0.0f;
    [SerializeField] private float timeTilt = 0.125f;
    [SerializeField] private float timeDash = 0.25f;
    [SerializeField] private float timeChargeDash = 1.25f;
    [SerializeField] private float forcePush = 1.0f;
    [SerializeField] private int jumpCountMax = 2;
    [SerializeField] private int jumpCount = 0;
    [SerializeField] private int dashCountMax = 3;
    [SerializeField] private int dashCount = 0;

    [Header("Object Information")]
    [SerializeField] private Transform ladderTransform;
    [SerializeField] private ControllerColliderHit wallHit;
    [SerializeField] private RaycastHit hitSlope;

    [Header("Input")]
    [SerializeField] private InputPlayer inputPlayer;
    [SerializeField] private InputAction inputMove;
    [SerializeField] private InputAction inputWalk;
    [SerializeField] private InputAction inputJump;
    [SerializeField] private InputAction inputCrouch;
    [SerializeField] private InputAction inputSwim;
    [SerializeField] private InputAction inputDash;
    [SerializeField] private float inputX;
    [SerializeField] private float inputZ;
    [SerializeField] private float swimX;
    [SerializeField] private float swimZ;
    [SerializeField] private bool jump = false;
    [SerializeField] private bool crouch = false;
    [SerializeField] private bool swimOut = false;

    private Vector3 smoothCamera;
    private Vector3 smoothCenter;
    private float smoothHeight;
    private float smoothSwimHorizontal;
    private float smoothSwimVertical;

    private Coroutine coroutineGaitRun;
    private Coroutine coroutineGaitWalk;
    private Coroutine coroutineVault;
    private Coroutine coroutineSlide;
    private Coroutine coroutineWallRun;
    private Coroutine coroutineDash;

    public Vector3 CharacterVelocity
    {
        get { return characterVelocity; }
        set { characterVelocity = value; }
    }

    public bool CheckSlide
    {
        get { return checkSlide; }
    }

    public bool CheckSwim
    {
        get { return checkSwim; }
    }

    public bool CheckGrapple
    {
        get { return checkGrapple; }
        set { checkGrapple = value; }
    }

    public bool CheckCarry
    {
        get { return checkCarry; }
        set { checkCarry = value; }
    }

    private void Awake()
    {
        inputPlayer = new InputPlayer();

        inputMove = inputPlayer.Character.Move;

        inputWalk = inputPlayer.Character.Walk;
        inputWalk.started += _ => coroutineGaitWalk = StartCoroutine(Gait(speedWalk));
        inputWalk.canceled += _ => coroutineGaitRun = StartCoroutine(Gait(speedRun));

        inputJump = inputPlayer.Character.Jump;
        inputJump.started += _ => jump = true;
        inputJump.started += _ => StartCoroutine(CheckClearance());
        inputJump.canceled += _ => jump = false;

        inputCrouch = inputPlayer.Character.Crouch;
        inputCrouch.started += _ => StartCoroutine(CheckClearance());

        inputSwim = inputPlayer.Character.SwimOut;
        inputSwim.started += _ => swimOut = true;
        inputSwim.canceled += _ => swimOut = false;

        inputDash = inputPlayer.Character.Dash;
        inputDash.started += _ => coroutineDash = StartCoroutine(Dash());
    }

    private void OnEnable()
    {
        inputPlayer.Enable();
        inputMove.Enable();
        inputWalk.Enable();
        inputJump.Enable();
        inputCrouch.Enable();
        inputSwim.Enable();
        inputDash.Enable();

        speedMove = speedRun;
        dashCount = dashCountMax;

        Pause.onPauseResume -= OnEnable;
        Pause.onPauseResume += OnDisable;
    }

    private void OnDisable()
    {
        inputPlayer.Disable();
        inputMove.Disable();
        inputWalk.Disable();
        inputJump.Disable();
        inputCrouch.Disable();
        inputSwim.Disable();
        inputDash.Disable();

        StopAllCoroutines();

        Pause.onPauseResume += OnEnable;
        Pause.onPauseResume -= OnDisable;
    }

    private void Start()
    {
        characterTransform = gameObject.transform;
        characterCameraTransform = characterTransform.GetChild(0).transform;
        characterBodyTransform = characterTransform.GetChild(1).transform;
        characterController = gameObject.GetComponent<CharacterController>();
        characterLook = characterTransform.GetChild(0).GetComponent<CharacterLook>();

        sphereTransform = characterTransform.GetChild(characterTransform.childCount - 1);

        layerCharacter = LayerMask.GetMask("Player");
        layerSurface = LayerMask.GetMask("Ground");
        layerInteractable = LayerMask.GetMask("Interactable");

        cameraStand = new Vector3(0, heightStand / 2, 0);
        cameraCrouch = Vector3.zero;
        centerStand = Vector3.zero;
        centerCrouch = new Vector3(0, - (heightCrouch / 2) + 0.075f, 0);
    }

    private void Update()
    {
        InputMove();
        Gravity();
        Move();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Climb"))
        {
            checkClimb = true;
            ladderTransform = other.transform;
            StartCoroutine(Climb());
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            checkSwim = true;
            StartCoroutine(Swim());
        }

        if (checkClimb == true || checkSwim == true)
        {
            if (checkFall == true)
            {
                checkFall = false;
            }
            if (checkJump == true)
            {
                checkJump = false;
            }
            else if (crouch == true)
            {
                StartCoroutine(Crouch());

                if (checkSlide == true)
                {
                    StopCoroutine(coroutineSlide);
                    checkSlide = false;
                }
            }
            else if (checkWallRun == true)
            {
                StopCoroutine(coroutineWallRun);
                StartCoroutine(WallRunExit());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Climb"))
        {
            if (ladderTransform != null) StartCoroutine(ClimbExit());
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            characterVelocity.y = Mathf.Sqrt(-8 * gravity * jumpHeight);
            checkSwim = false;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody hitRigidbody = hit.collider.attachedRigidbody;
        MeshRenderer hitMeshRenderer = hit.transform.GetComponentInChildren<MeshRenderer>();
        float hitAngle = Vector3.Angle(Vector3.up, hit.normal);

        if (hitRigidbody != null && hitRigidbody.isKinematic == false)
        {
            Vector3 forceDirection = (hitRigidbody.transform.position - characterBodyTransform.position).normalized;

            if (hit.moveDirection.y >= 0) hitRigidbody.AddForceAtPosition(forceDirection * forcePush, hit.point, ForceMode.Impulse);
        }
        else if (hitAngle >= characterController.slopeLimit)
        {
            if (checkDash == true)
            {
                StopCoroutine(coroutineDash);
                checkDash = false;
            }

            if (checkSurface == true)
            {
                if (checkSlip == false && hit.point.y <= sphereTransform.position.y && hitAngle <= 85) StartCoroutine(Slip());
            }
            else
            {
                if (wallHit != hit)
                {
                    if (characterController.radius < hitMeshRenderer.bounds.size.x || characterController.radius < hitMeshRenderer.bounds.size.z)
                    {
                        if (characterController.height < hitMeshRenderer.bounds.size.y)
                        {
                            bool checkWallLeft = Physics.Raycast(characterBodyTransform.position, -characterBodyTransform.right, out RaycastHit hitLeft,
                                                                 characterController.radius * 2, ~layerInteractable, QueryTriggerInteraction.Ignore);
                            bool checkWallRight = Physics.Raycast(characterBodyTransform.position, characterBodyTransform.right, out RaycastHit hitRight,
                                                                  characterController.radius * 2, ~layerInteractable, QueryTriggerInteraction.Ignore);

                            wallHit = hit;

                            if (coroutineWallRun != null) StopCoroutine(coroutineWallRun);

                            if (checkWallLeft == true && hitLeft.transform == wallHit.transform) coroutineWallRun = StartCoroutine(WallRun(1, -headTiltAngle));
                            else if (checkWallRight == true && hitRight.transform == wallHit.transform) coroutineWallRun = StartCoroutine(WallRun(-1, headTiltAngle));
                            else if (checkWallRun == true) StartCoroutine(WallRunExit());
                        }
                    }
                }
            }
        }
    }

    private void InputMove()
    {
        inputX = inputMove.ReadValue<Vector2>().x;
        inputZ = inputMove.ReadValue<Vector2>().y;

        move = new Vector3(inputX, 0, inputZ);
        move = characterTransform.TransformDirection(move);
        move.Normalize();
    }

    private void Gravity()
    {
        bool contact = Physics.CheckSphere(sphereTransform.position, sphereRadius, ~layerCharacter, QueryTriggerInteraction.Ignore);

        if (checkSurface != contact)
        {
            checkSurface = contact;
            StartCoroutine(CheckSurface());
        }

        if (checkSurface == false)
        {
            if (checkVault == true || checkWallRun == true || checkClimb == true || checkSwim == true || checkDash == true || checkGrapple == true) return;
            characterVelocity.y += gravity * Time.deltaTime;
        }
    }

    private void Friction()
    {
        float velocityHorizontal;
        float velocityHorizontalDrop;
        float velocityHorizontalNew;

        velocityHorizontal = Mathf.Sqrt(Mathf.Pow(characterVelocity.x, 2) + Mathf.Pow(characterVelocity.z, 2));
        velocityHorizontalDrop = velocityHorizontal > friction ? velocityHorizontal * friction * Time.deltaTime : Mathf.Pow(friction, 2) * Time.deltaTime;
        velocityHorizontalNew = velocityHorizontal - velocityHorizontalDrop > 0 ? (velocityHorizontal - velocityHorizontalDrop) / velocityHorizontal : 0;

        characterVelocity.x *= velocityHorizontalNew;
        characterVelocity.z *= velocityHorizontalNew;
    }

    private void Acceleration(float speed)
    {
        float speedCurrent = Vector3.Dot(characterVelocity, move);
        float speedAdd = speed - speedCurrent;

        if (speedAdd <= 0) return;

        if (speedCurrent < speed)
        {
            float speedAcceleration = speed * acceleration * Time.deltaTime;

            if (speedAcceleration > speedAdd) speedAcceleration = speedAdd;

            characterVelocity.x += move.x * speedAcceleration;
            characterVelocity.z += move.z * speedAcceleration;
        }
    }

    private void AirControl()
    {
        float velocityHorizontal;
        float velocityVertical;
        float dot;
        float k;

        velocityHorizontal = Mathf.Sqrt(Mathf.Pow(characterVelocity.x, 2) + Mathf.Pow(characterVelocity.z, 2));
        velocityVertical = characterVelocity.y;

        characterVelocity.y = 0;
        characterVelocity.Normalize();

        dot = Vector3.Dot(characterVelocity, move);
        k = 32 * Mathf.Pow(dot, 2) * airControl * Time.deltaTime;

        if (dot > 0)
        {
            characterVelocity.x = characterVelocity.x * velocityHorizontal + move.x * k;
            characterVelocity.z = characterVelocity.z * velocityHorizontal + move.z * k;

            characterVelocity.Normalize();
        }

        characterVelocity.x *= velocityHorizontal;
        characterVelocity.y = velocityVertical;
        characterVelocity.z *= velocityHorizontal;
    }

    private void MoveGround()
    {
        Friction();
        Acceleration(speedMove);
    }

    private void MoveAir()
    {
        Acceleration(speedAir);
        AirControl();
    }

    private void Move()
    {
        if (checkSlip == false && checkVault == false && checkSlide == false && checkWallRun == false && checkClimb == false && checkSwim == false && checkDash == false)
        {
            if (checkSurface == true) MoveGround();
            else MoveAir();
        }

        characterController.Move(characterVelocity * Time.deltaTime);
    }

    private IEnumerator CheckSurface()
    {
        if (checkSurface == true)
        {
            gravity = -25;
            jumpCount = 0;
            checkFall = false;
            checkJump = false;

            characterVelocity.y = -7.5f;

            if (checkClimb == true) StartCoroutine(ClimbExit());
        }
        else
        {
            if (checkClimb == true || checkSwim == true || checkDash == true || checkGrapple == true) yield break;

            float depth = 7.5f;

            if (checkJump == true)
            {
                gravity = -37.5f;
            }
            else
            {
                checkFall = true;
                StartCoroutine(Jump(reactionTimeAir));
            }

            if (Physics.Raycast(sphereTransform.position, Vector3.down, depth) == false)
            {
                while (checkCrouch == true) yield return null;
                StartCoroutine(CheckClearance());
            }
        }

        yield break;
    }

    private IEnumerator CheckClearance()
    {
        if (checkVault == true || checkWallRun == true || checkSwim == true) yield break;

        if (characterController.height == heightCrouch)
        {
            float distanceClear;
            bool clear;

            distanceClear = heightStand - heightCrouch;
            clear = !Physics.Raycast(characterCameraTransform.position, Vector3.up, distanceClear, ~layerInteractable, QueryTriggerInteraction.Ignore);

            if (clear == false)
            {
                yield break;
            }
            else if (jump == true)
            {
                StartCoroutine(Crouch());
                if (checkSlide == false) yield break;
            }

            if (checkSlide == true)
            {
                checkSlide = false;
                StopCoroutine(coroutineSlide);
            }
        }

        if (jump == true) StartCoroutine(Jump(reactionTimeJump));
        else if (checkCrouch == false) StartCoroutine(Crouch());

        yield break;
    }

    private IEnumerator Slip()
    {
        characterVelocity.x = 0;
        characterVelocity.z = 0;

        Physics.SphereCast(characterBodyTransform.position, sphereRadius, -characterBodyTransform.up, out hitSlope, characterController.height / 2);

        checkSlip = true;

        while (checkSurface == true && Vector3.Angle(Vector3.up, hitSlope.normal) > characterController.slopeLimit)
        {
            Physics.SphereCast(characterBodyTransform.position, sphereRadius, -characterBodyTransform.up, out hitSlope, characterController.height / 2);
            characterVelocity += new Vector3(hitSlope.normal.x, 0, hitSlope.normal.z);

            yield return null;
        }

        checkSlip = false;

        yield break;
    }

    private IEnumerator Gait(float speedTarget)
    {
        if (crouch == true) yield break;

        if (coroutineGaitRun != null && speedTarget == speedWalk) StopCoroutine(coroutineGaitRun);
        if (coroutineGaitWalk != null && speedTarget == speedRun) StopCoroutine(coroutineGaitWalk);

        while (Mathf.Round(speedMove * 10) / 10 != speedTarget)
        {
            speedMove = Mathf.Lerp(speedMove, speedTarget, gaitRate * Time.deltaTime);
            yield return null;
        }

        speedMove = speedTarget;

        yield break;
    }

    private IEnumerator Jump(float reactionTime)
    {
        while (reactionTime >= 0)
        {
            reactionTime -= Time.deltaTime;

            if (jump == true)
            {
                if (checkSurface == true || (checkFall == true && checkJump == false) || ((checkFall == true || checkJump == true) && jumpCount < jumpCountMax))
                {
                    characterVelocity.y = Mathf.Sqrt(-2 * gravity * jumpHeight);
                    break;
                }
                else if (checkClimb == true)
                {
                    StartCoroutine(ClimbExit());
                    break;
                }
                else if (checkWallRun == true)
                {
                    characterVelocity += (wallHit.normal + Vector3.up).normalized * Mathf.Sqrt(-2 * gravity * jumpHeight);
                    break;
                }
            }

            yield return null;
        }

        if (reactionTime >= 0)
        {
            if (checkJump == false) checkJump = true;
            if (checkFall == false) checkFall = false;

            jumpCount++;

            jump = false;
        }

        if (coroutineVault != null && checkVault == false) StopCoroutine(coroutineVault);
        coroutineVault = StartCoroutine(Vault());

        yield break;
    }

    private IEnumerator Crouch()
    {
        if (checkSurface == false && crouch == false) yield break;

        crouch = !crouch;

        checkCrouch = true;

        if (crouch == true)
        {
            if (characterTransform.InverseTransformDirection(characterVelocity).z > speedWalk) coroutineSlide = StartCoroutine(Slide());

            cameraTarget = cameraCrouch;
            centerTarget = centerCrouch;
            heightTarget = heightCrouch;

            speedMove = speedCrouch;
        }
        else
        {
            cameraTarget = cameraStand;
            centerTarget = centerStand;
            heightTarget = heightStand;

            speedMove = speedRun;
        }

        while (Mathf.Abs(characterController.height - heightTarget) > 0.001f)
        {
            characterCameraTransform.localPosition = Vector3.SmoothDamp(characterCameraTransform.localPosition, cameraTarget, ref smoothCamera, timeCrouch);

            characterController.center = Vector3.SmoothDamp(characterController.center, centerTarget, ref smoothCenter, timeCrouch);
            characterController.height = Mathf.SmoothDamp(characterController.height, heightTarget, ref smoothHeight, timeCrouch);

            yield return null;
        }

        characterCameraTransform.localPosition = cameraTarget;

        characterController.center = centerTarget;
        characterController.height = heightTarget;

        checkCrouch = false;

        yield break;
    }

    private IEnumerator Vault()
    {
        if (checkCarry == true) yield break;

        Vector3 climbPoint;
        Vector3 climbDirection;
        float offsetVertical = 0.5f;
        float offsetHorizontal = 1;
        float rayLength = 1;

        while (checkJump == true)
        {
            Vector3 ray1Origin = characterCameraTransform.position + characterBodyTransform.up * offsetVertical + characterBodyTransform.forward * offsetHorizontal;
            Vector3 ray1Direction = -characterBodyTransform.up;

            bool ray1Contact = Physics.Raycast(ray1Origin, ray1Direction, out RaycastHit ray1Hit, rayLength, layerSurface, QueryTriggerInteraction.Ignore);

            if (ray1Contact == true)
            {
                Vector3 ray2Origin = characterCameraTransform.position;
                Vector3 ray2Direction = characterBodyTransform.forward;

                bool ray2Contact = Physics.Raycast(ray2Origin, ray2Direction, out RaycastHit ray2Hit, rayLength, layerSurface, QueryTriggerInteraction.Ignore);

                if (ray2Contact == true)
                {
                    if (Vector3.Distance(ray2Hit.point, ray1Hit.point) < rayLength)
                    {
                        Vector3 climbPointHorizontal;
                        Vector3 checkSphereHorizontal;

                        climbPoint = new Vector3(ray2Hit.point.x, ray1Hit.point.y, ray2Hit.point.z);
                        climbPointHorizontal = new Vector3(climbPoint.x, 0, climbPoint.z);

                        checkSphereHorizontal = Vector3.zero;

                        characterVelocity = Vector3.zero;

                        checkVault = true;

                        while (Vector3.Distance(climbPointHorizontal, checkSphereHorizontal) > 0.1f)
                        {
                            if (sphereTransform.position.y < climbPoint.y)
                            {
                                characterVelocity.y = speedVault;
                            }
                            else
                            {
                                checkSphereHorizontal = new Vector3(sphereTransform.position.x, 0, sphereTransform.position.z);

                                climbDirection = (climbPoint - sphereTransform.position);
                                climbDirection.Normalize();

                                characterVelocity = climbDirection * speedVault * 2.5f;
                            }

                            yield return null;
                        }

                        checkVault = false;
                    }
                }
            }

            yield return null;
        }

        coroutineVault = null;

        yield break;
    }

    private IEnumerator Slide()
    {
        if (checkCarry == true) yield break;

        Vector3 positionPrevious = characterBodyTransform.position;
        Vector3 slideDirection;
        Vector3 slideHorizontal = move;
        Vector3 slideVertical = Vector3.down;
        float slideSpeed = speedDash;

        characterLook.LookXReference = characterLook.MouseX;

        characterVelocity.y = -1;

        checkSlide = true;

        while (Mathf.Round(slideSpeed * 10) / 10 > 3.75f)
        {
            slideDirection = characterBodyTransform.position - positionPrevious;
            slideDirection.Normalize();

            if (Physics.Raycast(characterBodyTransform.position, slideDirection, 1, ~layerCharacter, QueryTriggerInteraction.Ignore) == true) break;

            if (checkSurface == true)
            {
                if (Mathf.Round(Vector3.Angle(Vector3.up, slideDirection)) > 90) slideSpeed = Mathf.Lerp(slideSpeed, speedSlide, Time.deltaTime);
                else if (Mathf.Round(Vector3.Angle(Vector3.up, slideDirection)) == 90) slideSpeed = Mathf.Lerp(slideSpeed, 0, Time.deltaTime);
                else slideSpeed = Mathf.Lerp(slideSpeed, 0, Time.deltaTime * 2);

                slideVertical = Vector3.down * 7.5f;
            }
            else
            {
                slideVertical.y += gravity * Time.deltaTime;
            }

            positionPrevious = characterBodyTransform.position;

            characterVelocity = (slideHorizontal * slideSpeed) + slideVertical;

            yield return null;
        }

        checkSlide = false;

        yield break;
    }

    private IEnumerator WallRun(int inputCancel, float headTiltTarget)
    {
        if (checkCarry == true) yield break;

        Vector3 wallRunDirection = Vector3.Cross(Vector3.up, wallHit.normal).normalized;
        float timePassed = 0;
        bool checkWall = true;

        if ((Mathf.Round(Vector3.Dot(characterBodyTransform.forward, characterVelocity)) < 0 || characterVelocity.y < -7.5f) && checkWallRun == false) yield break;

        if (Vector3.Dot(characterCameraTransform.forward, wallRunDirection) < 0) wallRunDirection = -wallRunDirection;

        if (timeWallRun == 0) timeWallRun = Mathf.Lerp(1.25f, 1.75f, characterVelocity.magnitude / speedDash);

        checkJump = false;
        checkWallRun = true;

        while (inputX != inputCancel && checkSurface == false && jump == false && checkWall == true && timeWallRun > 0)
        {
            checkWall = Physics.Raycast(characterBodyTransform.position, -wallHit.normal, characterController.radius * 2, ~layerInteractable);

            if (timeWallRun > 1)
            {
                if (timePassed < timeTilt)
                {
                    headTilt = Mathf.LerpAngle(0, headTiltTarget, timePassed / timeTilt);
                    timePassed += Time.deltaTime;
                }
            }
            else
            {
                if (timeWallRun > 0) headTilt = Mathf.LerpAngle(0, headTiltTarget, timeWallRun);
            }

            timeWallRun -= Time.deltaTime;

            characterCameraTransform.localRotation = Quaternion.Euler(characterCameraTransform.localEulerAngles.x, characterCameraTransform.localEulerAngles.y, headTilt);

            characterVelocity = wallRunDirection * speedMove;

            yield return null;
        }

        if (jump == true)
        {
            StartCoroutine(Jump(reactionTimeJump));
            jumpCount = 1;
        }

        StartCoroutine(WallRunExit());

        yield break;
    }

    private IEnumerator WallRunExit()
    {
        float headTiltStart = characterCameraTransform.localEulerAngles.z;
        float timePassed = 0;

        checkWallRun = false;
        timeWallRun = 0;
        wallHit = null;

        if (checkJump == false && checkClimb == false) checkFall = true;

        while (timePassed < timeTilt)
        {
            headTilt = Mathf.LerpAngle(headTiltStart, 0, timePassed / timeTilt);
            characterCameraTransform.localRotation = Quaternion.Euler(characterCameraTransform.localEulerAngles.x, characterCameraTransform.localEulerAngles.y, headTilt);

            timePassed += Time.deltaTime;

            yield return null;
        }

        characterCameraTransform.localRotation = Quaternion.Euler(characterCameraTransform.localEulerAngles.x, characterCameraTransform.localEulerAngles.y, 0);

        yield break;
    }

    private IEnumerator Climb()
    {
        if (checkCarry == true) yield break;

        Vector3 directionUp = Vector3.Dot(ladderTransform.up, characterBodyTransform.up) > 0 ? ladderTransform.up : -ladderTransform.up;

        while (checkClimb == true)
        {
            float angle = Vector3.Angle(ladderTransform.forward, characterBodyTransform.forward);

            Vector3 climbVertical = characterCameraTransform.localRotation.x < 0.125f ? directionUp * inputZ : -directionUp * inputZ;
            Vector3 climbHorizontal = angle < 90 ? ladderTransform.right * (inputX / 3) : -ladderTransform.right * (inputX / 3);

            characterVelocity = (climbVertical + climbHorizontal) * speedClimb;

            yield return null;
        }

        yield break;
    }

    private IEnumerator ClimbExit()
    {
        Vector3 characterOnLadder;
        Vector3 ladderToCharacter;
        Vector3 direction;
        float ladderLimitLeft;
        float ladderLimitRight;
        float value;

        characterOnLadder = ladderTransform.TransformDirection(characterBodyTransform.position);

        ladderToCharacter = characterBodyTransform.position - ladderTransform.position;
        ladderToCharacter.y = 0;
        ladderToCharacter.Normalize();

        ladderLimitLeft = ladderTransform.TransformDirection(ladderTransform.position).x - (ladderTransform.GetComponent<BoxCollider>().size.x / 2);
        ladderLimitRight = ladderTransform.TransformDirection(ladderTransform.position).x + (ladderTransform.GetComponent<BoxCollider>().size.x / 2);

        if ((characterCameraTransform.localRotation.x < 0.125f && inputZ < 0) || (characterCameraTransform.localRotation.x > 0.125f && inputZ > 0))
        {
            if (checkSurface == true)
            {
                characterVelocity.y = 0;
            }
        }
        else if (characterBodyTransform.position.y > ladderTransform.position.y + (ladderTransform.GetComponent<BoxCollider>().size.y / 2))
        {
            if ((characterCameraTransform.localRotation.x < 0.125f && inputZ > 0) || (characterCameraTransform.localRotation.x > 0.125f && inputZ < 0))
            {
                characterVelocity.y = Mathf.Sqrt(-2 * gravity);
            }
        }
        else if (characterOnLadder.x < ladderLimitLeft || characterOnLadder.x > ladderLimitRight)
        {
            value = Vector3.Angle(ladderTransform.forward, characterTransform.forward);
            direction = value < 90 ? ladderTransform.right : -ladderTransform.right;

            characterVelocity = direction * inputX * speedAir;
        }
        else if (jump == true)
        {
            value = Vector3.Dot(ladderTransform.forward, ladderToCharacter);
            direction = value > 0 ? ladderTransform.forward : -ladderTransform.forward;

            characterVelocity = direction * Mathf.Sqrt(-2 * gravity * jumpHeight);
        }
        else yield break;

        ladderTransform = null;
        checkClimb = false;

        yield break;
    }

    private IEnumerator Swim()
    {
        while (checkSwim == true)
        {
            Vector3 swimHorizontal;
            Vector3 swimVertical;
            float smoothInputX = inputX != 0 ? 0.125f : 0.25f;
            float smoothInputZ = inputZ != 0 ? 0.125f : 0.25f;

            swimX = Mathf.SmoothDamp(swimX, inputX, ref smoothSwimHorizontal, smoothInputX);
            swimZ = Mathf.SmoothDamp(swimZ, inputZ, ref smoothSwimVertical, smoothInputZ);

            swimHorizontal = characterCameraTransform.forward * swimZ + characterCameraTransform.right * swimX;
            swimVertical = swimOut == true ? Vector3.up * speedSwim : -Vector3.up * speedSink;

            if (swimHorizontal != Vector3.zero && swimOut == false) swimVertical = -Vector3.up;

            characterVelocity = (swimHorizontal * speedSwim) + swimVertical;

            yield return null;
        }

        yield break;
    }

    private IEnumerator Dash()
    {
        if (checkCrouch == true || checkVault == true || checkWallRun == true || checkClimb == true || checkDash == true || checkCarry == true) yield break;
        else if (crouch == true || checkSlide == true) StartCoroutine(CheckClearance());

        if (move != Vector3.zero && dashCount > 0)
        {
            Vector3 direction = move;
            Vector3 momentum = new Vector3(inputX, 0, inputZ);
            float timeStart = Time.time;

            dashCount--;

            if (checkCharge == false)
            {
                StartCoroutine(ChargeDash());
                checkCharge = true;
            }

            checkDash = true;

            while (Time.time <= timeStart + timeDash)
            {
                characterVelocity = direction * speedDash;
                yield return null;
            }

            checkDash = false;

            characterVelocity = (characterTransform.right * momentum.x + characterTransform.forward * momentum.z) * speedMove;
        }

        yield break;
    }

    private IEnumerator ChargeDash()
    {
        while (dashCount < dashCountMax)
        {
            yield return new WaitForSeconds(timeChargeDash);
            dashCount++;
        }

        checkCharge = false;

        yield break;
    }
}
