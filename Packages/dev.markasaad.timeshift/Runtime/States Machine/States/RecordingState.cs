using System;

using UnityEngine;

[Serializable]
public class RecordingState : TimeState {
    public float SamplingRate = 0.02f;

    private float m_timer;

    public override void OnEnterState() {
        m_timer = 0f;
    }

    public override void OnExitState() {
    }

    public override void Update() {
        m_timer += Time.deltaTime;

        while (m_timer >= SamplingRate) {
            m_timer -= SamplingRate;
            TimeManager.Instance.SaveState();
        }
    }

    public override bool CanTransitionTo(Type targetStateType) {
        return true;
    }
}