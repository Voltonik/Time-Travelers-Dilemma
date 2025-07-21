public class TimeBuffer<T> {
    private readonly T[] m_buffer;
    private int m_nextPtr = 0;
    private int m_count = 0;

    public int Capacity { get; }
    public int Count => m_count;
    public bool IsEmpty => m_count == 0;

    /// <summary>
    /// Indexer: 0 is the oldest element, Count-1 is the newest.
    /// Throws if index is out of range or not yet filled.
    /// </summary>
    public T this[int index] {
        get {
            if (index < 0 || index >= m_count)
                throw new System.IndexOutOfRangeException();
            // Oldest is at (m_nextPtr - m_count + index + Capacity) % Capacity
            int realIdx = (m_nextPtr - m_count + index + Capacity) % Capacity;
            return m_buffer[realIdx];
        }
    }

    public TimeBuffer(int capacity) {
        Capacity = capacity;
        m_buffer = new T[capacity];
    }

    public void Push(T item) {
        m_buffer[m_nextPtr] = item;
        m_nextPtr = (m_nextPtr + 1) % Capacity;
        if (m_count < Capacity) {
            m_count++;
        }
    }

    public T PopPrevious() {
        if (IsEmpty)
            throw new System.InvalidOperationException("Buffer is empty");
        m_nextPtr = (m_nextPtr - 1 + Capacity) % Capacity;
        T item = m_buffer[m_nextPtr];
        m_count--;
        return item;
    }

    public T PushNext() {
        if (IsEmpty)
            throw new System.InvalidOperationException("Buffer is empty");
        m_nextPtr = (m_nextPtr + 1) % Capacity;
        if (m_count < Capacity) {
            m_count++;
        }
        return m_buffer[m_nextPtr];
    }

    public void Clear() {
        m_nextPtr = m_count = 0;
    }
}