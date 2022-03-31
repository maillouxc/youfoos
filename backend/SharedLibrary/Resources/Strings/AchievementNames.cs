using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YouFoos.SharedLibrary.Resources.Strings
{
    /// <summary>
    /// The names of all achievements in the YouFoos system.
    /// </summary>
    public static class AchievementNames
    {
        #pragma warning disable CS1591

        public const string LookMom = "Look, Mom!";
        public const string IWasntReady = "I Wasn't Ready!";
        public const string ThankUNext = "Thank U, Next.";
        public const string OnARoll = "On a Roll";
        public const string TheBestOffense = "The Best Offense...";
        public const string ItsNotAnAddiction = "It's Not An Addiction";
        public const string Penultimate = "Penultimate";
        public const string SlowRoller = "Slow Roller";
        public const string ComebackKing = "Comeback King";
        public const string Seppuku = "Seppuku";
        public const string SoreBack = "Sore Back";
        public const string KingOfTheWorld = "King of the World";
        public const string ReproducableBug = "Reproducible Bug";

        #pragma warning restore CS1591

        /// <summary>
        /// Returns the names of all achievements, as a list of strings.
        /// </summary>
        public static List<string> GetAll()
        {
            FieldInfo[] fields = typeof(AchievementNames).GetFields(BindingFlags.Public | BindingFlags.Static);

            return fields
                .Where(f => f.FieldType == typeof(string))
                .Where(f => f.IsLiteral && !f.IsInitOnly)
                .Select(f => (string) f.GetValue(obj: null))
                .ToList();
        }
    }
}
