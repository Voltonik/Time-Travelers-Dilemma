using System;

using UnityEngine;

[Serializable]
public class FastForwardState : TimeState {
    public float FastForwardTime = 5f;

    private float m_timer;

    public override void OnEnterState() {
        m_timer = 0f;
    }

    public override void OnExitState() {

    }

    public override void Update(Action onSaveState, Action onLoadNextState, Action onLoadPreviousState) {
        onLoadNextState?.Invoke();

        m_timer += Time.deltaTime;

        if (m_timer >= FastForwardTime) {
            TimeManager.Instance.SetStateToType<RecordingState>();
            return;
        }
    }
}