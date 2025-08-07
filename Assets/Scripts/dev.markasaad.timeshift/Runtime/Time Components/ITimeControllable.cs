public interface ITimeControllable {
    void SaveState();
    void LoadPreviousState();
    void LoadNextState();
}