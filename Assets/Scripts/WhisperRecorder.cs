using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhisperRecorder : MonoBehaviour
{
    public AudioClip recordedClip;
    public int recordTime = 3;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void StartRecording()
    {
        recordedClip = Microphone.Start(null, false, recordTime, 44100);
    }

    public void StopRecording()
    {
        if (Microphone.IsRecording(null))
        {
            Microphone.End(null);
            audioSource.clip = recordedClip;
        }
    }

    public AudioClip GetClip() => recordedClip;
}
