using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("プレイヤー移動")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float deadMaxMoveDistance = 3f;
    [SerializeField] float maxJumpCharge = 8f; // 最大ジャンプ力
    [SerializeField] float jumpChargeRate = 10f; // ジャンプ力のチャージ速度


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
    public GameObject nextPlayer;
    public GameObject Minigame;
    [SerializeField] GameObject ghostBoundaryVisual;
    [SerializeField] GameObject messageSelectUI;
    [SerializeField] GameObject doorPrefab;         // 配置したいDoorのプレハブ
    [SerializeField] GameObject playerMessageCicleUI;

    [SerializeField] int doorCount = 1;


    [SerializeField] CircularMessageSelector circularMessageSelector;


    private Rigidbody rb;
    private PlayerController playerController;
    public GameObject spawnedGhost; // 生成された死体の参照

    private float xRotation = 0f;

    private Vector3 deathPosition; //死んだ位置を保存する

    private float currentJumpCharge;
    //[SerializeField] float jumpFoce = 3f;

    private bool isChargingJump = false;

    private bool isGrounded = true;
    private bool isFirstPerson = true;
    private bool isDying = false;


    void Start()
    {
        circularMessageSelector.playerCenter = playerMessageCicleUI.transform;

        //プレイヤーステータス初期化
        currentHP = maxHP;

        bloodDrawingUI.SetActive(false);
        Minigame.SetActive(false);

        //コンポーネント関連
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();

        //カメラ関連
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SwitchCameraView(true);

    }
    void OnEnable()
    {
        Debug.Log("SetActiveされたよ");
        if (Minigame == null)
        {
            Debug.LogWarning("Minigame が設定されていません！（nullです）");
        }
        else
        {
            Minigame.SetActive(false);
            bloodDrawingUI.SetActive(false);

            Debug.Log("Minigame を非表示にしたよ");
        }

    }
    void Update()
    {
        

        PlayerMoveMent();

        //カメラ関連
        HandleViewSwitch();
        HandleMouseLook();
        UpdateThirdPersonCamera();

        if (isDying == true)
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
        Vector3 move = Vector3.zero;


        if (Input.GetKey(KeyCode.W)) move += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) move += Vector3.back;
        if (Input.GetKey(KeyCode.A)) move += Vector3.left;
        if (Input.GetKey(KeyCode.D)) move += Vector3.right;

        
        move = Camera.main.transform.TransformDirection(move);
        move.y = 0;

        Vector3 nextPos = transform.position + move.normalized * moveSpeed * Time.deltaTime;


        if (isDying && Vector3.Distance(deathPosition, nextPos) > deadMaxMoveDistance)
        {
            return; // 範囲外なら動かない
        }

        rb.MovePosition(nextPos);


        //ジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isChargingJump = true;
            currentJumpCharge = jumpForce;

        }

        if (Input.GetKey(KeyCode.Space) && isChargingJump)
        {
            currentJumpCharge += jumpChargeRate * Time.deltaTime;
            currentJumpCharge = Mathf.Clamp(currentJumpCharge, 0f, maxJumpCharge);
        }

        // Spaceキーを離したときにジャンプ
        if (Input.GetKeyUp(KeyCode.Space) && isChargingJump && isGrounded)
        {
            isChargingJump = false;
            rb.velocity = new Vector3(rb.velocity.x, currentJumpCharge, rb.velocity.z);
            currentJumpCharge = jumpForce;
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
        deathPosition = transform.position;
        bloodDrawingUI.SetActive(true); //血で描くUIなど表示

        if (ghostBoundaryVisual != null)
        {
            GameObject ghostCircle = Instantiate(ghostBoundaryVisual, deathPosition + Vector3.up * 0.05f, Quaternion.identity);
            ghostCircle.GetComponent<CircleDrawer>().radius = deadMaxMoveDistance;
        }


    }


    void Die()
    {
        Debug.Log("プレイヤーは死んだぜ(>_<)");
        isDying = false;
        // 円周上にDoorを生成
        if (doorPrefab != null)
        {
            for (int i = 0; i < doorCount; i++)
            {
                // 円周角度を計算（ラジアン）
                float angle = i * Mathf.PI * 2f / doorCount;

                // XY平面で配置（Yは地面、XZで円周）
                Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * deadMaxMoveDistance;
                Vector3 spawnPos = deathPosition + offset + Vector3.up * 0.5f; // 少し浮かせる

                Quaternion rot = Quaternion.LookRotation(-offset.normalized); // 中心向きに回転

                Instantiate(doorPrefab, spawnPos, rot);
            }
        }

        bloodDrawingUI.SetActive(false);

        //死亡位置にゴースト生成
        Instantiate(deadSymbol, transform.position + new Vector3(0, -1, 0), Quaternion.Euler(90f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
        Debug.Log("死体をスポーン");


        messageSelectUI.SetActive(true);
        ////プレイヤー非表示に
        //gameObject.SetActive(false);

        //Minigame.SetActive(true);

        //if (nextPlayer != null)
        //{
        //    nextPlayer.SetActive(true);
        //    Debug.Log("次のプレイヤーに切り替え");
        //}

        //playerController.enabled = false;

    }


    void OnCollisionStay(Collision other)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision other)
    {
        isGrounded = false;
    }

}
