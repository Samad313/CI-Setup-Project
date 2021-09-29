using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public enum AnimState { Idle, Run, Jump, Pushing, HoldingIdle, HoldingUp, HoldingDown, ClimbingLedge, WaterIdle, WaterSwimming};
public enum CharName {Boy, Girl, Dog, MysteryBoy };

public class PlayerController : MonoBehaviour
{
    #region Variables
    [SerializeField] private Texture2D icon = default;
    [SerializeField] private Vector3 iconScale = default;
    [SerializeField] private Animator myAnimator = default;
    [SerializeField] private SkeletonMecanim mySkeletonMecanim = default;
    [SerializeField] private GameObject pointLight = default;
    [SerializeField] private Transform handRotationGroup = default;
    [SerializeField] private CapsuleCollider capsuleCollider = default;
    [SerializeField] private BoxCollider boxCollider = default;

    [SerializeField] private CharName charName = default;
    [SerializeField] private float maxSpeed = 7;
    [SerializeField] private float jumpHeight = 7;
    [SerializeField] private float rotationSpeed = 5.0f;
    [SerializeField] private float stairHeight = 0.2f;

    [SerializeField] private float climbingTopOffset = 0.65f;

    [SerializeField] private Vector3 pushingBoxOffset = default;
    [SerializeField] private Vector3 ropeClimbingHandOffset = default;
    [SerializeField] private float pushingOrbOffset = default;

    [SerializeField] private float bouyancy = 2.0f;
    [SerializeField] private float swimmingSpeed = 2.0f;
    [SerializeField] private float rotationSpeedInWater = 3.0f;

    private Rigidbody myRigidbody;
    private Transform iconTransform;
    private Transform spineVisualTransform;
    private Transform waterBubbles;
    private AnimState animState;

    private bool isActive = false;
    private bool controlsDisabled = false;

    private bool rightInput = false;
    private bool leftInput = false;
    private bool sideInput = false;
    private bool anyInput = false;

    private float animatorSpeed = 1.0f;
    private bool animatorGrounded = false;

    private Vector3 lastPosition;
    private Vector3 lastClimbingPosition;
    private bool grounded;

    private float wantedHorizontalInput = 0.0f;
    private float horizontalInput = 0.0f;
    private float currentHorizontalSpeed = 0;
    private float wantedRotationZ = 0;
    private float jumpWait = 0.0f;
    private float groundedDelay = -1.0f;
    private bool jumpingStraightUp = true;

    public float CurrentHorizontalSpeed { get { return currentHorizontalSpeed; } }

    public float minGroundNormalY = .65f;
    public float gravityModifier = 1f;
    private Vector3 targetVelocity;
    private Vector3 groundNormal;
    private Vector3 velocity;
    private RaycastHit[] hitBuffer = new RaycastHit[16];
    private List<RaycastHit> hitBufferList = new List<RaycastHit>(16);
    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    private float pushingBoxDelay = -1.0f;
    private bool startedPushingBox = false;
    private bool isPushingBox = false;
    private bool isPushedObjectBox = false;
    private int boxDropped = 0;
    private Vector3 normalVisualPosition;
    private float runDelay = 1000.0f;

    private Climbable myClimbable;
    private Transform myClimbingPositionObject;
    private bool isClimbing = false;
    private ClimbStates climbingState = ClimbStates.UnMounted;
    private bool climbJumpWait = true;

    private Water myWater;
    private bool inWater = false;
    private Vector3 currentWaterVelocity;
    private bool initialWaterDive = true;
    private Vector3 waterLastPosition;
    private float delayBeforeEnteringWater = -10;
    private float currentWaterOffset = 0;
    private int nearestSideOfWaterEntry = 3;
    private float offsetFromCenter = 0.0f;

    private bool onTopOfPlatform = false;
    private Transform platformTop;
    private float onPlatformWaitTime = -1.0f;

    private float verticalSpeed = 0.0f;

    private Renderer myRenderer;

    [SerializeField] private float xrayOffsetMinX = 1.0f;
    [SerializeField] private float xrayOffsetMaxX = 0.75f;
    [SerializeField] private float xrayOffsetMinY = 1.0f;
    [SerializeField] private float xrayOffsetMaxY = 0.75f;
    [SerializeField] private float maxXRayValue = 0.6f;

    private float lastZoomedInXrayValue = 0.0f;
    public float LastZoomedInXrayValue { get => lastZoomedInXrayValue; }

    private bool bottomTouching = false;
    private float bottomTouchingDelay = -1.0f;

    private bool onMushroom = false;
    private Vector3 offsetFromMushroomTop;
    private Transform mushroomTopToFollow;
    private float lastVelocityYBeforeGrounded;

    private bool inMud = false;

    [SerializeField] private float mudSpeedMultiplier = 0.3f;

    private Transform flyingCreatureTop;
    private BigFlyingCreature bigFlyingCreature;

    private bool mountedOnBigFlyingCreature = false;

    private float groundedTime = 0.0f;

    #endregion

    #region Getters
    public AnimState PlayerAnimationState
    {
        get { return animState; }
    }

    #endregion


    // Use this for initialization
    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myRenderer = myAnimator.GetComponent<Renderer>();
        animatorSpeed = myAnimator.speed;
        animState = AnimState.Idle;
        lastPosition = transform.position;
        spineVisualTransform = myAnimator.transform;
        normalVisualPosition = spineVisualTransform.localPosition;
        offsetFromCenter = transform.position.y - transform.Find("center").position.y;
        grounded = true;
        SpawnIcon();
    }

    void Start()
    {
        if(GameplayManager.instance)
        {
            if (GameplayManager.instance.HasWaterInLevel)
            {
                waterBubbles = Instantiate(FXManager.instance.GetWaterBubbles(), myAnimator.transform);
                waterBubbles.gameObject.SetActive(false);
            }
        }
        else
        {
            SetIsActive(true);
        }
    }

    public bool IsInWater()
    {
        if (inWater || isClimbing)
            return true;

        return false;
    }

    public void ClampWaterY()
    {
        if(myWater==null || bottomTouching || inWater == false)
        {
            return;
        }

        Vector3 newPosition = transform.position;

        newPosition.y = Mathf.Clamp(newPosition.y, -10000, myWater.GetTopBound().position.y + offsetFromCenter - 0.05f);
        transform.position = newPosition;
    }

    void Update()
    {
        if (GameplayManager.instance)
        {
            if (GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn)
            {
                lastPosition = transform.position;
                return;
            }
        }

        rightInput = false;
        leftInput = false;
        anyInput = false;

        delayBeforeEnteringWater -= Time.deltaTime;

        CalculateAndSetXRayAlpha();
        SetChildOffsets();
        SetRotationZ();
        
        if (isClimbing)
        {
            myAnimator.SetBool("inwater", false);
            Climb();
            return;
        }

        myAnimator.SetBool("inwater", inWater);

        bottomTouchingDelay -= Time.deltaTime;
        if (bottomTouchingDelay < 0)
        {
            bottomTouching = false;
        }

        if (inWater)
        {
            ClampWaterY();
        }

        if (isActive&&!inWater)
        {
            jumpWait -= Time.deltaTime;
            rightInput = InputManager.right;
            leftInput = InputManager.left;

            if(GameplayManager.instance)
            {
                if (controlsDisabled || GameplayManager.instance.GameStarted == false || GameplayManager.instance.LevelCompleted)
                {
                    rightInput = false;
                    leftInput = false;
                }
            }

            CheckForBoxDropped();

            if(mountedOnBigFlyingCreature==false)
                ComputeMovement();

            sideInput = leftInput || rightInput;
            anyInput = sideInput || InputManager.jumpDown;

            if (GameplayManager.instance)
            {
                if (controlsDisabled || GameplayManager.instance.GameStarted == false || GameplayManager.instance.LevelCompleted)
                {
                    anyInput = false;
                }
            }

            SetCharacterDirection();

            //SetRopeClimbingHandOffset();

            SetAnimatorBoolsAndFloats();

            SetAnimationsAndSounds();
        }

        if (mountedOnBigFlyingCreature)
            BigFlyingCreatureMovement();
        else
            ActualMovement();
    }

    private void BigFlyingCreatureMovement()
    {
        if(controlsDisabled==false)
        {
            Vector2 movementVector = Vector2.zero;

            if (rightInput)
                movementVector.x = 1.0f;
            else if (leftInput)
                movementVector.x = -1.0f;

            if (InputManager.up)
                movementVector.y = 1.0f;
            else if (InputManager.down)
                movementVector.y = -1.0f;

            bigFlyingCreature.Move(movementVector);
        }

        transform.position = new Vector3(flyingCreatureTop.position.x, flyingCreatureTop.position.y, 0);

        if(Input.GetKeyDown(KeyCode.B))
        {
            if(controlsDisabled==false)
                UnmountBigFlyingCreature();
        }
    }

    private void SetRotationZ()
    {
        if (!inWater || isClimbing)
            wantedRotationZ = 0;

        if (isActive)
        {
            if (inWater && !isClimbing)
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, wantedRotationZ), Time.deltaTime * rotationSpeedInWater);
            else
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, wantedRotationZ), Time.deltaTime * rotationSpeed);
        }
    }

    private void SetAnimationsAndSounds()
    {
        if (isPushingBox && startedPushingBox == false)
        {
            startedPushingBox = true;
            animState = AnimState.Pushing;
            if(AudioManager.instance)
            {
                AudioManager.instance.StopSoundEffect("Run");
                AudioManager.instance.PlaySoundEffect("PushingCrateChar");
            }
        }

        if (sideInput && grounded && jumpWait < 0)
        {
            if (animState == AnimState.Idle || animState == AnimState.Jump)
            {
                if (animState == AnimState.Jump)
                {
                    if (AudioManager.instance && FXManager.instance)
                    {
                        FXManager.instance.KidLanded(transform.position);
                        AudioManager.instance.PlaySoundEffect("JumpLand");
                        if(jumpingStraightUp==false)
                        {
                            myAnimator.SetBool("waitbeforerun", true);
                            myAnimator.SetBool("justlanded", true);
                            myAnimator.SetBool("jumpend", true);
                            StartCoroutine("SetJustLandedFalseAfterADelay");
                        }
                    }  
                }

                if (AudioManager.instance && FXManager.instance)
                {
                    AudioManager.instance.PlaySoundEffect("Run");
                    FXManager.instance.SetRunningDust(true, charName);
                }
                
                animState = AnimState.Run;
                jumpingStraightUp = true;
                //Debug.Log("RUN");
            }
        }

        if (sideInput && animState == AnimState.Jump && jumpingStraightUp)
        {
            myAnimator.SetTrigger("jumprun");
            jumpingStraightUp = false;
        }

        if (InputManager.jumpDown && controlsDisabled == false && !inMud)
        {
            if(GameplayManager.instance==null || GameplayManager.instance.GameStarted)
            {
                if ((animState == AnimState.Idle || animState == AnimState.Run || animState == AnimState.Pushing) && groundedDelay > 0)
                {
                    jumpWait = 0.2f;
                    if (sideInput)
                    {
                        jumpingStraightUp = false;
                        myAnimator.SetTrigger("jumprun");
                        if(Input.GetKey(KeyCode.T))
                        {
                            myAnimator.SetBool("torope", true);
                            StartCoroutine("toropefalse");
                        }
                    }
                    else
                    {
                        jumpingStraightUp = true;
                        myAnimator.SetTrigger("jump");
                    }


                    if (animState == AnimState.Run)
                    {
                        if(AudioManager.instance)
                        {
                            AudioManager.instance.StopSoundEffect("Run");
                            FXManager.instance.SetRunningDust(false, charName);
                        }
                    }
                    if (AudioManager.instance)
                    {
                        AudioManager.instance.PlaySoundEffect("Jump");
                    }
                    animState = AnimState.Jump;

                    SetOnTopOfPlatformFalse();
                    onPlatformWaitTime = -1.0f;
                    //Debug.Log("JUMP");
                }
            }
        }
        if (grounded && jumpWait < 0 && !anyInput)
        {
            //Debug.Log("IDLE");
            if (animState != AnimState.Idle)
            {
                if (animState == AnimState.Run)
                {
                    if (AudioManager.instance)
                    {
                        AudioManager.instance.StopSoundEffect("Run");
                        FXManager.instance.SetRunningDust(false, charName);
                    } 
                }
                else if (animState == AnimState.Jump)
                {
                    if (AudioManager.instance)
                    {
                        FXManager.instance.KidLanded(transform.position);
                        AudioManager.instance.PlaySoundEffect("JumpLand");
                        if(jumpingStraightUp==false)
                        {
                            myAnimator.SetBool("waitbeforerun", true);
                            myAnimator.SetBool("justlanded", true);
                            myAnimator.SetBool("jumpend", true);
                            StartCoroutine("SetJustLandedFalseAfterADelay");
                        }
                    } 
                }

                jumpingStraightUp = true;
                //Debug.Log("IDLE");
                animState = AnimState.Idle;
            }

            //AudioManager.instance.StopSoundEffect("Run");
        }

        if (animState == AnimState.Run)
        {
            if (grounded == false && groundedDelay < 0)
            {
                animState = AnimState.Jump;
                if (AudioManager.instance)
                {
                    AudioManager.instance.StopSoundEffect("Run");
                    FXManager.instance.SetRunningDust(false, charName);
                } 
                myAnimator.SetTrigger("inair");
                jumpingStraightUp = false;
            }
        }

        if (animState == AnimState.Idle)
        {
            if (grounded == false && groundedDelay < 0)
            {
                animState = AnimState.Jump;
                myAnimator.SetTrigger("inair");
                jumpingStraightUp = false;
            }
        }

        pushingBoxDelay -= Time.deltaTime;
        if (pushingBoxDelay < 0 && isPushingBox)
        {
            isPushingBox = false;
            startedPushingBox = false;
            if (AudioManager.instance)
            {
                AudioManager.instance.StopSoundEffect("PushingCrateChar");
            }
        }

        groundedDelay -= Time.deltaTime;
        if(grounded)
            groundedTime += Time.deltaTime;

        Vector3 extraOffset = Vector3.zero;
        if (isPushingBox && sideInput)
        {
            float pushingOffsetZ = pushingBoxOffset.z;
            if (isPushedObjectBox == false)
                pushingOffsetZ = pushingOrbOffset;
            if (mySkeletonMecanim.skeleton.ScaleX < 0)
                pushingOffsetZ *= -1.0f;

            extraOffset = new Vector3(0, pushingBoxOffset.y, pushingOffsetZ);
        }

        spineVisualTransform.localPosition = Vector3.Lerp(spineVisualTransform.localPosition, normalVisualPosition + extraOffset, Time.deltaTime * 15.0f);
    }

    private IEnumerator toropefalse()
    {
        yield return new WaitForSeconds(1.5f);
        myAnimator.SetBool("torope", false);
    }

    private IEnumerator SetJustLandedFalseAfterADelay()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        myAnimator.SetBool("jumpend", false);
        yield return new WaitForSeconds(0.15f);
        myAnimator.SetBool("waitbeforerun", false);
        yield return new WaitForSeconds(0.25f);
        myAnimator.SetBool("justlanded", false);
    }

    private void SetAnimatorBoolsAndFloats()
    {
        myAnimator.SetFloat("groundNormal", Mathf.Abs(groundNormal.x));

        if (sideInput)
        {
            if (rightInput && groundNormal.x < 0)
                myAnimator.SetFloat("groundNormal", 0);
            if (leftInput && groundNormal.x > 0)
                myAnimator.SetFloat("groundNormal", 0);
        }

        animatorGrounded = grounded;

        myAnimator.SetBool("boxpush", isPushingBox);
        myAnimator.SetBool("grounded", animatorGrounded);
        myAnimator.SetBool("sideInput", sideInput);
    }

    private void SetCharacterDirection()
    {
        if (rightInput)
        {
            mySkeletonMecanim.skeleton.ScaleX = 1.0f;
        }
        else if (leftInput)
        {
            mySkeletonMecanim.skeleton.ScaleX = -1.0f;
        }
    }

    private void SetRopeClimbingHandOffset()
    {
        ropeClimbingHandOffset.x = Mathf.Abs(ropeClimbingHandOffset.x) * mySkeletonMecanim.skeleton.ScaleX;
    }

    private void CheckForBoxDropped()
    {
        if (boxDropped != 0)
        {
            if (boxDropped == 1)
            {
                rightInput = false;
                if (InputManager.rightUp)
                {
                    boxDropped = 0;
                }
            }
            else if (boxDropped == 2)
            {
                leftInput = false;
                if (InputManager.leftUp)
                {
                    boxDropped = 0;
                }
            }
        }
    }

    private void ComputeMovement()
    {
        Vector3 move = Vector3.zero;

        if (rightInput)
        {
            runDelay -= Time.deltaTime;
            wantedHorizontalInput = 1.0f;
        }
        else if (leftInput)
        {
            runDelay -= Time.deltaTime;
            wantedHorizontalInput = -1.0f;
        }
        else
        {
            runDelay = 0.05f;
            wantedHorizontalInput = 0.0f;
        }

        horizontalInput = Mathf.Lerp(horizontalInput, wantedHorizontalInput, Time.deltaTime * 20.0f);

        if(runDelay<0)
        {
            move.x = horizontalInput;
        }

        if (InputManager.jumpDown && groundedDelay > 0 && controlsDisabled == false)
        {
            if(GameplayManager.instance==null || GameplayManager.instance.GameStarted)
            {
                if(!inMud && !onMushroom)
                {
                    velocity.y = jumpHeight;
                    SetOnTopOfPlatformFalse();
                    onPlatformWaitTime = -1.0f;
                }
            }
        }

        targetVelocity = Vector3.zero;

        float speedOnStairs = 1.0f;
        myAnimator.SetFloat("runanimationspeed", 1.0f);
        if (grounded && verticalSpeed > 0)
        {
            float t = Mathf.InverseLerp(0, 0.001f, verticalSpeed * Time.deltaTime);
            speedOnStairs = Mathf.Lerp(1.0f, 0.8f, t);
            myAnimator.SetFloat("runanimationspeed", speedOnStairs);
        }
        
        if (isPushingBox)
            targetVelocity = move * maxSpeed * 0.65f;
        else if (inMud)
            targetVelocity = move * maxSpeed * mudSpeedMultiplier;
        else
        {
            targetVelocity = move * maxSpeed * speedOnStairs;
        }
            
    }

    public float GetCharacterCurrentFlipX()
    {
        return mySkeletonMecanim.skeleton.ScaleX;
    }

    public Vector3 GetRopeClimbingHandOffset()
    {
        return ropeClimbingHandOffset;
    }

    public Transform GetHandRotationGroup()
    {
        return handRotationGroup;
    }

    public void SetIdle()
    {
        myAnimator.SetBool("grounded", true);
        myAnimator.SetFloat("runFloat", 0);
        myAnimator.SetBool("boxpush", false);
        myAnimator.SetBool("holding", false);
        AudioManager.instance.StopSoundEffect("Run");
        AudioManager.instance.StopSoundEffect("PushingCrateCar");
    }

    public void PushingBox(bool value, bool isBox)
    {
        //Debug.Log("PUSHING");
        pushingBoxDelay = 0.1f;
        isPushingBox = true;
        isPushedObjectBox = isBox;
    }

    public void BoxDropped()
    {
        //return;
        if(InputManager.right)
            boxDropped = 1;
        else
            boxDropped = 2;
    }

    private bool wallInFront = false;

    void ActualMovement()
    {
        if(GameplayManager.instance)
        {
            if (GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn)
            {
                lastPosition = transform.position;
                return;
            }
        }

        if(!isActive)
        {
            if (onTopOfPlatform)
            {
                float yPos = platformTop.position.y;
                transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
            }

            //if (onPlatformWaitTime < 0)
            //{
            //    onTopOfPlatform = false;
            //}

            //onPlatformWaitTime -= Time.deltaTime;
            lastPosition = transform.position;
            return;
        }

        lastPosition = transform.position;

        myRigidbody.velocity = Vector3.zero;

        if (isClimbing || inWater)
        {
            lastPosition = transform.position;
            //if(inWater)
            //{
            //    HandleInWater();
            //}
            
            return;
        }

        velocity += gravityModifier * Physics.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        if(grounded==true)
        {
            groundedTime += Time.deltaTime;
        }
        grounded = false;

        Vector3 deltaPosition = velocity * Time.deltaTime;
        Vector3 moveAlongGround = new Vector3(groundNormal.y, -groundNormal.x, 0);
        Vector3 move = moveAlongGround * deltaPosition.x;

        wallInFront = false;
        bool smallStair = Movement(move, false);

        move = Vector3.up * deltaPosition.y;
        Movement(move, true);
        //if(smallStair)
        //    rb2d.position = rb2d.position - moveAlongGround * deltaPosition.x * 1.0f;
        currentHorizontalSpeed = Mathf.Abs(lastPosition.x - transform.position.x);
        verticalSpeed = Mathf.Abs(lastPosition.y - transform.position.y);

        currentHorizontalSpeed2 = Mathf.Lerp(currentHorizontalSpeed2, currentHorizontalSpeed, Time.deltaTime * 35.0f);

        if ((leftInput || rightInput) && wallInFront==false)
        {
            myAnimator.SetFloat("runFloat", 1.0f);
        }
        else
        {
            myAnimator.SetFloat("runFloat", 0);
        }

        if(onTopOfPlatform)
        {
            float yPos = platformTop.position.y;
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        }

        //if(sideInput)
        //{
            if (onPlatformWaitTime < 0)
            {
                SetOnTopOfPlatformFalse();
            }

            onPlatformWaitTime -= Time.deltaTime;
        //}

        if(onMushroom)
        {
            transform.position = offsetFromMushroomTop + mushroomTopToFollow.position;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            //RaycastHit raycastHit;
            //Physics.Raycast(transform.position, Vector3.down, out raycastHit, 1.0f, 1<<0);
            //float tY = raycastHit.point.y;
            //transform.position = new Vector3(transform.position.x, tY, transform.position.z);
        }
    }

    private float currentHorizontalSpeed2 = 0.0f;

    void FixedUpdate()
    {
        if(GameplayManager.instance)
        {
            if (GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn || !isActive)
            {
                lastPosition = transform.position;
                return;
            }
        }
        
        if (isClimbing || inWater)
        {
            lastPosition = transform.position;
            if (inWater)
            {
                HandleInWater();
            }
        }
    }

    bool Movement(Vector3 move, bool yMovement)
    {
        float distance = move.magnitude;
        bool smallStair = false;

        if (distance > minMoveDistance)
        {
            //int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            
            if(yMovement)
            {
                hitBuffer = Physics.BoxCastAll(transform.position + new Vector3(0, 0.65f, 0), new Vector3(0.37f, 0.65f, 0.35f), move, Quaternion.identity, distance + shellRadius, 1 << 0 | 1 << 11 | 1 << 17);
            }
            else
            {
                RaycastHit[] tempHitBuffer = Physics.BoxCastAll(transform.position + new Vector3(0, 0.65f, 0), new Vector3(0.37f, 0.65f, 0.35f), move, Quaternion.identity, distance + shellRadius, 1 << 0 | 1 << 11 | 1 << 17);
                boxCenter = transform.position + new Vector3(0, 0.65f + stairHeight / 2.0f, 0);
                boxSize = new Vector3(0.37f, 0.65f - stairHeight / 2.0f, 0.35f);
                hitBuffer = Physics.BoxCastAll(boxCenter, boxSize, move, Quaternion.identity, distance + shellRadius, 1 << 0 | 1 << 11 | 1 << 17);
                
                if (tempHitBuffer.Length > 0 && hitBuffer.Length == 0)
                    smallStair = true;
            }

            hitBufferList.Clear();
            if(!smallStair)
            {
                for (int i = 0; i < hitBuffer.Length; i++)
                {
                    hitBufferList.Add(hitBuffer[i]);
                }
            }

            if(hitBufferList.Count==0)
                groundedTime = 0.0f;

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                if(yMovement==false)
                wallInFront = true;
                Vector3 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY)
                {
                    if(grounded==false && groundedDelay < 0 && onMushroom==false)
                    {
                        lastVelocityYBeforeGrounded = velocity.y;
                    }

                    grounded = true;
                    groundedDelay = 0.15f;
                    
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }
                else
                {
                    groundedTime = 0.0f;
                }

                float projection = Vector3.Dot(velocity, currentNormal);
                if (projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }

            
        }
        
        if (smallStair&&groundedDelay>0&&isPushingBox==false)
        {
            transform.position = transform.position + move.normalized * (distance+0.0f) + new Vector3(0, 0.1f, 0);
            
        }
        else
        {
            transform.position = transform.position + move.normalized * distance;
        }
            //distance = distance * 3.0f;
        
        return smallStair;
    }

    Vector3 boxCenter;
    Vector3 boxSize;
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }

    public CharName GetCharName()
    {
        return charName;
    }

    public void EnablePlayer()
    {
        myAnimator.speed = animatorSpeed;

        if(!isClimbing)
            GetComponent<Rigidbody>().isKinematic = false;
    }

    public void DisablePlayer(bool disableAnimation)
    {
        if(disableAnimation)
            myAnimator.speed = 0;
        else
            myAnimator.speed = animatorSpeed;

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        if (GameplayManager.instance.LevelCompleted)
        {
            velocity = Vector3.zero;
            targetVelocity = Vector3.zero;
        }
    }

    public void SetIsActive(bool value)
    {
        isActive = value;
        if (isActive)
        {
            EnablePlayer();
        }
        else
        {
            DisablePlayer(true);
        }
    }

    public void SetRigidbody(bool value)
    {
        isActive = value;
        if (value)
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            velocity = Vector3.zero;
            targetVelocity = Vector3.zero;
        }
    }

    public bool GetIsActive()
    {
        return isActive;
    }

    public void TouchedByPlatform(Transform platformTop)
    {
        if(grounded)
        {
            if(transform.position.y > platformTop.position.y - 0.05f)
            {
                transform.parent = platformTop;
                onTopOfPlatform = true;
                this.platformTop = platformTop;
                onPlatformWaitTime = 0.2f;
            }
        }
    }

    public void UnMount()
    {
        climbingState = 0;

        myAnimator.SetBool("holding", false);
        myAnimator.SetBool("climbingUpLedge", false);
        myAnimator.SetBool("climb", false);

        if(myClimbingPositionObject)
        {
            Destroy(myClimbingPositionObject.gameObject);
        }
        if (climbingState == ClimbStates.ClimbingUp1 || climbingState == ClimbStates.ClimbingUp2)
            return;

        if (isClimbing == false)
            return;

        if(animState==AnimState.HoldingIdle)
        {
            myAnimator.SetTrigger("jumprun");
            jumpingStraightUp = false;
            //AudioManager.instance.PlaySoundEffect("Jump");
            animState = AnimState.Jump;
        }
        
        myClimbable.UnMount(transform, (int)charName);
        
        isClimbing = false;
        climbingState = 0;
        targetVelocity = Vector3.zero;
        velocity = Vector3.zero;
        horizontalInput = 0.0f;
        transform.position = lastClimbingPosition;
        lastPosition = lastClimbingPosition;
        GetComponent<Rigidbody>().isKinematic = false;
        myRigidbody.velocity = Vector3.zero;
    }

    public void MountMe(Climbable inputClimbable, ClimbDirection inputClimbDirection)
    {
        GetComponent<Rigidbody>().isKinematic = true;
        grounded = false;
        myAnimator.SetBool("grounded", grounded);
        if (isClimbing)
            return;

        climbJumpWait = true;
        myClimbable = inputClimbable;

        if (inputClimbDirection == ClimbDirection.left)
        {
            //wantedRotation = Quaternion.Euler(0, 269.0f, 0);
            mySkeletonMecanim.skeleton.ScaleX = -1.0f;
        }
            
        else if (inputClimbDirection == ClimbDirection.right)
        {
            //wantedRotation = Quaternion.Euler(0, 91.0f, 0);
            mySkeletonMecanim.skeleton.ScaleX = 1.0f;
        }

        SetRopeClimbingHandOffset();
        isClimbing = true;

        myClimbingPositionObject = new GameObject("MyClimbingPositionObject").transform;
        myClimbable.Mounted(transform, climbingTopOffset, myClimbingPositionObject, (int)charName);
        climbingState = ClimbStates.GoingTowardsPosition;
        AudioManager.instance.StopSoundEffect("Run");
        FXManager.instance.SetRunningDust(false, charName);
        myAnimator.SetBool("holding", true);
        animState = AnimState.HoldingIdle;
        
    }

    public void SetClimbingState(ClimbStates state)
    {
        climbingState = state;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 0)
        {
            if (inWater)
            {
                CheckIfBottomIsTouching();
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 0)
        {
            if (inWater)
            {
                CheckIfBottomIsTouching();
            }
        }
    }

    private void CheckIfBottomIsTouching()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.Find("center").position, new Vector3(0, -1.0f, 0), out hit, -offsetFromCenter, 1 << 0))
        {
            bottomTouchingDelay = 0.1f;
            bottomTouching = true;
        }
        else
        {
            bottomTouching = false;
        }
    }

    private void Climb()
    {
        if(climbingState==ClimbStates.GoingTowardsPosition)   //Wait Before Mounting
        {
            myClimbable.Climb(transform, myClimbingPositionObject, (int)charName);
            transform.position = myClimbingPositionObject.position;
            
        }
        else if (climbingState == ClimbStates.OnClimbable)   //Going up / sliding
        {
            myAnimator.SetBool("climb", false);
            if ((InputManager.up || InputManager.jump) &&isActive)
            //if (InputManager.jump&&isActive)
            {
                if (((InputManager.right && InputManager.jump) || (InputManager.left && InputManager.jump)) && climbJumpWait==false && myClimbable.IsAlmostReachedTop((int)charName)==false)
                //if ((InputManager.right || InputManager.left) && climbJumpWait==false && myClimbable.IsAlmostReachedTop((int)charName)==false)
                {
                    UnMount();
                    velocity.y = jumpHeight;
                    if (InputManager.right)
                    {
                        horizontalInput = 1.0f;
                        wantedHorizontalInput = 1.0f;
                    }
                    else if (InputManager.left)
                    {
                        horizontalInput = -1.0f;
                        wantedHorizontalInput = -1.0f;
                    }

                    myAnimator.SetTrigger("jumprun");
                    jumpingStraightUp = false;
                    AudioManager.instance.PlaySoundEffect("Jump");
                    animState = AnimState.Jump;

                    myClimbable.CharJumpedOff(InputManager.right, (int)charName);
                    
                    return;
                }
                else
                {
                    SetRopeClimbingHandOffset();
                    myAnimator.SetBool("climb", true);
                    myClimbable.MoveUp(transform, (int)charName);
                }
            }

            if(InputManager.down&&isActive)
            {
                SetRopeClimbingHandOffset();
                myAnimator.SetBool("climb", true);
                myClimbable.MoveDown(transform, (int)charName);
            }

            myClimbable.Climb(transform, myClimbingPositionObject, (int)charName);
            transform.position = myClimbingPositionObject.position;

            lastClimbingPosition = transform.position;

            //if (InputManager.up && myClimbable.IsReachedTop((int)charName)&&isActive)
            if ((InputManager.jump || InputManager.up) && myClimbable.IsReachedTop((int)charName)&&isActive)
            {
                myAnimator.SetBool("climbingUpLedge", true);
                animState = AnimState.ClimbingLedge;
                climbingState = ClimbStates.ClimbingUp1;
                myClimbable.ReachedTop((int)charName);
            }

            if(isActive)
            {
                bool sideInput = InputManager.right || InputManager.left;

                if (InputManager.jumpUp)
                {
                    climbJumpWait = false;
                }

                if (InputManager.jump == false || sideInput == false)
                    climbJumpWait = false;
            }
            
        }
        else if (climbingState == ClimbStates.ClimbingUp1 || climbingState == ClimbStates.ClimbingUp2)   //Climbing
        {
            if(isActive)
            {
                myClimbable.Climb(transform, myClimbingPositionObject, (int)charName);
                transform.position = myClimbingPositionObject.position;
            }
        }
    }

    public void SwimmingToClimbing()
    {
        inWater = false;
        myAnimator.SetBool("climbingUpLedge", true);
        myAnimator.SetBool("inwater", inWater);
        animState = AnimState.ClimbingLedge;
        //climbingState = 30;
        myClimbable.ReachedTop((int)charName);
    }

    public bool GetIsClimbing()
    {
        return isClimbing;
    }

    public bool GetInWater()
    {
        return inWater;
    }

    public void SetWaterState(bool value, Water inputWater)
    {
        myWater = inputWater;
        myRigidbody.useGravity = !value;
        inWater = value;
        initialWaterDive = true;
        waterBubbles.gameObject.SetActive(value);
        pointLight.SetActive(value);
        if (value == true)
        {
            animState = AnimState.WaterIdle;
            currentWaterVelocity = (transform.position - lastPosition) / Time.deltaTime;
            currentWaterVelocity.y = Mathf.Clamp(currentWaterVelocity.y, -20.0f, 1000.0f);
            currentWaterVelocity.z = 0;
            waterLastPosition = lastPosition;
            myRigidbody.velocity = Vector3.zero;

            
        }
        else
        {
            if(animState!=AnimState.ClimbingLedge)
            {
                animState = AnimState.Jump;
                myAnimator.SetTrigger("jumprun");
            }
            
            jumpingStraightUp = false;
            targetVelocity = Vector3.zero;
            velocity = Vector3.zero;
            delayBeforeEnteringWater = 0.5f;
        }
    }

    public void SetWaterNearestSide(int inputNearestSide)
    {
        nearestSideOfWaterEntry = inputNearestSide;
        if (nearestSideOfWaterEntry == 3)
            currentWaterVelocity.x = 0;
    }

    public float GetDelayBeforeEnteringWater()
    {
        return delayBeforeEnteringWater;
    }

    private void HandleInWater()
    {
        bool controlsDisabledInWater = initialWaterDive && nearestSideOfWaterEntry == 3;

        bool sideInput = InputManager.left || InputManager.right;
        bool upDownInput = InputManager.up || InputManager.down;
        //bool upDownInput = InputManager.jump || InputManager.down;
        bool anyInput = InputManager.left || InputManager.right || InputManager.up || InputManager.down;
        //bool anyInput = InputManager.left || InputManager.right || InputManager.jump || InputManager.down;

        if (controlsDisabledInWater)
        {
            sideInput = false;
            upDownInput = false;
            anyInput = false;
        }

        if(controlsDisabledInWater == false)
        {
            if (InputManager.right)
            {
                mySkeletonMecanim.skeleton.ScaleX = 1.0f;
            }
            else if (InputManager.left)
            {
                mySkeletonMecanim.skeleton.ScaleX = -1.0f;
            }
        
            if (sideInput && upDownInput)
            {
                //if (InputManager.jump)
                if (InputManager.up)
                    wantedRotationZ = 45.0f;
                else if (InputManager.down)
                    wantedRotationZ = 135.0f;

                if (InputManager.right)
                    wantedRotationZ *= -1.0f;
            }
            else
            {
                if (InputManager.right)
                    wantedRotationZ = -90.0f;
                else if (InputManager.left)
                    wantedRotationZ = 90.0f;
                //else if (InputManager.jump)
                else if (InputManager.up)
                {
                    wantedRotationZ = 0.0f;

                }    
                else if (InputManager.down)
                    wantedRotationZ = 180.0f;
            }
        }
        
        if (anyInput)
        {
            myAnimator.SetBool("movinginwater", true);
            if (swimmingInputStarted)
            {
                swimmingInputStarted = false;
                myAnimator.SetFloat("waterswimrandom", Random.value);
            }
        }
        else
        {
            swimmingInputStarted = true;
            wantedRotationZ = 0.0f;
            myAnimator.SetBool("movinginwater", false);
        }

        if (transform.position.y > myWater.GetTopBound().position.y + offsetFromCenter - 1.0f)
        {
            myAnimator.SetBool("onwatersurface", true);
            wantedRotationZ = 0.0f;
        }
        else
        {
            myAnimator.SetBool("onwatersurface", false);
        }


        float velocityClampValue = (transform.position.y - waterLastPosition.y) / Time.deltaTime;
        if(velocityClampValue>=0)
        {
            if (velocityClampValue < 0.3f)
                velocityClampValue = 0.3f;
            currentWaterVelocity.y = Mathf.Clamp(currentWaterVelocity.y, -1000.0f, velocityClampValue);
        }
        

        waterLastPosition = transform.position;

        if (!upDownInput)
        {
            if (initialWaterDive)
                currentWaterVelocity.y += -currentWaterVelocity.y * bouyancy * Time.deltaTime;
            else
                currentWaterVelocity.y += bouyancy * Time.deltaTime;

            if (initialWaterDive)
            {
                if (Mathf.Abs(currentWaterVelocity.y) < 0.5f)
                    initialWaterDive = false;
            }
        }
        else
        {
            initialWaterDive = false;
        }
        

        if(!sideInput)
            currentWaterVelocity.x = Mathf.MoveTowards(currentWaterVelocity.x, 0, 4.0f * Time.deltaTime);

        currentWaterVelocity.z = 0;

        if(controlsDisabledInWater == false)
        {
            if (InputManager.left)
            {
                currentWaterVelocity.x -= swimmingSpeed * Time.deltaTime;
            }
            if (InputManager.right)
            {
                currentWaterVelocity.x += swimmingSpeed * Time.deltaTime;
            }
            //if (InputManager.jump)
            if (InputManager.up)
            {
                currentWaterVelocity.y += swimmingSpeed * Time.deltaTime;
            }
            if (InputManager.down)
            {
                currentWaterVelocity.y -= swimmingSpeed * Time.deltaTime;
            }
        }
        

        currentWaterVelocity.x = Mathf.Clamp(currentWaterVelocity.x, -3.0f, 3.0f);
        if(initialWaterDive==false)
            currentWaterVelocity.y = Mathf.Clamp(currentWaterVelocity.y, -3.0f, 3.0f);

        Vector3 newPosition = transform.position;

        newPosition += currentWaterVelocity * Time.deltaTime;
        newPosition.y = Mathf.Clamp(newPosition.y, -10000, myWater.GetTopBound().position.y + offsetFromCenter - 0.05f);
        myRigidbody.MovePosition( newPosition);


        myAnimator.SetBool("initialdive", initialWaterDive);
    }

    private bool swimmingInputStarted = true;

    private void SpawnIcon()
    {
        if(GameData.instance==null)
        {
            return;
        }

        iconTransform = Instantiate(GameData.instance.IconTransform, null);
        iconTransform.position = transform.Find("center").position;
        iconTransform.localScale = iconScale;
        iconTransform.GetComponent<Renderer>().material.mainTexture = icon;
        iconTransform.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0);
        iconTransform.parent = mySkeletonMecanim.transform;
    }

    public Transform GetIconTransform()
    {
        return iconTransform;
    }

    public Transform GetCenter()
    {
        return transform.Find("center");
    }

    private void SetChildOffsets()
    {
        float waterOffset = 0;
        if (inWater&&!isClimbing)
        {
            if (transform.position.y < myWater.GetTopBound().position.y + offsetFromCenter - 1.0f)
                waterOffset = 0.8f;
        }

        float lerpSpeed = inWater ? 1.0f : 5.0f;

        currentWaterOffset = Mathf.Lerp(currentWaterOffset, waterOffset, Time.deltaTime * lerpSpeed);

        transform.Find("center").localPosition = new Vector3(transform.Find("center").localPosition.x, 1.0f- currentWaterOffset, transform.Find("center").localPosition.z);
        transform.Find("Visual").localPosition = new Vector3(transform.Find("Visual").localPosition.x, -currentWaterOffset, transform.Find("Visual").localPosition.z);

        boxCollider.center = new Vector3(0, 0.8f - currentWaterOffset, 0);
        capsuleCollider.center = new Vector3(0, 0.8f - currentWaterOffset, 0);
    }

    public Vector3 GetCurrentWaterVelocity()
    {
        return currentWaterVelocity;
    }

    public GameObject GetMyVisual()
    {
        return myAnimator.gameObject;
    }

    public void SetControlsDisabled(bool value)
    {
        controlsDisabled = value;
    }

    public void DisableRunDelay()
    {
        runDelay = 0.05f;
    }

    public void SetSittingAnimation()
    {
        myAnimator.SetBool("holding", true);
        myAnimator.SetBool("grounded", false);
        isClimbing = true;
    }

    public void SittingToIdleAnimation()
    {
        myAnimator.SetBool("holding", false);
        myAnimator.SetBool("grounded", true);
        isClimbing = false;
    }

    public void SetFacingDirection(float value)
    {
        mySkeletonMecanim.skeleton.ScaleX = value;
    }

    public void SetSittingWithBirdIdle()
    {
        myAnimator.SetBool("sittingwithbird", true);
        myAnimator.SetBool("sittingwithbirdidle", true);
    }

    public void SetSittingWithBirdShock()
    {
        myAnimator.SetBool("sittingwithbird", true);
        myAnimator.SetBool("sittingwithbirdidle", false);
    }

    public void ShockToIdleGirl()
    {
        myAnimator.SetBool("sittingwithbird", false);
        myAnimator.SetBool("sittingwithbirdidle", false);
    }

    public void SetBenSleeping()
    {
        myAnimator.SetBool("bensleeping", true);
        myAnimator.SetBool("bensleepingidle", true);
    }

    public void SetBenShock()
    {
        myAnimator.SetBool("bensleeping", true);
        myAnimator.SetBool("bensleepingidle", false);
    }

    public void ShockToIdleBoy()
    {
        myAnimator.SetBool("bensleeping", false);
        myAnimator.SetBool("bensleepingidle", false);
    }

    public bool IsIdlePlaying()
    {
        return myAnimator.GetCurrentAnimatorStateInfo(0).IsName("idle 0");
    }

    public void SeeingPhoenix()
    {
        myAnimator.SetBool("seeingphoenix", true);
        StartCoroutine("SeeingPhoenixOff");
    }

    private IEnumerator SeeingPhoenixOff()
    {
        yield return new WaitForSeconds(2.0f);
        myAnimator.SetBool("seeingphoenix", false);
    }

    public void SetShadow(bool value)
    {
        transform.Find("BlobShadow").gameObject.SetActive(value);
    }

    private void CalculateAndSetXRayAlpha()
    {
        CalculateZoomedInXrayValue();
        SetXRayAlpha(lastZoomedInXrayValue);
    }

    public void ZoomingOutXRayValue(bool isFarEnough)
    {
        CalculateZoomedInXrayValue();
        if(isFarEnough&&!GameplayManager.instance.IsFTUE)
        {
            SetXRayAlpha(0.6f);
        }
        else
        {
            SetXRayAlpha(lastZoomedInXrayValue);
        }
    }

    private void SetXRayAlpha(float alpha)
    {
        myRenderer.sharedMaterial.SetFloat("_OverlayPwr", alpha);
    }

    public void CalculateZoomedInXrayValue()
    {
        if(GameplayManager.instance)
        {
            if (GameplayManager.instance.IsFTUE)
            {
                lastZoomedInXrayValue = 0.0f;
                return;
            }
        }

        if(GameplayManager.instance==null)
        {
            lastZoomedInXrayValue = 0;
            return;
        }
        //0 left 1 right 2 bottom 3 top
        Piece myCurrentPiece = GameplayManager.instance.GetPieceFromPosition(transform.position);
        Vector3 piecePosition = myCurrentPiece.transform.position;
        float multiplier = 1.0f;
        if (transform.position.x < piecePosition.x)
        {
            multiplier = -1.0f;
        }

        float maxValue = piecePosition.x + multiplier * GameplayManager.instance.GetPieceWidth() / 2.0f;

        maxValue -= multiplier * xrayOffsetMaxX;
        float minValue = maxValue - multiplier * xrayOffsetMinX;

        float tX = Mathf.InverseLerp(minValue, maxValue, transform.position.x);
        if ((myCurrentPiece.GetAreSidesMatched(0) == false && multiplier < 0) || (myCurrentPiece.GetAreSidesMatched(1) == false && multiplier > 0))
        {
            tX = 0;
        }

        multiplier = 1.0f;
        if (transform.position.y < piecePosition.y)
        {
            multiplier = -1.0f;
        }

        maxValue = piecePosition.y + multiplier * GameplayManager.instance.GetPieceHeight() / 2.0f;
        maxValue -= multiplier * xrayOffsetMaxY;
        minValue = maxValue - multiplier * xrayOffsetMinY;

        float tY = Mathf.InverseLerp(minValue, maxValue, transform.position.y - offsetFromCenter);
        if ((myCurrentPiece.GetAreSidesMatched(2) == false && multiplier < 0) || (myCurrentPiece.GetAreSidesMatched(3) == false && multiplier > 0))
        {
            tY = 0;
        }

        float t = tX > tY ? tX : tY;
        lastZoomedInXrayValue = t * maxXRayValue;
    }

    public void EnteredMushroom(Transform topToFollow)
    {
        if (onMushroom == false && grounded == false && groundedDelay < 0)
        {
            lastVelocityYBeforeGrounded = velocity.y;
        }

        mushroomTopToFollow = topToFollow;
        offsetFromMushroomTop = transform.position - topToFollow.position;
        onMushroom = true;
    }

    public void MushroomJump(float upwardForce, float minForce)
    {
        float t = Mathf.InverseLerp(-2.5f, -10.0f, lastVelocityYBeforeGrounded);
        velocity.y = Mathf.Lerp(minForce, upwardForce, t);
        SetOnTopOfPlatformFalse();
        
        onPlatformWaitTime = -1.0f;
        onMushroom = false;
        mushroomTopToFollow = null;
    }

    public void SetInMud(bool value)
    {
        inMud = value;
        myAnimator.SetBool("inmud", value);
    }

    private void SetOnTopOfPlatformFalse()
    {
        onTopOfPlatform = false;
        transform.parent = null;
    }

    public bool MountedOnBigFlyingCreature(Transform flyingCreatureTop, BigFlyingCreature bigFlyingCreature)
    {
        if (grounded)
        {
            if (transform.position.y > flyingCreatureTop.position.y - 0.05f)
            {
                transform.parent = flyingCreatureTop;
                this.flyingCreatureTop = flyingCreatureTop;
                this.bigFlyingCreature = bigFlyingCreature;

                mountedOnBigFlyingCreature = true;
                myRigidbody.useGravity = false;
                return true;
            }
        }

        return false;
    }

    private void UnmountBigFlyingCreature()
    {
        bigFlyingCreature.Unmounted(transform);
        transform.parent = null;
        flyingCreatureTop = null;
        bigFlyingCreature = null;

        mountedOnBigFlyingCreature = false;
        myRigidbody.useGravity = true;

        velocity.y = jumpHeight;
    }
}