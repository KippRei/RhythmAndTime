using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MNote : Note
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.ToString() == "Player")
        {
            if (transform.position.x < 0)
            {
                DestroyNote(timing.leftMNoteQ);
            }

            else if (transform.position.x > 0)
            {
                DestroyNote(timing.rightMNoteQ);
            }

            else if (transform.position.x == 0)
            {
                DestroyNote(timing.middleMNoteQ);
            }
        }
        if (collision.name.ToString() == "Offscreen")
        {
            if (transform.position.x == timing.left.x)
            {
                DestroyNote(timing.leftMNoteQ);
            }
            else if (transform.position.x == timing.middle.x)
            {
                DestroyNote(timing.middleMNoteQ);
            }
            else if (transform.position.x == timing.right.x)
            {
                DestroyNote(timing.rightMNoteQ);
            }
        }
    }

    public void DestroyNote(Queue<MNote> noteQ)
    {
        noteQ.Dequeue();
        Destroy(gameObject);
    }
};