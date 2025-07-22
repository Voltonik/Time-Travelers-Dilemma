using System.Collections.Generic;

public class TimelineScrapper<T> {
    private List<HistoryBuffer<T>> m_timelines = new List<HistoryBuffer<T>>();

    public int m_currentTimeline = 0;
    private bool m_revertedTimeline;

    public TimelineScrapper(int capacity) {
        m_timelines.Add(new HistoryBuffer<T>(capacity));
    }

    public HistoryBuffer<T> GetTimeline() {
        return m_currentTimeline < m_timelines.Count ? m_timelines[m_currentTimeline] : null;
    }

    private void NewTimeline() {
        m_timelines.Add(new HistoryBuffer<T>(m_timelines[m_currentTimeline].Capacity));
        m_currentTimeline++;
    }

    private void RevertTimeline() {
        if (m_currentTimeline > 0) {
            m_currentTimeline--;
            m_timelines.RemoveAt(m_timelines.Count - 1);
        }
    }

    public bool TryScrapeBack(out T state) {
        if (!m_timelines[m_currentTimeline].CanGoBack()) {
            NewTimeline();
            state = default;
            return false;
        }

        state = m_timelines[m_currentTimeline].PopPrevious();
        return true;
    }

    public bool TryScrapeForward(out T state) {
        if (!m_revertedTimeline) {
            if (!m_timelines[m_currentTimeline].CanGoBack()) {
                RevertTimeline();
                m_revertedTimeline = true;
                return TryScrapeForward(out state);
            }

            state = m_timelines[m_currentTimeline].PopPrevious();
            return true;
        }

        if (!m_timelines[m_currentTimeline].CanGoForward()) {
            m_revertedTimeline = false;
            state = default;
            return false;
        }

        state = m_timelines[m_currentTimeline].PushNext();
        return true;
    }
}