using System;

public class HistoryBuffer<T> {
    private readonly T[] m_buffer;
    private readonly float[] m_timestamps;
    private int m_count = 0; // Number of valid snapshots
    private int m_cursor = -1; // Current position in timeline (-1 = empty)
    private int m_start = 0; // Index of oldest snapshot

    public int Capacity { get; }
    public int Count => m_count;
    public bool IsEmpty => m_count == 0;
    public int Cursor => m_cursor;

    public bool CanGoBack() => !IsEmpty && m_cursor > 0;
    public bool CanGoForward() => !IsEmpty && m_cursor < m_count - 1;

    public T this[int index] {
        get {
            if (index < 0 || index >= m_count)
                throw new System.IndexOutOfRangeException();
            int realIdx = (m_start + index) % Capacity;
            return m_buffer[realIdx];
        }
    }

    public HistoryBuffer(int capacity) {
        Capacity = capacity;
        m_buffer = new T[capacity];
        m_timestamps = new float[capacity];
        m_count = 0;
        m_cursor = -1;
        m_start = 0;
    }

    public void Push(T item, float timestamp) {
        if (m_cursor < m_count - 1) {
            m_count = m_cursor + 1; // Erase any states in the future
        }
        if (m_count == Capacity) {
            m_start = (m_start + 1) % Capacity;
            m_count--;
        }
        int insertIdx = (m_start + m_count) % Capacity;
        m_buffer[insertIdx] = item;
        m_timestamps[insertIdx] = timestamp;
        m_count++;
        m_cursor = m_count - 1;
    }

    public void EraseOldSnapshots(float currentTime, float keepSeconds) {
        while (m_count > 0) {
            float oldestTimestamp = m_timestamps[m_start];
            if (currentTime - oldestTimestamp > keepSeconds) {
                m_start = (m_start + 1) % Capacity;
                m_count--;
                if (m_cursor > 0) m_cursor--;
            } else {
                break;
            }
        }
    }

    public T PopPrevious() {
        if (!CanGoBack())
            throw new System.InvalidOperationException("No previous state to rewind to");
        m_cursor--;
        return this[m_cursor];
    }

    public T PushNext() {
        if (!CanGoForward())
            throw new System.InvalidOperationException("No next state to fast-forward to");
        m_cursor++;
        return this[m_cursor];
    }

    public T Current() {
        if (IsEmpty || m_cursor < 0)
            throw new System.InvalidOperationException("Buffer is empty");
        return this[m_cursor];
    }

    public void Clear() {
        m_count = 0;
        m_cursor = -1;
        m_start = 0;
        Array.Clear(m_timestamps, 0, m_timestamps.Length);
    }
}