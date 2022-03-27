using System;

namespace YouFoos.DataAccess.Entities.Stats
{
    /// <summary>
    /// Represents a datapoint of a single stat value at a given time, that can be used for graphing.
    /// </summary>
    public class StatHistoryDataPoint
    {
        /// <summary>
        /// The time that the value of the stat was captured at.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The value of the stat at the time indiciated in the Timestamp.
        /// </summary>
        public object Value { get; set; }
    }
}
