using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using YouFoos.SharedLibrary.Resources.Strings;

namespace YouFoos.DataAccess.Entities
{
    /// <summary>
    /// Represents the current progress of a user toward unlocking a particular achievement.
    /// </summary>
    public class AchievementStatus
    {
        /// <summary>
        /// Only used so we can stick this object into MongoDB without errors.
        /// </summary>
        [JsonIgnore]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string Id { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AchievementStatus(string userId, string name, int eventsNeeded)
        {
            UserId = userId;
            Name = name;
            NumRequired = eventsNeeded;
            NumQualifyingEvents = 0;
        }

        /// <summary>
        /// The user ID of the user who this status item is for.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The name of the achievement.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The textual description of the achievement.
        /// </summary>
        [BsonIgnore]
        public string Description
        {
            get
            {
                // To save db space and performance, we don't store the description - we generate it on the fly.
                return Name switch
                {
                    // We could use fancy reflection here to get the variable with the same name, but that's gross.
                    AchievementNames.ComebackKing => AchievementDescriptions.ComebackKing,
                    AchievementNames.ItsNotAnAddiction => AchievementDescriptions.ItsNotAnAddiction,
                    AchievementNames.IWasntReady => AchievementDescriptions.IWasntReady,
                    AchievementNames.KingOfTheWorld => AchievementDescriptions.KingOfTheWorld,
                    AchievementNames.LookMom => AchievementDescriptions.LookMom,
                    AchievementNames.OnARoll => AchievementDescriptions.OnARoll,
                    AchievementNames.Penultimate => AchievementDescriptions.Penultimate,
                    AchievementNames.Seppuku => AchievementDescriptions.Seppuku,
                    AchievementNames.SlowRoller => AchievementDescriptions.SlowRoller,
                    AchievementNames.SoreBack => AchievementDescriptions.SoreBack,
                    AchievementNames.ThankUNext => AchievementDescriptions.ThankUNext,
                    AchievementNames.TheBestOffense => AchievementDescriptions.TheBestOffense,
                    AchievementNames.ReproducableBug => AchievementDescriptions.ReproducibleBug,
                    _ => throw new ArgumentOutOfRangeException(nameof(Name))
                };
            }

            private set {}
        }

        private int _numQualifyingEvents;
        
        /// <summary>
        /// The number of things the user has done that meet the criteria of the achievement.
        /// 
        /// This is useful for achievements that need the user to do a thing x number of times.
        /// If an achievement does not need to be done x number of times, just set this to 1.
        /// </summary>
        public int NumQualifyingEvents 
        {
            get
            {
                return _numQualifyingEvents;
            }
            set 
            {
                if (value > NumRequired)
                {
                    value = NumRequired;
                }

                _numQualifyingEvents = value;
            }
        }

        /// <summary>
        /// The number of qualifying events required to unlock the achievement.
        /// </summary>
        public int NumRequired { get; private set; }

        /// <summary>
        /// The integer value of progress towards completion of the achievement.
        /// 
        /// This value is updated automatically whenever the number of qualifying events changes.
        /// </summary>
        public int ProgressPercent { get; private set; }

        /// <summary>
        /// When the achievement was unlocked, if it has been unlocked.
        /// </summary>
        public DateTime? UnlockedDateTime { get; set; } = null;

        /// <summary>
        /// This method should be called whenever the progress on an achievement is updated.
        /// 
        /// Unfotunately, we cannot call it automatically because it needs an input of the date that the most recent game was played.
        /// If we don't pass in that date, we cannot accurately track achievement unlock date when doing remediation.
        /// </summary>
        public void RecalculateProgress(DateTime mostRecentGameDate)
        {
            ProgressPercent = (100 * NumQualifyingEvents) / NumRequired;

            if (ProgressPercent == 100 && UnlockedDateTime == null)
            {
                UnlockedDateTime = mostRecentGameDate;
            }
        }
    }
}
