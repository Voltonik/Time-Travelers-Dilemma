using System;

[Serializable]
public class RewindState : TimeState {
    public float RewindTime = 5f;

    public override void OnEnterState() {
    }

    public override void OnExitState() {

    }

    public override void Update(Action onSaveState, Action onLoadNextState, Action onLoadPreviousState) {
        onLoadPreviousState?.Invoke();
    }

    public override bool CanTransitionTo(Type targetStateType) {
        if (targetStateType == typeof(FastForwardState) || targetStateType == typeof(RewindState)) {
            return false;
        }

        return true;
    }
}