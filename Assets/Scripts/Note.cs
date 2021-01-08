using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note: MonoBehaviour
{
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.ToString() == "Offscreen")
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name.ToString() == "Left" || collision.collider.name.ToString() == "Right")
        {
            audioSource.Play();
        }
    }
}