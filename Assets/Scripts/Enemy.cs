using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{

    bool isBig = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Movement()
    {
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
}
