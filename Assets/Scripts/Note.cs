using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note: MonoBehaviour
{
    Music music;
    AudioSource audioSource;
    float noteTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        music = GameObject.FindObjectOfType<Music>();
        noteTime = music.musicSource.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.ToString() == "Offscreen")
        {
            DestroyNote();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name.ToString() == "Left" || collision.collider.name.ToString() == "Right")
        {
            //audioSource.Play();
        }
    }

    public float GetTime()
    {
        return noteTime;
    }

    public void DestroyNote()
    {
        Destroy(gameObject);
    }
}