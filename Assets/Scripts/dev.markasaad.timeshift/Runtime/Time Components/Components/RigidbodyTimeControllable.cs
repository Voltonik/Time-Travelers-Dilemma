using UnityEngine;

public struct RigidbodyStateSnapshot {
    public Vector3 LinearVelocity;
    public Vector3 AngularVelocity;
    public Vector3 Position;
    public Quaternion Rotation;
}

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyTimeControllable : BaseTimeControllable<RigidbodyStateSnapshot> {
    [Header("Debug View")]
    public bool ShowDebugGizmos = true;
    public Color GizmoColor = new Color(0, 1, 1, 0.3f);

    private Rigidbody m_rigidbody;

    protected override void Awake() {
        base.Awake();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    protected override RigidbodyStateSnapshot CaptureState() {
        return new RigidbodyStateSnapshot {
            LinearVelocity = m_rigidbody.velocity,
            AngularVelocity = m_rigidbody.angularVelocity,
            Position = m_rigidbody.position,
            Rotation = m_rigidbody.rotation
        };
    }

    protected override void ApplyState(RigidbodyStateSnapshot state) {
        m_rigidbody.velocity = state.LinearVelocity;
        // m_rigidbody.angularVelocity = state.AngularVelocity;
        m_rigidbody.position = state.Position;
        m_rigidbody.rotation = state.Rotation;
    }

    private void OnDrawGizmosSelected() {
        if (!ShowDebugGizmos || m_timeline == null || m_timeline.GetTimeline() == null || m_timeline.GetTimeline().Count == 0)
            return;

        Gizmos.color = GizmoColor;
        Vector3 scale = Vector3.one;
        var t = transform;
        if (t != null)
            scale = Vector3.Scale(t.localScale, Vector3.one);

        for (int i = 0; i < m_timeline.GetTimeline().Count; i++) {
            var snap = m_timeline.GetTimeline()[i];
            Gizmos.matrix = Matrix4x4.TRS(snap.Position, snap.Rotation, scale);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }
        Gizmos.matrix = Matrix4x4.identity;
    }
}