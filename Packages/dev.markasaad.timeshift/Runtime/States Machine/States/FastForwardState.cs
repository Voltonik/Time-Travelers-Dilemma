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

    public override void Update() {
        TimeManager.Instance.LoadNextState();

        m_timer += Time.deltaTime;
        if (m_timer >= FastForwardTime) {
            m_timer = 0f;
            TimeManager.Instance.ResetEmptyTimelines();
        }
    }

    public override bool CanTransitionTo(Type targetStateType) {
        if (targetStateType == typeof(RewindState) || targetStateType == typeof(FastForwardState)) {
            return false;
        }

        return true;
    }
}