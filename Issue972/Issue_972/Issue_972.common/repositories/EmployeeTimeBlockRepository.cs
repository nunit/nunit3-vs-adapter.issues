using Issue_972.common.ExtraObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Issue_972.common
{
    public class EmployeeTimeBlockRepository : AbstractRepository<EmployeeTimeBlock>
    {
        public EmployeeTimeBlockRepository(AdministrationContext administrationContext) : base(administrationContext, administrationContext.EmployeeTimeBlocks)
        {
        }

        public override Task<EmployeeTimeBlock> GetById(int id)
        {
            throw new NotImplementedException("It does not have an Id");
        }

        public IQueryable<TimeBlock> GetAllTimeBlocksByEmployeeId(int id)
        {
            return _administrationContext.EmployeeTimeBlocks.Where(entry => entry.EmployeeId == id).Select(entry => entry.TimeBlock);
        }

        public IQueryable<CalculateHoursObj> GetCalculateHoursObjsByEmployeeInTimeRange(int id, DateTime start, DateTime end)
        {
            return _administrationContext.EmployeeTimeBlocks.Where(entry => entry.EmployeeId == id && entry.Start <= end && entry.End >= start)
                .Select(entry => new CalculateHoursObj{Start = entry.Start, End = entry.End, TimeBlock = entry.TimeBlock });
        }

    }
}
