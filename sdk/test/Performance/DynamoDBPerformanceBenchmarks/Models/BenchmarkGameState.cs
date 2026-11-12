using Amazon.DynamoDBv2.DataModel;

namespace AWSSDK.Benchmarks.MockedDynamoDB;

// A deep, wide object graph modeled on a real-world multiplayer match state: ~30 top-level members, an
// options object with ~50 members, and arrays/lists of nested objects. Most of the per-item serialization
// cost of DynamoDBContext scales with the number of nested objects and values, which flat models hide.
[DynamoDBTable("BenchmarkGameStates")]
public class BenchmarkGameState
{
    [DynamoDBHashKey]
    public string GameId { get; set; } = string.Empty;
    public BenchmarkGameStatus Status { get; set; }
    public int Version { get; set; }
    public int CurrentRoundNumber { get; set; } = 1;
    public bool HasTimedOut { get; set; }
    public BenchmarkGameOptions Options { get; set; } = new();
    public BenchmarkGameContext? Context { get; set; }
    public BenchmarkSquad[] Squads { get; set; } = [];
    public BenchmarkRound[] Rounds { get; set; } = [];
    public uint TickRate { get; set; } = 10;
    public uint Tick { get; set; }
    public uint? CrashedAtTick { get; set; }
    public uint? AbandonedAtTick { get; set; }
    public uint? EndedAtTick { get; set; }
    public uint? StartedAtTick { get; set; }
    public bool IsBotGame { get; set; }
    public string ServerNodeId { get; set; } = string.Empty;
    public string ServerVersion { get; set; } = string.Empty;
    public string? AbandonedServerNodeId { get; set; }
    public byte[]? AbandonedTrace { get; set; }
    public string? HostId { get; set; }
    public List<BenchmarkSpawnPoint>? SpareSpawnPoints { get; set; }
    public bool WasAborted { get; set; }
}

public enum BenchmarkGameStatus
{
    Created = 0,
    Ongoing = 1,
    Finished = 2
}

public enum BenchmarkPlatform
{
    Pc = 0,
    Console = 1,
    Mobile = 2
}

public enum BenchmarkAbility
{
    Overdrive = 1,
    Cloak = 2,
    Boost = 3,
    Radar = 4
}

public enum BenchmarkRoundStartingBehavior
{
    Default = 0,
    ManualAll = 1,
    ManualFirst = 2
}

public enum BenchmarkRuleset
{
    None = 0,
    Standard = 1,
    Hardcore = 2,
    Classic = 3
}

public enum BenchmarkGameContextType
{
    None = 0,
    Ladder = 3,
    Event = 7
}

public class BenchmarkGameContext
{
    public BenchmarkGameContextType Type { get; set; }
    public string Id { get; set; } = string.Empty;
}

public class BenchmarkGameOptions
{
    public int InitialHullPoints { get; set; } = 2_500;
    public bool IndividualInitialHullPoints { get; set; }
    public int InitialHullPointsSquadOne { get; set; } = 2_500;
    public int InitialHullPointsSquadTwo { get; set; } = 2_500;
    public int? DamageTakenSquadOne { get; set; }
    public int? DamageTakenSquadTwo { get; set; }
    public uint RoundTime { get; set; } = 90;
    public uint MaxRoundTime { get; set; }
    public uint RespawnDelay { get; set; }
    public int MatchTimeOut { get; set; } = 3600;
    public int MaxNumberOfRounds { get; set; }
    public bool? AllowDraw { get; set; }
    public int[] RepairRounds { get; set; } = [4];
    public BenchmarkPhysicsOptions PhysicsOptions { get; set; } = new();
    public string ArenaSlug { get; set; } = "nebula-7";
    public bool IsRanked { get; set; }
    public bool? UseDynamicLighting { get; set; }
    public BenchmarkArena? Arena { get; set; }
    public BenchmarkRoundOptions[]? RoundOptions { get; set; }
    public int RoundsBeforeOvertime { get; set; } = 6;
    public int OvertimeDamageIncrement { get; set; } = 10;
    public int StreakBonusIncrement { get; set; }
    public double? InitialDamageMultiplierSquadOne { get; set; }
    public double? InitialDamageMultiplierSquadTwo { get; set; }
    public bool DisableRepairs { get; set; }
    public bool IsTeamGame { get; set; }
    public BenchmarkGameContext? GameContext { get; set; }
    public BenchmarkRoundStartingBehavior RoundStartingBehavior { get; set; }
    public BenchmarkRuleset Ruleset { get; set; }
    public bool CountAllHits { get; set; }
    public bool HostAutoStartRounds { get; set; }
    public string SpawnPoolIdentifier { get; set; } = string.Empty;
    public bool UseHandPlacedSpawns { get; set; }
    public uint ExtraWaitTimeBetweenRounds { get; set; }
    public int RoundCountdownDelay { get; set; } = 3;
    public Dictionary<string, string>? BotBehaviors { get; set; }
    public bool AllowSpectators { get; set; }
    public bool SpectatorsSeeEnemyLoadouts { get; set; }
    public bool PublishLiveState { get; set; }
    public int SeriesGameNumber { get; set; }
    public string HudLayout { get; set; } = "compact";
    public int? SeasonPassTier { get; set; }
    public List<BenchmarkAbility> AbilityLoadout { get; set; } = [];
    public bool FogOfWar { get; set; }
    public int VisionRangeSquadOne { get; set; } = 800;
    public int VisionRangeSquadTwo { get; set; } = 800;
}

public class BenchmarkPhysicsOptions
{
    public bool DisableGravity { get; set; }
    public bool DisableFriendlyFire { get; set; }
    public bool DisableCollisions { get; set; }
    public bool ThrottleCap { get; set; }
}

public class BenchmarkArena
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public BenchmarkBoundingBox Bounds { get; set; } = new();
    public int MaxEngagementRange { get; set; }
}

public class BenchmarkBoundingBox
{
    public BenchmarkVector Min { get; set; } = new() { X = -1000, Y = -1000 };
    public BenchmarkVector Max { get; set; } = new() { X = 1000, Y = 1000 };
}

public class BenchmarkVector
{
    public double X { get; set; }
    public double Y { get; set; }
}

public class BenchmarkRoundOptions
{
    public int RoundNumber { get; set; }
    public double Multiplier { get; set; }
    public bool IsRepairRound { get; set; }
}

public class BenchmarkSpawnPoint
{
    public string? Id { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public string Sector { get; set; } = string.Empty;
    public string? Zone { get; set; }
    public double? Yaw { get; set; }
    public double? Pitch { get; set; }
    public double? Roll { get; set; }
}

public class BenchmarkRound
{
    public int RoundNumber { get; set; }
    public BenchmarkSpawnPoint SpawnPoint { get; set; } = new();
    public bool HasProcessedRoundTimeout { get; set; }
    public bool IsRepairRound { get; set; }
    public double Multiplier { get; set; } = 1.0;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime? TimerStartTime { get; set; }
    public bool IsPaused { get; set; }
    public HashSet<string>? PauseRequests { get; set; }
    public string? ForfeitedByPilotId { get; set; }
    public uint? StartTick { get; set; }
    public uint? EndAtTick { get; set; }
}

public class BenchmarkSquad
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int HullPoints { get; set; }
    public BenchmarkPilot[] Pilots { get; set; } = [];
    public List<BenchmarkRoundResult> RoundResults { get; set; } = [];
}

public class BenchmarkPilot
{
    public string PilotId { get; set; } = string.Empty;
    public string Faction { get; set; } = string.Empty;
    public List<BenchmarkShot> Shots { get; set; } = [];
    public int Rating { get; set; }
    public BenchmarkTargetLock? TargetLock { get; set; }
    public BenchmarkPlatform Platform { get; set; }
    public string? PublicProfile { get; set; }
    public bool IsConnected { get; set; }
    public bool IsBot { get; set; }
    public List<BenchmarkAbility> AbilityLoadout { get; set; } = [];
    public HashSet<int> RoundsWithCloak { get; set; } = [];
    public HashSet<int> RoundsWithBoost { get; set; } = [];
    public List<BenchmarkDamageMarker> DamageMarkers { get; set; } = [];
}

public class BenchmarkShot
{
    public int RoundNumber { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Range { get; set; }
    public DateTime FiredAt { get; set; }
    public int? Damage { get; set; }
}

public class BenchmarkTargetLock
{
    public int RoundNumber { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public DateTime LockedAt { get; set; }
}

public class BenchmarkRoundResult
{
    public int RoundNumber { get; set; }
    public int DamageDealt { get; set; }
    public int HullBefore { get; set; }
    public int HullAfter { get; set; }
}

public class BenchmarkDamageMarker
{
    public int RoundNumber { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public int Damage { get; set; }
}
