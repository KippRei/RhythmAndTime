using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosition : MonoBehaviour
{
    public AudioClip tap;
    AudioSource audioSource;

    // Start is called before the first frame update
    //void Start()
    //{
    //    audioSource = GetComponent<AudioSource>();
    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log(collision.name.ToString());
    //    if (collision.name.ToString() == "Note(Clone)" || collision.name.ToString() == "MNote(Clone)" || collision.name.ToString() == "BNote(Clone)")
    //    {            
    //        audioSource.PlayOneShot(tap);
    //    }
    //}
}
