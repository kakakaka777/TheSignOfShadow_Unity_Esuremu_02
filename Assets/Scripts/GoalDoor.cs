using UnityEngine;

public class GoalDoor : MonoBehaviour
{
    public Transform door;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool isOpened = false;

    void Update()
    {
        if (!isOpened && GameManager.Instance.hasKey)
        {
            isOpened = true;
            StartCoroutine(OpenDoor());
        }
    }

    System.Collections.IEnumerator OpenDoor()
    {
        Quaternion startRot = door.localRotation;
        Quaternion endRot = Quaternion.Euler(0, openAngle, 0);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * openSpeed;
            door.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
    }
}
