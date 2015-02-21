using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Type;
using NUnit.Framework;
using Themis.NHibernate.Impl;
using Themis.Tests.NHibernate.Data;

namespace Themis.Tests.NHibernate
{
    [TestFixture]
    public class FilterParameterProviderTests
    {
        private static readonly Expression<Func<ManagerRole, Guid>>[] Expressions = new Expression<Func<ManagerRole, Guid>>[]
                                                                                     {
                                                                                         r => r.Id,
                                                                                         r => r.ManagedUnit.Id,
                                                                                         r => r.ManagedUnit.ParentUnit.Id,
                                                                                         r => r.ManagedUnit.Manager.Id,
                                                                                         r => r.ForEmployee.Id,
                                                                                         r => r.ForEmployee.EmployingUnit.Id,
                                                                                         r => r.ForEmployee.EmployingUnit.ParentUnit.Id,
                                                                                         // doubled
                                                                                         r => r.ForEmployee.EmployingUnit.ParentUnit.Id,
                                                                                         r => r.ForEmployee.EmployingUnit.Id,
                                                                                     };


        [Test]
        public void GetFilterParameters_ReturnsMappedForEachUniqueExpression( )
        {
            var memberExpressions = Expressions.Select(e => e.Body).Cast<MemberExpression>();
            var test = new FilterParameterProvider(typeof(ManagerRole), memberExpressions);
            var parameters = test.GetFilterParameters();

            var paramTypes = parameters.Values.Select(v => v.GetType()).ToArray();

            Assert.AreEqual(7, parameters.Count, "There are 7 unique expressions");
            Assert.AreEqual(paramTypes[0], typeof(GuidType));
            Assert.AreEqual(paramTypes[1], typeof(GuidType));
            Assert.AreEqual(paramTypes[2], typeof(GuidType));
            Assert.AreEqual(paramTypes[3], typeof(GuidType));
            Assert.AreEqual(paramTypes[4], typeof(GuidType));
            Assert.AreEqual(paramTypes[5], typeof(GuidType));
            Assert.AreEqual(paramTypes[6], typeof(GuidType));
        }

        [Test]
        public void SetupFilter_SetsAllFilterParameters( )
        {
            var r = new ManagerRole
                        {
                            Id = Guid.NewGuid(),
                            ManagedUnit = new Unit
                                    {
                                        Id = Guid.NewGuid(),
                                        ParentUnit = new Unit(),
                                        Manager = new Employee()
                                    },
                            ForEmployee = new Employee
                                    {
                                        Id = Guid.NewGuid(),
                                        EmployingUnit = new Unit
                                                {
                                                    Id = Guid.NewGuid(),
                                                    ParentUnit = new Unit()
                                                }
                                    }
                        };

            var memberExpressions = Expressions.Select(e => e.Body).Cast<MemberExpression>();
            var test = new FilterParameterProvider(typeof(ManagerRole), memberExpressions);

            var paramNames = test.GetFilterParameters().Keys.ToArray();
            var result = test.GetFilterParametersValues(r);

            Assert.AreEqual((Guid)result[paramNames[0]], r.Id);
            Assert.AreEqual((Guid)result[paramNames[1]], r.ManagedUnit.Id);
            Assert.AreEqual((Guid)result[paramNames[2]], r.ManagedUnit.ParentUnit.Id);
            Assert.AreEqual((Guid)result[paramNames[3]], r.ManagedUnit.Manager.Id);
            Assert.AreEqual((Guid)result[paramNames[4]], r.ForEmployee.Id);
            Assert.AreEqual((Guid)result[paramNames[5]], r.ForEmployee.EmployingUnit.Id);
            Assert.AreEqual((Guid)result[paramNames[6]], r.ForEmployee.EmployingUnit.ParentUnit.Id);
        }
    }
}