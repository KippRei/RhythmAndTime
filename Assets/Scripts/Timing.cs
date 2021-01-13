using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Timing : MonoBehaviour
{
    [SerializeField] Text counterText = null;
    [SerializeField] Text timingText = null;

    // For BPM and sync
    [SerializeField] public float beat = 120f;
    [SerializeField] float subDiv = 1f;
    [SerializeField] float visualOffset = 0;
    [SerializeField] float musicOffset = 0;
    [SerializeField] float playerOffset = 0;

    // Song time used for timing
    Music music;
    double startTime = 0;
    public double curTime = 0;
    public double lastTime = 0;

    // Timing Windows
    float perfectTiming = .050f;
    float goodTiming = .067f;
    float okTiming = .084f;

    // Keys for Input 
    char inputKey;

    // Hit Indicators
    GameObject leftTarget;
    GameObject rightTarget;
    GameObject middleTarget;

    // For reading charts
    char[] buffer;
    int chartLength = 0;
    int count = 0;

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
    public AudioClip tap;

    // Start is called before the first frame update
    void Start()
    {
        music = GameObject.Find("Music").GetComponent<Music>();
        leftTarget = GameObject.Find("Left");
        rightTarget = GameObject.Find("Right");
        middleTarget = GameObject.Find("Middle");
        audioSource = GetComponent<AudioSource>();
        using (StreamReader sr = new StreamReader(@"F:\GitRepos\RhythmAndTime\Assets\Charts\test.txt"))
        {
            chartLength = (int)sr.BaseStream.Length;
            buffer = new char[chartLength];
            sr.Read(buffer, 0, chartLength);
        }
        startTime = music.songPos;
        lastTime = startTime + musicOffset;
        beat = 60 / beat;
    }

    // Update is called once per frame
    void Update()
    {
        curTime = music.songPos;
        if (curTime - lastTime >= beat)
        {
            //audioSource.PlayOneShot(tap);
            lastTime += beat;
            if (count == chartLength) { count = 0; }
            char chartCount = buffer[count];
            counterText.text = chartCount.ToString();
            if ((chartCount - 48) == 1)
            {
                CreateLeftNote(lastTime);
            }
            if ((chartCount - 48) == 2)
            {
                CreateMiddleNote(lastTime);
            }
            if ((chartCount - 48) == 3)
            {
                CreateRightNote(lastTime);
            }            
            count++;
        }

        // Middle Input ("M")
        if (Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.Play();
            Debug.Log("Middle Hit: " + (curTime + playerOffset));
            timingText.text = CheckTiming(middleNoteQ, curTime + playerOffset);
        }

        // Left Input ("L")
        if (Input.GetKeyDown(KeyCode.F))
        {
            audioSource.Play();
            Debug.Log("Left Hit: " + (curTime + playerOffset));
            timingText.text = CheckTiming(leftNoteQ, curTime + playerOffset);
        }

        // Right Input ("R")
        if (Input.GetKeyDown(KeyCode.J))
        {
            audioSource.Play();
            Debug.Log("Right Hit: " + (curTime + playerOffset));
            timingText.text = CheckTiming(rightNoteQ, curTime + playerOffset);
        }
    }

    // Calculates difference and compares it to target timing
    string CheckTiming(Queue<Note> noteQ, double hit)
    {
        if (noteQ.Count != 0)
        {
            double target = noteQ.Peek().GetTime();
            Debug.Log("Target:" + target);

            if (hit > target - perfectTiming && hit <= target)
            {
                //display perfect
                noteQ.Peek().DestroyNote();
                return "Perfect! (early)";
            }
            else if (hit < target + perfectTiming && hit > target)
            {
                //display perfect
                noteQ.Peek().DestroyNote();
                return "Perfect! (late)";
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

    void CreateLeftNote(double myTime)
    {
        Note thisNote = Instantiate<Note>(note, left, Quaternion.identity);
        leftNoteQ.Enqueue(thisNote);
        float distanceToHit = thisNote.transform.position.y - leftTarget.transform.position.y;
        float scroll = (distanceToHit + visualOffset) / (4 * beat);
        StartCoroutine(ScrollNote(thisNote, myTime, scroll, leftNoteQ));
    }

    void CreateRightNote(double myTime)
    {
        Note thisNote = Instantiate<Note>(note, right, Quaternion.identity);
        rightNoteQ.Enqueue(thisNote);
        float distanceToHit = thisNote.transform.position.y - rightTarget.transform.position.y;
        float scroll = (distanceToHit + visualOffset) / (4 * beat);
        StartCoroutine(ScrollNote(thisNote, myTime, scroll, rightNoteQ));
    }

    void CreateMiddleNote(double myTime)
    {
        Note thisNote = Instantiate<Note>(note, middle, Quaternion.identity);
        middleNoteQ.Enqueue(thisNote);
        float distanceToHit = thisNote.transform.position.y - middleTarget.transform.position.y;
        float scroll = (distanceToHit + visualOffset) / (4 * beat);
        StartCoroutine(ScrollNote(thisNote, myTime, scroll, middleNoteQ));
    }

    IEnumerator ScrollNote(Note thisNote, double noteTime, double myScroll, Queue<Note>noteQ)
    {
        Vector3 thisPos = thisNote.transform.position;
        double timeOffset = music.songPos - noteTime;
        thisPos.y -= (float)(timeOffset * myScroll);
        thisNote.transform.position = thisPos;
        while (thisNote != null)
        {
            if (thisNote!=null)
            {
                thisPos.y -= (float)(Time.smoothDeltaTime * myScroll);
                thisNote.transform.position = thisPos;
                /*if (thisNote.transform.position.y < middleTarget.transform.position.y - 1)
                {
                    //timingText.text = "Miss...";
                }*/
            }
            yield return null;
        }
        noteQ.Dequeue();
        yield return null;
    }

    void Ping()
    {
        audioSource.PlayOneShot(tap);
    }
}
