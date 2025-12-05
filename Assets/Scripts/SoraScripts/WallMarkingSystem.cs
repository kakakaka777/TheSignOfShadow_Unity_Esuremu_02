using UnityEngine;

public class WallMarkingSystem : MonoBehaviour
{
    public Camera playerCamera;            // プレイヤーのカメラ
    public GameObject scratchDecalPrefab;  // 傷のPrefab（デカール）
    public float maxDistance = 5f;         // 傷をつけられる距離

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 左クリックで傷をつける
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.Log("当たっているぜ");
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                // 壁に傷をつける
                CreateScratchMark(hit);
            }
        }
    }

    void CreateScratchMark(RaycastHit hit)
    {
        // 傷マークを壁の表面に沿って貼り付け
        GameObject scratch = Instantiate(scratchDecalPrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));

        // ランダムに回転を加えて自然に見せる
        //scratch.transform.Rotate(Vector3.up, Random.Range(0f, 360f), Space.Self);

        // オプション：親を壁にすることで一緒に動くようにする
        scratch.transform.SetParent(hit.collider.transform);
    }
}

