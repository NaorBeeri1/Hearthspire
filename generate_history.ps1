# Set author identity (Match your GitHub account email & name)
$gitName  = "Naor"
$gitEmail = "your-github-email@example.com"

# Initialize git repository
if (-not (Test-Path ".git")) {
    git init
    git config user.name "$gitName"
    git config user.email "$gitEmail"
}

# Helper function to create backdated commit
function Make-DatedCommit {
    param(
        [string]$DateStr,
        [string]$Message,
        [string[]]$FilesToAdd = @(".")
    )
    foreach ($file in $FilesToAdd) {
        git add $file
    }
    
    # Check if there are staged changes
    $status = git status --porcelain
    if (-not [string]::IsNullOrWhiteSpace($status)) {
        $env:GIT_AUTHOR_DATE    = "$DateStr"
        $env:GIT_COMMITTER_DATE = "$DateStr"
        git commit -m "$Message"
        Remove-Item Env:\GIT_AUTHOR_DATE
        Remove-Item Env:\GIT_COMMITTER_DATE
        Write-Host "Committed: [$DateStr] - $Message" -ForegroundColor Cyan
    } else {
        Write-Host "Skipped (no changes): $Message" -ForegroundColor Yellow
    }
}

# --- STAGE 1: PROJECT SETUP & SCAFFOLDING (Sep 14 - Sep 20) ---
Make-DatedCommit "2026-09-14 18:23:10" "Initial project scaffolding and gitignore configuration" @(".gitignore", "Packages", "ProjectSettings")
Make-DatedCommit "2026-09-16 19:45:00" "Setup base camera and 2D rendering pipeline settings" @("Assets/Settings*", "Assets/UniversalRenderPipelineGlobalSettings*")
Make-DatedCommit "2026-09-18 20:12:35" "Add foundational scene bootstraps and persistent architecture" @("Assets/Scripts/Core/PersistentObject.cs", "Assets/Scripts/Core/SceneBootstrap.cs")

# --- STAGE 2: PLAYER CORE & INPUT (Sep 21 - Sep 27) ---
Make-DatedCommit "2026-09-21 17:34:12" "Implement basic 2D player movement controller with physics clamping" @("Assets/Scripts/Player/PlayerMovement.cs")
Make-DatedCommit "2026-09-23 18:15:40" "Implement smooth camera follow with background boundary clamping" @("Assets/Scripts/Core/CameraFollow.cs")
Make-DatedCommit "2026-09-25 21:05:19" "Add player health state container and damage listeners" @("Assets/Scripts/Player/PlayerHealth.cs")

# --- STAGE 3: CORE COMBAT & PROJECTILES (Sep 28 - Oct 04) ---
Make-DatedCommit "2026-09-29 19:10:44" "Implement directional projectile shooting logic" @("Assets/Scripts/Player/PlayerShooting.cs")
Make-DatedCommit "2026-10-01 18:50:22" "Add Fireball projectile behaviors and collision layer triggers" @("Assets/Scripts/Player/Fireball.cs")
Make-DatedCommit "2026-10-03 16:30:15" "Add player skin customization controller" @("Assets/Scripts/Player/PlayerAppearance.cs")

# --- STAGE 4: ENEMY ARCHITECTURE & AI (Oct 05 - Oct 11) ---
Make-DatedCommit "2026-10-06 18:42:00" "Implement enemy base class and health tracking" @("Assets/Scripts/Enemies/Enemy.cs", "Assets/Scripts/Enemies/EnemyDamage.cs")
Make-DatedCommit "2026-10-08 19:15:30" "Add enemy patrol mechanics and vision-based player chase" @("Assets/Scripts/Enemies/PatrolEnemy.cs")
Make-DatedCommit "2026-10-10 17:20:10" "Integrate unified EnemyController with dynamic tier scaling" @("Assets/Scripts/Enemies/EnemyController.cs", "Assets/Scripts/Enemies/DamageDiagnostic.cs")

# --- STAGE 5: TOWN HUB & INTERIOR EXPLORATION (Oct 12 - Oct 18) ---
Make-DatedCommit "2026-10-13 18:05:40" "Implement town building door interaction and scene transit" @("Assets/Scripts/Core/BuildingInteraction.cs", "Assets/Scripts/Core/ExitBuilding.cs")
Make-DatedCommit "2026-10-15 20:44:12" "Add BedSanctuary healing mechanics and resting economy" @("Assets/Scripts/Core/BedSanctuary.cs")
Make-DatedCommit "2026-10-17 19:30:25" "Add Wardrobe panel UI for equipping unlocked skins" @("Assets/Scripts/Core/Wardrobe.cs")

# --- STAGE 6: ECONOMY, MERCHANTS & BLACKSMITH (Oct 19 - Oct 25) ---
Make-DatedCommit "2026-10-20 18:12:00" "Build central GameManager with currency management and persistence" @("Assets/Scripts/Core/GameManager.cs")
Make-DatedCommit "2026-10-21 21:10:30" "Add pickups for coins and souls upon enemy defeat" @("Assets/Scripts/UI/CoinPickup.cs", "Assets/Scripts/UI/SoulPickup.cs")
Make-DatedCommit "2026-10-22 17:55:18" "Implement Soul Merchant shop logic and exchange UI" @("Assets/Scripts/UI/MerchantShop.cs", "Assets/Scripts/UI/ShopUIController.cs")
Make-DatedCommit "2026-10-24 19:20:45" "Implement Blacksmith stall interaction and infinite damage forge" @("Assets/Scripts/UI/BlacksmithShop.cs", "Assets/Scripts/UI/BlacksmithUIController.cs")

# --- STAGE 7: MINIMAP & ROGUELIKE ALTAR (Oct 26 - Nov 01) ---
Make-DatedCommit "2026-10-27 18:35:10" "Implement dynamic circular radar minimap with POI compass markers" @("Assets/Scripts/UI/MinimapRadar.cs", "Assets/Scripts/UI/MinimapFollow.cs")
Make-DatedCommit "2026-10-29 20:15:00" "Add permanent upgrade static container with paragon cost scaling" @("Assets/Scripts/Core/PermanentUpgrades.cs")
Make-DatedCommit "2026-10-31 16:40:22" "Integrate Ancient Soul Altar dialogue, purchase logic and UI panel" @("Assets/Scripts/UI/AltarInteraction.cs", "Assets/Scripts/UI/AltarUIController.cs")

# --- STAGE 8: RAID SYSTEM, RIFTS & FINAL SCENES (Nov 02 - Nov 07) ---
Make-DatedCommit "2026-11-02 18:50:11" "Build raid wave spawner and boss encounter controller" @("Assets/Scripts/Core/RaidWaveManager.cs", "Assets/Scripts/Core/RaidSurvivalController.cs", "Assets/Scripts/Enemies/RaidEnemySpawner.cs")
Make-DatedCommit "2026-11-04 19:30:40" "Implement extraction return portals and secure coin vaulting" @("Assets/Scripts/Core/RaidPortal.cs", "Assets/Scripts/Core/ReturnPortal.cs")
Make-DatedCommit "2026-11-05 21:12:15" "Add Volatile Rift affix generator and dynamic URP lighting mutators" @("Assets/Scripts/Core/RiftManager.cs", "Assets/Scripts/Core/AffixRiftPortal.cs", "Assets/Scripts/UI/RiftHUDDisplay.cs")
Make-DatedCommit "2026-11-06 18:22:00" "Assemble and link complete scene hierarchy and UI canvases" @("Assets/Scenes", "Assets/Scripts/UI/PersistentUI.cs", "Assets/Scripts/UI/PlayerBarUpdater.cs", "Assets/Scripts/UI/ButtonEventDebugger.cs", "Assets/Scripts/UI/GameMenuController.cs")
Make-DatedCommit "2026-11-07 11:30:00" "Final balance pass, sprite asset imports, and project polish" @(".")

Write-Host "`nAll commits generated successfully!" -ForegroundColor Green