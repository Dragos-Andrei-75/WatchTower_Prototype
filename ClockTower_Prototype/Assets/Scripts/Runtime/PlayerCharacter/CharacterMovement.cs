using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
    [Header("Character Object and Component References")]
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Transform characterCameraTransform;
    [SerializeField] private Transform characterBodyTransform;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private CharacterLook characterLook;

    [Header("Other Object and Component References")]
    [SerializeField] private Transform ladderTransform;
    [SerializeField] private ControllerColliderHit wallHit;

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
    [SerializeField] private bool checkMove = false;
    [SerializeField] private bool checkSlip = false;
    [SerializeField] private bool checkFall = false;
    [SerializeField] private bool checkWalk = false;
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

    [Header("Character Attributes")]
    [SerializeField] private Vector3 move;
    [SerializeField] private Vector3 characterStationary;
    [SerializeField] private Vector3 characterVelocity;
    [SerializeField] private float mass = 7.5f;
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
    [SerializeField] private float clearance = 0.85f;
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

    private Coroutine coroutineGait;
    private Coroutine coroutineVault;
    private Coroutine coroutineSlide;
    private Coroutine coroutineWallRun;
    private Coroutine coroutineDash;

    public delegate IEnumerator ActionSlide();
    public event ActionSlide OnActionSlide;

    private Coroutine CoroutineGait
    {
        get
        {
            return coroutineGait;
        }
        set
        {
            if (coroutineGait != null) StopCoroutine(coroutineGait);

            coroutineGait = value;
        }
    }

    public CharacterController Controller
    {
        get { return characterController; }
    }

    public Vector3 MoveInput
    {
        get { return move; }
    }

    public Vector3 CharacterVelocity
    {
        get { return characterVelocity; }
        set { characterVelocity = value; }
    }

    public Vector3 CharacterStationary
    {
        get { return characterStationary; }
        set { characterStationary = value; }
    }

    public float SpeedRun
    {
        get { return speedRun; }
    }

    public bool CheckSlide
    {
        get { return checkSlide; }
    }

    public bool CheckGrapple
    {
        get
        {
            return checkGrapple;
        }
        set
        {
            if (checkClimb == true && value == true) checkClimb = false;

            StartCoroutine(Move());

            checkGrapple = value;
        }
    }

    private void Awake()
    {
        characterTransform = gameObject.transform;
        characterCameraTransform = characterTransform.GetChild(0).transform;
        characterBodyTransform = characterTransform.GetChild(1).transform;
        characterController = gameObject.GetComponent<CharacterController>();
        characterLook = characterTransform.GetChild(0).GetComponent<CharacterLook>();

        sphereTransform = characterTransform.GetChild(characterTransform.childCount - 1);

        layerCharacter = LayerMask.GetMask("Player");
        layerSurface = LayerMask.GetMask("Surface");
        layerInteractable = LayerMask.GetMask("Interactable");

        cameraStand = new Vector3(0, heightStand / 2, 0);
        cameraCrouch = Vector3.zero;
        centerStand = Vector3.zero;
        centerCrouch = new Vector3(0, -(heightCrouch / 2) + 0.075f, 0);

        characterStationary = new Vector3(0, -mass, 0);

        speedMove = speedRun;
        dashCount = dashCountMax;

        inputPlayer = new InputPlayer();

        inputMove = inputPlayer.Character.Move;
        inputWalk = inputPlayer.Character.Walk;
        inputJump = inputPlayer.Character.Jump;
        inputCrouch = inputPlayer.Character.Crouch;
        inputSwim = inputPlayer.Character.SwimOut;
        inputDash = inputPlayer.Character.Dash;
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

        inputMove.started += InputMove;
        inputMove.started += Move;

        inputWalk.started += Gait;
        inputWalk.canceled += Gait;

        inputJump.started += CheckClearance;
        inputJump.started += Move;

        inputCrouch.started += CheckClearance;

        inputSwim.started += InputSwim;
        inputSwim.canceled += InputSwim;

        inputDash.started += Dash;

        Pause.OnPauseResume -= OnEnable;
        Pause.OnPauseResume += OnDisable;
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

        inputMove.started -= InputMove;
        inputMove.started -= Move;

        inputWalk.started -= Gait;
        inputWalk.canceled -= Gait;

        inputJump.started -= CheckClearance;
        inputJump.started -= Move;

        inputCrouch.started -= CheckClearance;

        inputSwim.started -= InputSwim;
        inputSwim.canceled -= InputSwim;

        inputDash.started -= Dash;

        Pause.OnPauseResume += OnEnable;
        Pause.OnPauseResume -= OnDisable;
    }

    private void Update() => Gravity();

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
            else if (checkJump == true)
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
            characterVelocity.y = Mathf.Sqrt(-10 * gravity * jumpHeight);
            checkSwim = false;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody hitRigidbody = hit.collider.attachedRigidbody;
        Bounds hitBounds = hit.collider.GetComponent<MeshRenderer>().bounds;
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
                if (checkSlip == false && hit.point.y <= sphereTransform.position.y && hitAngle <= 85) StartCoroutine(Slip(hit.normal));
            }
            else
            {
                if (wallHit != hit)
                {
                    bool ampleSize = false;
                    bool wallHitLeft = false;
                    bool wallHitRight = false;

                    if (characterController.height * 2 < hitBounds.size.y && (characterController.radius * 2 < hitBounds.size.x || characterController.radius * 2 < hitBounds.size.z))
                    {
                        bool checkWallLeft = Physics.Raycast(characterCameraTransform.position, -characterBodyTransform.right, out RaycastHit hitLeft,
                                                             characterController.radius * 2, layerSurface, QueryTriggerInteraction.Ignore);
                        bool checkWallRight = Physics.Raycast(characterCameraTransform.position, characterBodyTransform.right, out RaycastHit hitRight,
                                                              characterController.radius * 2, layerSurface, QueryTriggerInteraction.Ignore);

                        ampleSize = true;

                        wallHit = hit;

                        if (coroutineWallRun != null) StopCoroutine(coroutineWallRun);

                        if (checkWallLeft == true && hitLeft.transform == wallHit.transform)
                        {
                            wallHitLeft = true;
                            coroutineWallRun = StartCoroutine(WallRun(-headTiltAngle, 1));
                        }
                        else if (checkWallRight == true && hitRight.transform == wallHit.transform)
                        {
                            wallHitRight = true;
                            coroutineWallRun = StartCoroutine(WallRun(headTiltAngle, -1));
                        }
                    }

                    if (ampleSize == false || (wallHitLeft == false && wallHitRight == false)) StartCoroutine(WallRunExit());
                }
            }
        }
    }

    private void Gravity()
    {
        bool contact = Physics.CheckSphere(sphereTransform.position, sphereRadius, layerSurface, QueryTriggerInteraction.Ignore) ||
                       Physics.CheckSphere(sphereTransform.position, sphereRadius, layerInteractable, QueryTriggerInteraction.Ignore);

        if (checkSurface != contact)
        {
            checkSurface = contact;
            StartCoroutine(CheckSurface());
        }

        if (checkSurface == false)
        {
            if (checkVault == false && checkWallRun == false && checkClimb == false && checkSwim == false && checkDash == false && checkGrapple == false)
            {
                characterVelocity.y += gravity * Time.deltaTime;
            }
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

    private void InputMove(InputAction.CallbackContext contextInputMove) => StartCoroutine(InputMove());

    private void InputSwim(InputAction.CallbackContext contextInputSwim) => swimOut = contextInputSwim.ReadValueAsButton();

    private void CheckClearance(InputAction.CallbackContext contextCheckClearance) => StartCoroutine(CheckClearance());

    private void Move(InputAction.CallbackContext contextMove) => StartCoroutine(Move());

    private void Gait(InputAction.CallbackContext contextCheckClearance) => CoroutineGait = StartCoroutine(Gait());

    private void Dash(InputAction.CallbackContext contextMoveInput) => coroutineDash = StartCoroutine(Dash());

    private IEnumerator InputMove()
    {
        while (inputMove.ReadValue<Vector2>() != Vector2.zero)
        {
            inputX = inputMove.ReadValue<Vector2>().x;
            inputZ = inputMove.ReadValue<Vector2>().y;

            move = new Vector3(inputX, 0, inputZ);
            move = characterTransform.TransformDirection(move);
            move.Normalize();

            yield return null;
        }

        move = Vector3.zero;

        inputX = 0;
        inputZ = 0;

        yield break;
    }

    private IEnumerator CheckSurface()
    {
        if (checkSurface == true)
        {
            gravity = -25;
            jumpCount = 0;
            checkFall = false;
            checkJump = false;

            characterVelocity.y = -mass;

            if (checkClimb == true) StartCoroutine(ClimbExit());
        }
        else
        {
            if (checkClimb == true || checkSwim == true || checkDash == true || checkGrapple == true) yield break;

            if (checkJump == true)
            {
                gravity = -37.5f;
            }
            else
            {
                checkFall = true;
                StartCoroutine(Jump(reactionTimeAir));
            }

            if (crouch == true)
            {
                float depth = 7.5f;

                if (Physics.Raycast(sphereTransform.position, Vector3.down, depth, ~layerCharacter, QueryTriggerInteraction.Ignore) == false)
                {
                    while (checkCrouch == true) yield return null;
                    StartCoroutine(CheckClearance());
                }
            }
        }

        yield break;
    }

    private IEnumerator CheckClearance()
    {
        if (checkVault == true || checkSwim == true) yield break;

        if (characterController.height == heightCrouch)
        {
            bool clear = !Physics.Raycast(characterCameraTransform.position, Vector3.up, clearance, ~layerInteractable, QueryTriggerInteraction.Ignore);

            if (clear == false)
            {
                yield break;
            }
            else if (inputJump.ReadValue<float>() == 1)
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

        if (inputJump.ReadValue<float>() == 1) StartCoroutine(Jump(reactionTimeJump));
        else if (checkCrouch == false) StartCoroutine(Crouch());

        yield break;
    }

    private IEnumerator Move()
    {
        if (checkMove == true) yield break;

        checkMove = true;

        while (move != Vector3.zero || characterVelocity != characterStationary || characterController.isGrounded == false)
        {
            if (checkSlip == false && checkVault == false && checkSlide == false && checkWallRun == false && checkClimb == false && checkSwim == false && checkDash == false)
            {
                if (checkSurface == true) MoveGround();
                else MoveAir();
            }

            characterController.Move(characterVelocity * Time.deltaTime);

            yield return null;
        }

        checkMove = false;

        yield break;
    }

    private IEnumerator Slip(Vector3 hitSlopeNormal)
    {
        RaycastHit hitSlope;

        characterVelocity.x = 0;
        characterVelocity.z = 0;

        checkSlip = true;

        while (checkSurface == true && Vector3.Angle(Vector3.up, hitSlopeNormal) > characterController.slopeLimit)
        {
            Physics.SphereCast(characterBodyTransform.position, sphereRadius, -characterBodyTransform.up, out hitSlope, characterController.height / 2);
            characterVelocity += new Vector3(hitSlope.normal.x, 0, hitSlope.normal.z);
            hitSlopeNormal = hitSlope.normal;

            yield return null;
        }

        checkSlip = false;

        yield break;
    }

    private IEnumerator Gait()
    {
        if (checkSurface == false || crouch == true) yield break;

        float speedTarget;

        checkWalk = !checkWalk;

        if (checkWalk == true) speedTarget = speedWalk;
        else speedTarget = speedRun;

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

            jump = Convert.ToBoolean(inputJump.ReadValue<float>());

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

        jump = false;

        if (reactionTime >= 0)
        {
            if (checkJump == false) checkJump = true;
            if (checkFall == true) checkFall = false;

            jumpCount++;
        }

        if (coroutineVault != null && checkVault == false) StopCoroutine(coroutineVault);
        coroutineVault = StartCoroutine(Vault());

        yield break;
    }

    private IEnumerator Crouch()
    {
        if (checkSurface == false && crouch == false) yield break;

        crouch = !crouch;

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

            speedMove = checkWalk == true ? speedWalk : speedRun;
        }

        checkCrouch = true;

        while (Mathf.Abs(characterController.height - heightTarget) > 0.001f)
        {
            characterCameraTransform.localPosition = Vector3.SmoothDamp(characterCameraTransform.localPosition, cameraTarget, ref smoothCamera, timeCrouch);

            characterController.center = Vector3.SmoothDamp(characterController.center, centerTarget, ref smoothCenter, timeCrouch);
            characterController.height = Mathf.SmoothDamp(characterController.height, heightTarget, ref smoothHeight, timeCrouch);

            yield return null;
        }

        checkCrouch = false;

        characterCameraTransform.localPosition = cameraTarget;

        characterController.center = centerTarget;
        characterController.height = heightTarget;

        yield break;
    }

    private IEnumerator Vault()
    {
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
                        Transform climbPointTransform = new GameObject("Climb Point").transform;
                        Vector3 climbPoint = new Vector3(ray2Hit.point.x, ray1Hit.point.y, ray2Hit.point.z);
                        Vector3 climbDirection;
                        Vector3 climbPointHorizontal;
                        Vector3 checkSphereHorizontal;

                        Instantiate(climbPointTransform);

                        climbPointTransform.transform.position = climbPoint;
                        climbPointTransform.transform.SetParent(ray1Hit.transform);

                        climbPointHorizontal = climbPointTransform.position;
                        checkSphereHorizontal = Vector3.zero;

                        characterVelocity = Vector3.zero;

                        checkVault = true;

                        while (Vector3.Distance(climbPointHorizontal, checkSphereHorizontal) > 0.1f)
                        {
                            if (sphereTransform.position.y < climbPointTransform.position.y)
                            {
                                characterVelocity.y = speedVault;
                            }
                            else
                            {
                                climbPointHorizontal = new Vector3(climbPointTransform.position.x, 0, climbPointTransform.position.z);
                                checkSphereHorizontal = new Vector3(sphereTransform.position.x, 0, sphereTransform.position.z);

                                climbDirection = (climbPoint - sphereTransform.position);
                                climbDirection.Normalize();

                                characterVelocity = climbDirection * speedVault * 2.5f;

                                if (climbPointTransform.position != climbPoint) characterVelocity += characterBodyTransform.up * speedVault * 3.75f;
                                else break;
                            }

                            yield return null;
                        }

                        checkVault = false;

                        Destroy(climbPointTransform.gameObject);
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
        Vector3 positionPrevious;
        Vector3 slideDirection;
        Vector3 slideMove;
        float slideSpeed;

        positionPrevious = characterBodyTransform.position;
        slideMove = move;
        slideSpeed = speedDash;

        characterLook.LookXReference = characterLook.MouseX;

        characterVelocity.y = -1;

        checkSlide = true;

        if (OnActionSlide != null) StartCoroutine(OnActionSlide());

        while (Mathf.Round(slideSpeed * 10) / 10 > 2.5f)
        {
            slideDirection = characterBodyTransform.position - positionPrevious;
            slideDirection.Normalize();

            if (Physics.Raycast(characterBodyTransform.position, slideDirection, 1, ~layerCharacter, QueryTriggerInteraction.Ignore) == true) break;

            if(checkSurface == true)
            {
                if (Mathf.Round(Vector3.Angle(Vector3.up, slideDirection)) > 90) slideSpeed = Mathf.Lerp(slideSpeed, speedSlide, Time.deltaTime);
                else if (Mathf.Round(Vector3.Angle(Vector3.up, slideDirection)) == 90) slideSpeed = Mathf.Lerp(slideSpeed, 0, Time.deltaTime);
                else slideSpeed = Mathf.Lerp(slideSpeed, 0, Time.deltaTime * 2);
            }
            else
            {
                slideSpeed = Mathf.Lerp(slideSpeed, 0, Time.deltaTime / 2);
            }

            positionPrevious = characterBodyTransform.position;

            characterVelocity = (slideMove * slideSpeed) + new Vector3(0, characterVelocity.y, 0);

            yield return null;
        }

        checkSlide = false;

        yield break;
    }

    private IEnumerator WallRun(float headTiltTarget, int inputCancel)
    {
        if ((Mathf.Round(Vector3.Dot(characterBodyTransform.forward, characterVelocity.normalized)) < 0 || characterVelocity.y < -mass) && checkWallRun == false) yield break;

        Vector3 wallRunDirection = Vector3.Cross(Vector3.up, wallHit.normal).normalized;
        float timePassed = 0;
        bool checkWall = true;

        if (Vector3.Dot(characterCameraTransform.forward, wallRunDirection) < 0) wallRunDirection = -wallRunDirection;

        if (timeWallRun == 0) timeWallRun = Mathf.Lerp(1.25f, 1.75f, characterVelocity.magnitude / speedDash);

        checkJump = false;
        checkWallRun = true;

        while (inputX != inputCancel && checkSurface == false && checkJump == false && checkWall == true && timeWallRun > 0)
        {
            checkWall = Physics.Raycast(characterBodyTransform.position, -wallHit.normal, characterController.radius * 2, ~layerInteractable, QueryTriggerInteraction.Ignore);

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

        StartCoroutine(WallRunExit());

        yield break;
    }

    private IEnumerator WallRunExit()
    {
        float headTiltStart = characterCameraTransform.localEulerAngles.z;
        float timePassed = 0;

        jumpCount = 1;

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
        Vector3 directionUp = Vector3.Dot(ladderTransform.up, characterBodyTransform.up) > 0 ? ladderTransform.up : -ladderTransform.up;

        while (checkClimb == true)
        {
            float angle = Vector3.Angle(ladderTransform.forward, characterBodyTransform.forward);

            Vector3 climbVertical = characterCameraTransform.localRotation.x < 0.125f ? directionUp * inputZ : -directionUp * inputZ;
            Vector3 climbHorizontal = angle < 90 ? ladderTransform.right * (inputX / 2.5f) : -ladderTransform.right * (inputX / 2.5f);

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
        Vector3 swimHorizontal;
        Vector3 swimVertical;
        float smoothInputX;
        float smoothInputZ;

        while (checkSwim == true)
        {
            smoothInputX = inputX != 0 ? 0.125f : 0.25f;
            smoothInputZ = inputZ != 0 ? 0.125f : 0.25f;

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
