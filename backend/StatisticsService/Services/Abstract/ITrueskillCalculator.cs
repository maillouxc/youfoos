using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.StatisticsService.Services
{
    public interface ITrueskillCalculator
    {
        void CalculateNewRatings1V1(User winner, User loser);

        void CalculateNewRatings2V2(User winingOffense, User winningDefense, User losingOffense, User losingDefense);

        double CalculateMatchQuality1V1(User goldPlayer, User blackPlayer);

        double CalculateMatchQuality2V2(User goldOffense, User goldDefense, User blackOffense, User blackDefense);
    }
}
