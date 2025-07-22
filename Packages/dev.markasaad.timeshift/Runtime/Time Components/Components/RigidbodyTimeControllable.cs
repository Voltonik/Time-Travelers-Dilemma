using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyTimeControllable : MonoBehaviour, ITimeControllable {
    private struct RigidbodySnapshot {
        public Vector3 LinearVelocity;
        public Vector3 AngularVelocity;
        public Vector3 Position;
        public Quaternion Rotation;
    }

    [Header("Debug View")]
    public bool ShowDebugGizmos = true;
    public Color GizmoColor = new Color(0, 1, 1, 0.3f);

    private TimelineScrapper<RigidbodySnapshot> m_timelines;

    private Rigidbody m_rigidbody;

    private void Awake() {
        int bufferSize = TimeManager.Instance.SnapshotBufferSize;
        m_timelines = new TimelineScrapper<RigidbodySnapshot>(bufferSize);
    }

    private void Start() {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable() {
        TimeManager.Instance.RegisterTimeControllable(this);
    }

    private void OnDisable() {
        if (TimeManager.Instance != null) {
            TimeManager.Instance.UnregisterTimeControllable(this);
        }
    }

    private bool ShouldSave(RigidbodySnapshot last, RigidbodySnapshot current) {
        float posChange = Vector3.Distance(last.Position, current.Position);
        float rotChange = Quaternion.Angle(last.Rotation, current.Rotation);
        float velChange = (last.LinearVelocity - current.LinearVelocity).magnitude;

        return posChange > 0.1f || rotChange > 5f || velChange > 0.5f;
    }

    public void SaveState() {
        var newSnapshot = new RigidbodySnapshot {
            LinearVelocity = m_rigidbody.linearVelocity,
            AngularVelocity = m_rigidbody.angularVelocity,
            Position = m_rigidbody.position,
            Rotation = m_rigidbody.rotation
        };

        if (m_timelines.GetTimeline().IsEmpty || ShouldSave(m_timelines.GetTimeline().Current(), newSnapshot)) {
            m_timelines.GetTimeline().Push(newSnapshot);
            Debug.Log($"saved snapshot to timeline: {m_timelines.m_currentTimeline}");
        }
    }

    public void LoadPreviousState() {
        if (!m_timelines.TryScrapeBack(out var state)) {
            TimeManager.Instance.NotifyTimelineEmpty(this);
            return;
        }
        SetRigidbodyState(state);
    }


    public void LoadNextState() {
        if (!m_timelines.TryScrapeForward(out var state)) {
            TimeManager.Instance.NotifyTimelineEmpty(this);
            return;
        }
        SetRigidbodyState(state);
    }

    private void SetRigidbodyState(RigidbodySnapshot state) {
        m_rigidbody.linearVelocity = state.LinearVelocity;
        m_rigidbody.angularVelocity = state.AngularVelocity;
        m_rigidbody.position = state.Position;
        m_rigidbody.rotation = state.Rotation;
    }

    private void OnDrawGizmosSelected() {
        if (!ShowDebugGizmos || m_timelines == null || m_timelines.GetTimeline() == null || m_timelines.GetTimeline().Count == 0)
            return;

        Gizmos.color = GizmoColor;
        Vector3 scale = Vector3.one;
        var t = transform;
        if (t != null)
            scale = Vector3.Scale(t.localScale, Vector3.one);

        for (int i = 0; i < m_timelines.GetTimeline().Count; i++) {
            var snap = m_timelines.GetTimeline()[i];
            Gizmos.matrix = Matrix4x4.TRS(snap.Position, snap.Rotation, scale);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }
        Gizmos.matrix = Matrix4x4.identity;
    }
}