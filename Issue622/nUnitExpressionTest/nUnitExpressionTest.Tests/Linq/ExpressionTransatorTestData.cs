using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using Moq;
using nUnitExpressionTest.Model;
using nUnitExpressionTest.Linq;

namespace nUnitExpressionTest.Tests.Linq
{
    public static class ExpressionTransatorTestData
    {
        public static IEnumerable<TestCaseData> TranslateData
        {
            get
            {
                yield return new TestCaseData(
                    (Expression<Func<Case, bool>>)((Case c) => c.Approval == "test"),
                    "approval=test"
                    );

                yield return new TestCaseData(
                    (Expression<Func<Case, bool>>)((Case c) => c.AutoCreatedCase == false),
                    "auto_created_case=false"
                    );

                yield return new TestCaseData(
                    (Expression<Func<Case, bool>>)((Case c) => c.AutoCreatedCase == true),
                    "auto_created_case=true"
                    );

                yield return new TestCaseData(
                    (Expression<Func<Case, bool>>)((Case c) => c.Approval == "test" && c.AutoCreatedCase == false),
                    "approval=test^auto_created_case=false"
                    );

                yield return new TestCaseData(
                    (Expression<Func<Case, bool>>)((Case c) => c.Approval == "test" || c.AutoCreatedCase == false),
                    "approval=test^ORauto_created_case=false"
                    );

                yield return new TestCaseData(
                    (Expression<Func<Case, bool>>)((Case c) => c.Approval == "test" || c.AutoCreatedCase == true),
                    "approval=test^ORauto_created_case=true"
                    );

                // Mon Jan 2 15:04:05 MST 2006
                // yyyy-MM-dd HH:mm:ss
                DateTimeOffset testDateTime = new DateTimeOffset(2006, 1, 2, 3, 4, 5, TimeSpan.FromHours(-7));

                yield return new TestCaseData(
                    (Expression<Func<Case, bool>>)((Case c) => c.Approval == "test"
                        && c.AutoCreatedCase
                        || c.ActivityDue < testDateTime),
                    "approval=test^auto_created_case^ORactivity_due<2006-01-02+10%3A04%3A05"
                    );

                // test using a "new DateTime" in the expression
                yield return new TestCaseData(
                    (Expression<Func<Case, bool>>)((Case c) => c.Approval == "test"
                        && c.AutoCreatedCase
                        || c.ActivityDue < new DateTimeOffset(2006, 1, 2, 3, 4, 5, TimeSpan.FromHours(-7))),
                    "approval=test^auto_created_case^ORactivity_due<2006-01-02+10%3A04%3A05"
                    );

                // test using a "new DateTime" in the expression
                yield return new TestCaseData(
                    (Expression<Func<Case, bool>>)((Case c) => c.Approval == "test"
                        && c.AutoCreatedCase
                        || c.ActivityDue < new DateTimeOffset(2006, 1, 2, 3, 4, 5, TimeSpan.FromHours(-7))),
                    "approval=test^auto_created_case^ORactivity_due<2006-01-02+10%3A04%3A05"
                    );


                var mockAPI = new Mock<IQueryProvider>();
                mockAPI.Setup(r => r.CreateQuery(It.IsAny<Expression>())).Returns<Expression>((e) =>
                {
                    return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(e.Type), new object[] { mockAPI.Object, e });
                });
                mockAPI.Setup(r => r.CreateQuery<Case>(It.IsAny<Expression>())).Returns<Expression>((e) =>
                {
                    return new Query<Case>(mockAPI.Object, e);
                });
                mockAPI.Setup(r => r.CreateQuery<Case>(It.IsAny<Expression<Func<Case, bool>>>())).Returns<Expression<Func<Case, bool>>>((e) =>
                 {
                     return new Query<Case>(mockAPI.Object, e);
                 });

                Query<Case> query = new Query<Case>(mockAPI.Object);

                yield return new TestCaseData(
                    query.Where(c => c.Approval == "test").Expression,
                    "approval=test"
                    );

                // test ref-link converions
                yield return new TestCaseData(
                    query.Where(c => c.Account == "86837a386f0331003b3c498f5d3ee4ca").Expression,
                    "account=86837a386f0331003b3c498f5d3ee4ca"
                    );

                Guid bobGuid = Guid.ParseExact("86837a386f0331003b3c498f5d3ee4ca", "N");
                yield return new TestCaseData(
                    query.Where(c => c.Account == bobGuid).Expression,
                    "account=86837a386f0331003b3c498f5d3ee4ca"
                    );

                //Check order by
                yield return new TestCaseData(
                    query.Where(c => c.Approval == "test").OrderBy(c => c.Number).Expression,
                    "approval=test^ORDERBYnumber"
                    );

                yield return new TestCaseData(
                    query.Where(c => c.Approval == "test").OrderBy(c => c.Number).ThenByDescending(c => c.Category)
                        .Expression,
                    "approval=test^ORDERBYnumber^ORDERBYDESCcategory"
                    );

                // check multiple where
                yield return new TestCaseData(
                   query.Where(c => c.Approval == "test").Where(c => c.Number != "5").Expression,
                   "approval=test^number!=5"
                   );

                // test dot walks
                yield return new TestCaseData(
                    query.Where(c => c.Account.Name == "bob").Expression,
                    "account.name=bob"
                    );

                yield return new TestCaseData(
                    query.Where(c => c.Account.ParentAccount.Name == "bob").Expression,
                    "account.account_parent.name=bob"
                    );

                yield return new TestCaseData(
                    query.Where(c => c.Account.ParentAccount.Name == "bob").Expression,
                    "account.account_parent.name=bob"
                    );

                yield return new TestCaseData(
                    query.Where(c => c.Account.ParentAccount.Name == "bob").Expression,
                    "account.account_parent.name=bob"
                    );

                yield return new TestCaseData(
                    query.Where(c => c.Account.ParentAccount.Name == "bob").Expression,
                    "account.account_parent.name=bob"
                    );


                yield return new TestCaseData(
                    query.Where(c => c.Account.ParentAccount.ParentAccount.Name == "bob").Expression,
                    "account.account_parent.account_parent.name=bob"
                    );

                yield return new TestCaseData(
                    query.Where(c =>c.Account.ParentAccount.ParentAccount.Name == "bob"
                        && c.Active == true
                        || c.Status == CaseStatus.New
                    ).Expression,
                    "account.account_parent.account_parent.name=bob^active=true^ORstate=1"
                    );
            }
        }
    }
}