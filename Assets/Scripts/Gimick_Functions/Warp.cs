using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp : MonoBehaviour
{
    [SerializeField] Transform warpToPosition;

    [Tooltip("このオブジェクトに触れたtagがついているオブジェクトが飛ばされる")]
    [SerializeField] string warpTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(warpTag))
        {
            other.transform.position = warpToPosition.position;
        }
    }
}
