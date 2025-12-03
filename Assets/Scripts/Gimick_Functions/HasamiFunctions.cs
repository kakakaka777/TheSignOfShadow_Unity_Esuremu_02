using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasamiFunctions : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform Lside;
    [SerializeField] Transform Rside;
    [SerializeField] float closeDistance = 3f;
    [SerializeField] float rotationSpeed = 200f;
    [SerializeField] float reopenDelay = 2f;
    [SerializeField] float openRotateZ = 45f;


    private bool isClosed = false;
    private bool isReopening = false;
    private bool isPlayerEnter = false;


    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        Debug.Log("パックン罠とプレイヤーの距離： " + distance);

        if (distance > closeDistance && !isClosed && isPlayerEnter)
        {
            // トラップ閉じる処理
            StopAllCoroutines();
            StartCoroutine(CloseTrap());
        }
        else if (distance <= closeDistance && isClosed && !isReopening)
        {
            // プレイヤーが近くに戻った時、再び開く
            StartCoroutine(ReopenTrapAfterDelay());
        }
    }

    IEnumerator CloseTrap()
    {
        isClosed = true;

        while (Mathf.Abs(Lside.localEulerAngles.z) > 1f || Mathf.Abs(Rside.localEulerAngles.z) > 1f)
        {
            float newZ_L = Mathf.MoveTowardsAngle(Lside.localEulerAngles.z, 0f, rotationSpeed * Time.deltaTime);
            float newZ_R = Mathf.MoveTowardsAngle(Rside.localEulerAngles.z, 0f, rotationSpeed * Time.deltaTime);

            Lside.localEulerAngles = new Vector3(Lside.localEulerAngles.x, Lside.localEulerAngles.y, newZ_L);
            Rside.localEulerAngles = new Vector3(Rside.localEulerAngles.x, Rside.localEulerAngles.y, newZ_R);

            yield return null;
        }
        Debug.Log("とりばさみが閉じたよ");
    }

    IEnumerator ReopenTrapAfterDelay()
    {
        isReopening = true;
        yield return new WaitForSeconds(reopenDelay);

        while (Mathf.Abs(Mathf.DeltaAngle(Lside.localEulerAngles.z, openRotateZ)) > 1f || Mathf.Abs(Mathf.DeltaAngle(Rside.localEulerAngles.z, openRotateZ)) > 1f)
        {
            float newZ_L = Mathf.MoveTowardsAngle(Lside.localEulerAngles.z, 45f, rotationSpeed * Time.deltaTime);
            float newZ_R = Mathf.MoveTowardsAngle(Rside.localEulerAngles.z, -45f, rotationSpeed * Time.deltaTime);

            Lside.localEulerAngles = new Vector3(Lside.localEulerAngles.x, Lside.localEulerAngles.y, newZ_L);
            Rside.localEulerAngles = new Vector3(Rside.localEulerAngles.x, Rside.localEulerAngles.y, newZ_R);

            yield return null;
        }

        isClosed = false;
        isReopening = false;
        Debug.Log("とりばさみが開いたよ");


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerEnter = true;
            Debug.Log("Playerに当たってるよ");
        }
        
    }

}

