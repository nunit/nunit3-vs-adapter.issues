using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Issue_972.common
{
    public enum HourTypeEnum
    {
        Work,
        Break,
        Leave,
        OverTimeBreak
    }

    public enum TimeType
    {
        Start,
        End
    }
    public class TimeBlock
    {
        public TimeBlock()
        {
        }

        public TimeBlock(HourTypeEnum hourType, DayOfWeek dayOfWeek, TimeType timeType, TimeSpan time, string description)
        {
            HourType = hourType;
            DayOfWeek = dayOfWeek;
            TimeType = timeType;
            Time = time;
            Description = description;
        }

        public int Id { get; set; }

        public HourTypeEnum HourType { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeType TimeType { get; set; }

        public TimeSpan Time { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return $"ID: {Id}, description: {Description}, type: {Enum.GetName(typeof(HourTypeEnum), HourType)}, week day: {DayOfWeek}, time: {Time}";
        }

    }
    /* Example:
     * 1, 1, *9:00*, *9:15*, "Standard first break time monday"
     * 1, 1, *8:00*, *8:15*, "Heat shift first break time monday"
     * 0, 5, *7:45*, *15:30*, "Standard friday workday"
     */
}
