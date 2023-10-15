using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class CharacterMove : MonoBehaviour
{
    [Header("Character Object and Component References")]
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Transform characterCameraTransform;
    [SerializeField] private Transform characterBodyTransform;
    [SerializeField] private CharacterController characterController;

    [Header("Other Object and Component References")]
    [SerializeField] private Transform ladderTransform;
    [SerializeField] private ControllerColliderHit wallHit;

    [Header("Physics")]
    [SerializeField] private float gravity = -25.0f;
    [SerializeField] private float friction = 10.0f;

    [Header("Character States")]
    [SerializeField] private Transform checkSphereTransform;
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
    [SerializeField] private bool checkTurnClamp = false;
    [SerializeField] private bool checkGrapple = false;

    [Header("Character Attributes")]
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
    [SerializeField] private float speedTurn = 100.0f;
    [SerializeField] private float acceleration = 12.5f;
    [SerializeField] private float gaitRate = 2.5f;
    [SerializeField] private float airControl = 0.75f;
    [SerializeField] private float jumpHeight = 1.25f;
    [SerializeField] private float heightStand = 1.85f;
    [SerializeField] private float heightCrouch = 1.0f;
    [SerializeField] private float clearance = 0.85f;
    [SerializeField] private float reactionTimeAir = 0.5f;
    [SerializeField] private float reactionTimeJump = 0.25f;
    [SerializeField] private float reactionTimeCrouch = 0.375f;
    [SerializeField] private float timeCrouch = 0.125f;
    [SerializeField] private float timeWallRun = 0.0f;
    [SerializeField] private float timeTilt = 0.125f;
    [SerializeField] private float timeDash = 0.25f;
    [SerializeField] private float timeChargeDash = 1.25f;
    [SerializeField] private float forcePush = 1.0f;
    [SerializeField] private float headTilt = 0.0f;
    [SerializeField] private float headTiltAngle = 12.5f;
    [SerializeField] private int jumpCountMax = 2;
    [SerializeField] private int jumpCount = 0;
    [SerializeField] private int dashCountMax = 3;
    [SerializeField] private int dashCount = 0;

    [Header("Input Move")]
    [SerializeField] private InputCharacterMove inputCharacterMove;
    [SerializeField] private Vector3 move;
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
    private Coroutine coroutineWallRun;
    private Coroutine coroutineDash;

    public delegate void ActionTurn();
    public event ActionTurn OnActionTurn;

    public CharacterController Controller
    {
        get { return characterController; }
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

    public bool CheckTurnClamp
    {
        get { return checkTurnClamp; }
    }

    public bool CheckGrapple
    {
        get
        {
            return checkGrapple;
        }
        set
        {
            checkGrapple = value;

            if (checkGrapple == true)
            {
                if (checkMove == false) StartCoroutine(Move());
                if (checkClimb == true) StartCoroutine(ClimbExit());
            }
        }
    }

    private void Awake()
    {
        characterTransform = gameObject.transform;
        characterCameraTransform = characterTransform.GetChild(0).transform;
        characterBodyTransform = characterTransform.GetChild(1).transform;
        characterController = gameObject.GetComponent<CharacterController>();

        checkSphereTransform = characterTransform.GetChild(characterTransform.childCount - 1);

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

        inputCharacterMove = InputCharacterMove.Instance;
    }

    private void OnEnable()
    {
        inputCharacterMove.InputMove.started += InputMove;
        inputCharacterMove.InputMove.started += Move;

        inputCharacterMove.InputWalk.started += Gait;
        inputCharacterMove.InputWalk.canceled += Gait;

        inputCharacterMove.InputJump.started += CheckClearance;
        inputCharacterMove.InputJump.started += Move;

        inputCharacterMove.InputCrouch.started += CheckClearance;

        inputCharacterMove.InputSwim.started += InputSwim;
        inputCharacterMove.InputSwim.canceled += InputSwim;

        inputCharacterMove.InputDash.started += Dash;
    }

    private void OnDisable()
    {
        inputCharacterMove.InputMove.started -= InputMove;
        inputCharacterMove.InputMove.started -= Move;

        inputCharacterMove.InputWalk.started -= Gait;
        inputCharacterMove.InputWalk.canceled -= Gait;

        inputCharacterMove.InputJump.started -= CheckClearance;
        inputCharacterMove.InputJump.started -= Move;

        inputCharacterMove.InputCrouch.started -= CheckClearance;

        inputCharacterMove.InputSwim.started -= InputSwim;
        inputCharacterMove.InputSwim.canceled -= InputSwim;

        inputCharacterMove.InputDash.started -= Dash;
    }

    private void Update() => Gravity();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Climb"))
        {
            ladderTransform = other.transform;
            StartCoroutine(Climb());
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
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
        if (hit.rigidbody != null && hit.rigidbody.isKinematic == false)
        {
            Vector3 forceDirection = (hit.rigidbody.transform.position - characterBodyTransform.position).normalized;

            if (hit.moveDirection.y >= 0) hit.rigidbody.AddForceAtPosition(forceDirection * forcePush, hit.point, ForceMode.Impulse);
        }
        else if (Vector3.Angle(Vector3.up, hit.normal) >= characterController.slopeLimit)
        {
            if (checkDash == true)
            {
                StopCoroutine(coroutineDash);
                checkDash = false;
            }

            if (checkSurface == true)
            {
                if (checkSlip == false && hit.point.y <= checkSphereTransform.position.y && Vector3.Angle(Vector3.up, hit.normal) <= 85) StartCoroutine(Slip(hit.normal));
            }
            else
            {
                if (wallHit != hit)
                {
                    Bounds hitBounds = hit.collider.GetComponent<MeshRenderer>().bounds;
                    bool wallHitRight = false;
                    bool wallHitLeft = false;
                    bool ampleSize = false;

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
        bool contact = Physics.CheckSphere(checkSphereTransform.position, sphereRadius, layerSurface, QueryTriggerInteraction.Ignore) ||
                       Physics.CheckSphere(checkSphereTransform.position, sphereRadius, layerInteractable, QueryTriggerInteraction.Ignore);

        if (checkSurface != contact)
        {
            checkSurface = contact;
            CheckSurface();
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

    private void MoveSurface()
    {
        Friction();
        Acceleration(speedMove);
    }

    private void MoveAir()
    {
        Acceleration(speedAir);
        AirControl();
    }

    private void CheckSurface()
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
            if (checkVault == true || checkClimb == true || checkSwim == true || checkDash == true || checkGrapple == true) return;

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
                int depth = 5;

                if (Physics.Raycast(checkSphereTransform.position, Vector3.down, depth, ~layerCharacter, QueryTriggerInteraction.Ignore) == false) StartCoroutine(Crouch());
                else if (checkSlide == true) characterVelocity.y = 0;
            }
        }
    }

    private void CheckClearance()
    {
        if (checkCrouch == true || checkVault == true || checkSwim == true) return;

        if (characterController.height == heightCrouch)
        {
            bool clear = !Physics.Raycast(characterCameraTransform.position, Vector3.up, clearance, ~layerInteractable, QueryTriggerInteraction.Ignore);

            if (clear == false)
            {
                return;
            }
            else if (inputCharacterMove.InputJump.ReadValue<float>() == 1)
            {
                if (checkSlide == true) StartCoroutine(Jump(reactionTimeJump));

                StartCoroutine(Crouch());

                return;
            }
        }

        if (inputCharacterMove.InputJump.ReadValue<float>() == 1) StartCoroutine(Jump(reactionTimeJump));
        else StartCoroutine(Crouch());
    }

    private void CheckClearance(InputAction.CallbackContext contextCheckClearance) { CheckClearance(); }

    private void InputMove(InputAction.CallbackContext contextInputMove) { StartCoroutine(InputMove()); }

    private void InputSwim(InputAction.CallbackContext contextInputSwim) { swimOut = contextInputSwim.ReadValueAsButton(); }

    private void Move(InputAction.CallbackContext contextMove) { StartCoroutine(Move()); }

    private void Gait(InputAction.CallbackContext contextGait) { if (coroutineGait != null) StopCoroutine(coroutineGait); coroutineGait = StartCoroutine(Gait()); }

    private void Dash(InputAction.CallbackContext contextDash) { coroutineDash = StartCoroutine(Dash()); }

    private IEnumerator InputMove()
    {
        while (inputCharacterMove.InputMove.ReadValue<Vector2>() != Vector2.zero)
        {
            inputX = inputCharacterMove.InputMove.ReadValue<Vector2>().x;
            inputZ = inputCharacterMove.InputMove.ReadValue<Vector2>().y;

            move = new Vector3(inputX, 0, inputZ);
            move = characterBodyTransform.TransformDirection(move);
            move.Normalize();

            yield return null;
        }

        move = Vector3.zero;

        inputX = 0;
        inputZ = 0;

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
                if (checkSurface == true) MoveSurface();
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

        checkWalk = Convert.ToBoolean(inputCharacterMove.InputWalk.ReadValue<float>());

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

            jump = Convert.ToBoolean(inputCharacterMove.InputJump.ReadValue<float>());

            if (jump == true)
            {
                if (checkSurface == true || (checkFall == true && checkJump == false) || ((checkFall == true || checkJump == true) && jumpCount < jumpCountMax))
                {
                    if (checkSlide == true)
                    {
                        characterVelocity.x *= 2;
                        characterVelocity.z *= 2;
                    }

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
            if (checkFall == true) checkFall = false;

            jumpCount++;

            jump = false;
        }

        if (coroutineVault == null && checkVault == false) coroutineVault = StartCoroutine(Vault());

        yield break;
    }

    private IEnumerator Crouch()
    {
        if (checkSurface == false && crouch == false)
        {
            float reactionTime = reactionTimeCrouch;

            while (checkSurface == false && reactionTime >= 0)
            {
                reactionTime -= Time.deltaTime;
                yield return null;
            }

            if (checkSurface == true && reactionTime < 0) yield break;
        }

        crouch = !crouch;

        if (crouch == true)
        {
            if (characterBodyTransform.InverseTransformDirection(characterController.velocity).z > speedWalk) StartCoroutine(Slide());

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

            if (checkSlide == true) timeCrouch = 0.075f;
            else timeCrouch = 0.125f;
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

        while (checkFall == true || checkJump == true)
        {
            Vector3 rayOrigin1 = characterCameraTransform.position + characterBodyTransform.up * offsetVertical + characterBodyTransform.forward * offsetHorizontal;
            Vector3 rayDirection1 = -characterBodyTransform.up;

            bool rayContact1 = Physics.Raycast(rayOrigin1, rayDirection1, out RaycastHit rayHit1, rayLength, layerSurface, QueryTriggerInteraction.Ignore);

            if (rayContact1 == true)
            {
                Vector3 rayOrigin2 = characterCameraTransform.position;
                Vector3 rayDirection2 = characterBodyTransform.forward;

                bool rayContact2 = Physics.Raycast(rayOrigin2, rayDirection2, out RaycastHit rayHit2, rayLength, layerSurface, QueryTriggerInteraction.Ignore);

                if (rayContact2 == true)
                {
                    if (Vector3.Distance(rayHit1.point, rayHit2.point) < rayLength)
                    {
                        Transform climbPoint;
                        Vector3 climbPointPrevious;
                        Vector3 climbDirection;
                        float climbVelocity;

                        climbPoint = new GameObject("Climb Point").transform;

                        Instantiate(climbPoint);

                        climbPoint.position = rayHit1.point;
                        climbPoint.SetParent(rayHit1.transform);

                        climbPointPrevious = rayHit1.point;

                        characterVelocity.x = 0;
                        characterVelocity.z = 0;

                        checkVault = true;

                        while (checkSphereTransform.position.y < climbPoint.position.y)
                        {
                            climbVelocity = climbPointPrevious.magnitude - climbPoint.position.magnitude;
                            characterVelocity.y = climbVelocity + speedVault;
                            climbPointPrevious = climbPoint.position;

                            yield return null;
                        }

                        climbDirection = climbPoint.position - checkSphereTransform.position;
                        climbDirection.Normalize();

                        characterVelocity = climbDirection * Mathf.Pow(speedVault, 1.5f);

                        if (climbPoint.position != rayHit1.point) characterVelocity += characterBodyTransform.up * Mathf.Pow(speedVault, 1.5f);

                        Destroy(climbPoint.gameObject);

                        checkVault = false;

                        break;
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
        float slideAngle;
        float slideSpeed;

        positionPrevious = characterBodyTransform.position;
        slideMove = move;
        slideSpeed = speedDash;

        checkSlide = true;

        while (characterBodyTransform.InverseTransformDirection(characterController.velocity).z > 2.5f && crouch == true)
        {
            slideDirection = characterBodyTransform.position - positionPrevious;
            slideDirection.Normalize();

            if(checkSurface == true)
            {
                slideAngle = Mathf.Cos(Vector3.Angle(Vector3.up, slideDirection) * Mathf.Deg2Rad);
                slideAngle = Mathf.Round(slideAngle * 100) / 100;

                if (slideAngle < 0) slideSpeed = Mathf.Lerp(slideSpeed, speedSlide, Time.deltaTime);
                else if (slideAngle == 0) slideSpeed = Mathf.Lerp(slideSpeed, 0, Time.deltaTime);
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

        Vector3 wallHitVelocity;
        Vector3 wallHitPositionPrevious = wallHit.transform.position;
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

            wallHitVelocity = (wallHit.transform.position - wallHitPositionPrevious) / Time.deltaTime;
            wallHitPositionPrevious = wallHit.transform.position;

            characterVelocity = (wallRunDirection * speedMove) + wallHitVelocity;

            yield return null;
        }

        StartCoroutine(WallRunExit());

        yield break;
    }

    private IEnumerator WallRunExit()
    {
        float headTiltStart = characterCameraTransform.localEulerAngles.z;
        float timePassed = 0;

        if (checkJump == false && checkClimb == false)
        {
            checkFall = true;
            coroutineVault = StartCoroutine(Vault());
        }

        while (timePassed < timeTilt)
        {
            headTilt = Mathf.LerpAngle(headTiltStart, 0, timePassed / timeTilt);
            characterCameraTransform.localRotation = Quaternion.Euler(characterCameraTransform.localEulerAngles.x, characterCameraTransform.localEulerAngles.y, headTilt);

            timePassed += Time.deltaTime;

            yield return null;
        }

        characterCameraTransform.localRotation = Quaternion.Euler(characterCameraTransform.localEulerAngles.x, characterCameraTransform.localEulerAngles.y, 0);

        jumpCount = 1;

        wallHit = null;
        timeWallRun = 0;
        checkWallRun = false;

        yield break;
    }

    private IEnumerator Climb()
    {
        Vector3 directionUp;
        Vector3 climbVertical;
        Vector3 climbHorizontal;
        float dot;

        directionUp = Vector3.Dot(characterBodyTransform.up, ladderTransform.up) > 0 ? ladderTransform.up : -ladderTransform.up;

        checkClimb = true;

        while (checkClimb == true && checkSurface == true)
        {
            dot = Vector3.Dot(characterBodyTransform.forward, (characterBodyTransform.position - ladderTransform.position).normalized);

            if (dot > 0 && inputZ > 0) climbVertical = characterBodyTransform.forward;
            else if (dot < 0 && inputZ < 0) climbVertical = -characterBodyTransform.forward;
            else climbVertical = directionUp * Mathf.Abs(inputZ);

            characterVelocity = (climbVertical + (characterBodyTransform.right * inputX)) * speedClimb;

            yield return null;
        }

        while (checkClimb == true)
        {
            dot = Vector3.Dot(characterBodyTransform.forward, ladderTransform.forward);

            climbVertical = characterCameraTransform.localRotation.x < 0.125f ? directionUp * inputZ : -directionUp * inputZ;
            climbHorizontal = dot > 0 ? ladderTransform.right * (inputX / 2.5f) : -ladderTransform.right * (inputX / 2.5f);

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
        float dot;

        characterOnLadder = ladderTransform.TransformDirection(characterBodyTransform.position);

        ladderToCharacter = characterBodyTransform.position - ladderTransform.position;
        ladderToCharacter.Normalize();

        ladderLimitLeft = ladderTransform.TransformDirection(ladderTransform.position).x - (ladderTransform.GetComponent<BoxCollider>().size.x / 2);
        ladderLimitRight = ladderTransform.TransformDirection(ladderTransform.position).x + (ladderTransform.GetComponent<BoxCollider>().size.x / 2);

        if ((characterCameraTransform.localRotation.x < 0.125f && inputZ > 0) || (characterCameraTransform.localRotation.x > 0.125f && inputZ < 0))
        {
            if (characterBodyTransform.position.y > ladderTransform.position.y + (ladderTransform.GetComponent<BoxCollider>().size.y / 2))
            {
                characterVelocity.y = Mathf.Sqrt(-2 * gravity);
            }
        }
        else if (characterOnLadder.x < ladderLimitLeft || characterOnLadder.x > ladderLimitRight)
        {
            dot = Vector3.Dot(ladderTransform.forward, characterBodyTransform.forward);
            direction = dot > 0 ? ladderTransform.right : -ladderTransform.right;

            characterVelocity = direction * inputX * speedAir;
        }
        else if (jump == true)
        {
            dot = Vector3.Dot(ladderTransform.forward, ladderToCharacter);
            direction = dot > 0 ? ladderTransform.forward : -ladderTransform.forward;

            characterVelocity = direction * Mathf.Sqrt(-2 * gravity * jumpHeight);
        }

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

        checkSwim = true;

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
        else if (crouch == true || checkSlide == true) CheckClearance();

        if (move != Vector3.zero && dashCount > 0)
        {
            Vector3 direction = move;
            Vector3 momentum = new Vector3(inputX, 0, inputZ);
            float timeStart = Time.time;

            dashCount--;

            checkDash = true;

            if (checkCharge == false) StartCoroutine(Charge());

            while (Time.time <= timeStart + timeDash)
            {
                characterVelocity = direction * speedDash;
                yield return null;
            }

            checkDash = false;

            characterVelocity = (characterBodyTransform.right * momentum.x + characterBodyTransform.forward * momentum.z) * speedMove;
        }

        yield break;
    }

    private IEnumerator Charge()
    {
        checkCharge = true;

        while (dashCount < dashCountMax)
        {
            yield return new WaitForSeconds(timeChargeDash);
            dashCount++;
        }

        checkCharge = false;

        yield break;
    }

    private IEnumerator TurnClamp(Vector3 direction)
    {
        Quaternion rotationTo = Quaternion.LookRotation(direction, characterBodyTransform.up);
        Quaternion rotationFrom = characterBodyTransform.rotation;

        checkTurnClamp = true;

        if (OnActionTurn != null) OnActionTurn();

        while (characterController.velocity.normalized != direction) yield return null;

        while (characterController.velocity.normalized == direction)
        {
            characterBodyTransform.rotation = Quaternion.RotateTowards(rotationFrom, rotationTo, speedTurn * Time.deltaTime);
            yield return null;
        }

        while (characterBodyTransform.rotation != characterCameraTransform.rotation)
        {
            characterBodyTransform.rotation = Quaternion.RotateTowards(rotationFrom, characterCameraTransform.rotation, speedTurn * Time.deltaTime);
            yield return null;
        }

        checkTurnClamp = false;

        yield break;
    }
}
