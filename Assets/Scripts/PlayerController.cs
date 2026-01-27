using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("プレイヤー移動")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float acceleration = 50f; // 加速度（キビキビ動くか、慣性がつくか）
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float deadMaxMoveDistance = 3f;
    [SerializeField] float maxJumpCharge = 8f; // 最大ジャンプ力
    [SerializeField] float jumpChargeRate = 10f; // ジャンプ力のチャージ速度
    [SerializeField] float gravityScale = 1.5f; // 通常時の重力倍率
    [SerializeField] float fallGravityMult = 2.5f; // 落下時の重力倍率（ここを上げるとキビキビする）

    [Header("プレイヤーステータス")]
    [SerializeField] GameObject Player1;
    [SerializeField] GameObject Player2;
    public float maxHP = 100;
    public float currentHP = 0;
    public float dyingTimer = 3f;

    [Header("接地判定")]
    [SerializeField] LayerMask groundLayer; // 地面として認識するレイヤー
    [SerializeField] float groundCheckRadius = 0.3f; // 足元の判定範囲
    [SerializeField] float groundCheckOffset = 0.1f; // 足元からどれくらい下を調べるか

    [Header("カメラ設定")]
    [SerializeField] Camera firstPersonCamera;
    [SerializeField] Camera thirdPersonCamera;
    [SerializeField] Transform thirdPersonFollowTarget;
    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] float thirdPersonDistance = 3f;

    [Header("カメラ設定")]
    [SerializeField] GameObject playerChange_Ui;


    [Space(20)]
    public GameObject bloodDrawingUI;
    public GameObject deadSymbol;
    public GameObject nextPlayer;
    public GameObject Minigame;
    public Transform startPoint;
    [SerializeField] GameObject ghostBoundaryVisual;
    [SerializeField] GameObject messageSelectUI;
    [SerializeField] GameObject doorPrefab;         // 配置したいDoorのプレハブ
    [SerializeField] GameObject playerMessageCicleUI;
    [SerializeField] GameObject WinUI;
    [SerializeField] GameObject DefeatUI;
    [SerializeField] GameObject ZankitUI;



    [SerializeField] int doorCount = 1;
    [SerializeField] GameObject timeLimit;

    [SerializeField] CircularMessageSelector circularMessageSelector;
    [SerializeField] DeathSpawnController deathSpawnController;


    private Rigidbody rb;
    private PlayerController playerController;
    public GameObject spawnedGhost; // 生成された死体の参照

    [SerializeField] Transform goalPoint;
    [SerializeField] GameObject cursorIcon;


    [Header("切り替えUIフェード設定")]
    [Min(0f)] public float fadeDuration = 4f;   // 透明になるまでの秒数
    [Min(0f)] public float startDelay = 0f;     // フェード開始までの待ち時間（不要なら0）

    private float xRotation = 0f;

    private Vector3 deathPosition; //死んだ位置を保存する
    private Vector3 moveInput; // Updateで受け取った入力

    private float currentJumpCharge;
    //[SerializeField] float jumpFoce = 3f;

    private bool isChargingJump = false;
    
    private bool isGrounded = true;
    private bool isFirstPerson = true;
    private bool isDying = false;
    private bool isDead = false;
    private bool isCameraOn = true;
    private bool isCanMove = true;
    private bool isGameOver = false;

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;
    [SerializeField] MessageFunction[] messageFunctions; //ドアを通ったあと、メッセージ機能を封印したいため

    [Header("フェード設定")]
    [Min(0.01f)]

    [SerializeField] bool playOnEnable = true;   // 有効化されたら自動でフェード開始
    [SerializeField] bool useUnscaledTime = true; // ポーズ中でも進めたいならON

    [Header("フェード完了後")]
    [SerializeField] bool disableAfterFade = true; // フェード後に親を非アクティブ
    [SerializeField] bool destroyAfterFade = false; // フェード後に破棄（disableより優先）

    private Collider playerCollider;
    
    private GameObject spawnedObject;
    private void Awake()
    {
        canvasGroup = playerChange_Ui.GetComponent<CanvasGroup>();
        playerCollider = GetComponent<BoxCollider>();

    }

    void Start()
    {
       
        // たまにデバッグ用にコメントアウトするかもだから、終わったらなおすんだぜYou
        //this.gameObject.transform.position = startPoint.position;
        

        isGameOver = false;

        WinUI.SetActive(false);
        DefeatUI.SetActive(false);


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

        

        FadeOut();

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

        circularMessageSelector.playerCenter = playerMessageCicleUI.transform;
        PlayerManager.playTransform = this.transform;

        ZankitUI.SetActive(true);

        
        Debug.Log("isDamageOnlyOnce : " + PlayerManager.isDamageOnlyOnce);

        if (messageFunctions != null)
        {
            foreach (var mf in messageFunctions)
            {
                if (mf != null)
                {
                    mf.canUse = false;   // ← ここで封印
                }
            }
        }

        DefeatUI.SetActive(false);
        WinUI.SetActive(false);

        
    }
    void Update()
    {
        Debug.Log("isDamageOnlyOnce" + PlayerManager.isDamageOnlyOnce);

        

        if (isCanMove == true)
        {
            PlayerMoveMent();
        }
       

        //カメラ関連
        if (isCameraOn == true)
        {
            HandleViewSwitch();
            HandleMouseLook();
            UpdateThirdPersonCamera();
        }
        

        if (isDying == true)
        {
            dyingTimer -= Time.deltaTime;
            if (dyingTimer <= 0f)
            {
                Die();
            }
            return;
        }

      
        
        if (PlayerManager.deathNumber == 0)
        {
            GameOver();
        }


        PlayerRespawnAndChangeByButton();

    }

    // 物理挙動はFixedUpdateで行う
    void FixedUpdate()
    {
        if (isCanMove)
        {
            CheckGround(); // 接地判定
            ApplyMovement(); // 移動適用
            ApplyCustomGravity(); // 重力調整
        }
    }

    void PlayerMoveMent()
    {
        // 1. 入力ベクトルの取得と正規化
        float h = 0f;
        float v = 0f;
        if (Input.GetKey(KeyCode.W)) v += 1f;
        if (Input.GetKey(KeyCode.S)) v -= 1f;
        if (Input.GetKey(KeyCode.A)) h -= 1f;
        if (Input.GetKey(KeyCode.D)) h += 1f;

        // カメラの向きに合わせてベクトルを変換
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0; // 上下方向の影響を排除
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredMove = forward * v + right * h;

        // 【改善点①】ベクトルの正規化（斜め移動対策）
        if (desiredMove.magnitude > 1f)
        {
            desiredMove.Normalize();
        }

        moveInput = desiredMove;

        // ジャンプチャージ処理
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

        // ジャンプ実行
        if (Input.GetKeyUp(KeyCode.Space) && isChargingJump)
        {
            isChargingJump = false;
            if (isGrounded)
            {
                Jump(currentJumpCharge);
            }
            currentJumpCharge = jumpForce; // リセット
        }

    }

    // 物理移動（FixedUpdate内）
    void ApplyMovement()
    {
        // isDying時の制限
        if (isDying && deathPosition != Vector3.zero)
        {
            if (Vector3.Distance(deathPosition, transform.position + moveInput * moveSpeed * Time.fixedDeltaTime) > deadMaxMoveDistance)
            {
                moveInput = Vector3.zero; // 移動させない
            }
        }

        // 現在の速度（Y軸＝重力落下成分 は維持する）
        Vector3 targetVelocity = new Vector3(moveInput.x * moveSpeed, rb.velocity.y, moveInput.z * moveSpeed);

        // 速度を直接書き換えてキビキビ動かす（慣性を減らす）
        // ※もっと慣性をつけたい場合は Vector3.MoveTowards や Lerp を使う
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
    }

    // 【改善点②】カスタム重力（FixedUpdate内）
    void ApplyCustomGravity()
    {
        // 落下中は重力を強くする（キビキビさせるコツ）
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallGravityMult - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            // ジャンプ中にボタンを離したら、上昇を早めに止める（小ジャンプの制御）
            rb.velocity += Vector3.up * Physics.gravity.y * (gravityScale - 1) * Time.fixedDeltaTime;
        }
    }

    void Jump(float force)
    {
        // 既存のY速度をリセットしてからジャンプ力を加える（安定する）
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
    }

    // 【改善点③】接地判定（SphereCastを使用）
    void CheckGround()
    {
        // 足元から少し上の位置から、下に向かって球体を飛ばして地面があるか調べる
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        // SphereCast(原点, 半径, 方向, ヒット情報, 最大距離, レイヤー)
        // Physics.AllLayers だと自分自身に当たる可能性があるので、本来はLayerMask推奨
        // ここでは簡易的に Player以外とするか、Groundレイヤーを設定してください

        // ※注意：Ground Layerが設定されていない場合は全てに当たります
        isGrounded = Physics.SphereCast(origin, groundCheckRadius, Vector3.down, out RaycastHit hit, 0.5f + groundCheckOffset, groundLayer);

        // デバッグ用（シーンビューで赤い線が見えます）
        Debug.DrawRay(origin, Vector3.down * (0.5f + groundCheckOffset), isGrounded ? Color.green : Color.red);
    }

    // 12/16までで一旦こっちで
    void PlayerRespawnAndChangeByButton()
    {
        //if (PlayerManager.isRButtonUsed == true) return;

        if (PlayerManager.isMessageSelect == true)
        {
            

            if (Input.GetKeyDown(KeyCode.R))
            {
                PlayerManager.isDamageOnlyOnce = false;
                isDead = false;
                UiManager.isTimeCountDown = true;
                UiManager.isTimeCountStart = true;
                Debug.Log("isTimeCountDown : " + UiManager.isTimeCountDown);

                cursorIcon.SetActive(false);
                PlayerManager.onlyFadeOut = 2;
                // プレイヤー切り替え
                if (PlayerManager.playerID == 0)
                {
                    // 今のプレイヤーを非表示に
                    if (Player1 != null) Player1.SetActive(false);

                    // 次プレイヤーを表示
                    if (Player2 != null) Player2.SetActive(true);
                    Player2.transform.position = startPoint.position;

                    PlayerManager.playerID = 1;

                    

                    if (Player2 != null) playerChange_Ui.SetActive(true);
                   


                    PlayerManager.isMessageSelect = false;

                    PlayerManager.isRButtonUsed = true;
                    //if (nextPlayer != null) Biolea2r.SetActive(true);
                    //if (nextPlayer != null) Biolear.SetActive(true);

                }
                else if (PlayerManager.playerID == 1)
                {

                    // 今のプレイヤーを非表示に
                    if (Player2 != null) Player2.SetActive(false);

                    // 次プレイヤーを表示
                    if (Player1 != null) Player1.SetActive(true);
                    Player1.transform.position = startPoint.position;

                    PlayerManager.playerID = 0;

                    

                    if (Player1 != null) playerChange_Ui.SetActive(true);

                    PlayerManager.isMessageSelect = false;

                    PlayerManager.isRButtonUsed = true;


                    //if (nextPlayer != null) Biolea2r.SetActive(true);
                    //if (nextPlayer != null) Biolear.SetActive(true);
                }

            }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            Goal();
        }
    }

    //void ontriggerenter(collider other)
    //{
    //    if (other.comparetag("damage") && !isdying)
    //    {
    //        enterdyingstate();
    //    }
    //}



    //void EnterDyingState()
    //{
    //    isDying = true;
    //    Debug.Log("プレイヤーは死にそうだぜ(>_<)");
    //    deathPosition = transform.position;
    //    bloodDrawingUI.SetActive(true); //血で描くUIなど表示

    //    if (ghostBoundaryVisual != null)
    //    {
    //        GameObject ghostCircle = Instantiate(ghostBoundaryVisual, deathPosition + Vector3.up * 0.05f, Quaternion.identity);
    //        ghostCircle.GetComponent<CircleDrawer>().radius = deadMaxMoveDistance;
    //    }


    //}

    public void EnterDyingState()
    {
        if (PlayerManager.isDamageOnlyOnce == true) return;

        //isDying = true;
        Debug.Log("プレイヤーは死にそうだぜ(>_<)");
        deathPosition = transform.position;
        bloodDrawingUI.SetActive(true); //血で描くUIなど表示
        PlayerManager.isDamageOnlyOnce = true;
        //if (ghostBoundaryVisual != null)
        //{
        //    GameObject ghostCircle = Instantiate(ghostBoundaryVisual, deathPosition + Vector3.up * 0.05f, Quaternion.identity);
        //    ghostCircle.GetComponent<CircleDrawer>().radius = deadMaxMoveDistance;
        //}


    }

    public void Die()
    {
        Debug.Log("=== Die() 開始 ===");
        Debug.Log("isDead = " + isDead);
        Debug.Log("isDamageOnlyOnce = " + PlayerManager.isDamageOnlyOnce);
        Debug.Log("deathSpawnController = " + deathSpawnController);

        if (isDead)
        {
            Debug.Log("isDead で return");
            return;
        }
        isDead = true;

        

        //Debug.Log("Die() 呼ばれた: " + Time.frameCount + "フレーム目");
       

        deathSpawnController.Spawn();

        if (PlayerManager.isDamageOnlyOnce == true)
        {
            Debug.Log("isDamageOnlyOnce で return");
            return;
        }

        Debug.Log("プレイヤーは死んだぜ(>_<)");
        isDying = false;

        // 死亡時、カウントダウン
        UiManager.isTimeCountDown = false;

        PlayerManager.deathNumber -= 1;
        PlayerManager.isDamageOnlyOnce = true;
        
        // 扉生成
        
        Debug.Log("扉生成っぴ！！");
        /*
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
        */
        bloodDrawingUI.SetActive(false);

        //死亡位置にゴースト生成
        //Instantiate(deadSymbol, transform.position + new Vector3(0, -1, 0), Quaternion.Euler(90f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
        Debug.Log("死体をスポーン");


        messageSelectUI.SetActive(true);
        ////プレイヤー非表示に


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

    void Goal()
    {
        WinUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCanMove = false;
        isCameraOn = false;
        circularMessageSelector.enabled = false;

    }

    void GameOver()
    {
        if (isGameOver == true) return;

        DefeatUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCanMove = false;
        isCameraOn = false;
        circularMessageSelector.enabled = false;
        isGameOver = true;
    }

    public void FadeOut()
    {

        if (PlayerManager.onlyFadeOut == 2)
        {
            playerChange_Ui.SetActive(true);
            canvasGroup.alpha = 1;

            StartFade(targetAlpha: 0f);
        }
       

        
    }

    public void FadeIn()
    {
        StartFade(targetAlpha: 1f);
    }

    void StartFade(float targetAlpha)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            
        }
        fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float t = 0f;

        // フェード中にクリックできなくしたい場合（UIの入力を止める）
        //canvasGroup.blocksRaycasts = false;
        //canvasGroup.interactable = false;

        while (t < fadeDuration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float rate = Mathf.Clamp01(t / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, rate);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        
        
            // FadeInしたとき入力を戻したいならここでONにする
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        
    }
}
