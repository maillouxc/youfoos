using System;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Entities.Stats;
using YouFoos.StatisticsService.Utils;

namespace YouFoos.StatisticsService.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="ITrueskillCalculator"/>.
    /// </summary>
    public class TrueskillCalculator : ITrueskillCalculator
    {
        /// <summary>
        /// Beta is a tweakable constant that describes how far things are spread apart in the TrueSkill system.
        /// 
        /// You can think of it as the number of skill points to guarantee abount an 80% win chance over someone else.
        /// 
        /// The value chosen here is not magic but has simply been chosen because it works well.
        /// By default, it's a good start to set beta to half the initial standard deviation.
        /// </summary>
        public const double Beta = Trueskill.DefaultInitialMean / 6.0;

        /// <summary>
        /// Often called tau, this property helps stop rankings from stagnating, by introducing volatility to rankings.
        /// 
        /// Without tau, TrueSkill would always cause a player's skill standard deviation to decrease, meaning
        /// that if a player who has played for a long time but suddenly learns an awesome new shot would find
        /// it extremely difficult to climb the leaderboard with their newfound skills.
        /// </summary>
        public const double DynamicsFactor = Trueskill.DefaultInitialMean / 300.0;

        public void CalculateNewRatings1V1(User winner, User loser)
        {
            // TODO check nulls

            double winMeanDelta = winner.Skill1V1.Mean - loser.Skill1V1.Mean;
            double loseMeanDelta = loser.Skill1V1.Mean - winner.Skill1V1.Mean;
            double sumSqStdDevs = Square(winner.Skill1V1.StdDev) + Square(loser.Skill1V1.StdDev);

            // We can't assign the skill directly because then one user's skill would affect the calculation of the other's
            var newWinnerSkill = CalculateNewSkill(winner.Skill1V1, won: true, isDoubles: false, winMeanDelta, sumSqStdDevs);
            var newLoserSkill = CalculateNewSkill(loser.Skill1V1, won: false, isDoubles: false, loseMeanDelta, sumSqStdDevs);

            winner.Skill1V1 = newWinnerSkill;
            loser.Skill1V1 = newLoserSkill;
        }

        public void CalculateNewRatings2V2(User winningOffense, User winningDefense, User losingOffense, User losingDefense)
        {
            // TODO check nulls
 
            double winSumSqStdDevs = Square(winningOffense.Skill2V2.StdDev) + Square(winningDefense.Skill2V2.StdDev);
            double loseSumSqStdDevs = Square(losingOffense.Skill2V2.StdDev) + Square(losingDefense.Skill2V2.StdDev);
            double sumSqStdDevs = winSumSqStdDevs + loseSumSqStdDevs;

            double winSumRatingMean = winningOffense.Skill2V2.Mean + winningDefense.Skill2V2.Mean;
            double loseSumRatingMean = losingOffense.Skill2V2.Mean + losingDefense.Skill2V2.Mean;
            double meanDelta = winSumRatingMean - loseSumRatingMean;

            var winOffNewSkill = CalculateNewSkill(winningOffense.Skill2V2, won: true, isDoubles: true, meanDelta, sumSqStdDevs);
            var winDefNewSkill = CalculateNewSkill(winningDefense.Skill2V2, won: true, isDoubles: true, meanDelta, sumSqStdDevs);
            var loseOffNewSkill = CalculateNewSkill(losingOffense.Skill2V2, won: false, isDoubles: true, meanDelta, sumSqStdDevs);
            var loseDefNewSkill = CalculateNewSkill(losingDefense.Skill2V2, won: false, isDoubles: true, meanDelta, sumSqStdDevs);

            winningOffense.Skill2V2 = winOffNewSkill;
            winningDefense.Skill2V2 = winDefNewSkill;
            losingOffense.Skill2V2 = loseOffNewSkill;
            losingDefense.Skill2V2 = loseDefNewSkill;
        }

        public double CalculateMatchQuality1V1(User goldPlayer, User blackPlayer)
        {
            // TODO check nulls

            const int totalPlayers = 2;
            double betaSquared = Square(Beta);
            double sumSquaredStdDevs = Square(goldPlayer.Skill1V1.StdDev) + Square(blackPlayer.Skill1V1.StdDev);
            double squaredRatingMeanDifference = Square(goldPlayer.Skill1V1.Mean - blackPlayer.Skill1V1.Mean);
            double denominator = totalPlayers * betaSquared + sumSquaredStdDevs;
            double sqrtPart = Math.Sqrt((totalPlayers * betaSquared) / denominator);
            double expPart = Math.Exp((-1 * squaredRatingMeanDifference) / (2 * denominator));

            return expPart * sqrtPart;
        }

        public double CalculateMatchQuality2V2(User goldOffense, User goldDefense, User blackOffense, User blackDefense)
        {
            // TODO check nulls
            
            const int totalPlayers = 4;

            double goldSumSqStdDevs = Square(goldOffense.Skill2V2.StdDev) + Square(goldDefense.Skill2V2.StdDev);
            double blackSumSqStdDevs = Square(blackOffense.Skill2V2.StdDev) + Square(blackDefense.Skill2V2.StdDev);
            double sumSquaredStdDevs = goldSumSqStdDevs + blackSumSqStdDevs;
            double goldSumRatingMean = goldOffense.Skill2V2.Mean + goldDefense.Skill2V2.Mean;
            double blackSumRatingMean = blackOffense.Skill2V2.Mean + blackDefense.Skill2V2.Mean;
            double squaredDiffRatingsMean = Square(goldSumRatingMean - blackSumRatingMean);
            
            double denominator = (totalPlayers * Square(Beta)) + sumSquaredStdDevs;
            double sqrtPart = Math.Sqrt((totalPlayers * Square(Beta)) / denominator);
            double expPart = Math.Exp((-1 * squaredDiffRatingsMean) / (2 * denominator));

            return expPart * sqrtPart;
        }

        private static Trueskill CalculateNewSkill(Trueskill currentSkill, bool won, bool isDoubles, double meanDelta, double sumSqStdDevs)
        {
            if (currentSkill == null) throw new ArgumentNullException(nameof(currentSkill));

            int totalPlayers = isDoubles ? 4 : 2;

            double drawMargin = GaussianFunctions.InverseCumulativeTo(.5 * 1, 0, 1) * Math.Sqrt(2) * Beta;
            double c = Math.Sqrt(sumSqStdDevs + (totalPlayers * Square(Beta)));
            double v = VExceedsMargin(meanDelta, drawMargin, c);
            double w = WExceedsMargin(meanDelta, drawMargin, c);

            double volatileStdDev = Square(currentSkill.StdDev) + Square(DynamicsFactor);
            double meanMultiplier = volatileStdDev / c;
            double stdDevMultiplier = volatileStdDev / Square(c);
            double sign = won ? 1 : -1;

            double newStdDev = Math.Sqrt(volatileStdDev * (1 - w * stdDevMultiplier));
            double newMean = currentSkill.Mean + (sign * meanMultiplier * v);

            return new Trueskill(newMean, newStdDev);
        }

        private static double VExceedsMargin(double teamPerformanceDifference, double drawMargin, double c)
        {
            return VExceedsMargin(teamPerformanceDifference / c, drawMargin / c);
        }

        private static double VExceedsMargin(double teamPerformanceDifference, double drawMargin)
        {
            double denominator = GaussianFunctions.CumulativeTo(teamPerformanceDifference - drawMargin);

            if (denominator < 2.222758749e-162) return -teamPerformanceDifference + drawMargin;

            return GaussianFunctions.At(teamPerformanceDifference - drawMargin) / denominator;
        }

        private static double WExceedsMargin(double teamPerformanceDifference, double drawMargin, double c)
        {
            return WExceedsMargin(teamPerformanceDifference / c, drawMargin / c);
        }

        private static double WExceedsMargin(double teamPerformanceDifference, double drawMargin)
        {
            double denominator = GaussianFunctions.CumulativeTo(teamPerformanceDifference - drawMargin);

            if (denominator < 2.222758749e-162) return (teamPerformanceDifference < 0.0) ? 1.0 : 0.0;

            double vWin = VExceedsMargin(teamPerformanceDifference, drawMargin);
            return vWin * (vWin + teamPerformanceDifference - drawMargin);
        }

        private static double Square(double value)
        {
            return value * value;
        }
    }
}
