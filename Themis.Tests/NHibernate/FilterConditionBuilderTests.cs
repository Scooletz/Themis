using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Themis.Tests.NHibernate.Data;

namespace Themis.Tests.NHibernate
{
    [TestFixture]
    public class FilterConditionBuilderTests
    {
        [Test]
        public void Test()
        {
            Expression<Func<RecruitmentMotion, ManagerRole, bool>> expr = (e, role) =>
                                                            e.ForUnit == role.ManagedUnit ||
                                                            e.Owner == role.ForEmployee &&
                                                            !(e.ForUnit.Manager == role.ForEmployee);
        }
    }
}