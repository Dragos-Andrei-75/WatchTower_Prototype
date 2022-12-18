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
    [SerializeField] private bool checkJump = false;
    [SerializeField] private bool checkCrouch = false;
    [SerializeField] private bool checkVault = false;
    [SerializeField] private bool checkSlide = false;
    [SerializeField] private bool checkWallRun = false;
    [SerializeField] private bool checkClimb = false;
    [SerializeField] private bool checkSwim = false;
    [SerializeField] private bool checkDash = false;
    [SerializeField] private bool checkCharge = false;

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
    [SerializeField] private float rateGait = 2.5f;
    [SerializeField] private float airControl = 1.0f;
    [SerializeField] private float jumpHeight = 1.25f;
    [SerializeField] private float reactionTimeAir = 0.5f;
    [SerializeField] private float reactionTimeJump = 0.25f;
    [SerializeField] private float heightStand = 1.85f;
    [SerializeField] private float heightCrouch = 1.0f;
    [SerializeField] private float headTilt = 0.0f;
    [SerializeField] private float headTiltAngle = 12.5f;
    [SerializeField] private float timeCrouch = 0.125f;
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
            if (checkJump == true)
            {
                checkJump = false;
            }
            else if (crouch == true)
            {
                StartCoroutine(Crouch());
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
            if (ladderTransform != null)
            {
                StartCoroutine(ClimbExit());
                ladderTransform = null;
                checkClimb = false;
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            characterVelocity.y = Mathf.Sqrt(-gravity);
            checkSwim = false;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody hitRigidbody = hit.collider.attachedRigidbody;
        MeshRenderer hitMeshRenderer = hit.transform.GetComponent<MeshRenderer>();
        float hitAngle = Vector3.Angle(Vector3.up, hit.normal);

        if (hitRigidbody != null && hitRigidbody.isKinematic == false)
        {
            Vector3 forceDirection = hitRigidbody.transform.position - characterBodyTransform.position;

            forceDirection.Normalize();

            if (hit.moveDirection.y >= 0) hitRigidbody.AddForceAtPosition(forceDirection * forcePush, hit.point, ForceMode.Impulse);
        }
        else if (hitAngle >= characterController.slopeLimit && hitAngle <= 90)
        {
            if (checkDash == true)
            {
                StopCoroutine(coroutineDash);
                checkDash = false;
            }

            if (checkSurface == true)
            {
                float offset = hit.point.y - sphereTransform.position.y;

                if (checkSlip == false && offset > characterController.stepOffset)
                {
                    Physics.SphereCast(characterBodyTransform.position, sphereRadius, -characterBodyTransform.up, out hitSlope, characterController.height / 2, ~layerSurface);
                    StartCoroutine(Slip());
                }
            }
            else
            {
                if (inputZ >= 0 && wallHit != hit && characterController.height < hitMeshRenderer.bounds.size.y)
                {
                    bool checkWallLeft = Physics.Raycast(characterBodyTransform.position, -characterBodyTransform.right, out RaycastHit hitLeft, 1, ~layerInteractable);
                    bool checkWallRight = Physics.Raycast(characterBodyTransform.position, characterBodyTransform.right, out RaycastHit hitRight, 1, ~layerInteractable);

                    wallHit = hit;

                    if (coroutineWallRun != null) StopCoroutine(coroutineWallRun);

                    if (checkWallLeft == true && hitLeft.transform == hit.transform) coroutineWallRun = StartCoroutine(WallRun(-headTiltAngle));
                    else if (checkWallRight == true && hitRight.transform == hit.transform) coroutineWallRun = StartCoroutine(WallRun(headTiltAngle));
                    else StartCoroutine(WallRunExit());
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
        bool contact = Physics.CheckSphere(sphereTransform.position, sphereRadius, layerSurface) || Physics.CheckSphere(sphereTransform.position, sphereRadius, layerInteractable);

        if (checkSurface != contact)
        {
            checkSurface = contact;
            StartCoroutine(CheckSurface());
        }

        if (checkSurface == false)
        {
            if (checkVault == true || checkWallRun == true || checkClimb == true || checkSwim == true || checkDash == true) return;
            characterVelocity.y += gravity * Time.deltaTime;
        }
    }

    private void Friction()
    {
        float velocityHorizontal;
        float velocityHorizontalDrop;
        float velocityHorizontalNew;

        velocityHorizontal = Mathf.Sqrt(characterVelocity.x * characterVelocity.x + characterVelocity.z * characterVelocity.z);
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
        if (move == Vector3.zero) return;

        float velocityHorizontal;
        float velocityVertical;
        float dot;
        float k;

        velocityHorizontal = Mathf.Sqrt(characterVelocity.x * characterVelocity.x + characterVelocity.z * characterVelocity.z);
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
            characterVelocity.y = -7.5f;
            gravity = -25;
            jumpCount = 0;
            checkJump = false;

            if (checkClimb == true) StartCoroutine(ClimbExit());
        }
        else
        {
            if (checkClimb == true || checkSwim == true || checkDash == true) yield break;

            float depth = 7.5f;

            if (checkJump == true) gravity = -37.5f;
            else StartCoroutine(Jump(reactionTimeAir));

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
        if (checkVault == true || checkWallRun == true) yield break;

        if (characterController.height == heightCrouch)
        {
            float distanceClear;
            bool clear;

            distanceClear = heightStand - heightCrouch;
            clear = Physics.Raycast(characterCameraTransform.position, Vector3.up, distanceClear) == false;

            if (clear == false)
            {
                yield break;
            }
            else if (jump == true)
            {
                StartCoroutine(Crouch());
                if (checkSlide == false) yield break;
            }
        }

        if (checkSlide == true)
        {
            checkSlide = false;
            StopCoroutine(coroutineSlide);
        }

        if (jump == true) StartCoroutine(Jump(reactionTimeJump));
        else if (checkCrouch == false) StartCoroutine(Crouch());

        yield break;
    }

    private IEnumerator Slip()
    {
        characterVelocity.x = 0;
        characterVelocity.z = 0;

        checkSlip = true;

        while (checkSurface == true && Vector3.Angle(Vector3.up, hitSlope.normal) > characterController.slopeLimit)
        {
            Physics.SphereCast(characterBodyTransform.position, sphereRadius, -characterBodyTransform.up, out hitSlope, characterController.height / 2, ~layerSurface);
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
            speedMove = Mathf.Lerp(speedMove, speedTarget, rateGait * Time.deltaTime);
            yield return null;
        }

        speedMove = speedTarget;

        yield break;
    }

    private IEnumerator Jump(float reactionTime)
    {
        bool checkFall = reactionTime == reactionTimeAir;

        while (reactionTime >= 0)
        {
            reactionTime -= Time.deltaTime;

            if (jump == true)
            {
                if (checkSurface == true || (checkSurface == false && checkFall == true) || (checkJump == true && jumpCount < jumpCountMax))
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
            jumpCount++;

            checkJump = true;
            jump = false;
        }

        if (coroutineVault != null) StopCoroutine(coroutineVault);
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
        Vector3 climbPoint;
        Vector3 climbDirection;
        float offsetVertical = 0.5f;
        float offsetHorizontal = 1;
        float rayLength = 1;

        while (checkJump == true)
        {
            Vector3 ray1Origin = characterCameraTransform.position + characterBodyTransform.up * offsetVertical + characterBodyTransform.forward * offsetHorizontal;
            Vector3 ray1Direction = -characterBodyTransform.up;

            bool ray1Contact = Physics.Raycast(ray1Origin, ray1Direction, out RaycastHit ray1Hit, rayLength, layerSurface);

            if (ray1Contact == true)
            {
                Vector3 ray2Origin = characterCameraTransform.position;
                Vector3 ray2Direction = ray1Hit.point - ray2Origin;

                bool ray2Contact = Physics.Raycast(ray2Origin, ray2Direction, out RaycastHit ray2Hit, rayLength, layerSurface);

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

        yield break;
    }

    private IEnumerator Slide()
    {
        Vector3 positionPrevious = characterBodyTransform.position;
        Vector3 slideDirection = characterBodyTransform.forward;
        Vector3 slideHorizontal = move;
        Vector3 slideVertical = Vector3.down;
        float slideSpeed = speedDash;

        characterLook.LookXReference = characterLook.MouseX;

        checkSlide = true;

        while (Mathf.Round(slideSpeed * 10) / 10 > 3.75f)
        {
            if (checkSurface == true)
            {
                slideDirection = characterBodyTransform.position - positionPrevious;
                slideDirection.Normalize();
            }

            if (Physics.Raycast(characterBodyTransform.position, slideDirection, out RaycastHit hit, 1) == true)
            {
                if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Default") && hit.transform.gameObject.layer != LayerMask.NameToLayer("Interactable")) break;
            }

            if (Mathf.Round(Vector3.Angle(slideDirection, Vector3.up)) > 90) slideSpeed = Mathf.Lerp(slideSpeed, speedSlide, Time.deltaTime);
            else slideSpeed = Mathf.Lerp(slideSpeed, 0, Time.deltaTime);

            if (checkSurface == false) slideVertical.y += gravity * Time.deltaTime;
            else slideVertical.y = -7.5f;

            characterVelocity = (slideHorizontal * slideSpeed) + slideVertical;

            positionPrevious = characterBodyTransform.position;

            yield return null;
        }

        checkSlide = false;

        characterVelocity.y = -7.5f;

        yield break;
    }

    private IEnumerator WallRun(float headTiltTarget)
    {
        Vector3 wallRunDirection = Vector3.Cross(wallHit.normal, Vector3.up).normalized;
        float wallRunTime = Mathf.Lerp(1, 1.75f, characterVelocity.magnitude / speedDash);
        float timePassed = 0;
        bool checkWallFront = false;
        bool checkWallSide = true;

        if (Vector3.Dot(characterCameraTransform.forward, wallRunDirection) < 0) wallRunDirection = -wallRunDirection;

        checkJump = false;
        checkWallRun = true;

        while (checkSurface == false && jump == false && checkWallFront == false && checkWallSide == true && wallRunTime > 0)
        {
            checkWallFront = Physics.Raycast(characterBodyTransform.position, wallRunDirection, 1, ~layerInteractable);
            checkWallSide = Physics.Raycast(characterBodyTransform.position, -wallHit.normal, 1, ~layerInteractable);

            if (wallRunTime > 1)
            {
                if (timePassed < timeTilt)
                {
                    headTilt = Mathf.LerpAngle(0, headTiltTarget, timePassed / timeTilt);
                    timePassed += Time.deltaTime;
                }
            }
            else
            {
                if (wallRunTime > 0) headTilt = Mathf.LerpAngle(0, headTiltTarget, wallRunTime);
            }

            wallRunTime -= Time.deltaTime;

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

        wallHit = null;

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
        Vector3 directionUp = Vector3.Dot(ladderTransform.up, characterBodyTransform.up) > 0 ? ladderTransform.up : -ladderTransform.up;

        while (checkClimb == true)
        {
            float angle = Vector3.Angle(ladderTransform.forward, characterBodyTransform.forward);

            Vector3 climbVertical = characterCameraTransform.localRotation.x < 0.125f ? directionUp * inputZ : -directionUp * inputZ;
            Vector3 climbHorizontal = angle < 90 ? ladderTransform.right * inputX : -ladderTransform.right * inputX;

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
        float angle;

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
            angle = Vector3.Angle(ladderTransform.forward, characterTransform.forward);
            direction = angle < 90 ? ladderTransform.right : -ladderTransform.right;

            characterVelocity = direction * inputX * speedAir;
        }
        else if (jump == true)
        {
            angle = Vector3.Dot(ladderTransform.forward, ladderToCharacter);
            direction = angle > 0 ? ladderTransform.forward : -ladderTransform.forward;

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
            swimVertical = swimHorizontal != Vector3.zero || swimOut == true ? Vector3.up * speedSwim : -Vector3.up * speedSink;

            characterVelocity = (swimHorizontal * speedSwim) + swimVertical;

            yield return null;
        }

        yield break;
    }

    private IEnumerator Dash()
    {
        if (checkCrouch == true || checkVault == true || checkWallRun == true || checkClimb == true || checkDash == true) yield break;
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
