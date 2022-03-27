using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Entities.Enums;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.DataAccess.SharedTestUtils.TestData
{
    [ExcludeFromCodeCoverage]
    public static class TestGames
    {
        public static async Task InsertIntoDatabase(IMongoContext mongoContext, List<Game> games)
        {
            var gamesRepository = new GamesRepository(mongoContext);
            foreach (var game in games)
            {
                await gamesRepository.InsertGame(game);
            }
        }

        public static List<Game> GetAllTestGames()
        {
            return new List<Game>()
            {
                TestGame_2to1_Singles_NoOwnGoals,
                TestGame_1To2_Singles_AllOwnGoals,
                TestGame_1To3_Singles_2OwnGoals_1UndoneOwnGoal,
                TestGame_2To1_Singles_NoOwnGoals_SwappedPlayers,
                TestGame_3To1_Doubles_2GoGoal_1GdGoal_1BoGoal,
                TestGame_3To2_Doubles_3GoGoal_2BoGoal,
                TestGame_1To2_Doubles_1GoGoal_2GdOwnGoal,
                TestGame_1To2_Doubles_1GdGoal_2BoGoal,
                TestGame_2To3_Doubles_2GdGoal_2BoGoal_1BdGoal
            };
        }

        public static readonly Game TestGame_2to1_Singles_NoOwnGoals =
            new Game(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000000"), isInProgress: false)
            {
                GoldOffenseUserId = TestUsers.TestUser_1.Id,
                BlackOffenseUserId = TestUsers.TestUser_2.Id,
                GoldTeamScore = 2,
                BlackTeamScore = 1,
                IsDoubles = false,
                StartTimeUtc = DateTime.UtcNow.AddMinutes(-120),
                EndTimeUtc = DateTime.UtcNow.AddMinutes(-116),
                Goals = new List<Goal>()
                {
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000001"))
                    {
                        ScoringUserId = TestUsers.TestUser_1.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.BLACK,
                        TimeStampUtc = DateTime.UtcNow.AddSeconds(-30)
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000002"))
                    {
                        ScoringUserId = TestUsers.TestUser_1.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.BLACK,
                        TimeStampUtc = DateTime.UtcNow.AddSeconds(-25)
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000003"))
                    {
                        ScoringUserId = TestUsers.TestUser_2.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.GOLD,
                        TimeStampUtc = DateTime.UtcNow.AddSeconds(-20)
                    }
                }
            };

        public static readonly Game TestGame_1To2_Singles_AllOwnGoals =
            new Game(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000004"), isInProgress: false)
            {
                GoldOffenseUserId = TestUsers.TestUser_1.Id,
                BlackOffenseUserId = TestUsers.TestUser_2.Id,
                GoldTeamScore = 1,
                BlackTeamScore = 2,
                IsDoubles = false,
                StartTimeUtc = DateTime.UtcNow.AddMinutes(-110),
                EndTimeUtc = DateTime.UtcNow.AddMinutes(-104),
                Goals = new List<Goal>()
                {
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000005"))
                    {
                        ScoringUserId = TestUsers.TestUser_2.Id,
                        IsOwnGoal = true,
                        TeamScoredAgainst = TeamColor.BLACK
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000006"))
                    {
                        ScoringUserId = TestUsers.TestUser_1.Id,
                        IsOwnGoal = true,
                        TeamScoredAgainst = TeamColor.GOLD
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000007"))
                    {
                        ScoringUserId = TestUsers.TestUser_1.Id,
                        IsOwnGoal = true,
                        TeamScoredAgainst = TeamColor.GOLD
                    }
                }
            };

        public static readonly Game TestGame_1To3_Singles_2OwnGoals_1UndoneOwnGoal = 
            new Game(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000008"), isInProgress: false)
            {
                GoldOffenseUserId = TestUsers.TestUser_1.Id,
                BlackOffenseUserId = TestUsers.TestUser_2.Id,
                GoldTeamScore = 1,
                BlackTeamScore = 3,
                IsDoubles = false,
                StartTimeUtc = DateTime.UtcNow.AddMinutes(-105),
                EndTimeUtc = DateTime.UtcNow.AddMinutes(-100.5),
                Goals = new List<Goal>()
                {
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000009"))
                    {
                        ScoringUserId = TestUsers.TestUser_1.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.BLACK
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000010"))
                    {
                        ScoringUserId = TestUsers.TestUser_1.Id,
                        IsOwnGoal = true,
                        IsUndone = true,
                        TeamScoredAgainst = TeamColor.GOLD
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000011"))
                    {
                        ScoringUserId = TestUsers.TestUser_1.Id,
                        IsOwnGoal = true,
                        TeamScoredAgainst = TeamColor.GOLD
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000012"))
                    {
                        ScoringUserId = TestUsers.TestUser_1.Id,
                        IsOwnGoal = true,
                        TeamScoredAgainst = TeamColor.GOLD
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000013"))
                    {
                        ScoringUserId = TestUsers.TestUser_2.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.GOLD
                    }
                }
            };

        public static readonly Game TestGame_2To1_Singles_NoOwnGoals_SwappedPlayers = 
            new Game(id: Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000014"), isInProgress: false)
            {
                GoldOffenseUserId = TestUsers.TestUser_2.Id,
                BlackOffenseUserId = TestUsers.TestUser_1.Id,
                GoldTeamScore = 2,
                BlackTeamScore = 1,
                IsDoubles = false,
                StartTimeUtc = DateTime.UtcNow.AddMinutes(-80),
                EndTimeUtc = DateTime.UtcNow.AddMinutes(-70),
                Goals = new List<Goal>()
                {
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000015"))
                    {
                        ScoringUserId = TestUsers.TestUser_1.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.GOLD
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000016"))
                    {
                        ScoringUserId = TestUsers.TestUser_1.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.GOLD
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000017"))
                    {
                        ScoringUserId = TestUsers.TestUser_2.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.BLACK
                    }
                }
            };

        public static readonly Game TestGame_3To1_Doubles_2GoGoal_1GdGoal_1BoGoal = 
            new Game(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000018"), isInProgress: false)
            {
                GoldOffenseUserId = TestUsers.TestUser_1.Id,
                BlackOffenseUserId = TestUsers.TestUser_2.Id,
                GoldDefenseUserId = TestUsers.TestUser_3_Anon.Id,
                BlackDefenseUserId = TestUsers.TestUser_4.Id,
                GoldTeamScore = 3,
                BlackTeamScore = 1,
                IsDoubles = true,
                StartTimeUtc = DateTime.UtcNow.AddMinutes(-95),
                EndTimeUtc = DateTime.UtcNow.AddMinutes(-92),
                Goals = new List<Goal>()
                {
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000019"))
                    {
                        ScoringUserId = TestUsers.TestUser_1.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.BLACK
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000020"))
                    {
                        ScoringUserId = TestUsers.TestUser_2.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.GOLD
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000021"))
                    {
                        ScoringUserId = TestUsers.TestUser_3_Anon.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.BLACK
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000022"))
                    {
                        ScoringUserId = TestUsers.TestUser_1.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.BLACK
                    },
                }
            };
        
        public static readonly Game TestGame_3To2_Doubles_3GoGoal_2BoGoal = 
            new Game(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000023"), false)
            {
                GoldOffenseUserId = TestUsers.TestUser_1.Id,
                BlackOffenseUserId = TestUsers.TestUser_2.Id,
                GoldDefenseUserId = TestUsers.TestUser_3_Anon.Id,
                BlackDefenseUserId = TestUsers.TestUser_4.Id,
                GoldTeamScore = 3,
                BlackTeamScore = 2,
                IsDoubles = true,
                StartTimeUtc = DateTime.UtcNow.AddMinutes(-105),
                EndTimeUtc = DateTime.UtcNow.AddMinutes(-102),
                Goals = new List<Goal>()
                {
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000024"))
                    {
                        ScoringUserId = TestUsers.TestUser_1.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.BLACK
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000025"))
                    {
                        ScoringUserId = TestUsers.TestUser_1.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.BLACK
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000026"))
                    {
                        ScoringUserId = TestUsers.TestUser_1.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.BLACK
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000027"))
                    {
                        ScoringUserId = TestUsers.TestUser_2.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.GOLD
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000028"))
                    {
                        ScoringUserId = TestUsers.TestUser_2.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.GOLD
                    }
                }
            };

        public static readonly Game TestGame_1To2_Doubles_1GoGoal_2GdOwnGoal =
            new Game(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000029"))
            {
                GoldOffenseUserId = TestUsers.TestUser_1.Id,
                BlackOffenseUserId = TestUsers.TestUser_3_Anon.Id,
                GoldDefenseUserId = TestUsers.TestUser_2.Id,
                BlackDefenseUserId = TestUsers.TestUser_4.Id,
                GoldTeamScore = 1,
                BlackTeamScore = 2,
                IsDoubles = true,
                StartTimeUtc = DateTime.UtcNow.AddMinutes(-107),
                EndTimeUtc = DateTime.UtcNow.AddMinutes(-101),
                Goals = new List<Goal>()
                {
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000030"))
                    {
                        ScoringUserId = TestUsers.TestUser_1.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.BLACK
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000031"))
                    {
                        ScoringUserId = TestUsers.TestUser_2.Id,
                        IsOwnGoal = true,
                        TeamScoredAgainst = TeamColor.GOLD
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000032"))
                    {
                        ScoringUserId = TestUsers.TestUser_2.Id,
                        IsOwnGoal = true,
                        TeamScoredAgainst = TeamColor.GOLD
                    }
                }
            };
        
        public static readonly Game TestGame_1To2_Doubles_1GdGoal_2BoGoal =
            new Game(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000033"))
            {
                GoldOffenseUserId = TestUsers.TestUser_1.Id,
                BlackOffenseUserId = TestUsers.TestUser_3_Anon.Id,
                GoldDefenseUserId = TestUsers.TestUser_4.Id,
                BlackDefenseUserId = TestUsers.TestUser_2.Id,
                GoldTeamScore = 1,
                BlackTeamScore = 2,
                IsDoubles = true,
                StartTimeUtc = DateTime.UtcNow.AddMinutes(-117),
                EndTimeUtc = DateTime.UtcNow.AddMinutes(-114),
                Goals = new List<Goal>()
                {
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000034"))
                    {
                        ScoringUserId = TestUsers.TestUser_4.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.BLACK
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000035"))
                    {
                        ScoringUserId = TestUsers.TestUser_3_Anon.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.GOLD
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000036"))
                    {
                        ScoringUserId = TestUsers.TestUser_3_Anon.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.GOLD
                    }
                }
            };
        
        public static readonly Game TestGame_2To3_Doubles_2GdGoal_2BoGoal_1BdGoal =
            new Game(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000036"))
            {
                GoldOffenseUserId = TestUsers.TestUser_1.Id,
                BlackOffenseUserId = TestUsers.TestUser_3_Anon.Id,
                GoldDefenseUserId = TestUsers.TestUser_4.Id,
                BlackDefenseUserId = TestUsers.TestUser_2.Id,
                GoldTeamScore = 2,
                BlackTeamScore = 3,
                IsDoubles = true,
                StartTimeUtc = DateTime.UtcNow.AddMinutes(-120),
                EndTimeUtc = DateTime.UtcNow.AddMinutes(-117),
                Goals = new List<Goal>()
                {
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000037"))
                    {
                        ScoringUserId = TestUsers.TestUser_4.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.BLACK
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000038"))
                    {
                        ScoringUserId = TestUsers.TestUser_4.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.BLACK
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000039"))
                    {
                        ScoringUserId = TestUsers.TestUser_3_Anon.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.BLACK
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000040"))
                    {
                        
                        ScoringUserId = TestUsers.TestUser_3_Anon.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.BLACK
                    },
                    new Goal(Guid.Parse("d1d94d14-4b03-4d5a-aa1e-000000000041"))
                    {
                        ScoringUserId = TestUsers.TestUser_2.Id,
                        IsOwnGoal = false,
                        TeamScoredAgainst = TeamColor.BLACK
                    }
                }
            };
    }
}
