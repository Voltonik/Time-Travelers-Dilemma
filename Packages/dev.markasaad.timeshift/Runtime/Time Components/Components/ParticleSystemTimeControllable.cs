using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemTimeControllable : MonoBehaviour, ITimeControllable {
    private struct ParticleSystemSnapshot {
        public bool IsPlaying;
        public bool IsPaused;
        public float Time;
    }

    private TimelineScrapper<ParticleSystemSnapshot> m_timelines;
    private bool m_doneScraping = false;

    private ParticleSystem m_particleSystem;

    private void Awake() {
        int bufferSize = TimeManager.Instance.SnapshotBufferSize;
        m_timelines = new TimelineScrapper<ParticleSystemSnapshot>(bufferSize);
    }

    private void Start() {
        m_particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnEnable() {
        TimeManager.Instance.RegisterTimeControllable(this);
    }

    private void OnDisable() {
        if (TimeManager.Instance != null) {
            TimeManager.Instance.UnregisterTimeControllable(this);
        }
    }

    private bool ShouldSave(ParticleSystemSnapshot last, ParticleSystemSnapshot current) {
        float timeChange = Mathf.Abs(last.Time - current.Time);

        return last.IsPlaying != current.IsPlaying ||
               last.IsPaused != current.IsPaused ||
               timeChange > 0.01f;
    }

    public void SaveState() {
        var newSnapshot = new ParticleSystemSnapshot {
            IsPlaying = m_particleSystem.isPlaying,
            IsPaused = m_particleSystem.isPaused,
            Time = m_particleSystem.time,
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
        SetParticleSystemState(state);
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
        SetParticleSystemState(state);
    }

    private void SetParticleSystemState(ParticleSystemSnapshot state) {
        m_particleSystem.Simulate(state.Time, true, true, false);

        if (state.IsPlaying && !m_particleSystem.isPlaying) {
            m_particleSystem.Play();
        }
        if (state.IsPaused && m_particleSystem.isPlaying) {
            m_particleSystem.Pause();
        }
    }
}