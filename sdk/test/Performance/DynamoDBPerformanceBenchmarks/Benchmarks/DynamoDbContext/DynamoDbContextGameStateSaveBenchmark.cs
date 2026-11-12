using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using BenchmarkDotNet.Attributes;

namespace AWSSDK.Benchmarks.MockedDynamoDB.DynamoDbContext;

[MemoryDiagnoser]
public class DynamoDbContextGameStateSaveBenchmark
{
    private const int RoundCount = 8;
    private const int BatchSize = 25;
    private const int TraceBytes = 5 * 1024;

    private IDynamoDBContext _context = null!;
    private BenchmarkGameState _game = null!;
    private List<BenchmarkGameState> _abandonedGames = null!;

    [GlobalSetup]
    public void Setup()
    {
        var responseProvider = new DynamoDBResponseProvider(
            BenchmarkItemSize.Small,
            BenchmarkAttributeCount.Count10,
            BenchmarkAnnotationStyle.Minimal,
            BenchmarkObjectComplexity.Flat);
        var client = MockDynamoDbClientFactory.Create(responseProvider);

        _context = new DynamoDBContextBuilder()
            .ConfigureContext(config =>
            {
                config.DisableFetchingTableMetadata = true;
                config.Conversion = DynamoDBEntryConversion.V2;
            })
            .WithDynamoDBClient(() => client)
            .Build();

        _game = CreateFinishedGame("game-0");
        _abandonedGames = Enumerable.Range(0, BatchSize).Select(CreateAbandonedGame).ToList();
    }

    [Benchmark]
    public Task SaveAsync() => _context.SaveAsync(_game);

    [Benchmark]
    public Task BatchWriteAsync()
    {
        var batch = _context.CreateBatchWrite<BenchmarkGameState>();
        batch.AddPutItems(_abandonedGames);
        return batch.ExecuteAsync();
    }

    private static BenchmarkGameState CreateAbandonedGame(int index)
    {
        var game = CreateFinishedGame($"game-{index}");
        game.Status = BenchmarkGameStatus.Ongoing;
        game.AbandonedServerNodeId = game.ServerNodeId;
        game.ServerNodeId = string.Empty;
        game.AbandonedTrace = new byte[TraceBytes];
        new Random(index).NextBytes(game.AbandonedTrace);
        return game;
    }

    private static BenchmarkGameState CreateFinishedGame(string gameId)
    {
        var random = new Random(42);
        var start = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        var rounds = Enumerable.Range(1, RoundCount).Select(roundNumber => new BenchmarkRound
        {
            RoundNumber = roundNumber,
            SpawnPoint = new BenchmarkSpawnPoint
            {
                Id = Convert.ToBase64String(NextGuid(random).ToByteArray())[..22],
                X = random.NextDouble() * 2000 - 1000,
                Y = random.NextDouble() * 2000 - 1000,
                Sector = "c3",
                Yaw = random.NextDouble() * 360,
                Pitch = random.NextDouble() * 10,
                Roll = 0
            },
            Multiplier = 1 + (roundNumber - 1) * 0.5,
            IsRepairRound = roundNumber == 4,
            StartTime = start.AddSeconds(roundNumber * 120),
            TimerStartTime = start.AddSeconds(roundNumber * 120 + 10),
            EndTime = start.AddSeconds(roundNumber * 120 + 100),
            StartTick = (uint)(roundNumber * 1200),
            EndAtTick = (uint)(roundNumber * 1200 + 1000)
        }).ToArray();

        BenchmarkSquad CreateSquad(string name, string faction, string pilotId)
        {
            var pilot = new BenchmarkPilot
            {
                PilotId = pilotId,
                Faction = faction,
                Rating = 1450,
                IsConnected = true
            };
            var squad = new BenchmarkSquad
            {
                Id = NextGuid(random),
                Name = name,
                HullPoints = 900,
                Pilots = [pilot]
            };

            foreach (var round in rounds)
            {
                pilot.Shots.Add(new BenchmarkShot
                {
                    RoundNumber = round.RoundNumber,
                    X = random.NextDouble() * 2000 - 1000,
                    Y = random.NextDouble() * 2000 - 1000,
                    Range = random.NextDouble() * 4_000,
                    FiredAt = round.StartTime!.Value.AddSeconds(45),
                    Damage = random.Next(0, 500)
                });
                squad.RoundResults.Add(new BenchmarkRoundResult
                {
                    RoundNumber = round.RoundNumber,
                    DamageDealt = random.Next(0, 500),
                    HullBefore = 2500,
                    HullAfter = 2300
                });
            }

            return squad;
        }

        return new BenchmarkGameState
        {
            GameId = gameId,
            Status = BenchmarkGameStatus.Finished,
            Version = 3,
            CurrentRoundNumber = RoundCount,
            Options = new BenchmarkGameOptions
            {
                IsRanked = true,
                Ruleset = BenchmarkRuleset.Standard,
                Arena = new BenchmarkArena
                {
                    Name = "Nebula Seven",
                    Slug = "nebula-7",
                    MaxEngagementRange = 4_000
                },
                SpawnPoolIdentifier = "ranked-standard"
            },
            Squads =
            [
                CreateSquad("alpha", "vanguard", "a71e3c90b4d24f8e9c1b2a3d"),
                CreateSquad("omega", "syndicate", "c2f84d1e7a9b40c3b5d6e7f8")
            ],
            Rounds = rounds,
            Tick = 12000,
            StartedAtTick = 1200,
            EndedAtTick = 10600,
            ServerNodeId = "9b2d7e41c0a34f5e8d6c1b0a2f3e4d5c",
            ServerVersion = "1.2.3",
            SpareSpawnPoints = []
        };
    }

    private static Guid NextGuid(Random random)
    {
        var bytes = new byte[16];
        random.NextBytes(bytes);
        return new Guid(bytes);
    }
}
