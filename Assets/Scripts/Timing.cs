using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Timing : MonoBehaviour
{
    [SerializeField] Text counterText = null;
    [SerializeField] Text timingText = null;
    [SerializeField] Music music;
    // Need to change names of Enemy and Player class
    [SerializeField] Enemy enemy;
    [SerializeField] Player player;
    // For BPM and sync
    [Range(90.0f, 200.0f)] [SerializeField] float beat = 120f;
    [SerializeField] float subDiv = 1f;
    [SerializeField] float visualOffset = 0;
    [SerializeField] float musicOffset = 0;
    int count = 0;
    // Song time used for timing
    float startTime = 0;
    float curTime = 0;
    float lastTime = 0;
    // Timing Windows
    float perfectTiming = .024f;
    float goodTiming = .038f;
    float okTiming = .064f;
    // Keys for Input 
    char inputKey;

    // Hit Indicators
    GameObject leftTarget;
    GameObject rightTarget;
    GameObject middleTarget;

    // For reading charts
    char[] buffer;
    int chartLength = 0;
    // For notes and movement
    public Note note;
    Vector3 right = new Vector3(4.515f, 6, -1);
    Vector3 left = new Vector3(-4.515f, 6, -1);
    Vector3 middle = new Vector3(0, 6, -1);
    // To hold creation time of notes. Compare creation to current (against beat) to calculate accuracy.
    public Queue<Note> leftNoteQ = new Queue<Note>();
    public Queue<Note> rightNoteQ = new Queue<Note>();
    public Queue<Note> middleNoteQ = new Queue<Note>();

    // Click sound
    AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    { 
        leftTarget = GameObject.Find("Left");
        rightTarget = GameObject.Find("Right");
        middleTarget = GameObject.Find("Middle");
        audioSource = GetComponent<AudioSource>();
        using (StreamReader sr = new StreamReader(@"F:\repos\unity2d\RhythmAndTime\Assets\Charts\test.txt"))
        {
            chartLength = (int)sr.BaseStream.Length;
            buffer = new char[chartLength];
            sr.Read(buffer, 0, chartLength);
        }
        startTime = music.musicSource.time;
        lastTime = startTime;
        beat = 60 / beat;
        beat /= subDiv;
    }

    // Update is called once per frame
    void Update()
    {
        curTime = music.musicSource.time;
        if (curTime - lastTime >= beat)
        {
            //audioSource.Play();

            if (count == chartLength) { count = 0; }
            char chartCount = buffer[count];
            counterText.text = chartCount.ToString();
            if ((chartCount - 48) == 0)
            {
                CreateLeftNote(curTime);
            }
            if ((chartCount - 48) == 1)
            {
                CreateRightNote(curTime);
            }
            if ((chartCount - 48) == 2)
            {
                CreateMiddleNote(curTime);
            }

            lastTime += beat;
            count++;
        }

        // Middle Input ("M")
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("Middle Hit: " + music.musicSource.time);
            timingText.text = CheckTiming(middleNoteQ);
        }

        // Left Input ("L")
        if (Input.GetKeyDown(KeyCode.F))
        {
            //Debug.Log("Left Hit: " + music.musicSource.time);
            timingText.text = CheckTiming(leftNoteQ);
        }

        // Right Input ("R")
        if (Input.GetKeyDown(KeyCode.J))
        {
            //Debug.Log("Right Hit: " + music.musicSource.time);
            timingText.text = CheckTiming(rightNoteQ);
        }
    }

    /// TODO: change lstTime variable name
    // Gets current time of button pressed and lastTime var from Update.
    // Calculates difference and compares it to target timing
    string CheckTiming(Queue<Note> noteQ)
    {
        if (noteQ.Count != 0)
        {
            float target = noteQ.Peek().GetTime() + beat;
            //Debug.Log("Target:" + target);
            float hit = music.musicSource.time;

            if (hit < target + perfectTiming && hit > target - perfectTiming)
            {
                //display perfect
                noteQ.Peek().DestroyNote();
                return "Perfect!";
            }
            else if (hit < target + goodTiming && hit > target - goodTiming)
            {
                // good
                noteQ.Peek().DestroyNote();
                return "Good";
            }
            else if (hit < target + okTiming && hit > target - okTiming)
            {
                //ok
                noteQ.Peek().DestroyNote();
                return "Okay";
            }
        }
        return "";
    }

    // TODO: why does beat/2 cause notes to sync up better than beat?
    void CreateLeftNote(float time)
    {
        Note thisNote = Instantiate<Note>(note, left, Quaternion.identity);
        leftNoteQ.Enqueue(thisNote);
        float distanceToHit = thisNote.transform.position.y - leftTarget.transform.position.y;
        float scrollTime = distanceToHit / (beat);
        StartCoroutine(ScrollNote(thisNote, time, scrollTime, leftNoteQ));
    }

    void CreateRightNote(float time)
    {
        Note thisNote = Instantiate<Note>(note, right, Quaternion.identity);
        rightNoteQ.Enqueue(thisNote);
        float distanceToHit = thisNote.transform.position.y - leftTarget.transform.position.y;
        float scrollTime = distanceToHit / (beat);
        StartCoroutine(ScrollNote(thisNote, time, scrollTime, rightNoteQ));
    }

    void CreateMiddleNote(float time)
    {
        Note thisNote = Instantiate<Note>(note, middle, Quaternion.identity);
        middleNoteQ.Enqueue(thisNote);
        float distanceToHit = thisNote.transform.position.y - leftTarget.transform.position.y;
        float scrollTime = distanceToHit / (beat);
        StartCoroutine(ScrollNote(thisNote, time, scrollTime, middleNoteQ));
    }

    IEnumerator ScrollNote(Note thisNote, float time, float scrollTime, Queue<Note>noteQ)
    {
        Vector3 thisPos = thisNote.transform.position;
        while (thisNote != null)
        {
            if (thisNote!=null)
            {
                thisPos.y -= Time.smoothDeltaTime * scrollTime;
                thisNote.transform.position = thisPos;
                if (thisNote.transform.position.y < middleTarget.transform.position.y - 1)
                {
                    timingText.text = "Miss...";
                }
                yield return null;
            }
        }
        noteQ.Dequeue();
        yield return null;
    }
}
