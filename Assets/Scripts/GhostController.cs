using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public float moveRange = 10f;
    public GameObject carveMarkPrefab;
    public GameObject objectToPlace;
    public GameObject whisperPrefab;

    private Vector3 deathPos;
    private bool usedMessage = false;

    void Start()
    {
        deathPos = transform.position;
    }

    void Update()
    {
        if (usedMessage) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v);
        Vector3 nextPos = transform.position + move * Time.deltaTime * 2f;

        if (Vector3.Distance(deathPos, nextPos) < moveRange)
            transform.position = nextPos;

        if (Input.GetKeyDown(KeyCode.Alpha1)) LeaveCarve();
        if (Input.GetKeyDown(KeyCode.Alpha2)) LeaveObject();
        if (Input.GetKeyDown(KeyCode.Alpha3)) LeaveWhisper();
    }

    void LeaveCarve()
    {
        Instantiate(carveMarkPrefab, transform.position + Vector3.forward, Quaternion.identity);
        usedMessage = true;
    }

    void LeaveObject()
    {
        Instantiate(objectToPlace, transform.position + Vector3.down * 0.5f, Quaternion.identity);
        usedMessage = true;
    }

    void LeaveWhisper()
    {
        GameObject whisper = Instantiate(whisperPrefab, transform.position, Quaternion.identity);
        whisper.GetComponent<WhisperTrigger>().SetClip(GetComponent<WhisperRecorder>().recordedClip);
        usedMessage = true;
    }
}
