using System;

namespace Issue_972.common.ExtraObjects
{
    public class CalculateHoursObj
    {
        public System.DateTime Start { get; set; }
        public System.DateTime End { get; set; }
        public TimeBlock TimeBlock { get; set; }

        private DateOnly? startDate = null;
        private DateOnly? endDate = null;
        public DateOnly StartDate { 
            get {
                if (startDate is null) startDate = DateOnly.FromDateTime(Start);
                return (DateOnly)startDate;
            } set { startDate = value; } }
        public DateOnly EndDate { 
            get { 
                if(endDate is null) endDate = DateOnly.FromDateTime(End);
                return (DateOnly)endDate; 
            } set { endDate = value; } }
    }
}