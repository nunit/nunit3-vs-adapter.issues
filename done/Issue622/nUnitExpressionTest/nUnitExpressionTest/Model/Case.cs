using System;

namespace nUnitExpressionTest.Model
{
    public class Case
    {
        public string Approval { get; set; }
        public bool AutoCreatedCase { get; set; }
        public DateTimeOffset ActivityDue { get; set; }
        public Account Account { get; set; }
        public string Number { get; set; }
        public string Category { get; set; }
        public bool Active { get; set; }
        public CaseStatus Status { get; set; }
    }
}
