using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableBox : MonoBehaviour
{
    [SerializeField]
    private float pushForce = 10.0f;

    [SerializeField]
    private float pushUpForce = 0.2f;

    [SerializeField]
    private float horizontalContactOffset = 0.4f;

    private Rigidbody myRigidbody;
    //private CapsuleCollider myCapsuleCollider;

    private Vector3 lastPosition;

    private float actualHorizontalContactOffset;
    private float actualHorizontalContactOffsetZero;
    private float currentActualHorizontalContactOffset;

    private Vector3 savedVelocity;

    private bool isActive = true;

    private float wantedVelocityX = 0;
    private float currentVelocityX = 0;

    [SerializeField]
    private float acceleration = 2.0f;

    [SerializeField]
    private bool isBox = true;
    [SerializeField]
    private bool isBattery = false;

    private bool stopRotation = true;
    private float wantedRotationZ = 0.0f;

    private bool isMoving = false;
    private float lastX = 0;

    private float currentDiffX = 0;

    private float zoomInWaitForLastX = -1.0f;

    private bool boxDroppedFromGround = false;

    private PlayerController lastPusher;

    [SerializeField]
    private bool droppable = false;

    [SerializeField]
    private bool canShatter = false;

    [SerializeField]
    private GameObject snow = default;

    private bool moveToButton = false;
    private Transform myButtonTransform;

    [SerializeField]
    private string myPushingSound = "BoxPush";

    [SerializeField]
    private string myDropSound = "BoxDrop";

    private float lastSavedVelocityY = 0;

    [SerializeField]
    private Transform blobShadow = default;

    private bool inWater = false;

    private Vector3 currentWaterVelocity;

    [SerializeField]
    private float bouyancy = 4.0f;

    private Water myWater;

    [SerializeField]
    private float waterPushDownwardForce = 4.0f;

    private bool floatWobble = false;
    private float floatWobbleSinLerp = 0;
    private Vector3 visualOriginalPosition;

    private bool isGrounded = false;
    private float groundedDelay = 0;

    private bool isPushed = false;

    private bool bottomTouching = false;
    private float bottomTouchingDelay = -1.0f;


    // Start is called before the first frame update
    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        //myCapsuleCollider = GetComponent<CapsuleCollider>();

        actualHorizontalContactOffset = horizontalContactOffset + transform.localScale.x / 2.0f;
        actualHorizontalContactOffsetZero = transform.localScale.x / 2.0f; ;
        currentActualHorizontalContactOffset = actualHorizontalContactOffset;
        lastPosition = myRigidbody.position;
        savedVelocity = Vector3.zero;
        lastX = transform.position.x;
        lastPusher = null;

        visualOriginalPosition = transform.Find("Visual").localPosition;
    }

    public bool GetCanShatter()
    {
        return canShatter;
    }

    public bool IsOrb()
    {
        return !isBox;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SetIsMoving();
        if(GameplayManager.instance)
            if (GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn)
                return;

        if (!isActive)
            return;

        SetHorizontalContactOffset();

        if(blobShadow)
        {
            blobShadow.position = transform.position + new Vector3(0, 1.25f, 0);
            blobShadow.eulerAngles = new Vector3(90, 0, 0);
        }
        

        if (moveToButton)
        {
            float tX = Mathf.Lerp(transform.position.x, myButtonTransform.position.x, Time.deltaTime * 2.0f);
            transform.position = new Vector3(tX, transform.position.y, transform.position.z);
            return;
        }
        if (Mathf.Abs(myRigidbody.velocity.y) < 0.01f && groundedDelay < 0)
        {
            isGrounded = true;
            groundedDelay = 0.2f;
        }
        else if(groundedDelay<0)
        {
            isGrounded = false;
            groundedDelay = 0.1f;
        }

        groundedDelay -= Time.deltaTime;

        if (isGrounded)
        {
            boxDroppedFromGround = false;
        }

        if (myRigidbody.velocity.y < -0.4f)
        {
            if(boxDroppedFromGround == false&&droppable)
            {
                //Piece myCurrentPiece = PiecesManager.instance.GetPieceFromPosition(new Vector2(transform.position.x, transform.position.y));

                if(lastPusher)
                {
                    lastPusher.BoxDropped();
                    boxDroppedFromGround = true;
                    //Cutscene.instance.StartCutscene(transform, 2.0f);
                }
            }
            if (myRigidbody.velocity.y < -3.0f)
                wantedVelocityX = 0;
        }

        if(isBox || (isPushed&&!isBox))
        {
            currentVelocityX = Mathf.Lerp(currentVelocityX, wantedVelocityX, Time.deltaTime * acceleration);
            myRigidbody.velocity = new Vector3(currentVelocityX, myRigidbody.velocity.y, 0);
        }

        if (inWater)
        {
            myRigidbody.AddForce(new Vector3(0, bouyancy, 0), ForceMode.VelocityChange);
            Vector3 newPosition = transform.position;

            //newPosition += currentWaterVelocity * Time.deltaTime;
            if(bottomTouching==false)
                newPosition.y = Mathf.Clamp(newPosition.y, -10000, myWater.GetTopBound().position.y - 0.5f);
            myRigidbody.position = newPosition;

            if(transform.position.y > myWater.GetTopBound().position.y - 0.6f)
            {
                floatWobble = true;
            }
            else
            {
                floatWobble = false;
            }
        }
        else
        {
            floatWobble = false;

        }

        if(floatWobble)
        {
            floatWobbleSinLerp += Time.deltaTime * 3.0f;
        }
        else
        {
            floatWobbleSinLerp = Mathf.Lerp(floatWobbleSinLerp, 0, Time.deltaTime * 5.0f);
        }

        transform.Find("Visual").localPosition = visualOriginalPosition + new Vector3(0, Mathf.Sin(floatWobbleSinLerp)*0.03f, 0);
        
        if ((lastPosition - myRigidbody.position).magnitude > 0.001f)
        {
            float moveDirection = 1.0f;
            if (myRigidbody.position.x < lastPosition.x)
                moveDirection = -1.0f;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, new Vector3(moveDirection, 0, 0), out hit, currentActualHorizontalContactOffset, 1 << 0 | 1 << 10 | 1 << 17 | 1 << 11))
            {
                float xPoint = hit.point.x - currentActualHorizontalContactOffset * moveDirection;
                if (hit.transform.gameObject.layer == 10)
                    xPoint = transform.position.x;

                myRigidbody.position = new Vector3(xPoint, myRigidbody.position.y, myRigidbody.position.z);
                float clampedVelocityY = myRigidbody.velocity.y;
                if(inWater)
                {
                    clampedVelocityY = Mathf.Clamp(clampedVelocityY, -2.0f, 100000.0f);
                }
                myRigidbody.velocity = new Vector3(0, clampedVelocityY, 0);
                wantedVelocityX = 0;
                currentVelocityX = 0;
            }
        }

        //if (inWater && myRigidbody.velocity.y>0)
        //{
        //    float clampedVelocityY = myRigidbody.velocity.y;
        //    clampedVelocityY = Mathf.Clamp(clampedVelocityY, -2.0f, 100000.0f);
        //    float velocityT = Mathf.InverseLerp(myWater.GetTopBound().position.y - 1.0f, myWater.GetTopBound().position.y - 0.5f, myRigidbody.position.y);

        //    if(showDebug)
        //    Debug.Log(velocityT);
        //    clampedVelocityY = Mathf.Lerp(clampedVelocityY, 0, velocityT);
        //    myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, clampedVelocityY, myRigidbody.velocity.z);
        //}

        lastPosition = myRigidbody.position;
        wantedVelocityX = 0;
        isPushed = false;

        if (isBox && stopRotation)
            transform.localEulerAngles = new Vector3(0, 0, wantedRotationZ);


        bottomTouchingDelay -= Time.deltaTime;
        if(bottomTouchingDelay<0)
        {
            bottomTouching = false;
        }
    }

    private void SetHorizontalContactOffset()
    {
        currentActualHorizontalContactOffset = actualHorizontalContactOffset;
        if (isBox)
            return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, new Vector3(0f, -1.0f, 0), out hit, 3.0f, 1 << 0 ))
        {
            float angleFromUp = Vector3.Angle(Vector3.up, hit.normal);
            float lerpT = Mathf.InverseLerp(0, 25.0f, angleFromUp);
            currentActualHorizontalContactOffset = Mathf.Lerp(actualHorizontalContactOffset, actualHorizontalContactOffsetZero, lerpT);
        }
    }

    public void FixToButton(Transform buttonTransform)
    {
        myButtonTransform = buttonTransform;
        moveToButton = true;
        StartCoroutine("DisableColliderAfterSometime");
    }

    private IEnumerator DisableColliderAfterSometime()
    {
        if(transform.Find("Visual").GetComponent<Renderer>().material.HasProperty("_tint"))
        {
            Color startingColor = transform.Find("Visual").GetComponent<Renderer>().material.GetColor("_tint");
            float t = 0.0f;
            while(t<1.0f)
            {
                transform.Find("Visual").GetComponent<Renderer>().material.SetColor("_tint", Color.Lerp(startingColor, new Color(1, 0.7f, 0.1f, 0.5f), t) );
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        //GetComponent<Collider>().enabled = false;
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.useGravity = false;

        if (isBattery)
        {
            myRigidbody.isKinematic = true;
            myRigidbody.detectCollisions = false;
        }

    }

    private void SetIsMoving()
    {
        if(GameplayManager.instance)
        {
            if (GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn)
            {
                zoomInWaitForLastX = 0.1f;
                lastX = transform.localPosition.x;
                currentDiffX = 0;
                if (isMoving == true)
                    AudioManager.instance.StopSoundEffect(myPushingSound);
                isMoving = false;
                return;
            }

        }

        zoomInWaitForLastX -= Time.deltaTime;
        if (zoomInWaitForLastX > 0)
        {
            lastX = transform.localPosition.x;
            currentDiffX = 0;
            return;
        }
            

        float diffX = Mathf.Abs(lastX - transform.localPosition.x);
        currentDiffX = Mathf.Lerp(currentDiffX, diffX, Time.deltaTime * 3.0f);
        
        if(currentDiffX > 0.01f)
        {
            if (isMoving == false)
            {
                AudioManager.instance.PlaySoundEffect(myPushingSound);
                
            }

            float soundVolumeToSet = Mathf.InverseLerp(0.01f, 0.1f, currentDiffX);
            soundVolumeToSet /= 2.5f;
            AudioManager.instance.SetSoundEffectVolume(myPushingSound, soundVolumeToSet);
            isMoving = true;
        }
        else
        {
            if (isMoving == true)
                AudioManager.instance.StopSoundEffect(myPushingSound);
            isMoving = false;
        }

        lastX = transform.localPosition.x;


        lastSavedVelocityY = myRigidbody.velocity.y;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 0)
        {
            if (Mathf.Abs(myRigidbody.velocity.y) > 6.0f || Mathf.Abs(lastSavedVelocityY) > 6.0f)
            {
                AudioManager.instance.PlaySoundEffect(myDropSound);
            }

            if(inWater)
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
        if (Physics.Raycast(transform.position, new Vector3(0, -1.0f, 0), out hit, 1.0f, 1 << 0))
        {
            bottomTouchingDelay = 0.1f;
            bottomTouching = true;
        }
        else
        {
            bottomTouching = false;
        }
    }

    private bool GetLeftInput(PlayerController playerController)
    {
        if (playerController.GetIsActive() == false)
            return false;

        return InputManager.left;
    }

    private bool GetRightInput(PlayerController playerController)
    {
        if (playerController.GetIsActive() == false)
            return false;

        return InputManager.right;
    }

    void OnTriggerStay(Collider other)
    {
        if (moveToButton)
            return;

        if (other.gameObject.layer==10)
        {
            if (other.transform.position.x>transform.position.x && GetLeftInput(other.GetComponent<PlayerController>()))
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, new Vector3(-1.0f, 0, 0), out hit, currentActualHorizontalContactOffset, 1 << 0 | 1 << 10 | 1 << 17 | 1<<11))
                {
                    float xPoint = hit.point.x + currentActualHorizontalContactOffset;
                    if (hit.transform.gameObject.layer == 10)
                        xPoint = transform.position.x;
                    myRigidbody.position = new Vector3(xPoint, myRigidbody.position.y, myRigidbody.position.z);
                    myRigidbody.velocity = new Vector3(0, myRigidbody.velocity.y, 0);
                    lastPosition = myRigidbody.position;
                    wantedVelocityX = 0;
                    currentVelocityX = 0;
                }
                else
                {
                    
                    if (isBox)
                    {
                        wantedVelocityX = -pushForce;
                        if(inWater)
                            myRigidbody.AddForce(new Vector3(0, -waterPushDownwardForce, 0), ForceMode.Force);
                    }
                    else
                    {
                        wantedVelocityX = -pushForce;
                        isPushed = true;
                        myRigidbody.AddForce(new Vector3(0, pushUpForce, 0), ForceMode.VelocityChange);
                        myRigidbody.AddTorque(new Vector3(0, 0, 1.5f));
                        myRigidbody.AddTorque(Random.insideUnitSphere * 0.5f);
                    }
                        
                    //Debug.Log("Box going LEFT");
                }

                
                lastPusher = other.GetComponent<PlayerController>();
                lastPusher.PushingBox(true, isBox);


            }
            else if (other.transform.position.x < transform.position.x && GetRightInput(other.GetComponent<PlayerController>()))
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, new Vector3(1.0f, 0, 0), out hit, currentActualHorizontalContactOffset, 1 << 0 | 1 << 10 | 1 << 17 | 1 << 11))
                {
                    
                    float xPoint = hit.point.x - currentActualHorizontalContactOffset;
                    if (hit.transform.gameObject.layer == 10)
                        xPoint = transform.position.x;

                    myRigidbody.position = new Vector3(xPoint, myRigidbody.position.y, myRigidbody.position.z);
                    myRigidbody.velocity = new Vector3(0, myRigidbody.velocity.y, 0);
                    lastPosition = myRigidbody.position;
                    wantedVelocityX = 0;
                    currentVelocityX = 0;
                }
                else
                {
                    
                    if (isBox)
                    {
                        wantedVelocityX = pushForce;
                        if (inWater)
                            myRigidbody.AddForce(new Vector3(0, -waterPushDownwardForce, 0), ForceMode.Force);
                    }
                        
                    else
                    {
                        wantedVelocityX = pushForce;
                        isPushed = true;
                        myRigidbody.AddForce(new Vector3(0, pushUpForce, 0), ForceMode.VelocityChange);
                        myRigidbody.AddTorque(new Vector3(0, 0, -1.5f));
                        myRigidbody.AddTorque(Random.insideUnitSphere*0.5f);
                    }
                        
                    //Debug.Log("Box going RIGHT");
                }
              
                lastPusher = other.GetComponent<PlayerController>();
                lastPusher.PushingBox(true, isBox);
            }
        } 
    }

    public void SetActive(bool value)
    {
        isActive = value;
        if (value==true)
        {
            myRigidbody.isKinematic = false;
            myRigidbody.velocity = savedVelocity;
        }
        else
        {
            savedVelocity = myRigidbody.velocity;
            myRigidbody.velocity = Vector3.zero;
            myRigidbody.isKinematic = true;
        }
    }

    public void FragileHit(float hitDirection)
    {
        StartCoroutine("FragileHitCoroutine", hitDirection);
    }

    private IEnumerator FragileHitCoroutine(float hitDirection)
    {
        stopRotation = false;
        float delayTime = 1.0f;
        if (snow)
            snow.SetActive(false);
        while (delayTime>0)
        {
            myRigidbody.AddForce(5 * hitDirection, 0, 0);
            delayTime -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        if(isBox)
        {
            wantedRotationZ = ((int)transform.localEulerAngles.z + 45) / 90;
            wantedRotationZ *= 90;
            stopRotation = true;
        }
    }

    public void SetWaterState(bool value, Water inputWater, int inputNearestSide)
    {
        myWater = inputWater;
        myRigidbody.useGravity = !value;
        inWater = value;
        if (value == true)
        {
            currentWaterVelocity = (transform.position - lastPosition) / Time.deltaTime;
            currentWaterVelocity.z = 0;
        }
        else
        {

        }
    }
}
