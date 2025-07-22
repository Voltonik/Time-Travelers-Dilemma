# Time Travelers Dilemma



https://github.com/user-attachments/assets/f77c242b-681f-4a91-8ba2-b157d7c902f3



Unity project showcasing the **Time Shift** package - a time manipulation system featuring rewind, fast-forward, and pause mechanics.

## 🎮 Sample Scene & Controls

**Player Movement:**
- **WASD** - Move
- **Mouse** - Look around
- **Space** - Jump

**Time Manipulation:**
- **1** - Toggle Recording/Pause
- **2** - Rewind
- **3** - Fast-Forward

## 📦 Time Shift Package

Core time manipulation system with state-based control and timeline management.

### Features
- **Recording State** - Captures object states
- **Pause State** - Freezes time (`Time.timeScale = 0`)
- **Rewind State** - Plays back timeline in reverse
- **Fast-Forward State** - Accelerates through timeline

### Usage
```csharp
TimeManager.Instance.SetStateToType<RewindState>();
TimeManager.Instance.SetStateToType<PauseState>();
TimeManager.Instance.SetStateToType<RecordingState>();
```

### Core Components
- **TimeManager** - FSM and central time control 
- **ITimeControllable** - Interface for objects that can go back and forward in time
- **RigidbodyTimeControllable** - Rigidbody physics support
- **TransformTimeControllable** - Transform position, rotation, and scale support
- **LightTimeControllable** - Light properties support (color, intensity, range, spot angle)
- **ParticleSystemTimeControllable** - Particle system state support
