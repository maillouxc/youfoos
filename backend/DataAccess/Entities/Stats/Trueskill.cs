namespace YouFoos.DataAccess.Entities.Stats
{
    /// <summary>
    /// Represents a rating under Microsoft's TrueSkill system.
    /// 
    /// This rating can be considered to be a Gaussian distribution and does not have a single skill value, unlike ELO.
    /// </summary>
    public class Trueskill
    {
        /// <summary>
        /// The initial value used for the mean.
        /// 
        /// The value is not magic - but was chosen simply because testing showed it produces good results.
        /// 
        /// To get a reasonable starting value, consider that we around 99% of the skills to be positive initially.
        /// This implies that we want "0" to be around 3 standard deviations away from the mean. 
        /// This also implies that the initial standard deviation must be s_0 = m_0 / 3.
        /// </summary>
        public const double DefaultInitialMean = 25.0;

        /// <summary>
        /// The initial value used for the standard deviation.
        /// 
        /// The value is not magic - but rather a value that was chosen simply because testing showed it produces good results.
        /// </summary>
        public const double DefaultInitialStdDev = DefaultInitialMean / 3.0;

        /// <summary>
        /// The mean value of the player's skill curve - often referred to as greek letter mu.
        /// </summary>
        public double Mean { get; private set; }
        
        /// <summary>
        /// The standard deviation of the player's skill curve - often referred to as greek letter sigma.
        /// </summary>
        public double StdDev { get; private set; }

        /// <summary>
        /// A very conservative unitless estimate of the player's actual skill level.
        /// This is the numerical value that can be used to rank players on a leaderboard.
        /// 
        /// This value represents 3 standard deviations to the left of the mean of the player's skill curve.
        /// 
        /// It's very likely that the player is actually better than this value, but extremely unlikely they are worse,
        /// which is why it is considered one of the best ways to order players under the TrueSkill system.
        /// </summary>
        public double ConservativeRating
        {
            get { return Mean - 3 * StdDev; }
            private set {}
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Trueskill(double mean = DefaultInitialMean, double stdDev = DefaultInitialStdDev)
        {
            Mean = mean;
            StdDev = stdDev;
        }

        /// <summary>
        /// Converts the object to a human friendly string.
        /// </summary>
        public override string ToString()
        {
            return string.Format("μ={0:0.0000}, σ={1:0.0000}", Mean, StdDev);
        }
    }
}
