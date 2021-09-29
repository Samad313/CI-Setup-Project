using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MountedPlayer
{
    public Transform playerTransform;
    public float mountTime;
    public bool mounted;

    public MountedPlayer(Transform playerTransform, float mountTime, bool mounted)
    {
        this.playerTransform = playerTransform;
        this.mountTime = mountTime;
        this.mounted = mounted;
    }
}

public class BigFlyingCreature : MonoBehaviour
{
    [SerializeField] private Transform top;
    [SerializeField] private Transform startingPosition;
    [SerializeField] private float goingBackSpeed = 2.0f;

    [SerializeField] private Vector2 forceMultiplier = new Vector2(2.0f, 2.0f);

    private Rigidbody myRigidbody;

    private bool mounted = false;

    private Vector3 savedVelocity;

    private bool isActive = true;

    [SerializeField]
    private List<MountedPlayer> mountedPlayers;

    // Start is called before the first frame update
    void Awake()
    {
        mountedPlayers = new List<MountedPlayer>();
        myRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn)
            return;

        if (!isActive)
            return;

        if (mounted==false)
        {
            transform.position = Vector3.MoveTowards(transform.position, startingPosition.position, Time.deltaTime * goingBackSpeed);
        }

        for (int i = mountedPlayers.Count-1; i >= 0; i--)
        {
            if(mountedPlayers[i].mounted==false)
            {
                mountedPlayers[i].mountTime += Time.deltaTime;
                if (mountedPlayers[i].mountTime > 0.05f)
                {
                    mountedPlayers.RemoveAt(i);
                    if (mountedPlayers.Count <= 0)
                        mounted = false;
                }   
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer==10)
        {
            MountPlayer(collision.transform);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 10)
        {
            MountPlayer(collision.transform);
        }
    }

    public void Move(Vector2 movementVector)
    {
        if (GameplayManager.instance.ZoomStatus != ZoomStatuses.ZoomedIn)
            return;

        myRigidbody.AddForce(movementVector * forceMultiplier);
    }

    private void MountPlayer(Transform playerTransform)
    {
        MountedPlayer thisMountedPlayer = mountedPlayers.Find(val => val.playerTransform == playerTransform);
        if (thisMountedPlayer == null)
        {
            if(playerTransform.GetComponent<PlayerController>().MountedOnBigFlyingCreature(top, this))
            {
                mounted = true;
                mountedPlayers.Add(new MountedPlayer(playerTransform, 0.0f, true));
                Debug.Log("Mounted");
            }
        } 
    }

    public void Unmounted(Transform playerTransform)
    {
        int index = -1;
        for (int i = 0; i < mountedPlayers.Count; i++)
        {
            if(mountedPlayers[i].playerTransform==playerTransform)
            {
                index = i;
                break;
            }
        }

        Debug.Log(index);
        if(index!=-1)
        {
            mountedPlayers[index].mounted = false;
        }
        
    }

    public void SetActive(bool value)
    {
        isActive = value;
        if (value == true)
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
}
