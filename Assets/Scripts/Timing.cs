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
    // Need to fix Enemy so that multiple enemies can be detected (not one hard-coded)
    [SerializeField] Enemy enemy;
    [SerializeField] Player player;
    [Range(90.0f, 200.0f)] [SerializeField] float beat = 120f;
    [SerializeField] float musicOffset = 0;
    [SerializeField] float visualOffset = 0;
    int count = 0;
    float startTime = 0;
    float curTime = 0;
    float lastTime = 0;
    float perfectTiming = .024f;
    float goodTiming = .038f;
    float okTiming = .064f;
    bool keyIsPressed = false;
    char inputKey;
    
    // For testing note movement
    bool moveLR = false;
    // For reading charts
    char[] buffer;
    int chartLength = 0;
    // For notes and movement
    public GameObject note;
    Vector3 right = new Vector3(4.515f, 6, -1);
    Vector3 left = new Vector3(-4.515f, 6, -1);
    Vector3 middle = new Vector3(0, 6, -1);
    [SerializeField] float scrollSpeed = .01f;

    AudioSource audioSource;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {        
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
    }

    // Update is called once per frame
    void Update()
    {
        curTime = music.musicSource.time;
        if (curTime - lastTime + musicOffset >= beat)
        {
            //audioSource.Play();

            if (count > chartLength - 1) { count = 0; }
            char chartCount = buffer[count];
            counterText.text = chartCount.ToString();
            if ((chartCount - 48) % 2 == 0)
            {
                CreateLeftNote(curTime);
            }
            if ((chartCount - 48) % 2 == 1)
            {
                CreateRightNote(curTime);
            }

            /*if (moveLR == false)
            {
                notePos.x += 10;
                note1.gameObject.transform.position = notePos;
                moveLR = true;
                lastTime += beat;
                count++;
            }
            else
            {
                notePos.x -= 10;
                note1.gameObject.transform.position = notePos;
                moveLR = false;
                lastTime += beat;
                count++;
            }*/
            lastTime += beat;
            count++;
        }

        // Middle Input ("M")
        if (Input.GetKeyDown(KeyCode.Space) && keyIsPressed == false)
        {
            keyIsPressed = true;
            timingText.text = CheckTiming(curTime, lastTime);
            keyIsPressed = false;
        }

        // Left Input ("L")
        if (Input.GetKeyDown(KeyCode.F) && keyIsPressed == false)
        {
            keyIsPressed = true;
            timingText.text = CheckTiming(curTime, lastTime);
            keyIsPressed = false;
        }

        // Right Input ("R")
        if (Input.GetKeyDown(KeyCode.J) && keyIsPressed == false)
        {
            keyIsPressed = true;
            timingText.text = CheckTiming(curTime, lastTime);
            keyIsPressed = false;
        }
    }

    /// TODO: change lstTime variable name
    // Gets current time of button pressed and lastTime var from Update.
    // Calculates difference and compares it to target timing
    string CheckTiming(float pressed, float lstTime)
    {
        float hit = pressed - lstTime;

        if (hit < perfectTiming && hit > -perfectTiming)
        {
            //display perfect
            return "Perfect!";
        }
        else if (hit < goodTiming && hit > -goodTiming)
        {
            // good
            return "Good";
        }
        else if (hit < okTiming && hit > -okTiming)
        {
            //ok
            return "Okay";
        }
        else
        {
            // miss
            return "Miss...";
        }
    }

    void CreateLeftNote(float time)
    {
        GameObject thisNote = Instantiate(note, left, Quaternion.identity);
        StartCoroutine(ScrollNote(thisNote, time));
    }

    void CreateRightNote(float time)
    {
        GameObject thisNote = Instantiate(note, right, Quaternion.identity);
        StartCoroutine(ScrollNote(thisNote, time));
    }

    void CreateMiddleNote(float time)
    {
        GameObject thisNote = Instantiate(note, middle, Quaternion.identity);
        StartCoroutine(ScrollNote(thisNote, time));
    }

    IEnumerator ScrollNote(GameObject thisNote, float time)
    {
        Vector3 thisPos = thisNote.transform.position;
        while (thisNote != null)
        {
            if (time + visualOffset <= music.musicSource.time)
            {
                thisPos.y -= scrollSpeed;
                thisNote.transform.position = thisPos;
                yield return null;
            }
            yield return null;
        }
    }
}
