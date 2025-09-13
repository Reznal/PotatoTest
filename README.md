# PotatoTest - Idle Potato Farming Game

## Game Overview
PotatoTest is an idle/incremental potato farming game built in Unity 2D. Players tap to farm potatoes, process them through various buildings, sell for cash, buy upgrades, and prestige for starch points to unlock powerful bonuses.

## Core Features

### Resource Management
- **Potatoes**: Primary resource harvested by tapping and farms
- **Cash**: Earned by selling processed potatoes, used for upgrades
- **Starch**: Prestige currency for permanent bonuses
- **Golden Potatoes**: Rare resource with special bonuses

### Farming System
- **Tap Farming**: Click the potato to harvest manually
- **Auto Farming**: Unlockable automation for passive income
- **Multiple Fields**: Unlock new farms with increasing production
- **Soil Types**: Upgrade soil for production bonuses (Basic, Rich, Fertile, Magical)

### Processing Chain
- **Potato Washer**: Basic processing for small cash bonus
- **Potato Fryer**: Medium processing with better returns
- **Chip Factory**: Advanced processing for maximum profit

### Upgrade System
- **Tap Power**: Increase manual farming efficiency
- **Click Multiplier**: Boost tap effectiveness
- **Automation**: Unlock and improve automated systems
- **Processing Speed**: Faster potato processing
- **Market Connections**: Better selling prices

### Prestige System
- **Starch Points**: Reset progress for permanent bonuses
- **Potato Gods**: Powerful entities providing massive multipliers
- **Global Multipliers**: Permanent production increases

### Events & Community
- **Weekly Events**: Limited-time bonuses (Potato Frenzy, Golden Hour, etc.)
- **Frenzy Storms**: Super-powered events affecting all aspects
- **Leaderboards**: Weekly and all-time rankings
- **Community Goals**: Server-wide objectives with rewards

### Technical Features
- **Save/Load System**: Persistent progress with offline calculation
- **Audio System**: Background music and sound effects
- **UI Framework**: Clean, responsive interface
- **Event System**: Decoupled architecture for easy expansion

## Getting Started

### Requirements
- Unity 2022.3 or later
- URP (Universal Render Pipeline)
- TextMeshPro

### Project Structure
```
Assets/
├── Scenes/
│   └── MainGame.unity          # Main game scene
├── Scripts/
│   ├── Core/                   # Core game mechanics
│   │   ├── GameManager.cs      # Central game controller
│   │   ├── ResourceManager.cs  # Resource handling
│   │   ├── FarmManager.cs      # Farming mechanics
│   │   ├── UpgradeManager.cs   # Upgrade system
│   │   └── PrestigeManager.cs  # Prestige mechanics
│   ├── Systems/                # Game systems
│   │   ├── ProcessingManager.cs # Potato processing
│   │   ├── SaveManager.cs       # Save/load functionality
│   │   ├── EventManager.cs      # Events and bonuses
│   │   ├── CommunityManager.cs  # Leaderboards and goals
│   │   └── AudioManager.cs      # Sound management
│   ├── UI/                     # User interface
│   │   ├── MainGameUI.cs       # Main UI controller
│   │   ├── GameUI.cs           # Game display elements
│   │   └── FarmPanel.cs        # Farm management UI
│   └── Tests/                  # Testing utilities
│       └── GameTester.cs       # Game testing tools
```

### How to Play
1. **Start**: Tap the potato to begin harvesting
2. **Upgrade**: Use cash to buy better tools and automation
3. **Process**: Build washing, frying, and chip facilities
4. **Prestige**: When ready, reset for starch points and permanent bonuses
5. **Events**: Participate in limited-time events for extra rewards
6. **Community**: Compete on leaderboards and contribute to community goals

### Balancing Notes
- Base tap power: 1 potato per tap
- Starting cash: $100
- Prestige requirement: 1,000,000 potatoes
- Offline efficiency: 50% of active production
- Event frequency: 10% chance every 5 minutes

## Development Notes

### Architecture
The game uses a modular architecture with separate managers for different systems:
- **GameManager**: Central coordinator
- **ResourceManager**: Handles all currency/resource logic
- **FarmManager**: Manages fields and production
- **UpgradeManager**: Handles player progression
- **PrestigeManager**: Manages reset mechanics
- Various system managers for audio, saves, events, etc.

### Extensibility
The system is designed for easy expansion:
- Add new resources by extending ResourceType enum
- Create new upgrades by adding to UpgradeManager
- Implement new events in EventManager
- Add processing buildings in ProcessingManager

### Testing
Use the GameTester component to:
- Run automated tests of core systems
- Manually add resources for testing
- Validate game balance and progression

## Future Enhancements
- Visual potato farm with animated graphics
- More complex processing chains
- Seasonal events and special potatoes
- Multiplayer features and guilds
- Mobile optimization with touch controls
- Cloud save synchronization
- In-app purchases and monetization

## License
This project is a demonstration of idle game mechanics and architecture.