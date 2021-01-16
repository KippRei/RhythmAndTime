using System;
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
    public float speedMod = 1; // How many beats it takes for note to scroll from instantiation point to target
    [SerializeField] float visualOffset = 0; // To subtly tweek where note lands on target (TODO: ISSUE: if this is too negative the scrollSpeed is negative and the track doesn't scroll at all! 
    [SerializeField] float musicOffset = 0;
    [SerializeField] float playerOffset = 0; // Allows subtle adjustment of timing for note hits

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
    bool rightIsDown = false;
    bool leftIsDown = false;
    bool keyReleased = true;

    // Hit Indicators
    public GameObject leftTarget;
    public GameObject rightTarget;
    public GameObject middleTarget;

    // Player
    GameObject player;

    // For reading charts
    Queue<string> beatChart = new Queue<string>(); // Parse each line from sr buffer into beatChart as a string for easier chart interpretation
    int chartLength = 0;
    int count = 0;
    bool endOfMeasure = true;
    int subDiv = 4;

    // For notes
    public Note note;
    public MNote mNote;
    public BNote bNote;
    public Vector3 right;
    public Vector3 left;
    public Vector3 middle;
    public Vector3 beatLocation;
    float distToTarget = 6f;

    // For note track
    GameObject track;
    float scrollSpeed;
    Vector3 trackPos;

    // To hold creation time of notes. Compare creation to current (against beat) to calculate accuracy.
    public Queue<Note> leftNoteQ = new Queue<Note>();
    public Queue<Note> rightNoteQ = new Queue<Note>();
    public Queue<Note> middleNoteQ = new Queue<Note>();
    public Queue<MNote> leftMNoteQ = new Queue<MNote>();
    public Queue<MNote> rightMNoteQ = new Queue<MNote>();
    public Queue<MNote> middleMNoteQ = new Queue<MNote>();
    public Queue<Note> beatNoteQ = new Queue<Note>();

    // Click sound
    AudioSource audioSource;
    public AudioClip tap;

    // Start is called before the first frame update
    void Start()
    {
        distToTarget *= speedMod;
        right = new Vector3(4.515f, distToTarget, -1);
        left = new Vector3(-4.515f, distToTarget, -1);
        middle = new Vector3(0, distToTarget, -1);
        beatLocation = new Vector3(.5f, distToTarget, 0);
        player = GameObject.Find("Player");
        music = GameObject.Find("Music").GetComponent<Music>();
        track = GameObject.Find("Track");
        leftTarget = GameObject.Find("Left");
        rightTarget = GameObject.Find("Right");
        middleTarget = GameObject.Find("Middle");
        audioSource = GetComponent<AudioSource>();
        using (StreamReader sr = new StreamReader(@"F:\GitRepos\RhythmAndTime\Assets\Charts\test.txt"))
        {
            chartLength = (int)sr.BaseStream.Length;
            while (sr.Peek() >= 0)
            {
                beatChart.Enqueue(sr.ReadLine().ToString());
            }
        }
        startTime = music.songPos;
        lastTime = startTime + musicOffset;
        beat = 60 / beat;
        scrollSpeed = (middle.y - middleTarget.transform.position.y + visualOffset) / (beat * 4); // TODO: Fix: hard-coded 4 (notes take 4 beats to scroll down screen) for testing
        subDiv = GetSubDiv() / 4; // TODO: Fix this: GetSubDiv() returns how many notes in measure not how many notes per beat
    }

    // Update is called once per frame
    void Update()
    {
        MoveTrack();
        SpawnNotes();
        CheckInput();
    }

    private void CheckInput()
    {        
        // Beat Input ("B")
        if (Input.GetButtonDown("beat"))
        {            
            //StartCoroutine(TargetHitAnim(middleTarget));
            //Debug.Log("Middle Hit: " + (curTime + playerOffset));
            timingText.text = CheckTiming(beatNoteQ, curTime + playerOffset);
        }
        //
        if (keyReleased)
        {
            if (Input.GetButtonDown("left")) { leftIsDown = true; }
            if (Input.GetButtonDown("right")) { rightIsDown = true; }
        }

        if (leftIsDown)
        {
            keyReleased = false;
            // Left Input ("L")
            if (Input.GetButtonDown("left"))
            {
                player.transform.position = leftTarget.transform.position;
                // StartCoroutine(TargetHitAnim(leftTarget));
                //Debug.Log("Left Hit: " + (curTime + playerOffset));
                timingText.text = CheckTiming(leftNoteQ, curTime + playerOffset);
            }

            if (Input.GetButton("left"))
            {
                player.transform.position = leftTarget.transform.position;
                //CheckMNotes(leftMNoteQ);
            }
            //

            //// Middle Input ("M")
            //if (Input.GetButtonDown("middle"))
            //{
            //    player.transform.position = middleTarget.transform.position;
            //    //StartCoroutine(TargetHitAnim(rightTarget));
            //    //Debug.Log("Right Hit: " + (curTime + playerOffset));
            //    timingText.text = CheckTiming(middleNoteQ, curTime + playerOffset);
            //}

            //if (Input.GetButton("middle"))
            //{
            //    player.transform.position = middleTarget.transform.position;
            //    //CheckMNotes(rightMNoteQ);
            //}
            ////

            // Right Input ("R")
            if (Input.GetButtonDown("right"))
            {
                player.transform.position = rightTarget.transform.position;
                //StartCoroutine(TargetHitAnim(rightTarget));
                //Debug.Log("Right Hit: " + (curTime + playerOffset));
                timingText.text = CheckTiming(rightNoteQ, curTime + playerOffset);
            }

            if (Input.GetButton("right"))
            {
                player.transform.position = rightTarget.transform.position;
                //CheckMNotes(rightMNoteQ);
            }
            //
        }
        else if (rightIsDown)
        {
            keyReleased = false;
            // Right Input ("R")
            if (Input.GetButtonDown("right"))
            {
                player.transform.position = rightTarget.transform.position;
                //StartCoroutine(TargetHitAnim(rightTarget));
                //Debug.Log("Right Hit: " + (curTime + playerOffset));
                timingText.text = CheckTiming(rightNoteQ, curTime + playerOffset);
            }

            if (Input.GetButton("right"))
            {
                player.transform.position = rightTarget.transform.position;
                //CheckMNotes(rightMNoteQ);
            }
            //

            // Left Input ("L")
            if (Input.GetButtonDown("left"))
            {
                player.transform.position = leftTarget.transform.position;
                // StartCoroutine(TargetHitAnim(leftTarget));
                //Debug.Log("Left Hit: " + (curTime + playerOffset));
                timingText.text = CheckTiming(leftNoteQ, curTime + playerOffset);
            }

            if (Input.GetButton("left"))
            {
                player.transform.position = leftTarget.transform.position;
                //CheckMNotes(leftMNoteQ);
            }
            //

            //// Middle Input ("M")
            //if (Input.GetButtonDown("middle"))
            //{
            //    player.transform.position = middleTarget.transform.position;
            //    //StartCoroutine(TargetHitAnim(rightTarget));
            //    //Debug.Log("Right Hit: " + (curTime + playerOffset));
            //    timingText.text = CheckTiming(middleNoteQ, curTime + playerOffset);
            //}

            //if (Input.GetButton("middle"))
            //{
            //    player.transform.position = middleTarget.transform.position;
            //    //CheckMNotes(rightMNoteQ);
            //}
            ////         
        }

        // TODO: Is there a better way to do this? (Allow holding either left or right and tapping other direction to override)
        if (Input.GetButtonUp("left") && !Input.GetButton("right") || Input.GetButtonUp("right") && !Input.GetButton("left"))
        {
            keyReleased = true;
            rightIsDown = false;
            leftIsDown = false;
            player.transform.position = middleTarget.transform.position;
        }
    }

    // Animates target bars (probably no longer need)
    //IEnumerator TargetHitAnim(GameObject target)
    //{
    //    Vector3 targetAnim = target.transform.localScale;
    //    Vector3 originalScale = targetAnim;
    //    targetAnim.x += .015f;
    //    targetAnim.y += .015f;
    //    target.transform.localScale = targetAnim;
    //    while (target.transform.localScale.x > originalScale.x && target.transform.localScale.y > originalScale.y)
    //    {
    //        targetAnim.x -= .005f;
    //        targetAnim.y -= .005f;
    //        target.transform.localScale = targetAnim;
    //        yield return new WaitForSeconds(.03f);
    //    }
    //    target.transform.localScale = originalScale;
    //    yield return null;
    //}

    private void SpawnNotes()
    {
        counterText.text = subDiv.ToString();
        curTime = music.songPos;

        if (curTime - lastTime >= beat / subDiv)
        {
            //audioSource.PlayOneShot(tap);
            lastTime += beat / subDiv;
            string thisBeat;

            if (beatChart.Peek().ToString() == ",")
            {
                beatChart.Dequeue(); // Remove "," and allow next note to play to keep in time
                subDiv = GetSubDiv() / 4; // Change subdivs on next beat
            }

            thisBeat = beatChart.Dequeue().ToString();
            // TODO: Change to elseif after ensuring one note per beat
            // TODO: Middle note... MNote only? (Tap)Note and MNote? 1 = MNote for middle at the moment
            if (thisBeat[0].ToString() == "1")
            {
                CreateLeftNote(lastTime);
            }
            //if (thisBeat[1].ToString() == "1")
            //{
            //    CreateMiddleNote(lastTime);
            //}
            if (thisBeat[2].ToString() == "1")
            {
                CreateRightNote(lastTime);
            }            
            if (thisBeat[0].ToString() == "2")
            {
                CreateLeftMNote(lastTime);
            }
            if (thisBeat[1].ToString() == "1")
            {
                CreateMiddleMNote(lastTime);
            }
            if (thisBeat[2].ToString() == "2")
            {
                CreateRightMNote(lastTime);
            }
            if (thisBeat[3].ToString() == "1")
            {
                CreateBeatNote();
            }
        }

        
    }

    private int GetSubDiv()
    {
        int subDiv = 0;
        foreach (string row in beatChart)
        {
            if (row == ",")
            {
                return subDiv;
            }
            subDiv++;
        }
        Debug.Log("GetSubDiv() Error!");
        return 0;
    }

    private void MoveTrack()
    {
        trackPos = track.transform.position;
        trackPos.y -= (Time.smoothDeltaTime * scrollSpeed);
        track.transform.position = trackPos;
    }

    // Calculates difference and compares it to target timing
    string CheckTiming(Queue<Note> noteQ, double hit)
    {
        if (noteQ.Count != 0)
        {
            double target = noteQ.Peek().GetTime();
            //Debug.Log("Target:" + target);

            if (hit > target - perfectTiming && hit <= target)
            {
                //display perfect
                noteQ.Peek().DestroyNote(noteQ);
                return "Perfect! (early)";
            }
            else if (hit < target + perfectTiming && hit > target)
            {
                //display perfect
                noteQ.Peek().DestroyNote(noteQ);
                return "Perfect! (late)";
            }
            else if (hit < target + goodTiming && hit > target - goodTiming)
            {
                // good
                noteQ.Peek().DestroyNote(noteQ);
                return "Good";
            }
            else if (hit < target + okTiming && hit > target - okTiming)
            {
                //ok
                noteQ.Peek().DestroyNote(noteQ);
                return "Okay";
            }
        }
        return "";
    }

    //void CheckMNotes(Queue<MNote> noteQ)
    //{
    //    //Debug.Log("Check mnotes");
    //    if (noteQ.Count != 0 && noteQ.Peek().GetTime() <= music.songPos + .01f && noteQ.Peek().GetTime() >= music.songPos -.01f)
    //    {
    //        noteQ.Peek().DestroyNote(noteQ);
    //        counterText.text = "match hit";            
    //    }
    //}

    #region Notes
    // Tap Notes
    void CreateLeftNote(double myTime)
    {
        Note thisNote = Instantiate<Note>(note, left, Quaternion.identity);
        leftNoteQ.Enqueue(thisNote);
        thisNote.transform.parent = track.transform;
    }

    void CreateRightNote(double myTime)
    {
        Note thisNote = Instantiate<Note>(note, right, Quaternion.identity);
        rightNoteQ.Enqueue(thisNote);
        thisNote.transform.parent = track.transform;
    }

    void CreateMiddleNote(double myTime)
    {
        Note thisNote = Instantiate<Note>(note, middle, Quaternion.identity);
        middleNoteQ.Enqueue(thisNote);
        thisNote.transform.parent = track.transform;
    }
    //

    // Match Notes
    void CreateLeftMNote(double myTime)
    {
        MNote thisNote = Instantiate<MNote>(mNote, left, Quaternion.identity);
        leftMNoteQ.Enqueue(thisNote);
        thisNote.transform.parent = track.transform;
    }

    void CreateRightMNote(double myTime)
    {
        MNote thisNote = Instantiate<MNote>(mNote, right, Quaternion.identity);
        rightMNoteQ.Enqueue(thisNote);
        thisNote.transform.parent = track.transform;
    }

    void CreateMiddleMNote(double myTime)
    {
        MNote thisNote = Instantiate<MNote>(mNote, middle, Quaternion.identity);
        middleMNoteQ.Enqueue(thisNote);
        thisNote.transform.parent = track.transform;
    }
    //

    // Beat Note
    private void CreateBeatNote()
    {
        BNote thisNote = Instantiate<BNote>(bNote, beatLocation, Quaternion.identity);
        beatNoteQ.Enqueue(thisNote);
        thisNote.transform.parent = track.transform;
    }
    #endregion
}
