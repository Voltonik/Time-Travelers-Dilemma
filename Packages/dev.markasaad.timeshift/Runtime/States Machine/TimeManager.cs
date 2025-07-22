using System;
using System.Collections.Generic;

using UnityEngine;

public class TimeManager : MonoBehaviour {
    private static TimeManager s_instance;
    public static TimeManager Instance {
        get {
            if (s_instance == null) {
                s_instance = FindAnyObjectByType<TimeManager>();
            }
            return s_instance;
        }
    }

    public int SnapshotBufferSize { get; private set; } = 32;

    [SerializeReference, SubclassSelector]
    public List<TimeState> States = new List<TimeState>();
    [SerializeReference, SubclassSelector]
    public TimeState CurrentState;

    private readonly HashSet<ITimeControllable> m_registeredControllables = new HashSet<ITimeControllable>();
    private readonly HashSet<ITimeControllable> m_emptyTimelines = new HashSet<ITimeControllable>();

    private Dictionary<Type, TimeState> m_stateCache = new Dictionary<Type, TimeState>();

    private Action m_onSaveState;
    private Action m_onLoadNextState;
    private Action m_onLoadPreviousState;

    public void RegisterTimeControllable(ITimeControllable timeControllable) {
        m_registeredControllables.Add(timeControllable);
        m_onSaveState += timeControllable.SaveState;
        m_onLoadNextState += timeControllable.LoadNextState;
        m_onLoadPreviousState += timeControllable.LoadPreviousState;
    }

    public void UnregisterTimeControllable(ITimeControllable timeControllable) {
        m_registeredControllables.Remove(timeControllable);
        m_emptyTimelines.Remove(timeControllable);
        m_onSaveState -= timeControllable.SaveState;
        m_onLoadNextState -= timeControllable.LoadNextState;
        m_onLoadPreviousState -= timeControllable.LoadPreviousState;
    }

    public void NotifyTimelineEmpty(ITimeControllable controllable) {
        m_emptyTimelines.Add(controllable);
        if (m_emptyTimelines.Count == m_registeredControllables.Count) {
            ResetEmptyTimelines();
        }
    }

    public void ResetEmptyTimelines() {
        m_emptyTimelines.Clear();
        SetStateToType<RecordingState>();
    }

    public void SetState(TimeState state) {
        if (CurrentState == state)
            return;

        if (!CanTransitionTo(state)) {
            Debug.LogWarning($"Cannot transition from {CurrentState?.GetType().Name} to {state?.GetType().Name}");
            return;
        }

        CurrentState?.OnExitState();
        CurrentState = state;
        CurrentState?.OnEnterState();
    }

    public void SetStateToType<T>() where T : TimeState {
        if (CurrentState is T)
            return;

        if (m_stateCache.TryGetValue(typeof(T), out var state) && state is T) {
            SetState(state);
            return;
        }

        Debug.LogWarning($"No state of type {typeof(T).Name} found.");
    }

    private bool CanTransitionTo(TimeState targetState) {
        if (CurrentState == null || targetState == null)
            return true;

        return CurrentState.CanTransitionTo(targetState.GetType());
    }

    private void Awake() {
        if (States.Count == 0) {
            enabled = false;
            return;
        }

        m_stateCache.Clear();
        foreach (var state in States) {
            m_stateCache[state.GetType()] = state;
        }

        float rewind = 0f, ff = 0f, samplingRate = 0.02f;
        if (m_stateCache.TryGetValue(typeof(RewindState), out var rewindState) && rewindState is RewindState r)
            rewind = r.RewindTime;
        if (m_stateCache.TryGetValue(typeof(FastForwardState), out var ffState) && ffState is FastForwardState f)
            ff = f.FastForwardTime;
        if (m_stateCache.TryGetValue(typeof(RecordingState), out var recState) && recState is RecordingState rec)
            samplingRate = rec.SamplingRate;

        float maxRewindOrFFTime = Mathf.Max(rewind, ff);
        SnapshotBufferSize = Mathf.CeilToInt(maxRewindOrFFTime / samplingRate) + 2;

        SetState(States[0]);
    }

    private void Update() {
        CurrentState.Update(m_onSaveState, m_onLoadNextState, m_onLoadPreviousState);
    }
}