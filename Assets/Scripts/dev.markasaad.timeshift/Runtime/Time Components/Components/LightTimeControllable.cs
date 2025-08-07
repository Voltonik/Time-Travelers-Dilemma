using UnityEngine;

public struct LightStateSnapshot {
    public bool Enabled;
    public Color Color;
    public float Intensity;
    public float Range;
    public float SpotAngle;
}

[RequireComponent(typeof(Light))]
public class LightTimeControllable : BaseTimeControllable<LightStateSnapshot> {
    private Light m_light;

    protected override void Awake() {
        base.Awake();
        m_light = GetComponent<Light>();
    }

    protected override LightStateSnapshot CaptureState() {
        return new LightStateSnapshot {
            Enabled = m_light.enabled,
            Color = m_light.color,
            Intensity = m_light.intensity,
            Range = m_light.range,
            SpotAngle = m_light.spotAngle
        };
    }

    protected override void ApplyState(LightStateSnapshot state) {
        m_light.enabled = state.Enabled;
        m_light.color = state.Color;
        m_light.intensity = state.Intensity;
        m_light.range = state.Range;
        m_light.spotAngle = state.SpotAngle;
    }
}
