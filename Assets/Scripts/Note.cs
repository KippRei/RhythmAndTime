using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note: MonoBehaviour
{
    Music music;
    AudioSource audioSource;
    public AudioClip tap;
    Timing timing;
    public double noteTime;
    double scrollTime;
    bool played = false;
    double currentTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        music = GameObject.Find("Music").GetComponent<Music>();
        timing = GameObject.Find("HitTiming").GetComponent<Timing>();
        scrollTime = (4 * timing.beat);
        noteTime = timing.lastTime;
    }

    private void Update()
    {
        currentTime = timing.curTime;
        if (noteTime + scrollTime <= currentTime && played == false)
        {
            //audioSource.PlayOneShot(tap);
            played = true;
        }
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
        if (collision.collider.name.ToString() == "Left" || collision.collider.name.ToString() == "Right" || collision.collider.name.ToString() == "Middle")
        {
            
        }
    }

    public double GetTime()
    {
        return noteTime + scrollTime;
    }

    public void DestroyNote()
    {
        Destroy(gameObject);
    }
}