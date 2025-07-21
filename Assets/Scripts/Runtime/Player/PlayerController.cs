using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    public CameraController CameraController;

    public float Speed = 5f;
    public float JumpForce = 5f;
    public float GroundCheckDistance = 0.2f;
    public LayerMask GroundMask;

    private Rigidbody m_rb;
    private bool m_isGrounded;
    private Vector3 m_moveInput;
    private float m_yaw;

    private void Start() {
        m_rb = GetComponent<Rigidbody>();
        m_rb.freezeRotation = true;

        if (CameraController != null) {
            CameraController.SetPlayerTransform(transform);
        } else {
            Debug.LogError("Player camera is not assigned");
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            if (TimeManager.Instance.CurrentState is RecordingState) {
                TimeManager.Instance.SetStateToType<PauseState>();
            } else if (TimeManager.Instance.CurrentState is PauseState) {
                TimeManager.Instance.SetStateToType<RecordingState>();
            }
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            TimeManager.Instance.SetStateToType<RewindState>();
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            TimeManager.Instance.SetStateToType<FastForwardState>();
        }
    }

    private void FixedUpdate() {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        m_moveInput = (transform.right * x + transform.forward * z).normalized;

        m_isGrounded = Physics.Raycast(transform.position, Vector3.down, GroundCheckDistance + 0.1f, GroundMask);

        if (Input.GetButtonDown("Jump") && m_isGrounded) {
            m_rb.linearVelocity = new Vector3(m_rb.linearVelocity.x, 0f, m_rb.linearVelocity.z);
            m_rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }

        Vector3 move = m_moveInput * Speed;
        Vector3 velocity = new Vector3(move.x, m_rb.linearVelocity.y, move.z);
        m_rb.linearVelocity = velocity;
    }

    private void LateUpdate() {
        float mouseX = Input.GetAxis("Mouse X") * (CameraController != null ? CameraController.MouseSensitivity : 100f) * Time.fixedDeltaTime;
        m_yaw += mouseX;
        transform.rotation = Quaternion.Euler(0f, m_yaw, 0f);
    }
}
