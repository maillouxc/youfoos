using MongoDB.Bson.Serialization.Attributes;
using YouFoos.DataAccess.Entities.Enums;

namespace YouFoos.DataAccess.Entities
{
    /// <summary>
    /// Represents an accolade in the system.
    /// 
    /// You can think of accolades as sort of superlatives - like "Highest Winrate", etc.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Accolade
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the accolade.</param>
        /// <param name="connotation">The connotation of the accolade - whether it is good or bad to have.</param>
        /// <param name="currentValue">The current value of the stat that the accolade is for.</param>
        /// <param name="userId">The ID of the user that the accolade is for, if there is one.</param>
        /// <param name="entityName">The name of the entity that the accolade is for, if applicable.</param>
        public Accolade(string name, 
                        AccoladeConnotation connotation, 
                        string currentValue, 
                        string userId = null, 
                        string entityName = null)
        {
            Name = name;
            Connotation = connotation;
            CurrentValue = currentValue;
            UserId = userId;
            EntityName = entityName;

            if (userId != null)
            {
                Type = "PlayerSpecific";
            }
            else if (entityName != null)
            {
                Type = "NonPlayerEntitySpecific";
            }
            else
            {
                Type = "NonEntitySpecific";
            }
        }

        /// <summary>
        /// The type of the accolade - either PlayerSpecific, NonPlayerEntitySpecific, or NonEntitySpecific.
        /// </summary>
        public string Type { get; set;  }

        /// <summary>
        /// The name of the accolade.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ID of the user who holds the accolade. Should be null if type is not PlayerSpecific.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The name of the entity that the accolade has been awarded to. Will be null if type is not NonPlayerEntitySpecific.
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Whether the accolade is considered a Positive, Neutral, or Bad thing to have.
        /// </summary>
        public AccoladeConnotation Connotation { get; set; }

        /// <summary>
        /// The current value of the stat tracked by the accolade.
        /// </summary>
        public string CurrentValue { get; set; }
    }
}
