using System;

[Serializable]
public abstract class TimeState {
    public virtual bool CanTransitionTo(Type targetStateType) => true;
    public abstract void OnEnterState();
    public abstract void OnExitState();
    public abstract void Update(Action onSaveState, Action onLoadNextState, Action onLoadPreviousState);
}