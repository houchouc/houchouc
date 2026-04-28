# Snack Truck Empire - Mobile Idle/Clicker Game

A pixel-art idle-clicker tycoon game where players start with a tiny roadside ice cream truck and grow into an absurd mobile food empire.

## Status: MVP Development 🚀

This repository contains the Minimum Viable Product (MVP) implementation of Snack Truck Empire based on the comprehensive Game Design Document.

## Quick Start

### Prerequisites
- Unity 2022.3 LTS or later
- TextMesh Pro (installed automatically)
- C# 9.0+

### Setup Steps

1. **Open in Unity**: Open `Assets/Scenes/MainGame.unity`
2. **Create Setup**: Create empty GameObject "Setup" and add `SceneSetup.cs`
3. **Press Play**: Scene will auto-initialize all systems
4. **Test**: Tap customers to earn cash, buy upgrades

## Project Structure

```
Assets/Scripts/
├── Core/              GameManager, InputHandler, SceneSetup
├── Systems/           Economy, Truck, Customer, Upgrade, Save, Analytics
├── UI/                UIManager, Button components
└── Utils/             GameConfig (all balance), VisualEffectsManager
```

## MVP Feature Set

✅ **Implemented:**
- Automatic truck movement with looping
- Customer spawning (normal + impatient types)
- Tap-to-sell mechanic with range detection
- Cash earning system with popups
- Combo multiplier (up to +50%)
- 6 upgrades with exponential cost curves
- Save/Load system (PlayerPrefs)
- Offline earnings (25% efficiency, 2-hour cap)
- Analytics event tracking
- Tutorial state machine

❌ **Not in MVP:**
- Ads, IAP, Prestige, Events, Multiple trucks/roads
- Animations, Sound, Particles
- Missions, Cosmetics

## Key Numbers

| Metric | Target |
|--------|--------|
| First sale | < 10 seconds |
| First upgrade | < 35 seconds |
| 5-minute survival | > 55% |
| Session length | 6-10 min |
| D1 retention | 35-45% |

## Balance Configuration

All constants in `GameConfig.cs` - edit for tuning:

```csharp
BASE_SALE_VALUE = 5.0
COMBO_CAP_EARLY = 0.50f
OFFLINE_EFFICIENCY_EARLY = 0.25f
OFFLINE_CAP_MINUTES_EARLY = 120
BIGGER_CONE_BASE_COST = 25
BIGGER_CONE_GROWTH = 1.18f
SPRINKLE_BOMB_UNLOCK_CASH = 750
```

## Architecture

- **GameManager**: Singleton orchestrator, initializes all systems
- **Event-driven**: Systems communicate via C# events, not direct calls
- **Data-driven**: Config constants, no magic numbers
- **Object pooling**: Customers reused, no GC spikes

## Next Steps

1. Add 3 more trucks (Hotdog, Taco, Ramen)
2. Add 2 more roads (Beach, Downtown)  
3. Implement missions system
4. Add rewarded ads (optional)
5. Create cosmetic shop

---

**Version**: MVP Alpha v0.1  
**Status**: 🟡 Core systems complete, ready for testing  
**Last Updated**: 2026-04-27
