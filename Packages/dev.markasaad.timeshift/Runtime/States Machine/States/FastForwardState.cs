using System;

[Serializable]
public class FastForwardState : TimeState {
    public float FastForwardTime = 5f;

    public override void OnEnterState() {
    }

    public override void OnExitState() {

    }

    public override void Update(Action onSaveState, Action onLoadNextState, Action onLoadPreviousState) {
        onLoadNextState?.Invoke();
    }

    public override bool CanTransitionTo(Type targetStateType) {
        if (targetStateType == typeof(RewindState) || targetStateType == typeof(FastForwardState)) {
            return false;
        }

        return true;
    }
}