using System;

using UnityEngine;

[Serializable]
public class RewindState : TimeState {
    public float RewindTime = 5f;

    private float m_timer;

    public override void OnEnterState() {
        m_timer = 0f;
    }

    public override void OnExitState() {

    }

    public override void Update(Action onSaveState, Action onLoadNextState, Action onLoadPreviousState) {
        onLoadPreviousState?.Invoke();

        m_timer += Time.deltaTime;

        if (m_timer >= RewindTime) {
            TimeManager.Instance.SetStateToType<RecordingState>();
            return;
        }
    }
}