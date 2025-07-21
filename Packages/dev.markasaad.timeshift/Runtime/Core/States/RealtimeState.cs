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

    public override void Update(Action onSaveState, Action onLoadNextState, Action onLoadPreviousState) {
        m_timer += Time.deltaTime;

        while (m_timer >= SamplingRate) {
            m_timer -= SamplingRate;
            onSaveState?.Invoke();
        }
    }
}