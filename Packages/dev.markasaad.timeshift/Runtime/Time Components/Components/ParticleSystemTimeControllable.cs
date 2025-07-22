using UnityEngine;

public struct ParticleSystemStateSnapshot {
    public bool IsPlaying;
    public bool IsPaused;
    public float Time;
}

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemTimeControllable : BaseTimeControllable<ParticleSystemStateSnapshot> {
    private ParticleSystem m_particleSystem;

    protected override void Awake() {
        base.Awake();
        m_particleSystem = GetComponent<ParticleSystem>();
    }

    protected override bool ShouldSave(ParticleSystemStateSnapshot last, ParticleSystemStateSnapshot current) {
        float timeChange = Mathf.Abs(last.Time - current.Time);
        return last.IsPlaying != current.IsPlaying ||
               last.IsPaused != current.IsPaused ||
               timeChange > 0.01f;
    }

    protected override ParticleSystemStateSnapshot CaptureState() {
        return new ParticleSystemStateSnapshot {
            IsPlaying = m_particleSystem.isPlaying,
            IsPaused = m_particleSystem.isPaused,
            Time = m_particleSystem.time,
        };
    }

    protected override void ApplyState(ParticleSystemStateSnapshot state) {
        m_particleSystem.Simulate(state.Time, true, true, false);
        if (state.IsPlaying && !m_particleSystem.isPlaying) {
            m_particleSystem.Play();
        }
        if (state.IsPaused && m_particleSystem.isPlaying) {
            m_particleSystem.Pause();
        }
    }
}