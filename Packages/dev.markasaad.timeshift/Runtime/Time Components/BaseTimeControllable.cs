using UnityEngine;

public abstract class BaseTimeControllable<TState> : MonoBehaviour, ITimeControllable where TState : new() {
    protected TimelineScrapper<TState> m_timeline;
    private bool m_doneScraping = false;

    protected virtual void Awake() {
        m_timeline = new TimelineScrapper<TState>(TimeManager.Instance.SnapshotBufferSize);
    }

    protected virtual void OnEnable() {
        TimeManager.Instance.RegisterTimeControllable(this);
    }

    protected virtual void OnDisable() {
        if (TimeManager.Instance != null) {
            TimeManager.Instance.UnregisterTimeControllable(this);
        }
    }

    protected abstract bool ShouldSave(TState last, TState current);

    protected abstract TState CaptureState();

    protected abstract void ApplyState(TState state);

    public void SaveState() {
        var newSnapshot = CaptureState();
        m_doneScraping = false;
        if (m_timeline.GetTimeline().IsEmpty || ShouldSave(m_timeline.GetTimeline().Current(), newSnapshot)) {
            m_timeline.GetTimeline().Push(newSnapshot);
        }
    }

    public void LoadPreviousState() {
        if (m_doneScraping) {
            return;
        }
        if (!m_timeline.TryScrapeBack(out var state)) {
            TimeManager.Instance.NotifyTimelineEmpty(this);
            m_doneScraping = true;
            return;
        }
        ApplyState(state);
    }

    public void LoadNextState() {
        if (m_doneScraping) {
            return;
        }
        if (!m_timeline.TryScrapeForward(out var state)) {
            TimeManager.Instance.NotifyTimelineEmpty(this);
            m_doneScraping = true;
            return;
        }
        ApplyState(state);
    }
}