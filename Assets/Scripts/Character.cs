using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] Rigidbody2D thisChar;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Movement();
        //Shoot();
    }

    public abstract void Movement();

    public abstract void Shoot();
}
