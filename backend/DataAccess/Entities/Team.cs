using System;
using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.DataAccess.Entities
{
    public class Team
    {
        public User Player1;

        public User Player2;

        public Team(User player1, User player2)
        {
            Player1 = player1;
            Player2 = player2;

            if (player1 == null)
            {
                throw new ArgumentNullException("Player1 cannot be null.");
            }
        }

        public double GetSummedRatings()
        {
            if (Player2 == null)
            {
                return Player1.Skill1V1.ConservativeRating;
            }

            return Player1.Skill2V2.ConservativeRating + Player2.Skill2V2.ConservativeRating;
        }
    }
}
