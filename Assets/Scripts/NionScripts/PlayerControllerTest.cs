using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerTest : MonoBehaviour
{
    [Header("プレイヤー移動")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;

    [Header("プレイヤーステータス")]
    public float maxHP = 100;
    public float currentHP = 0;
    public float dyingTimer = 3f;

    [Header("カメラ設定")]
    [SerializeField] Camera firstPersonCamera;
    [SerializeField] Camera thirdPersonCamera;
    [SerializeField] Transform thirdPersonFollowTarget;
    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] float thirdPersonDistance = 3f;

    [Space(20)]
    public GameObject bloodDrawingUI;
    public GameObject deadSymbol;
    public GameObject Player2;
    public GameObject Minigame;


    private Rigidbody rb;

    private float xRotation = 0f;

    private bool isGrounded = true;
    private bool isFirstPerson = true;
    private bool isDying = false;

    public int respawnCount = 0; // ★追加：リスポーン回数
    private Vector3 spawnPoint; // ★追加：初期位置を保存しておくための変数

    void Start()
    {
        //プレイヤーステータス初期化
        currentHP = maxHP;

        //コンポーネント関連
        rb = GetComponent<Rigidbody>();

        //カメラ関連
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SwitchCameraView(true);

    }

    void Update()
    {
        PlayerMoveMent();


        //カメラ関連
        HandleViewSwitch();
        HandleMouseLook();
        UpdateThirdPersonCamera();

        // ★追加：Eキーで危険フラグを設置
        if (Input.GetKeyDown(KeyCode.E) && !isDying)
        {
            PlaceDangerFlag();
        }

        if (isDying)
        {
            dyingTimer -= Time.deltaTime;
            if (dyingTimer <= 0f)
            {
                Die();
            }
            return;
        }
    }

    void PlayerMoveMent()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v);
        move = Camera.main.transform.TransformDirection(move);
        move.y = 0;
        rb.MovePosition(transform.position + move.normalized * moveSpeed * Time.deltaTime);
        Debug.Log("HERE");
        //ジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        }
    }

    void HandleViewSwitch()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFirstPerson = !isFirstPerson;
            SwitchCameraView(isFirstPerson);
        }
    }

    void SwitchCameraView(bool firstPerson)
    {
        firstPersonCamera.enabled = firstPerson;
        thirdPersonCamera.enabled = !firstPerson;
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        if (isFirstPerson)
        {
            firstPersonCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        else
        {
            thirdPersonCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        transform.Rotate(Vector3.up * mouseX);
    }

    void UpdateThirdPersonCamera()
    {
        if (!isFirstPerson && thirdPersonCamera != null && thirdPersonFollowTarget != null)
        {
            Vector3 desiredPosition = thirdPersonFollowTarget.position - thirdPersonFollowTarget.forward * thirdPersonDistance + Vector3.up * 2f;
            thirdPersonCamera.transform.position = desiredPosition;
            thirdPersonCamera.transform.LookAt(thirdPersonFollowTarget.position + Vector3.up * 1.5f);
        }
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damage") && !isDying)
        {
            EnterDyingState();
        }
    }

    void EnterDyingState()
    {
        isDying = true;
        Debug.Log("プレイヤーは死にそうだぜ(>_<)");
        bloodDrawingUI.SetActive(true); //血で描くUIなど表示
    }

    void Die()
    {
        Debug.Log("プレイヤーは死んだぜ(>_<)");
        isDying = false;
        bloodDrawingUI.SetActive(false);

        // ↓ 旧コード：プレイヤーを非アクティブにしていた
        // Instantiate(deadSymbol, transform.position, Quaternion.identity);
        // gameObject.SetActive(false);

        // ↓★変更：死亡位置にマーカーを置き、位置をスタート地点に戻す
        Instantiate(deadSymbol, transform.position, Quaternion.identity); // ← これはそのまま

        transform.position = spawnPoint; // ★追加：スタート地点へ移動
        currentHP = maxHP; // ★追加：HPを回復
        dyingTimer = 3f;   // ★追加：タイマー初期化
                           // ★追加：リスポーン回数をカウント
        respawnCount++;
        Debug.Log("リスポーン回数: " + respawnCount);
    }


    void OnCollisionStay(Collision other)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision other)
    {
        isGrounded = false;
    }

    [SerializeField] GameObject dangerFlagPrefab; // インスペクターでPrefabを割り当てる

    void PlaceDangerFlag() // ★追加
    {
        Vector3 placePosition = transform.position + transform.forward * 2f;
        GameObject flag = Instantiate(dangerFlagPrefab, placePosition, Quaternion.identity);

        // ★Debugログを出力
        Debug.Log("DangerFlagを設置しました at: " + placePosition);

        DangerFlag danger = flag.GetComponent<DangerFlag>();
        if (danger != null)
        {
            danger.SetOwner(gameObject);
        }
    }

    public int GetRespawnCount()
    {
        return respawnCount;
    }

}
