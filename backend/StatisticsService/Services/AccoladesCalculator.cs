using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Entities.Enums;
using YouFoos.DataAccess.Repositories;
using static YouFoos.DataAccess.Entities.Enums.StatsCategory;
using static YouFoos.DataAccess.Entities.Enums.AccoladeConnotation;
using static YouFoos.SharedLibrary.Resources.Strings.AccoladeTitles;

namespace YouFoos.StatisticsService.Services
{
    public class AccoladesCalculator : IAccoladesCalculator
    {
        private readonly IAccoladesRepository _accoladesRepository;
        private readonly IStatsRepository _statsRepository;
        private readonly IGamesRepository _gamesRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccoladesCalculator(
            IAccoladesRepository accoladesRepository, 
            IStatsRepository statsRepository,
            IGamesRepository gamesRepository)
        {
            _accoladesRepository = accoladesRepository;
            _statsRepository = statsRepository;
            _gamesRepository = gamesRepository;
        }

        public async Task RecalculateAllAccolades()
        {
            Log.Logger.Verbose("Starting recalculation of all accolades");

            var accolades = new List<Accolade>
            {
                // Player specific max or min stat value accolades
                await CalcPlayerSpecificStatAccolade<double>(StatsOverall, "Rank", HighestRank, Positive),
                await CalcPlayerSpecificStatAccolade<double>(StatsOverall, "GoalsPerMinute", HighestScorePerMinute, Positive),
                await CalcPlayerSpecificStatAccolade<double>(StatsOverall, "TotalTimePlayedSecs", MostTimePlayed, Neutral),
                await CalcPlayerSpecificStatAccolade<double>(StatsOverall, "AverageGameLengthSecs", LongestAvgGameLength, Neutral),
                await CalcPlayerSpecificStatAccolade<int>(StatsOverall, "LongestWinStreak", LongestWinStreak, Positive),
                await CalcPlayerSpecificStatAccolade<int>(StatsOverall, "LongestLossStreak", LongestLossStreak, Negative),
                await CalcPlayerSpecificStatAccolade<int>(StatsOverall, "ShutoutWins", MostShutoutWins, Positive),
                await CalcPlayerSpecificStatAccolade<double>(StatsOverall, "Rank", LowestRank, Negative, min: true),
                await CalcMaxOrMinWinrate(),
                await CalcMaxOrMinWinrate(min: true),

                // Per game average min or max player stat accolades
                await CalcPerGameAvgPlayerStatAccolade(Stats2V2, "GoalsScoredAsDefense", MostGoalsAsDefensePerGame, Positive),
                await CalcPerGameAvgPlayerStatAccolade(StatsOverall, "OwnGoals", MostOwngoalsPerGame, Negative),

                // Non-player entity specific accolades
                await CalcBestTableSide(),

                // Overall system stat accolades
                await CalcTotalTimePlayedSecs(),
                await CalcSystemTotalGoalsScored()
            };

            accolades.RemoveAll(a => a == null);

            await _accoladesRepository.InsertAccolades(accolades);

            Log.Logger.Information("Recalculated all accolades");
        }

        public async Task<Accolade> CalcPlayerSpecificStatAccolade<T>(
            StatsCategory statCategory, string statName, string accoladeName, AccoladeConnotation connotation, bool min = false)
        {
            var value = await _statsRepository.GetMaxOrMinStatValueForAccolades<T>(statCategory, statName, min);
            if (value == null) return null;
            string statValueStr = GetFormattedValue(statName, value.StatValue);

            return new Accolade(accoladeName, connotation, statValueStr, value.UserId);
        }

        public async Task<Accolade> CalcPerGameAvgPlayerStatAccolade(
            StatsCategory statCategory, string statName, string accoladeName, AccoladeConnotation connotation, bool min = false)
        {
            var value = await _statsRepository.GetMaxOrMinStatValuePerGameAvgForAccolades<double>(statCategory, statName, min);
            if (value == null) return null;
            string statValueStr = GetFormattedValue(statName, value.StatValue);

            return new Accolade(accoladeName, connotation, statValueStr, value.UserId);
        }

        public async Task<Accolade> CalcTotalTimePlayedSecs()
        {
            var totalTimeSeconds = await _gamesRepository.CountTimePlayedSecs();
            string statValueStr = GetFormattedValue("TotalTimePlayedSecs", totalTimeSeconds);
            return new Accolade(TotalTimePlayed, Neutral, statValueStr);
        }

        public async Task<Accolade> CalcSystemTotalGoalsScored()
        {
            var totalGoals = await _gamesRepository.CountGoalsScored();
            return new Accolade(OverallGoalsScored, Neutral, totalGoals.ToString());
        }

        public async Task<Accolade> CalcBestTableSide()
        {
            var goldSideWins = await _gamesRepository.CountTeamWins(TeamColor.GOLD);
            var blackSideWins = await _gamesRepository.CountTeamWins(TeamColor.BLACK);

            var bestTeam = (goldSideWins >= blackSideWins) ? TeamColor.GOLD : TeamColor.BLACK;

            double bestTeamWinrate = 0;
            int totalGames = goldSideWins + blackSideWins;
            if (bestTeam == TeamColor.GOLD)
            {
                bestTeamWinrate = (double) goldSideWins / totalGames;
            }
            else
            {
                bestTeamWinrate = (double) blackSideWins / totalGames;
            }

            string statValueStr = GetFormattedValue("Winrate", bestTeamWinrate);
            return new Accolade(BestTableSide, Neutral, statValueStr, null, bestTeam.ToString());
        }

        public async Task<Accolade> CalcMaxOrMinWinrate(bool min = false)
        {
            var connotation = min ? Negative : Positive;
            var title = min ? LowestWinrate : HighestWinrate;
            var result = await _statsRepository.CalcMaxOrMinWinrate(min);
            if (result == null) return null;
            string statValueStr = GetFormattedValue("Winrate", result.StatValue);

            return new Accolade(title, connotation, statValueStr, result.UserId);
        }

        private static string GetFormattedValue(string statName, object value)
        {
            string result;

            switch (value)
            {
                case double d:
                {
                    switch (statName)
                    {
                        case "Rank":
                        case "TotalTimePlayedSecs":
                        case "LongestGameLengthSecs":
                            result = d.ToString("F0");
                            break;
                        default:
                            result = d.ToString("F3");
                            break;
                    }
                    
                    break;
                }
                default:
                    result = value.ToString();
                    break;
            }

            return result;
        }
    }
}
