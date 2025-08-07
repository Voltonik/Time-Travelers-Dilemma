using System;

using UnityEngine;

[Serializable]
public class RewindState : TimeState {
    private float m_timer;

    public override void OnEnterState() {
        m_timer = 0f;
    }

    public override void OnExitState() {

    }

    public override void Update() {
        TimeManager.Instance.LoadPreviousState();

        m_timer += Time.deltaTime;
        if (m_timer >= TimeManager.Instance.GetStateByType<RecordingState>().SavedTime) {
            m_timer = 0f;
            TimeManager.Instance.ResetEmptyTimelines();
        }
    }

    public override bool CanTransitionTo(Type targetStateType) {
        if (targetStateType == typeof(FastForwardState) || targetStateType == typeof(RewindState)) {
            return false;
        }

        return true;
    }
}