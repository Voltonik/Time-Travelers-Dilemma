using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {
    public float MouseSensitivity = 100f;

    private Transform m_playerBody;
    private float m_xRotation = 0f;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetPlayerTransform(Transform player) {
        m_playerBody = player;
    }

    private void LateUpdate() {
        if (m_playerBody == null) return;

        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;
        m_xRotation -= mouseY;
        m_xRotation = Mathf.Clamp(m_xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(m_xRotation, 0f, 0f);
    }
}
