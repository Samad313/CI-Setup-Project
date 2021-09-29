using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{

    public float minGroundNormalY = .65f;
    public float gravityModifier = 1f;

    protected Vector3 targetVelocity;
    protected bool grounded;
    protected Vector3 groundNormal;
    protected Rigidbody rb2d;
    protected Vector3 velocity;
    protected RaycastHit[] hitBuffer = new RaycastHit[16];
    protected List<RaycastHit> hitBufferList = new List<RaycastHit>(16);


    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody>();
    }

    void Start()
    {
    }

    void Update()
    {
        

        targetVelocity = Vector3.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity()
    {

    }

    void FixedUpdate()
    {
        velocity += gravityModifier * Physics.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        grounded = false;

        Vector3 deltaPosition = velocity * Time.deltaTime;

        Vector3 moveAlongGround = new Vector3(groundNormal.y, -groundNormal.x, 0);

        Vector3 move = moveAlongGround * deltaPosition.x;

        Movement(move, false);

        move = Vector3.up * deltaPosition.y;

        Movement(move, true);
    }

    void Movement(Vector3 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > minMoveDistance)
        {
            //int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBuffer = Physics.BoxCastAll(transform.position + new Vector3(0, 0.65f, 0), new Vector3(0.15f, 0.65f, 0.35f), move, Quaternion.identity, distance + shellRadius, 1<<0 | 1<<11);
            hitBufferList.Clear();
            for (int i = 0; i < hitBuffer.Length; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector3 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY)
                {
                    grounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
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

        rb2d.position = rb2d.position + move.normalized * distance;
    }

}