using UnityEngine;

[RequireComponent(typeof(Light))]
public class Sun : MonoBehaviour {
    public float RotationSpeed = 10f;
    public Color DayColor = Color.white;
    public Color NightColor = Color.blue;
    public float DayIntensity = 1f;
    public float NightIntensity = 0.2f;
    public float TransitionDuration = 2f;

    private Light m_directionalLight;

    private bool m_isDay = true;
    private float m_transitionTimer = 0f;

    private Quaternion m_targetRotation;
    private Color m_targetColor;
    private float m_targetIntensity;

    void Start() {
        m_directionalLight = GetComponent<Light>();
        m_targetRotation = transform.rotation;
        m_targetColor = m_directionalLight.color;
        m_targetIntensity = m_directionalLight.intensity;
    }

    void Update() {
        transform.Rotate(Vector3.right, RotationSpeed * Time.deltaTime);

        m_transitionTimer += Time.deltaTime;
        if (m_transitionTimer >= TransitionDuration) {
            m_isDay = !m_isDay;
            m_transitionTimer = 0f;
            m_targetRotation = m_isDay ? Quaternion.Euler(50, 0, 0) : Quaternion.Euler(-30, 0, 0);
            m_targetColor = m_isDay ? DayColor : NightColor;
            m_targetIntensity = m_isDay ? DayIntensity : NightIntensity;
        }

        if (m_directionalLight != null) {
            m_directionalLight.color = Color.Lerp(m_directionalLight.color, m_targetColor, Time.deltaTime / TransitionDuration);
            m_directionalLight.intensity = Mathf.Lerp(m_directionalLight.intensity, m_targetIntensity, Time.deltaTime / TransitionDuration);
            m_directionalLight.transform.rotation = Quaternion.Slerp(m_directionalLight.transform.rotation, m_targetRotation, Time.deltaTime / TransitionDuration);
        }
    }
}
