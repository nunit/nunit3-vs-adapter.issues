using Microsoft.EntityFrameworkCore;
using Issue_972.common;
using Issue_972.common.ExtraObjects;


namespace Issue_972.API.Services
{
    public class OverTimeTable
    {
        public DateTime Time { get; set; }
        public DateTime CreatedOn { get; set; }
        public TimeType TimeType { get; set; }
    }
    public class EmployeeService
    {
        public EmployeeService()
        {
        }

        public async Task<float> CalculateOffTimeHours(DateTime startTime, DateTime endTime, IEnumerable<CalculateHoursObj> timeBlocks)
        {
            if (!timeBlocks.Any()) return 0.0f;
            static bool lambda(CalculateHoursObj x) => (x.TimeBlock.HourType == HourTypeEnum.Work && x.TimeBlock.TimeType == TimeType.End ||
                ((x.TimeBlock.HourType == HourTypeEnum.Break || x.TimeBlock.HourType == HourTypeEnum.Leave) && x.TimeBlock.TimeType == TimeType.Start));


            if (timeBlocks is IQueryable) return 8.0f;
            return 0.0f;
        }

        public async Task<float> CalculateOverTimeHours(DateTime startTime, DateTime endTime, IEnumerable<CalculateHoursObj> timeBlocks)
        {

            if (!await CheckOverTime(startTime, endTime, timeBlocks)) throw new InvalidDataException("Overtime not availabe in this time");
            static bool lambda(CalculateHoursObj x) => (x.TimeBlock.HourType == HourTypeEnum.Work && x.TimeBlock.TimeType == TimeType.Start ||
                ((x.TimeBlock.HourType == HourTypeEnum.Break || x.TimeBlock.HourType == HourTypeEnum.Leave || x.TimeBlock.HourType == HourTypeEnum.OverTimeBreak)
                && x.TimeBlock.TimeType == TimeType.End));

            return 8.0f;
        }

        public async Task<bool> CheckOverTime(DateTime startTime, DateTime endTime, IEnumerable<CalculateHoursObj> timeBlocks)
        {
            //Asumption: Over hours can only be calculated on a daily basis
            if (startTime.Date != endTime.Date) throw new ArgumentException("{startTime} and {endTime} aren't the same date");

            //A null list can't have invalid overtime data
            if (timeBlocks == null) return true;

            LinkedList<CalculateHoursObj> list;
            if (timeBlocks is IQueryable) list = new(await timeBlocks.AsQueryable().Where(t => t.TimeBlock.DayOfWeek == startTime.DayOfWeek).OrderBy(t => t.TimeBlock.Time).ToListAsync());
            else list = new(timeBlocks.Where(t => t.TimeBlock.DayOfWeek == startTime.DayOfWeek).OrderBy(t => t.TimeBlock.Time).ToList());

            //An empty list can't have invalid overtime data
            if (list == null || list.Last == null || list.First == null) return true;

            if (list.Count % 2 == 1) throw new InvalidDataException("List contains odd elements");


            /* We're gonna find the first ellement of list
             * And all nodes are the same day so we only have to look at time
             * If the current node is after startTime then the nod before is the first element of the lest
             * If there is no node before regard the current node as the first element
             */

            LinkedListNode<CalculateHoursObj>? node = list.First;
            while (node.Next is not null)
            {
                if (node.Value.TimeBlock.Time > startTime.TimeOfDay)
                {
                    if (node.Previous is null) break;
                    node = node.Previous;
                    break;
                }
                node = node.Next;
            }



            /* We're gonna walk through the list and check a couple of things
             * 1. If the last TimeBlock before was a start work
             * 2. If the last TimeBlock before was a start leave
             * 3. If the last TimeBlock before was an end break
             * 4. If there are any non "OverTimeBreak" TimeBlocks between start and end
             * If any of those is true return false
             * 
             * this function should only be called with both start and end on the same day so we don't need to loop
             */

            while (node != null)
            {
                HourTypeEnum hourType = node.Value.TimeBlock.HourType;
                TimeType timeType = node.Value.TimeBlock.TimeType;
                TimeSpan timeSpan = node.Value.TimeBlock.Time;

                if (timeSpan >= endTime.TimeOfDay) break;

                if (timeSpan <= startTime.TimeOfDay)
                    switch (hourType, timeType)
                    {
                        case (HourTypeEnum.Leave, TimeType.Start): return false;
                        case (HourTypeEnum.Work, TimeType.Start): return false;
                        case (HourTypeEnum.Break, TimeType.End): return false;
                    }

                else if(timeSpan > startTime.TimeOfDay)
                    switch (hourType)
                    {
                        case (HourTypeEnum.Work): return false;
                        case (HourTypeEnum.Leave): return false;
                        case (HourTypeEnum.Break): return false;
                    }

                node = node.Next;
            }

            return true;
        }


    }
}
