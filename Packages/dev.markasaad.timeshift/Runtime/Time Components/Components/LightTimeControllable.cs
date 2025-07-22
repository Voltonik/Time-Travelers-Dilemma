using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightTimeControllable : MonoBehaviour, ITimeControllable {
    private struct LightSnapshot {
        public bool Enabled;
        public Color Color;
        public float Intensity;
        public float Range;
        public float SpotAngle;
    }

    private TimelineScrapper<LightSnapshot> m_timelines;
    private bool m_doneScraping = false;

    private Light m_light;

    private void Awake() {
        int bufferSize = TimeManager.Instance.SnapshotBufferSize;
        m_timelines = new TimelineScrapper<LightSnapshot>(bufferSize);
    }

    private void Start() {
        m_light = GetComponent<Light>();
    }

    private void OnEnable() {
        TimeManager.Instance.RegisterTimeControllable(this);
    }

    private void OnDisable() {
        if (TimeManager.Instance != null) {
            TimeManager.Instance.UnregisterTimeControllable(this);
        }
    }

    private bool ShouldSave(LightSnapshot last, LightSnapshot current) {
        float colorChange = Vector4.Distance(last.Color, current.Color);
        float intensityChange = Mathf.Abs(last.Intensity - current.Intensity);
        float rangeChange = Mathf.Abs(last.Range - current.Range);
        float spotAngleChange = Mathf.Abs(last.SpotAngle - current.SpotAngle);

        return last.Enabled != current.Enabled ||
               colorChange > 0.01f ||
               intensityChange > 0.01f ||
               rangeChange > 0.1f ||
               spotAngleChange > 0.1f;
    }

    public void SaveState() {
        var newSnapshot = new LightSnapshot {
            Enabled = m_light.enabled,
            Color = m_light.color,
            Intensity = m_light.intensity,
            Range = m_light.range,
            SpotAngle = m_light.spotAngle
        };

        m_doneScraping = false;
        if (m_timelines.GetTimeline().IsEmpty || ShouldSave(m_timelines.GetTimeline().Current(), newSnapshot)) {
            m_timelines.GetTimeline().Push(newSnapshot);
        }
    }

    public void LoadPreviousState() {
        if (m_doneScraping) {
            return;
        }
        if (!m_timelines.TryScrapeBack(out var state)) {
            TimeManager.Instance.NotifyTimelineEmpty(this);
            m_doneScraping = true;
            return;
        }
        SetLightState(state);
    }

    public void LoadNextState() {
        if (m_doneScraping) {
            return;
        }
        if (!m_timelines.TryScrapeForward(out var state)) {
            TimeManager.Instance.NotifyTimelineEmpty(this);
            m_doneScraping = true;
            return;
        }
        SetLightState(state);
    }

    private void SetLightState(LightSnapshot state) {
        m_light.enabled = state.Enabled;
        m_light.color = state.Color;
        m_light.intensity = state.Intensity;
        m_light.range = state.Range;
        m_light.spotAngle = state.SpotAngle;
    }
}
