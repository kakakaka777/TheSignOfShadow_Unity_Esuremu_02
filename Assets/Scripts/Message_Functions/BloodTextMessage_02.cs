using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BloodTextMessage_02 : MessageFunction
{
    [Header("Ray設定")]
    [Tooltip("血文字をかける最大距離")]
    [SerializeField]float maxDistance = 6f;

    [Header("Particle設定")]
    public GameObject BloodText_Effect;
    private ParticleSystem paintParticleSystem;
    [SerializeField] Color paintColor = new Color(0.7f, 0f, 0f, 1f);
    [Min(0.05f)] public float particleLifetime = 4f; // これで徐々に消える
    [Min(0f)] public float sizeMin = 0.07f;
    [Min(0f)] public float sizeMax = 0.12f;

    [Tooltip("血文字をかける最大数")]
    [SerializeField] int bloodTextNumberMax = 6;
    private int bloodTextNumber = 6;


    [SerializeField] float minScale = 0.05f;
    [SerializeField] float maxScale = 0.1f;

    
    public float emitInterval = 0.02f;     // 低いほど滑らか（負荷↑）


    

    private void Awake()
    {

        // パーティクルの寿命(フェード)を進めるため、システム自体は回し続ける
        if (paintParticleSystem != null) paintParticleSystem.Play(true);

        
    }

    private void Update()
    {
        if (canUse == false)
        {
            return;
        }

        // 左クリックされたら印を残す
        // 回数制限必要っぽいな
        if (Input.GetKey(KeyCode.Mouse0))
        {
            RaycastHit hit;
            

            bloodTextNumber += 1;
            if (bloodTextNumber <= bloodTextNumberMax)
            {
                if (PlayerManager.playerID == 0)
                {
                    Ray ray01 = player01_FPCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray01, out hit, maxDistance))
                    {
                        CreateBloodText(hit);
                    }
                }
                if (PlayerManager.playerID == 1)
                {
                    Ray ray02 = player02_FPCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray02, out hit, maxDistance))
                    {
                        CreateBloodText(hit);
                    }
                }
            }
            else if (bloodTextNumber >= bloodTextNumberMax)
            {
                bloodTextNumber = bloodTextNumberMax;
            }



            Debug.Log("LeaveScarできる回数: " + bloodTextNumber + "/" + bloodTextNumberMax);

            Debug.Log("PlayerID: " + PlayerManager.playerID);
        }

        //public override void OnActivate(Vector3 playerPosition)
        //{

        //    Debug.Log("BloodText（メッセージを残す機能)を実行する");

        //    // “この能力を使える状態”にするだけ（クリックで描く）
        //    isActive = true;
        //    hasLastPos = false;
        //}
    }
    public override void OnActivate(Vector3 playerPosition)
    {
        Debug.Log("BloodText（メッセージを残す機能)を実行する");


        // メッセージUIなどから発動されたときの処理（例：周囲に印）
        RaycastHit hit;
        Vector3 rayOrigin = playerPosition + Vector3.up * 1f;
        Vector3 rayDir = transform.forward;

        bloodTextNumber = 0;

        SetCurso();

        if (Physics.Raycast(rayOrigin, rayDir, out hit, maxDistance))
        {
            CreateBloodText(hit);
        }
    }

    // 必要なら外部から呼ぶ（UIボタン等）
    

    //private void EmitPaint(RaycastHit hit)
    //{
    //    Vector3 pos = hit.point + hit.normal * surfaceOffset;

    //    // 板(Quad)の表向き(+Z)が法線方向を向く想定
    //    Quaternion rot = Quaternion.LookRotation(hit.normal);
    //    rot = Quaternion.AngleAxis(Random.Range(0f, 360f), hit.normal) * rot;

    //    var emit = new ParticleSystem.EmitParams
    //    {
    //        position = pos,
    //        velocity = Vector3.zero,
    //        startSize = Random.Range(sizeMin, sizeMax),
    //        startLifetime = particleLifetime,
    //        startColor = paintColor,

    //        // ParticleSystemのrotationはラジアン
    //        rotation3D = rot.eulerAngles * Mathf.Deg2Rad
    //    };

    //    paintParticleSystem.Emit(emit, 1);
    //}

    void CreateBloodText(RaycastHit hit)
    {
        // 表面に正対する回転
        Quaternion baseRot = Quaternion.LookRotation(hit.normal);

        // 法線方向（前方向）に対してランダムにひねる
        Quaternion randomRoll = Quaternion.Euler(0f, 0f, Random.Range(0f, 15f));


        GameObject scratch = Instantiate(
            BloodText_Effect,
            hit.point + hit.normal * 0.01f,
            baseRot * randomRoll  // ランダム回転を合成
        );

        // スケールをランダムに（元のスケールを基準に倍率をかける）
        float scale = Random.Range(minScale, maxScale);
        scratch.transform.localScale *= scale;

        scratch.transform.SetParent(hit.collider.transform);
    }
}
