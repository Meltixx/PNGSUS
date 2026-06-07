using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[BepInPlugin("com.meltixx.pngsus", "PNGSUS - Gorilla Tag Mod Menu", "2.5.0")]
public class PNGSUS : BaseUnityPlugin
{
    public static PNGSUS Instance { get; private set; }
    
    private bool menuOpen = false;
    private bool showSearch = false;
    private string searchQuery = "";
    private Vector2 scrollPosition = Vector2.zero;
    private int selectedCategory = 0;
    private int selectedModIndex = 0;
    private int currentPage = 0;
    private int modsPerPage = 10;
    
    private List<ModCategory> modCategories = new List<ModCategory>();
    private List<Mod> filteredMods = new List<Mod>();
    private List<Mod> currentPageMods = new List<Mod>();
    
    // UI Colors
    private Color backgroundColor = new Color(0.05f, 0.05f, 0.1f, 0.98f);
    private Color panelColor = new Color(0.1f, 0.1f, 0.15f, 0.95f);
    private Color categoryActiveColor = new Color(1f, 0.5f, 0f, 1f); // Orange
    private Color categoryInactiveColor = new Color(0.3f, 0.3f, 0.4f, 1f);
    private Color modEnabledColor = new Color(0.2f, 0.8f, 0.3f, 1f); // Green
    private Color modDisabledColor = new Color(0.8f, 0.2f, 0.2f, 1f); // Red
    private Color headerColor = new Color(1f, 1f, 1f, 1f);
    private Color textColor = new Color(0.95f, 0.95f, 0.95f, 1f);
    private Color accentColor = new Color(1f, 0.5f, 0f, 1f);
    
    private float fps = 0f;
    private float frameTime = 0f;
    private int enabledModCount = 0;
    
    // Favorite system
    private HashSet<string> favoriteMods = new HashSet<string>();
    private bool showFavoritesOnly = false;

    private void Awake()
    {
        Instance = this;
        InitializeModCategories();
        Logger.LogInfo("╔════════════════════════════════════════╗");
        Logger.LogInfo("║   PNGSUS - Gorilla Tag Mod Menu v2.5   ║");
        Logger.LogInfo("╠════════════════════════════════════════╣");
        Logger.LogInfo($"║ 🎮 Total Mods: {GetTotalModCount()}");
        Logger.LogInfo($"║ 📂 Categories: {modCategories.Count}");
        Logger.LogInfo("║ 🔥 Ready to dominate!");
        Logger.LogInfo("╚════════════════════════════════════════╝");
    }

    private int GetTotalModCount()
    {
        return modCategories.Sum(c => c.mods.Count);
    }

    private void InitializeModCategories()
    {
        // Category 1: Player Mods (80+)
        ModCategory playerMods = new ModCategory("👤 Player Mods", 0);
        AddPlayerMods(playerMods);
        modCategories.Add(playerMods);

        // Category 2: Game Mods (100+)
        ModCategory gameMods = new ModCategory("🎮 Game Mods", 1);
        AddGameMods(gameMods);
        modCategories.Add(gameMods);

        // Category 3: Visual Mods (90+)
        ModCategory visualMods = new ModCategory("✨ Visual Mods", 2);
        AddVisualMods(visualMods);
        modCategories.Add(visualMods);

        // Category 4: Cosmetics & Fun (85+)
        ModCategory cosmetics = new ModCategory("🎨 Cosmetics & Fun", 3);
        AddCosmeticMods(cosmetics);
        modCategories.Add(cosmetics);

        // Category 5: Utility Tools (60+)
        ModCategory utility = new ModCategory("🔧 Utility Tools", 4);
        AddUtilityMods(utility);
        modCategories.Add(utility);

        // Category 6: Advanced Features (35+)
        ModCategory advanced = new ModCategory("⚡ Advanced Features", 5);
        AddAdvancedMods(advanced);
        modCategories.Add(advanced);

        // Category 7: Competitive Mods (40+)
        ModCategory competitive = new ModCategory("🏆 Competitive Mods", 6);
        AddCompetitiveMods(competitive);
        modCategories.Add(competitive);

        // Category 8: Environmental Mods (50+)
        ModCategory environmental = new ModCategory("🌍 Environmental Mods", 7);
        AddEnvironmentalMods(environmental);
        modCategories.Add(environmental);

        // Category 9: Gameplay Modifiers (45+)
        ModCategory gameplay = new ModCategory("🎯 Gameplay Modifiers", 8);
        AddGameplayMods(gameplay);
        modCategories.Add(gameplay);

        // Category 10: Entertainment Modes (35+)
        ModCategory entertainment = new ModCategory("🎪 Entertainment Modes", 9);
        AddEntertainmentMods(entertainment);
        modCategories.Add(entertainment);

        // Category 11: System Optimization (40+)
        ModCategory optimization = new ModCategory("⚙️ System Optimization", 10);
        AddOptimizationMods(optimization);
        modCategories.Add(optimization);

        // Category 12: Security & Privacy (30+)
        ModCategory security = new ModCategory("🔐 Security & Privacy", 11);
        AddSecurityMods(security);
        modCategories.Add(security);
    }

    private void AddPlayerMods(ModCategory category)
    {
        for (int i = 1; i <= 20; i++)
            category.mods.Add(new Mod($"Speed Boost {i}x", $"Increase movement speed by {i * 0.1f}x", ModType.SpeedBoost));
        
        for (int i = 1; i <= 15; i++)
            category.mods.Add(new Mod($"Flight Mode {i}", $"Advanced flight variation {i}", ModType.Flight));
        
        for (int i = 1; i <= 15; i++)
            category.mods.Add(new Mod($"Size Modifier {i}", $"Change size to {(i * 0.2f).ToString("F1")}x", ModType.SizeModifier));
        
        string[] abilities = { "Super Jump", "Wall Run", "Air Dash", "Time Slow", "Phase Shift", 
            "Teleport Dash", "Grappling Hook", "Levitation", "Super Strength", "Speed Burst",
            "Invincibility", "Shield Bubble", "Double Jump", "Wall Climb", "Sliding",
            "Sprint Boost", "Momentum Multiplier", "Gravity Invert", "Freeze Frame", "Power Punch" };
        for (int i = 0; i < abilities.Length; i++)
            category.mods.Add(new Mod(abilities[i], $"Activate {abilities[i]} ability", ModType.SuperAbility));
        
        for (int i = 1; i <= 10; i++)
            category.mods.Add(new Mod($"Physics Hack {i}", $"Advanced physics modification {i}", ModType.PhysicsHack));
    }

    private void AddGameMods(ModCategory category)
    {
        string[] noclipTypes = { "Full No-Clip", "Selective No-Clip", "Smooth No-Clip", "Partial No-Clip", 
            "Advanced No-Clip", "Smart No-Clip", "Precision No-Clip", "Turbo No-Clip", "Phase No-Clip", "Quantum No-Clip" };
        for (int i = 0; i < noclipTypes.Length; i++)
            category.mods.Add(new Mod(noclipTypes[i], $"Walk through anything - {noclipTypes[i]}", ModType.NoClip));
        
        string[] godModes = { "Standard God Mode", "Invincible Mode", "Damage Immunity", "Health Recovery", "Shield Protection", "Ultra Protection", "Absolute God Mode", "Eternal Godhood" };
        for (int i = 0; i < godModes.Length; i++)
            category.mods.Add(new Mod(godModes[i], $"Take no damage - {godModes[i]}", ModType.GodMode));
        
        for (int i = 1; i <= 12; i++)
            category.mods.Add(new Mod($"Time Control {i}", $"Manipulate time at {(i * 0.1f).ToString("F1")} speed", ModType.TimeControl));
        
        for (int i = 1; i <= 15; i++)
            category.mods.Add(new Mod($"Gravity Hack {i}", $"Gravity modification variant {i}", ModType.GravityHack));
        
        string[] gameModeTweaks = { "Enhanced Freeze Tag", "Speed Tag Mode", "Infection Mode", "Infection Pro", "Infection Chaos",
            "Team Freeze Tag", "Last Gorilla Standing", "Elimination Mode", "Survival Mode", "Endless Mode",
            "Hardcore Mode", "Challenge Mode", "Time Attack", "Speed Run", "Obstacle Course",
            "Parkour Challenge", "Skill Test", "Endurance Test", "Reaction Test", "Precision Test",
            "Maze Mode", "Puzzle Mode", "Puzzle Extreme", "Adventure Mode", "Story Mode",
            "Sandbox Mode", "Creative Mode", "Unlimited Mode", "Custom Mode", "Hybrid Mode",
            "Fusion Mode", "Turbo Mode", "Ultra Mode", "Extreme Mode", "Chaos Mode",
            "Random Mode", "Surprise Mode", "Mystery Mode", "Legendary Mode", "God Mode Variant",
            "Nightmare Mode", "Insane Mode", "Custom Difficulty", "Variable Difficulty", "Dynamic Difficulty",
            "Progressive Difficulty", "Adaptive Difficulty", "Scaling Difficulty", "Custom Rules", "Tournament Mode",
            "Professional Mode", "Casual Mode", "Relaxed Mode", "Intense Mode", "Epic Mode",
            "Mythic Mode", "Ultimate Mode", "Infinite Mode", "Eternal Mode", "Omega Mode" };
        for (int i = 0; i < gameModeTweaks.Length; i++)
            category.mods.Add(new Mod(gameModeTweaks[i], $"Game mode variation: {gameModeTweaks[i]}", ModType.GameModeTweak));
        
        string[] extraGameMods = { "Infinite Stamina", "Stamina Control", "Energy Regeneration", "Quick Recovery",
            "Instant Respawn", "No Respawn Timer", "Custom Spawn", "Spawn Protection",
            "Unlimited Resources", "Resource Multiplier", "Double Points", "Triple Points",
            "Quad Points", "Point Modifier", "Score Booster", "Multiplier Stack",
            "Win Condition Tweak", "Lose Condition Tweak", "Draw Condition", "Custom Condition" };
        for (int i = 0; i < extraGameMods.Length; i++)
            category.mods.Add(new Mod(extraGameMods[i], $"Game mechanic: {extraGameMods[i]}", ModType.GameMechanic));
    }

    private void AddVisualMods(ModCategory category)
    {
        string[] trails = { "Rainbow Trail", "Neon Trail", "Fire Trail", "Ice Trail", "Electric Trail",
            "Plasma Trail", "Holographic Trail", "Particle Trail", "Light Trail", "Shadow Trail",
            "Ghost Trail", "Smoke Trail", "Water Trail", "Metal Trail", "Crystal Trail",
            "Diamond Trail", "Gold Trail", "Silver Trail", "Copper Trail", "Bronze Trail",
            "Rainbow Gradient", "Color Cycle", "Glitch Trail", "Matrix Trail", "Quantum Trail" };
        for (int i = 0; i < trails.Length; i++)
            category.mods.Add(new Mod(trails[i], $"Hand trail effect: {trails[i]}", ModType.TrailEffect));
        
        for (int i = 1; i <= 20; i++)
            category.mods.Add(new Mod($"Glow System {i}", $"Player glow variation {i}", ModType.GlowEffect));
        
        string[] screenEffects = { "Bloom Effect", "Chromatic Aberration", "Vignette", "Motion Blur",
            "Film Grain", "Color Grade Warm", "Color Grade Cool", "Invert Colors", "Posterize",
            "Pixelate", "Blur Screen", "Sharpen Screen", "Sepia Tone", "CRT Effect", "Retro Filter" };
        for (int i = 0; i < screenEffects.Length; i++)
            category.mods.Add(new Mod(screenEffects[i], $"Screen effect: {screenEffects[i]}", ModType.ScreenEffect));
        
        string[] envChanges = { "Night Vision", "Thermal Vision", "Infrared Vision", "X-Ray Vision", "Fog Removal",
            "Fog Increase", "Fog Color Custom", "Sky Color Custom", "Lighting Bright", "Lighting Dark",
            "Lighting Custom", "Shadow Toggle", "Reflection Enhancement", "Water Level Change", "Gravity Field",
            "Particle Enhancement", "Weather Control", "Season Control", "Time Control Visual", "Environment Pack" };
        for (int i = 0; i < envChanges.Length; i++)
            category.mods.Add(new Mod(envChanges[i], $"Environment change: {envChanges[i]}", ModType.EnvironmentalChange));
        
        for (int i = 1; i <= 10; i++)
            category.mods.Add(new Mod($"Custom Appearance {i}", $"Player appearance variation {i}", ModType.PlayerCustomization));
    }

    private void AddCosmeticMods(ModCategory category)
    {
        for (int i = 1; i <= 15; i++)
            category.mods.Add(new Mod($"Color Pack {i}", $"Custom color scheme {i}", ModType.CustomColor));
        
        string[] emotes = { "Dance", "Wave", "Celebrate", "Taunt", "Laugh", "Sit", "Lay Down", "Stretch",
            "Meditate", "Think", "Confused", "Angry", "Sad", "Happy", "Surprise", "Shock", "Cool Pose", "Victory Pose", "Flex", "Idle Animation" };
        for (int i = 0; i < emotes.Length; i++)
            category.mods.Add(new Mod(emotes[i], $"Animation emote: {emotes[i]}", ModType.Emote));
        
        for (int i = 1; i <= 20; i++)
            category.mods.Add(new Mod($"Particle Effect {i}", $"Custom particle system {i}", ModType.ParticleEffect));
        
        string[] sounds = { "Deep Voice", "High Voice", "Robot Voice", "Echo Effect", "Reverb Effect",
            "Pitch Up", "Pitch Down", "Speed Voice", "Slow Voice", "Custom Sounds",
            "Whisper Mode", "Loud Mode", "Muted", "Surround Sound", "Stereo Boost" };
        for (int i = 0; i < sounds.Length; i++)
            category.mods.Add(new Mod(sounds[i], $"Sound modification: {sounds[i]}", ModType.SoundModifier));
        
        string[] accessories = { "Crown", "Hat", "Beanie", "Helmet", "Headphones", "Halo", "Horns", "Antlers",
            "Wings", "Cape", "Scarf", "Backpack", "Weapon", "Shield", "Aura" };
        for (int i = 0; i < accessories.Length; i++)
            category.mods.Add(new Mod(accessories[i], $"Accessory: {accessories[i]}", ModType.Accessory));
    }

    private void AddUtilityMods(ModCategory category)
    {
        string[] cameraTools = { "Free Camera", "Follow Camera", "First Person", "Third Person", "Cinematic Camera",
            "Drone Camera", "Isometric Camera", "Top Down Camera", "Custom Camera", "Locked Camera", "Orbit Camera", "Fixed Camera" };
        for (int i = 0; i < cameraTools.Length; i++)
            category.mods.Add(new Mod(cameraTools[i], $"Camera mode: {cameraTools[i]}", ModType.CameraTool));
        
        string[] visionTools = { "Players ESP", "Item ESP", "Distance Display", "Health Display", "Nametag ESP",
            "Team ESP", "Coordinate Display", "Direction Indicator", "Radar System", "Wall Vision" };
        for (int i = 0; i < visionTools.Length; i++)
            category.mods.Add(new Mod(visionTools[i], $"Vision tool: {visionTools[i]}", ModType.VisionTool));
        
        string[] perfTools = { "FPS Counter", "Performance Monitor", "Frame Time Display", "Memory Monitor", "CPU Monitor",
            "GPU Monitor", "Network Monitor", "Latency Display", "Packet Display", "Performance Graph", "Stats Panel", "Debug Info" };
        for (int i = 0; i < perfTools.Length; i++)
            category.mods.Add(new Mod(perfTools[i], $"Performance tool: {perfTools[i]}", ModType.PerformanceTool));
        
        string[] teleportTypes = { "Teleport Forward", "Teleport Backward", "Teleport Up", "Teleport Down",
            "Teleport Left", "Teleport Right", "Teleport to Player", "Random Teleport", "Waypoint Teleport", "Bookmark Teleport" };
        for (int i = 0; i < teleportTypes.Length; i++)
            category.mods.Add(new Mod(teleportTypes[i], $"Teleportation: {teleportTypes[i]}", ModType.Teleportation));
        
        string[] settings = { "Save Profile", "Load Profile", "Reset All", "Export Settings", "Import Settings",
            "Cloud Save", "Auto Save", "Settings Menu" };
        for (int i = 0; i < settings.Length; i++)
            category.mods.Add(new Mod(settings[i], $"Settings: {settings[i]}", ModType.SettingsTool));
        
        string[] recording = { "Start Recording", "Stop Recording", "Screenshot", "Video Clip", "Replay System", "Cloud Upload", "GIF Export", "Stream Mode" };
        for (int i = 0; i < recording.Length; i++)
            category.mods.Add(new Mod(recording[i], $"Recording: {recording[i]}", ModType.RecordingTool));
    }

    private void AddAdvancedMods(ModCategory category)
    {
        string[] advFeatures = { "AI Companion", "Auto Farming", "Bot Control", "Script System", "API Access",
            "Console Command", "Debug Mode", "Developer Tools", "Network Hacks", "Server Tools",
            "Custom Lua Scripts", "Plugin Loader", "Mod Manager", "Hot Reload", "Version Manager",
            "Update Checker", "Crash Recovery", "Memory Optimizer", "Cache Manager", "Easter Egg Unlock",
            "Hidden Features", "Beta Access", "Experimental Mode", "Advanced Settings", "Power User Mode",
            "Expert Mode", "Professional Tools", "Industry Tools", "Research Mode", "Testing Suite",
            "Quality Assurance", "Performance Lab", "Security Test", "Stability Check", "Optimization Suite" };
        for (int i = 0; i < advFeatures.Length; i++)
            category.mods.Add(new Mod(advFeatures[i], $"Advanced feature: {advFeatures[i]}", ModType.AdvancedFeature));
    }

    private void AddCompetitiveMods(ModCategory category)
    {
        string[] compMods = { "Skill Boost", "Reaction Training", "Precision Training", "Speed Training", "Endurance Training",
            "Team Coordination", "Team Strategy", "Call Out System", "Replay Analysis", "Performance Stats",
            "Competitive Mode", "Tournament Mode", "Ranking System", "Leaderboard", "Achievement System",
            "Skill Levels", "Difficulty Scaling", "Opponent AI", "Practice Mode", "Training Simulation",
            "Accuracy Trainer", "Timing Trainer", "Positioning Trainer", "Resource Management", "Decision Training",
            "Communication Tools", "Analytics Dashboard", "Heat Map", "Movement Tracker", "Action Replay",
            "Benchmark Test", "Performance Analyzer", "Player Stats", "Match History", "Win Rate Tracker",
            "Progression System", "Ranking Algorithm", "Skill Rating", "ELO System", "Seasonal Pass" };
        for (int i = 0; i < compMods.Length; i++)
            category.mods.Add(new Mod(compMods[i], $"Competitive feature: {compMods[i]}", ModType.CompetitiveFeature));
    }

    private void AddEnvironmentalMods(ModCategory category)
    {
        string[] envMods = { "Map Modification", "Texture Override", "Skybox Change", "Lighting Override", "Shadow Control",
            "Weather Rain", "Weather Snow", "Weather Fog", "Weather Storm", "Weather Clear",
            "Day Cycle", "Night Cycle", "Sunrise Effect", "Sunset Effect", "Custom Lighting",
            "Color Grading", "Ambient Light", "Point Lights", "Spot Lights", "Area Lights",
            "Particle System", "Effect Intensity", "Bloom Control", "Depth of Field", "Motion Blur",
            "Water Effects", "Water Level", "Water Color", "Water Transparency", "Ocean Waves",
            "Ground Texture", "Rock Texture", "Wall Texture", "Floor Material", "Ceiling Material",
            "Custom Objects", "Object Placement", "Object Scale", "Object Rotation", "Object Physics",
            "Environmental Audio", "Ambient Sound", "Background Music", "Effect Sounds", "Custom Audio",
            "Space Modification", "Area Expansion", "Area Compression", "Custom Dimensions", "Dimension Portal" };
        for (int i = 0; i < envMods.Length; i++)
            category.mods.Add(new Mod(envMods[i], $"Environment: {envMods[i]}", ModType.EnvironmentalFeature));
    }

    private void AddGameplayMods(ModCategory category)
    {
        string[] gameMods = { "Easy Mode", "Normal Mode", "Hard Mode", "Insane Mode", "Custom Difficulty",
            "Damage Multiplier", "Health Multiplier", "Speed Multiplier", "Gravity Modifier", "Cooldown Reducer",
            "Ability Enhancement", "Power Up Duration", "Drop Rate Modifier", "Spawn Rate", "Enemy Difficulty",
            "Balance Tweaks", "Damage Scaling", "Health Scaling", "Resource Scaling", "Reward Scaling",
            "Gameplay Tweaks", "Mechanics Tweak", "Control Sensitivity", "Input Lag Reduction", "Responsiveness Boost",
            "Combo System", "Streak System", "Multiplier System", "Point System", "Reward System",
            "Progression Tweak", "Unlock All", "Level Skip", "Achievement Unlock", "Badge Unlock",
            "Custom Rules", "Custom Conditions", "Win Conditions", "Lose Conditions", "Tie Conditions",
            "Game Balance", "Fairness Settings", "Equality Mode", "Advantage Mode", "Challenge Mode" };
        for (int i = 0; i < gameMods.Length; i++)
            category.mods.Add(new Mod(gameMods[i], $"Gameplay: {gameMods[i]}", ModType.GameplayModifier));
    }

    private void AddEntertainmentMods(ModCategory category)
    {
        string[] entMods = { "Mini Game 1", "Mini Game 2", "Mini Game 3", "Mini Game 4", "Mini Game 5",
            "Mini Game 6", "Mini Game 7", "Mini Game 8", "Mini Game 9", "Mini Game 10",
            "Mini Game 11", "Mini Game 12", "Fun Mode 1", "Fun Mode 2", "Fun Mode 3",
            "Fun Mode 4", "Fun Mode 5", "Fun Mode 6", "Fun Mode 7", "Fun Mode 8",
            "Fun Mode 9", "Fun Mode 10", "Party Mode 1", "Party Mode 2", "Party Mode 3",
            "Party Mode 4", "Party Mode 5", "Party Mode 6", "Party Mode 7", "Party Mode 8",
            "Arcade Mode", "Sandbox Mode", "Creative Mode", "Exploration Mode", "Discovery Mode" };
        for (int i = 0; i < entMods.Length; i++)
            category.mods.Add(new Mod(entMods[i], $"Entertainment: {entMods[i]}", ModType.EntertainmentMode));
    }

    private void AddOptimizationMods(ModCategory category)
    {
        string[] optMods = { "FPS Boost", "Performance Optimization", "Memory Cleanup", "Cache Clearing", "Texture Optimization",
            "Model LOD", "Draw Call Reduction", "Batch Optimization", "Physics Optimization", "AI Optimization",
            "Animation Optimization", "Particle Optimization", "Light Optimization", "Shadow Optimization", "Post-Process Optimization",
            "Network Optimization", "Latency Reduction", "Bandwidth Optimization", "Server Optimization", "Client Optimization",
            "CPU Optimization", "GPU Optimization", "Memory Management", "Heat Management", "Power Efficiency",
            "Loading Speed", "Startup Optimization", "Shutdown Optimization", "Runtime Optimization", "Background Process",
            "Garbage Collection", "Memory Leaks Detection", "Resource Monitor", "System Monitor", "Performance Profiler",
            "Benchmark Mode", "Stress Test", "Load Test", "Stability Test", "Quality Test" };
        for (int i = 0; i < optMods.Length; i++)
            category.mods.Add(new Mod(optMods[i], $"Optimization: {optMods[i]}", ModType.OptimizationFeature));
    }

    private void AddSecurityMods(ModCategory category)
    {
        string[] secMods = { "Data Encryption", "Account Protection", "Password Manager", "Two Factor Auth", "Anti Cheat",
            "Anti Hack", "Anti Exploit", "Anti Bot", "Anti Spam", "Anti Abuse",
            "Privacy Mode", "Incognito Mode", "Offline Mode", "Local Storage", "Cloud Storage Secure",
            "Data Backup", "Data Restore", "Data Export", "Data Import", "Data Deletion",
            "Session Manager", "Login History", "Device Manager", "Access Control", "Permission Manager",
            "Audit Log", "Activity Monitor", "System Security", "Firewall Control", "VPN Support" };
        for (int i = 0; i < secMods.Length; i++)
            category.mods.Add(new Mod(secMods[i], $"Security: {secMods[i]}", ModType.SecurityFeature));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            menuOpen = !menuOpen;

        if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.LeftControl))
            showSearch = !showSearch;

        if (Input.GetKeyDown(KeyCode.B) && Input.GetKey(KeyCode.LeftControl))
        {
            showFavoritesOnly = !showFavoritesOnly;
            currentPage = 0;
        }

        frameTime += (Time.deltaTime - frameTime) * 0.1f;
        fps = 1f / frameTime;

        enabledModCount = modCategories.Sum(c => c.mods.Count(m => m.enabled));

        if (menuOpen)
            HandleMenuInput();
    }

    private void HandleMenuInput()
    {
        int maxPage = Mathf.Max(0, (filteredMods.Count - 1) / modsPerPage);

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            currentPage = Mathf.Min(maxPage, currentPage + 1);
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            currentPage = Mathf.Max(0, currentPage - 1);

        if (Input.GetKeyDown(KeyCode.E))
        {
            selectedCategory = (selectedCategory + 1) % modCategories.Count;
            currentPage = 0;
            RefreshFilteredMods();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            selectedCategory = (selectedCategory - 1 + modCategories.Count) % modCategories.Count;
            currentPage = 0;
            RefreshFilteredMods();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
            selectedModIndex = Mathf.Max(0, selectedModIndex - 1);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            selectedModIndex = Mathf.Min(currentPageMods.Count - 1, selectedModIndex + 1);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedModIndex < currentPageMods.Count)
                currentPageMods[selectedModIndex].enabled = !currentPageMods[selectedModIndex].enabled;
        }
    }

    private void RefreshFilteredMods()
    {
        if (showFavoritesOnly)
        {
            filteredMods = modCategories
                .SelectMany(c => c.mods)
                .Where(m => favoriteMods.Contains(m.name))
                .ToList();
        }
        else if (showSearch && !string.IsNullOrEmpty(searchQuery))
        {
            filteredMods = modCategories
                .SelectMany(c => c.mods)
                .Where(m => m.name.ToLower().Contains(searchQuery.ToLower()))
                .ToList();
        }
        else
        {
            filteredMods = new List<Mod>(modCategories[selectedCategory].mods);
        }
        
        UpdateCurrentPageMods();
    }

    private void UpdateCurrentPageMods()
    {
        int startIndex = currentPage * modsPerPage;
        currentPageMods = filteredMods.Skip(startIndex).Take(modsPerPage).ToList();
        selectedModIndex = 0;
    }

    private void OnGUI()
    {
        if (!menuOpen) return;
        DrawMainMenu();
    }

    private void DrawMainMenu()
    {
        GUI.backgroundColor = backgroundColor;
        GUI.color = Color.white;

        GUILayout.BeginArea(new Rect(20, 20, 1280, 720));

        GUI.color = accentColor;
        GUILayout.Label("🎮 PNGSUS - ULTIMATE MOD MENU", new GUIStyle(GUI.skin.label)
        {
            fontSize = 32,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = accentColor }
        });
        GUI.color = Color.white;

        GUILayout.BeginHorizontal();
        GUI.color = new Color(0.2f, 0.9f, 0.2f);
        GUILayout.Label($"FPS: {fps:F0}  |  Enabled: {enabledModCount}  |  Total: {GetTotalModCount()}  |  Category: {modCategories[selectedCategory].name}", GUILayout.Height(25));
        GUI.color = Color.white;
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("🔍 Search (Ctrl+F)", GUILayout.Height(35), GUILayout.Width(200)))
            showSearch = !showSearch;
        if (GUILayout.Button("⭐ Favorites (Ctrl+B)", GUILayout.Height(35), GUILayout.Width(200)))
        {
            showFavoritesOnly = !showFavoritesOnly;
            currentPage = 0;
        }
        if (GUILayout.Button("⚙️ Settings", GUILayout.Height(35), GUILayout.Width(200)))
        {
        }
        if (GUILayout.Button("💾 Save Profile", GUILayout.Height(35), GUILayout.Width(200)))
        {
        }
        GUILayout.EndHorizontal();

        if (showSearch)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(100));
            searchQuery = GUILayout.TextField(searchQuery, GUILayout.Height(30));
            GUILayout.EndHorizontal();
            RefreshFilteredMods();
        }
        else
        {
            RefreshFilteredMods();
        }

        GUILayout.Space(5);

        GUILayout.Label("📂 CATEGORIES (Q/E to switch)", GUILayout.Height(20));
        GUILayout.BeginHorizontal();
        for (int i = 0; i < modCategories.Count; i++)
        {
            GUI.backgroundColor = (i == selectedCategory) ? categoryActiveColor : categoryInactiveColor;
            if (GUILayout.Button(modCategories[i].name, GUILayout.Height(35)))
            {
                selectedCategory = i;
                currentPage = 0;
                RefreshFilteredMods();
            }
        }
        GUILayout.EndHorizontal();

        GUI.backgroundColor = backgroundColor;
        GUILayout.Space(5);

        GUILayout.Label($"📋 PAGE {currentPage + 1} - {modCategories[selectedCategory].name} ({filteredMods.Count} mods)", GUILayout.Height(20));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(450));

        for (int i = 0; i < currentPageMods.Count; i++)
        {
            Mod mod = currentPageMods[i];
            
            if (i == selectedModIndex)
                GUI.backgroundColor = new Color(0.2, 0.4, 0.8, 0.7f);
            else
                GUI.backgroundColor = panelColor;

            GUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Height(50));

            GUI.backgroundColor = mod.enabled ? modEnabledColor : modDisabledColor;
            if (GUILayout.Button(mod.enabled ? "✓" : "○", GUILayout.Width(50), GUILayout.Height(50)))
                mod.enabled = !mod.enabled;

            GUI.backgroundColor = panelColor;

            if (GUILayout.Button(favoriteMods.Contains(mod.name) ? "⭐" : "☆", GUILayout.Width(50), GUILayout.Height(50)))
            {
                if (favoriteMods.Contains(mod.name))
                    favoriteMods.Remove(mod.name);
                else
                    favoriteMods.Add(mod.name);
            }

            GUILayout.BeginVertical();
            GUI.color = textColor;
            GUILayout.Label($"{mod.name}", new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold });
            GUILayout.Label($"{mod.description}", new GUIStyle(GUI.skin.label) { fontSize = 10 });
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        GUI.color = Color.white;
        GUILayout.Space(5);

        int maxPage = Mathf.Max(0, (filteredMods.Count - 1) / modsPerPage);
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = currentPage > 0 ? categoryActiveColor : categoryInactiveColor;
        if (GUILayout.Button($"← PREVIOUS ({currentPage}/{maxPage})", GUILayout.Height(35)))
        {
            currentPage = Mathf.Max(0, currentPage - 1);
            UpdateCurrentPageMods();
        }
        GUI.backgroundColor = currentPage < maxPage ? categoryActiveColor : categoryInactiveColor;
        if (GUILayout.Button($"NEXT ({currentPage}/{maxPage}) →", GUILayout.Height(35)))
        {
            currentPage = Mathf.Min(maxPage, currentPage + 1);
            UpdateCurrentPageMods();
        }
        GUI.backgroundColor = backgroundColor;
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset All Mods", GUILayout.Height(30)))
        {
            foreach (var cat in modCategories)
                foreach (var mod in cat.mods)
                    mod.enabled = false;
        }
        if (GUILayout.Button("❌ CLOSE MENU (M Key)", GUILayout.Height(30)))
            menuOpen = false;
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }
}

public class ModCategory
{
    public string name;
    public int id;
    public List<Mod> mods = new List<Mod>();
    public bool expanded = true;

    public ModCategory(string categoryName, int categoryId)
    {
        name = categoryName;
        id = categoryId;
    }
}

public class Mod
{
    public string name;
    public string description;
    public ModType type;
    public bool enabled = false;
    public float cooldown = 0f;

    public Mod(string modName, string modDescription, ModType modType)
    {
        name = modName;
        description = modDescription;
        type = modType;
    }
}

public enum ModType
{
    SpeedBoost, Flight, SizeModifier, SuperAbility, PhysicsHack,
    NoClip, GodMode, TimeControl, GravityHack, GameModeTweak, GameMechanic,
    TrailEffect, GlowEffect, ScreenEffect, EnvironmentalChange, PlayerCustomization,
    CustomColor, Emote, ParticleEffect, SoundModifier, NameTag, Accessory,
    CameraTool, VisionTool, PerformanceTool, Teleportation, SettingsTool, RecordingTool,
    CompetitiveFeature, EnvironmentalFeature, GameplayModifier, EntertainmentMode,
    OptimizationFeature, SecurityFeature, AdvancedFeature
}
