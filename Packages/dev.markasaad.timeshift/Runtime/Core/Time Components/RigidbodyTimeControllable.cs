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

    private TimeBuffer<RigidbodySnapshot> m_snapshots;
    private Rigidbody m_rigidbody;

    private void Awake() {
        float rewind = 0f, ff = 0f, samplingRate = 0.02f;
        var tm = TimeManager.Instance;
        if (tm != null && tm.States != null) {
            foreach (var state in tm.States) {
                if (state is RewindState r) rewind = Mathf.Max(rewind, r.RewindTime);
                if (state is FastForwardState f) ff = Mathf.Max(ff, f.FastForwardTime);
                if (state is RecordingState rec) samplingRate = rec.SamplingRate;
            }
        }
        float maxRewindOrFFTime = Mathf.Max(rewind, ff);
        int bufferSize = Mathf.CeilToInt(maxRewindOrFFTime / samplingRate) + 2;
        m_snapshots = new TimeBuffer<RigidbodySnapshot>(bufferSize);

        Debug.Log($"{bufferSize} snapshots allocated for {maxRewindOrFFTime}s max rewind/ff time at {samplingRate}s sampling rate.");
    }

    private void Start() {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable() {
        TimeManager.Instance.RegisterTimeControllable(this);
    }

    private void OnDisable() {
        TimeManager.Instance.UnregisterTimeControllable(this);
    }

    public void SaveState() {
        m_snapshots.Push(new RigidbodySnapshot {
            LinearVelocity = m_rigidbody.linearVelocity,
            AngularVelocity = m_rigidbody.angularVelocity,
            Position = m_rigidbody.position,
            Rotation = m_rigidbody.rotation
        });
    }

    public void LoadPreviousState() {
        if (m_snapshots.IsEmpty) {
            var tm = TimeManager.Instance;
            if (tm != null) tm.SetStateToType<RecordingState>();
            return;
        }

        var state = m_snapshots.PopPrevious();
        m_rigidbody.linearVelocity = state.LinearVelocity;
        m_rigidbody.angularVelocity = state.AngularVelocity;
        m_rigidbody.position = state.Position;
        m_rigidbody.rotation = state.Rotation;
    }

    public void LoadNextState() {
        if (m_snapshots.IsEmpty) {
            var tm = TimeManager.Instance;
            if (tm != null) tm.SetStateToType<RecordingState>();
            return;
        }

        var state = m_snapshots.PushNext();
        m_rigidbody.linearVelocity = state.LinearVelocity;
        m_rigidbody.angularVelocity = state.AngularVelocity;
        m_rigidbody.position = state.Position;
        m_rigidbody.rotation = state.Rotation;
    }

    private void OnDrawGizmosSelected() {
        if (!ShowDebugGizmos || m_snapshots == null || m_snapshots.Count == 0)
            return;

        Gizmos.color = GizmoColor;
        Vector3 scale = Vector3.one;
        var t = transform;
        if (t != null)
            scale = Vector3.Scale(t.localScale, Vector3.one);

        for (int i = 0; i < m_snapshots.Count; i++) {
            var snap = m_snapshots[i];
            Gizmos.matrix = Matrix4x4.TRS(snap.Position, snap.Rotation, scale);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }
        Gizmos.matrix = Matrix4x4.identity;
    }
}