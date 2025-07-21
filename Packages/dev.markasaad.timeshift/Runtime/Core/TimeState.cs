using System;

[Serializable]
public abstract class TimeState {
    public abstract void OnEnterState();
    public abstract void OnExitState();
    public abstract void Update(Action onSaveState, Action onLoadNextState, Action onLoadPreviousState);
}