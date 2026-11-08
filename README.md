# Hearthspire ⚔️🔥

> A 2D Top-Down Rogue-Lite Action RPG built in **Unity 6 (URP 2D)** featuring dynamic raid encounters, multi-phase boss combat, risk-reward extraction loop mechanics, and an infinite persistent town hub progression system.

🎮 **Play / Download Standalone Build:** [studios-nb.itch.io/hearthspire](https://studios-nb.itch.io/hearthspire)

---

## 🌟 Core Gameplay Systems & Architecture

- **High-Stakes Extraction Raid Loop:** Players venture from the peaceful hub town into hostile wilderness portals, battling escalations of melee, ranged, and elite enemies. Surviving and extracting via timed portal gates secures acquired coins and souls; dying results in loss of raid escrow loot.
- **Dynamic Wave Orchestration:** Wave state machine managing timed enemy spawns, boss phases (Void Reaper), and affix modifiers with real-time UI radar tracking.
- **Volatile Rift Mutators:** Run mutators (*Glass Cannon*, *Darkness Falls*, *Soul Storm*) dynamically altering player damage multipliers, drop odds, and real-time 2D Universal Render Pipeline (URP) global lighting intensities.
- **Hub Economy & Infinite Meta-Progression:**
  - **Blacksmith Forge:** Infinite exponential damage progression calculations for fireballs (`Coins = 100 * Level`, `Souls = 5 * Level`).
  - **Soul Altar:** Persistent permanent passives (bonus soul drop multipliers, starting currency reserves, second-chance revive mechanics).
  - **Shop & Appearance System:** Dual-currency trade desk and dynamic character sprite / color palette swap runtime.
- **Engineered UI & Event Resilience:** 
  - Cross-scene persistent singletons (`GameManager`, `PersistentUI`, `GameMenuController`).
  - Automated duplicate `EventSystem` resolution and auto-healing hooks ensuring flawless input restoration across scene unloads.

---

## 🛠️ Technical Stack

- **Engine:** Unity 6 (6000.x)
- **Render Pipeline:** Universal Render Pipeline (URP 2D) with 2D dynamic spot/freeform lights
- **Language:** C# (.NET Standard)
- **Input:** Unity New Input System
- **Text Rendering:** TextMeshPro
- **Version Control:** Git LFS (Large File Storage for high-res sprites and audio)

---

## 🏗️ Project Structure

```text
Assets/
├── Scripts/
│   ├── Core/           # GameManager, SceneBootstrap, GameEventBus
│   ├── Player/         # PlayerController, PlayerHealth, PlayerAppearance
│   ├── Combat/         # Fireball, EnemyHealth, EnemyAI, BossController
│   ├── Systems/        # RaidManager, RiftManager, AltarManager, MinimapRadar
│   └── UI/             # GameMenuController, ShopUIController, BlacksmithUIController
├── Scenes/             # MainMenu, Hometown, HouseInterior, WildernessRaid
├── Sprites/            # Character sheets, environment tiles, URP lighting maps
└── Prefabs/            # Projectiles, spawners, persistent managers
