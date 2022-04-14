using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Issue_972.common
{
    public class EmployeeRepository : AbstractRepository<Employee>
    {
        public EmployeeRepository(AdministrationContext administrationContext) : base(administrationContext, administrationContext.Employees)
        {
        }

        public override async Task<Employee> GetById(int id)
        {
            return await _administrationContext.Employees.SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

    }
}
