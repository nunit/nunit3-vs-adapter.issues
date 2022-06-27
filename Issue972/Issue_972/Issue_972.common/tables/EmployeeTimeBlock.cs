using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Issue_972.common
{
    public class EmployeeTimeBlock
    {
        public EmployeeTimeBlock()
        {
        }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int TimeBlockId { get; set; }
        public TimeBlock TimeBlock { get; set; }

        //what happens if 2 tables overlap
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public override string ToString()
        {
            return $"employee ID: {EmployeeId}, time block ID: {TimeBlockId}";
        }

    }
}
