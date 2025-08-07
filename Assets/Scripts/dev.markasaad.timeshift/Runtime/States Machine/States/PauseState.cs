using System;

using UnityEngine;

[Serializable]
public class PauseState : TimeState {
    public override void OnEnterState() {
        Time.timeScale = 0f;
    }

    public override void OnExitState() {
        Time.timeScale = 1f;
    }

    public override void Update() {

    }

    public override bool CanTransitionTo(Type targetStateType) {
        return true;
    }
}