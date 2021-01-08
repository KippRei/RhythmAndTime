using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] float playerMovementSpeed = .01f;
    bool stopMoving = false;
    bool isBig = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    /*void Update()
    {
        if (stopMoving == false)
        {
            Vector3 playerPos = gameObject.transform.position;
            if (Input.GetKey(KeyCode.A))
            {
                Debug.Log("Left");
                playerPos.x -= playerMovementSpeed;
                gameObject.transform.position = playerPos;
            }
            if (Input.GetKey(KeyCode.D))
            {
                Debug.Log("Right");
                playerPos.x += playerMovementSpeed;
                gameObject.transform.position = playerPos;
            }
        }
    }*/

    public override void Movement()
    {
        /*Vector3 playerPos = gameObject.transform.position;
        if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("Left");
            playerPos.x -= playerMovementSpeed;
            gameObject.transform.position = playerPos;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Debug.Log("Right");
            playerPos.x += playerMovementSpeed;
            gameObject.transform.position = playerPos;
        }*/
        if (isBig == false)
        {
            isBig = true;
            gameObject.transform.localScale = new Vector3(.105f, .205f, 1);
        }
        else
        {
            isBig = false;
            gameObject.transform.localScale = new Vector3(.1f, .2f, 1);
        }
    }

    public override void Shoot()
    {
        return;
    }

    /*void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "Enemy")
        {
            stopMoving = true;
        }
        Debug.Log(col.GetContact(0).point);
        Debug.Log(col.GetContact(1).point);
    }*/
}
