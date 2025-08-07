using UnityEngine;

public struct TransformStateSnapshot {
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
}

public class TransformTimeControllable : BaseTimeControllable<TransformStateSnapshot> {
    private Transform m_transform;

    protected override void Awake() {
        base.Awake();
        m_transform = transform;
    }

    protected override TransformStateSnapshot CaptureState() {
        return new TransformStateSnapshot {
            Position = m_transform.position,
            Rotation = m_transform.rotation,
            Scale = m_transform.localScale
        };
    }

    protected override void ApplyState(TransformStateSnapshot state) {
        m_transform.position = state.Position;
        m_transform.rotation = state.Rotation;
        m_transform.localScale = state.Scale;
    }
}
