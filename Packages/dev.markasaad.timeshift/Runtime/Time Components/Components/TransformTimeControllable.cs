using UnityEngine;

public class TransformTimeControllable : MonoBehaviour, ITimeControllable {
    private struct TransformSnapshot {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
    }

    private TimelineScrapper<TransformSnapshot> m_timelines;

    private Transform m_transform;

    private void Awake() {
        int bufferSize = TimeManager.Instance.SnapshotBufferSize;
        m_timelines = new TimelineScrapper<TransformSnapshot>(bufferSize);
    }

    private void Start() {
        m_transform = transform;
    }

    private void OnEnable() {
        TimeManager.Instance.RegisterTimeControllable(this);
    }

    private void OnDisable() {
        if (TimeManager.Instance != null) {
            TimeManager.Instance.UnregisterTimeControllable(this);
        }
    }

    private bool ShouldSave(TransformSnapshot last, TransformSnapshot current) {
        float posChange = Vector3.Distance(last.Position, current.Position);
        float rotChange = Quaternion.Angle(last.Rotation, current.Rotation);
        float scaleChange = Vector3.Distance(last.Scale, current.Scale);

        return posChange > 0.01f || rotChange > 1f || scaleChange > 0.01f;
    }

    public void SaveState() {
        var newSnapshot = new TransformSnapshot {
            Position = m_transform.position,
            Rotation = m_transform.rotation,
            Scale = m_transform.localScale
        };

        if (m_timelines.GetTimeline().IsEmpty || ShouldSave(m_timelines.GetTimeline().Current(), newSnapshot)) {
            m_timelines.GetTimeline().Push(newSnapshot);
        }
    }

    public void LoadPreviousState() {
        if (!m_timelines.TryScrapeBack(out var state)) {
            TimeManager.Instance.NotifyTimelineEmpty(this);
            return;
        }
        SetTransformState(state);
    }

    public void LoadNextState() {
        if (!m_timelines.TryScrapeForward(out var state)) {
            TimeManager.Instance.NotifyTimelineEmpty(this);
            return;
        }
        SetTransformState(state);
    }

    private void SetTransformState(TransformSnapshot state) {
        m_transform.position = state.Position;
        m_transform.rotation = state.Rotation;
        m_transform.localScale = state.Scale;
    }
}
