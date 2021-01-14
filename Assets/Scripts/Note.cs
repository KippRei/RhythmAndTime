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
        scrollTime = (timing.noteSpeed * timing.beat);
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
            if (transform.position.x == timing.left.x)
            {
                DestroyNote(timing.leftNoteQ);
            }
            else if (transform.position.x == timing.middle.x)
            {
                DestroyNote(timing.middleNoteQ);
            }
            else if (transform.position.x == timing.right.x)
            {
                DestroyNote(timing.rightNoteQ);
            }
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

    public void DestroyNote(Queue<Note>noteQ)
    {
        noteQ.Dequeue();
        Destroy(gameObject);
    }
}