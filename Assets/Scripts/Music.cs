using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioSource musicSource;

    double songStart;
    public double songPos;
    [SerializeField] float offset;


    // Start is called before the first frame update
    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        musicSource.Play();
        songStart = AudioSettings.dspTime;
    }

    private void Update()
    {
        songPos = AudioSettings.dspTime - songStart - offset;
    }
}
