using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mud : MonoBehaviour
{
    private Transform[] playerTransforms;
    private PlayerController[] playerControllers;
    private bool[] playerInside;
    private Vector4 bounds;

    // Start is called before the first frame update
    void Start()
    {
        playerTransforms = GameplayManager.instance.GetAllPlayerTransforms();
        playerControllers = GameplayManager.instance.GetAllPlayers();

        playerInside = new bool[playerTransforms.Length];

        for (int i = 0; i < playerInside.Length; i++)
        {
            playerInside[i] = false;
        }

        bounds.x = -transform.localScale.x/2.0f;
        bounds.y = transform.localScale.x / 2.0f;
        bounds.z = -transform.localScale.y / 2.0f;
        bounds.w = transform.localScale.y / 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < playerTransforms.Length; i++)
        {
            playerControllers[i].SetInMud(InsideMe(playerTransforms[i].position));
        }
    }

    private bool InsideMe(Vector2 playerPosition)
    {
        if (playerPosition.x < transform.position.x + bounds.x || playerPosition.x > transform.position.x + bounds.y
            || playerPosition.y < transform.position.y + bounds.z || playerPosition.y > transform.position.y + bounds.w)
            return false;

        return true;
    }
}
