using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhisperTrigger : MonoBehaviour
{
    private AudioSource audioSource;
    public float triggerRadius = 7f;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;
        audioSource.loop = true;

        AudioReverbZone reverb = gameObject.AddComponent<AudioReverbZone>();
        reverb.reverbPreset = AudioReverbPreset.Cave;
        reverb.minDistance = 1;
        reverb.maxDistance = 10;
    }

    public void SetClip(AudioClip clip)
    {
        audioSource.clip = clip;
    }

    void Update()
    {
        if (player == null || audioSource.clip == null) return;

        float dist = Vector3.Distance(player.position, transform.position);
        if (dist < triggerRadius && !audioSource.isPlaying)
            audioSource.Play();
        else if (dist >= triggerRadius && audioSource.isPlaying)
            audioSource.Stop();
    }
}
