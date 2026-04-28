# Snack Truck Empire - MVP Setup Guide

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs           # Main game orchestrator
│   │   ├── InputHandler.cs          # Tap detection and input
│   │   └── SceneSetup.cs            # Programmatic scene creation
│   ├── Systems/
│   │   ├── EconomySystem.cs         # Cash, sales, combo mechanics
│   │   ├── TruckSystem.cs           # Truck movement and stats
│   │   ├── CustomerSystem.cs        # Customer spawning and management
│   │   ├── UpgradeSystem.cs         # Upgrade data and purchases
│   │   ├── SaveSystem.cs            # Persistence (save/load)
│   │   ├── AnalyticsManager.cs      # Event tracking
│   │   ├── AudioManager.cs          # Sound effects
│   │   └── TutorialManager.cs       # Tutorial state machine
│   ├── UI/
│   │   ├── UIManager.cs             # HUD and display management
│   │   └── Button.cs                # UI button wrapper
│   └── Data/
│       └── (Game configuration files - to be added)
├── Resources/
│   ├── Prefabs/
│   └── Audio/
│       └── (Sound clips to be added)
└── Scenes/
    └── (Main game scene)
```

## Getting Started

### Step 1: Create a New Unity Project
1. Create a new 3D or 2D project in Unity 2022.3 LTS or later
2. Import TextMesh Pro when prompted

### Step 2: Import the Code
1. Copy the Assets/Scripts folder to your project
2. Ensure you have UnityEngine.UI and TMPro in your using statements

### Step 3: Create the Main Scene
1. Create a new scene called "MainGame"
2. Create an empty GameObject called "Setup"
3. Add the SceneSetup.cs script to it
4. Play the scene - it will automatically create all game objects and initialize systems

### Step 4: Test the MVP Loop
- **First 10 seconds**: Truck should move, customer appears, tap customer to earn cash
- **First 30 seconds**: First upgrade button glows, buy it with earned cash
- **First 5 minutes**: Multiple customers, combo meter, unlock progress visible

## Core Systems

### GameManager
- Central orchestrator for all game systems
- Initializes all subsystems in Start()
- Manages game pause/resume
- Tracks offline earnings on app pause

### EconomySystem
- Handles cash currency and calculations
- Implements combo multiplier (max 50% in MVP)
- Calculates sale values based on upgrades
- Manages offline earnings (25% efficiency, 120 min cap in MVP)

### TruckSystem
- Moves truck horizontally across road continuously
- Manages truck sell range (customers must be within range to sell)
- Updates truck stats when upgrades purchased
- Speed upgrades affect throughput, not customer satisfaction

### CustomerSystem
- Spawns customers ahead of truck on road
- Two types: Normal (5s patience) and Impatient (3s patience)
- Tap detection and sale processing
- Combo reset on miss

### UpgradeSystem
- 6 MVP upgrades (Bigger Cone, Faster Scoop, Louder Bell, Wider Window, Combo Cup, Auto Bell)
- Data-driven configuration with cost curves
- Exponential growth rates per upgrade type
- Level-based cost calculation

### UIManager
- Displays cash, combo, and unlock progress
- Upgrade button with dynamic pricing
- Real-time stat updates from event system

### SaveSystem
- Persists cash, upgrades, lifetime earnings
- Stores last session time for offline calculations
- Uses PlayerPrefs (can be upgraded to cloud save later)

### AnalyticsManager
- Tracks MVP milestone events
- Event queue system ready for backend integration
- Debug logging of all events

### TutorialManager
- State machine tracking tutorial progression
- Triggers based on player actions and time
- Tracks first sale, first upgrade, 5-minute survival

## MVP Features Implemented

✅ **Core Gameplay**
- Automatic truck movement
- Customer spawning with patience timers
- Tap-to-sell mechanic
- Sell range detection
- Cash popups and visual feedback

✅ **Economy**
- Sale value formula: BaseValue × ProductUpgrade × ComboMultiplier
- 6 upgrades with exponential cost curves
- Combo system (up to +50% value)
- Offline earnings (25% efficiency, 2-hour cap)

✅ **UI/UX**
- Cash display (top-left)
- Combo counter (top-center)
- Unlock progress (top-right)
- Upgrade button (bottom-right) with dynamic cost/level

✅ **Progression**
- Sprinkle Bomb unlock goal at 750 lifetime cash
- Visual feedback on all purchases
- Upgrade effects propagated to game systems

✅ **Persistence**
- Save/load system via PlayerPrefs
- Offline earnings calculation on session resume

✅ **Analytics**
- MVP milestone events tracked
- Ready for Firebase/GameAnalytics integration

✅ **Tutorial**
- Tutorial state machine
- Tracks progression through first 5 minutes
- Event-driven tutorial advancement

## Gameplay Loop (First 5 Minutes)

```
0-10 sec:  Truck appears → Customer waves → Tap customer → Cash popup
10-30 sec: More customers → Build combo → Cash increases
30-60 sec: Upgrade button glows → Buy upgrade → Truck visibly improves
1-3 min:   Combo mechanic → Higher cash per sale
3-5 min:   Unlock progress visible → Goal becomes clear
```

## Testing Checklist

- [ ] **First Sale**: Tap customer when truck is in range
- [ ] **First Miss**: Tap customer when truck is NOT in range (customer shakes head)
- [ ] **First Upgrade**: Buy Bigger Cone upgrade, see cash value increase
- [ ] **Combo**: Tap 3+ customers quickly, see multiplier (x3, x4, etc)
- [ ] **Upgrade Progression**: Buy 3-4 upgrades, watch costs increase
- [ ] **Session Save**: Close app and reopen, cash/upgrades persist
- [ ] **Offline Earnings**: Close app for 1+ minute, reopen and claim earnings
- [ ] **D1 Retention**: Session length target 6-10 minutes first session

## Known Limitations (MVP Scope)

❌ **NOT Included in MVP**
- No ads
- No IAP
- No prestige
- No events
- No animations (system ready, assets needed)
- No sound effects (system ready, assets needed)
- No multiple trucks
- No multiple roads
- No cosmetics
- No missions
- No leaderboards

## Next Steps for Vertical Slice (After MVP Validation)

1. **Add 3 more trucks** with different themes
2. **Add 2 more roads** with new customer types
3. **Implement basic automation** (Auto Bell helper)
4. **Add visual animations** and particle effects
5. **Implement 3 missions** with rewards
6. **Add rewarded ads** for optional boosts
7. **Create event system** infrastructure
8. **Add cosmetic shop** with truck skins

## Debugging

### Common Issues

**"EconomySystem not found"**
- Ensure GameManager is created before other systems initialize
- Check that all AddComponent calls complete successfully

**"UIManager not found"**
- SceneSetup must run before GameManager initializes
- Ensure Canvas exists in scene

**Customers not spawning**
- Check CustomerSystem.Tick() is called from GameManager.Update()
- Verify spawn interval calculation in GetSpawnInterval()

**Taps not registering**
- Ensure InputHandler is on GameManager or separate GameObject
- Check Layer and Tag setup for Customer objects

### Debug Commands

Enable debug logging:
```csharp
Debug.Log($"Cash: {GameManager.Instance.Economy.GetCash()}");
Debug.Log($"Combo: {GameManager.Instance.Economy.GetComboCount()}");
Debug.Log($"Upgrade Level: {GameManager.Instance.Upgrades.GetUpgradeLevel("bigger_cone")}");
```

## File Size Target

MVP should be under 50MB (before art assets):
- Code: ~2MB
- UI assets: ~5MB (when created)
- Audio assets: ~5MB (when created)
- Sprites: ~20MB (when created)

## Performance Target

- Target 60 FPS on mid-range phones (iPhone 8/Samsung S8 equivalent)
- Particle effect cap: 100 active
- Customer limit: 20 active on screen
- Memory usage: <200MB at gameplay

---

**MVP Status**: Development in progress
**Last Updated**: [Date]
**Lead Developer**: Claude Code AI
