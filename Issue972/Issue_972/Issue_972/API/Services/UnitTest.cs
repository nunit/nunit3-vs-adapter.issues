using Issue_972.API.Services;
using Issue_972.common;
using Issue_972.common.ExtraObjects;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Issue_972.Tests.API.Services
{
    [Category("Whatever")]
    [TestFixture, Parallelizable(scope: ParallelScope.All)]
    public class EmployeeServiceTest
    {
        private static readonly EmployeeService EmployeeService = new();
        private static readonly DbContextOptions<AdministrationContext> options = new();
        private static readonly AdministrationContext administrationContext = new(options);
        private static readonly IRepository<Employee> employeeRepository = new EmployeeRepository(administrationContext);
        //private static readonly Random rnd = new();


        private static readonly DateTime standardStart = new(2022, 1, 1);
        private static readonly DateTime standardEnd = new(2050, 1, 1);
        private static readonly List<CalculateHoursObj> standardWeek = new()
        {
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Monday, TimeType.Start, new(7, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Monday, TimeType.Start, new(9, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Monday, TimeType.End, new(9, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Monday, TimeType.Start, new(12, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Monday, TimeType.End, new(12, 30, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Monday, TimeType.Start, new(15, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Monday, TimeType.End, new(15, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Monday, TimeType.End, new(16, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Tuesday, TimeType.Start, new(7, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Tuesday, TimeType.Start, new(9, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Tuesday, TimeType.End, new(9, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Tuesday, TimeType.Start, new(12, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Tuesday, TimeType.End, new(12, 30, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Tuesday, TimeType.Start, new(15, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Tuesday, TimeType.End, new(15, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Tuesday, TimeType.End, new(16, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.OverTimeBreak, DayOfWeek.Tuesday, TimeType.Start, new(18, 00, 00), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.OverTimeBreak, DayOfWeek.Tuesday, TimeType.End, new(18, 30, 00), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Wednesday, TimeType.Start, new(7, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Wednesday, TimeType.Start, new(9, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Wednesday, TimeType.End, new(9, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Wednesday, TimeType.Start, new(12, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Wednesday, TimeType.End, new(12, 30, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Wednesday, TimeType.Start, new(15, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Wednesday, TimeType.End, new(15, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Wednesday, TimeType.End, new(16, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Thursday, TimeType.Start, new(7, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Thursday, TimeType.Start, new(9, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Thursday, TimeType.End, new(9, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Thursday, TimeType.Start, new(12, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Thursday, TimeType.End, new(12, 30, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Thursday, TimeType.Start, new(15, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Thursday, TimeType.End, new(15, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Thursday, TimeType.End, new(16, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.OverTimeBreak, DayOfWeek.Thursday, TimeType.Start, new(18, 00, 00), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.OverTimeBreak, DayOfWeek.Thursday, TimeType.End, new(18, 30, 00), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Friday, TimeType.Start, new(7, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Friday, TimeType.Start, new(9, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Friday, TimeType.End, new(9, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Friday, TimeType.Start, new(12, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Friday, TimeType.End, new(12, 30, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Friday, TimeType.End, new(15, 30, 0), "") }
        };

        #region DateTimes

        private static readonly DateTime TenthMarch8_2022 = new(2022, 3, 10, 8, 0, 0, 0);
        private static readonly DateTime TenthMarch16_2022 = new(2022, 3, 10, 16, 0, 0, 0);
        private static readonly DateTime EleventhMarch16_2022 = new(2022, 3, 11, 16, 0, 0, 0);

        #endregion DateTimes

        [Test]
        [TestCaseSource(nameof(CalcHoursTestCalculateOverTimeHoursData))]
        public async Task CalcHoursTestCalculateOverTimeHours(DateTime start, DateTime end)
        {
            var list = new List<CalculateHoursObj>();
            var res = await EmployeeService.CalculateOverTimeHours(start, end, list);
            Assert.That(res, Is.EqualTo(8.0f));
        }
        public static IEnumerable<TestCaseData> CalcHoursTestCalculateOverTimeHoursData
        {
            get
            {
                yield return new TestCaseData(TenthMarch8_2022, TenthMarch16_2022); // .Returns(8.0f);
            }
        }

        [Test]
        [TestCaseSource(nameof(CalcHoursTestCalculateOverTimeHoursThrowsData))]
        public void CalcHoursTestCalculateOverTimeHoursThrows<T>(DateTime start, DateTime end, T _, List<CalculateHoursObj>? list = default) where T : System.Exception
        {
            if (list == null) list = new List<CalculateHoursObj>();
            Assert.ThrowsAsync<T>(async () =>
                await EmployeeService.CalculateOverTimeHours(start, end, list)
            );
        }
        public static IEnumerable CalcHoursTestCalculateOverTimeHoursThrowsData
        {
            get
            {
                yield return new TestCaseData(TenthMarch8_2022, EleventhMarch16_2022, new ArgumentException(),null);
                yield return new TestCaseData(TenthMarch8_2022, TenthMarch16_2022, new Exception(), standardWeek);
                yield return new TestCaseData(TenthMarch8_2022, TenthMarch16_2022, new System.IO.InvalidDataException(), new List<CalculateHoursObj> {
                    new CalculateHoursObj{Start = new(), End = new(), TimeBlock = new(0, 0, 0, new(0), "") },
                    new CalculateHoursObj{Start = new(), End = new(), TimeBlock = new(0, 0, 0, new(0), "") },
                    new CalculateHoursObj{Start = new(), End = new(), TimeBlock = new(0, 0, 0, new(0), "") } });
            }
        }


        [Test]
        [TestCaseSource(nameof(CalcHoursTestCalculateOffTimeHoursData))]
        public async Task<float> CalcHoursTestCalculateOffTimeHours(DateTime start, DateTime end)
        {
            var x = await EmployeeService.CalculateOffTimeHours(start, end, new List<CalculateHoursObj>()/*new List<CalculateHoursObj>()*/);
            Assert.That(x, Is.EqualTo(0.0f));
            return x;
        }
        public static IEnumerable CalcHoursTestCalculateOffTimeHoursData
        {
            get
            {
                yield return new TestCaseData(TenthMarch8_2022, TenthMarch16_2022).Returns(0.0f);
            }
        }



    }

    [TestFixture, Parallelizable(scope: ParallelScope.All), Explicit]
    public class EmployeeServiceTestSQL
    {
        private static readonly EmployeeService EmployeeService = new();
        private static readonly DbContextOptions<AdministrationContext> options = new();
        private static readonly AdministrationContext administrationContext = new(options);
        private static readonly IRepository<Employee> employeeRepository = new EmployeeRepository(administrationContext);
        //private static readonly Random rnd = new();


        private static readonly DateTime standardStart = new(2022, 1, 1);
        private static readonly DateTime standardEnd = new(2050, 1, 1);
        private static readonly List<CalculateHoursObj> standardWeek = new()
        {
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Monday, TimeType.Start, new(7, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Monday, TimeType.Start, new(9, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Monday, TimeType.End, new(9, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Monday, TimeType.Start, new(12, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Monday, TimeType.End, new(12, 30, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Monday, TimeType.Start, new(15, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Monday, TimeType.End, new(15, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Monday, TimeType.End, new(16, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Tuesday, TimeType.Start, new(7, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Tuesday, TimeType.Start, new(9, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Tuesday, TimeType.End, new(9, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Tuesday, TimeType.Start, new(12, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Tuesday, TimeType.End, new(12, 30, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Tuesday, TimeType.Start, new(15, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Tuesday, TimeType.End, new(15, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Tuesday, TimeType.End, new(16, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.OverTimeBreak, DayOfWeek.Tuesday, TimeType.Start, new(18, 00, 00), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.OverTimeBreak, DayOfWeek.Tuesday, TimeType.End, new(18, 30, 00), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Wednesday, TimeType.Start, new(7, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Wednesday, TimeType.Start, new(9, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Wednesday, TimeType.End, new(9, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Wednesday, TimeType.Start, new(12, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Wednesday, TimeType.End, new(12, 30, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Wednesday, TimeType.Start, new(15, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Wednesday, TimeType.End, new(15, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Wednesday, TimeType.End, new(16, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Thursday, TimeType.Start, new(7, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Thursday, TimeType.Start, new(9, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Thursday, TimeType.End, new(9, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Thursday, TimeType.Start, new(12, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Thursday, TimeType.End, new(12, 30, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Thursday, TimeType.Start, new(15, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Thursday, TimeType.End, new(15, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Thursday, TimeType.End, new(16, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.OverTimeBreak, DayOfWeek.Thursday, TimeType.Start, new(18, 00, 00), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.OverTimeBreak, DayOfWeek.Thursday, TimeType.End, new(18, 30, 00), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Friday, TimeType.Start, new(7, 45, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Friday, TimeType.Start, new(9, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Friday, TimeType.End, new(9, 15, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Friday, TimeType.Start, new(12, 00, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Break, DayOfWeek.Friday, TimeType.End, new(12, 30, 0), "") },
            new CalculateHoursObj { Start = standardStart, End = standardEnd, TimeBlock = new(HourTypeEnum.Work, DayOfWeek.Friday, TimeType.End, new(15, 30, 0), "") }
        };



        private static readonly DateTime TenthMarch8_2022 = new(2022, 3, 10, 8, 0, 0, 0);
        private static readonly DateTime TenthMarch16_2022 = new(2022, 3, 10, 16, 0, 0, 0);
        private static readonly DateTime EleventhMarch16_2022 = new(2022, 3, 11, 16, 0, 0, 0);



        private static readonly EmployeeTimeBlockRepository employeeTimeBlockRepository = new(administrationContext);

        private Employee employee = new();

        [SetUp]
        public async Task SetUp() => employee = await employeeRepository.AddReturn(new());

        [TearDown]
        public async Task TearDown() => await employeeRepository.Delete(employee);


        [Test]
        [TestCaseSource(nameof(CalcHoursTestCalculateOverTimeHoursSQLCallData))]
        public async Task<float> CalcHoursTestCalculateOverTimeHoursSQLCall(DateTime start, DateTime end)
        {
            System.Linq.IQueryable<CalculateHoursObj>? timeblocks = employeeTimeBlockRepository.GetCalculateHoursObjsByEmployeeInTimeRange(employee.Id, start, end);

            return await EmployeeService.CalculateOverTimeHours(start, end, timeblocks);
        }
        public static IEnumerable CalcHoursTestCalculateOverTimeHoursSQLCallData
        {
            get
            {
                yield return new TestCaseData(TenthMarch8_2022, TenthMarch16_2022).Returns(8.0f);
            }
        }
    }
}