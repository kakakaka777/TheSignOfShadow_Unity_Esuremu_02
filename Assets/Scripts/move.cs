using UnityEngine;

public class Move : MonoBehaviour
{
    [Header("移動速度")]
    public float moveSpeed = 5f;

    [Header("マウス感度")]
    public float mouseSensitivity = 150f;

    [Header("ジャンプ力")]
    public float jumpForce = 5f;

    private float yaw;   // 左右の回転
    private float pitch; // 上下の回転

    [Header("FPSカメラ（プレイヤーの子オブジェクト）")]
    public Transform cameraTransform;

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // -------------------------
        // マウス操作（視点回転）
        // -------------------------
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        transform.rotation = Quaternion.Euler(0, yaw, 0);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);

        // -------------------------
        // WASD移動
        // -------------------------
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);

        // -------------------------
        // マウスホイールでジャンプ
        // -------------------------
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    // 地面判定
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}

