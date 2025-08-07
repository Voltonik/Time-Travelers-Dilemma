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
    private float m_transitionTimer = 0f;


    void Start() {
        m_directionalLight = GetComponent<Light>();
    }

    void Update() {
        transform.Rotate(Vector3.right, RotationSpeed * Time.deltaTime);

        m_transitionTimer += Time.deltaTime;

        float t = Mathf.PingPong(m_transitionTimer / TransitionDuration, 1f);
        m_directionalLight.color = Color.Lerp(DayColor, NightColor, t);
        m_directionalLight.intensity = Mathf.Lerp(DayIntensity, NightIntensity, t);
    }
}
