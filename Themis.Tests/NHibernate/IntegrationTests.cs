using System;
using System.Linq;
using NHibernate;
using NUnit.Framework;
using Themis.Cfg;
using Themis.Tests.NHibernate.Data;
using Themis.Tests.NHibernate.RoleDefinitions;

namespace Themis.Tests.NHibernate
{
    [TestFixture]
    public class IntegrationTests : NHibernateBaseTest
    {
        private IDemandService _service;
        private Unit _root;
        private Unit _firstLeave;
        private Unit _secondLeave;

        [TestFixtureSetUp]
        public void SetupFixture( )
        {
            Init(typeof(IntegrationTests).Assembly);

            using (var s = Factory.OpenSession())
            {
                using (var tx = s.BeginTransaction())
                {
                    // units with managers
                    _root = CreateUnitWithManager(s, "RootUnit", null);
                    _firstLeave = CreateUnitWithManager(s, "1st leave", _root);
                    _secondLeave = CreateUnitWithManager(s, "2nd leave", _root);

                    // roles
                    s.Save(new ManagerRole { ForEmployee = _root.Manager, ManagedUnit = _root });
                    s.Save(new SupportingManagerRole { ForEmployee = _firstLeave.Manager, BarelyManagedUnit = _root });
                    s.Save(new SupportingManagerRole { ForEmployee = _secondLeave.Manager, BarelyManagedUnit = _root });

                    s.Save(new RecruitmentMotion { ForUnit = _root, Name = "Root recruitment motion 1", Owner = _root.Manager });
                    s.Save(new RecruitmentMotion { ForUnit = _root, Name = "Root recruitment motion 2", Owner = _root.Manager });
                    s.Save(new RecruitmentMotion { ForUnit = _root, Name = "Root recruitment motion 3", Owner = _root.Manager });

                    s.Save(new RecruitmentMotion { ForUnit = _secondLeave, Name = "1st leave recruitment motion 1", Owner = _root.Manager });
                    s.Save(new RecruitmentMotion { ForUnit = _secondLeave, Name = "1st leave recruitment motion 2", Owner = _root.Manager });
                    s.Save(new RecruitmentMotion { ForUnit = _secondLeave, Name = "1st leave recruitment motion 3", Owner = _root.Manager });

                    s.Save(new RecruitmentMotion { ForUnit = _firstLeave, Name = "2nd leave recruitment motion 1", Owner = _secondLeave.Manager });
                    s.Save(new RecruitmentMotion { ForUnit = _firstLeave, Name = "2nd leave recruitment motion 2", Owner = _secondLeave.Manager });
                    s.Save(new RecruitmentMotion { ForUnit = _firstLeave, Name = "2nd leave recruitment motion 3", Owner = _secondLeave.Manager });

                    tx.Commit();
                }
            }
        }

        protected override void OneLastTimeWithConfigurationBeforeFactoryIsCreated(global::NHibernate.Cfg.Configuration cfg)
        {
            Fluently.Configure()
                .AddRoleDefinition(new ManagerRoleDefinition())
                .AddRoleDefinition(new SupportingManagerRoleDefinition())
                .ConfigureNHibernate(cfg, s => _service = s);
        }

        [Test]
        public void RetrievingWhenNoFilteringWorksInStandardWay( )
        {
            using (var s = Factory.OpenSession())
            {
                Assert.AreEqual(3, s.QueryOver<Unit>().RowCountInt64());
                Assert.AreEqual(3, s.QueryOver<Employee>().RowCountInt64());
                Assert.AreEqual(3, s.QueryOver<BaseRole>().RowCountInt64());
                Assert.AreEqual(2, s.QueryOver<SupportingManagerRole>().RowCountInt64());
                Assert.AreEqual(1, s.QueryOver<ManagerRole>().RowCountInt64());
            }
        }

        [Test]
        public void RetrievingWhenDemandServiceInUsedIsFiltered()
        {
            using (var session = Factory.OpenSession())
            {
                var rootManagerRoles = session.QueryOver<BaseRole>().And(b => b.ForEmployee == _root.Manager).List();
                var firstLeaveManagerRoles = session.QueryOver<BaseRole>().And(b => b.ForEmployee == _firstLeave.Manager).List();
                var secondLeaveManagerRoles = session.QueryOver<BaseRole>().And(b => b.ForEmployee == _firstLeave.Manager).List();

                AssertFilterCount(session, 0);
                using (var scope = _service.ApplyFilters(session, rootManagerRoles.ToArray()))
                {
                    Assert.AreEqual(6, session.QueryOver<RecruitmentMotion>().RowCountInt64()); // the root owns 6 of them
                }
                
                AssertFilterCount(session, 0);

                using (var scope = _service.ApplyFilters(session, firstLeaveManagerRoles.ToArray()))
                {
                    Assert.AreEqual(3, session.QueryOver<RecruitmentMotion>().RowCountInt64()); // the root owns all of them
                }

                AssertFilterCount(session, 0);

                using (var scope = _service.ApplyFilters(session, secondLeaveManagerRoles.ToArray()))
                {
                    Assert.AreEqual(3, session.QueryOver<RecruitmentMotion>().RowCountInt64()); // the root owns all of them
                }

                AssertFilterCount(session, 0);
            }
        }

        [TestFixtureTearDown]
        public void FixtureTearDown( )
        {
            Clear();
        }

        private static Unit CreateUnitWithManager(ISession s, string name, Unit parent)
        {
            var u = new Unit { Name = name };
            var manager = new Employee
            {
                Name = "Manager of " + name + " name",
                Surname = "Manager of " + name + " surname",
                EmployementDate = new DateTime(2010, 12, 4),
                EmployingUnit = u
            };
            if (parent != null)
            {
                parent.AddChildUnit(u);
            }

            u.Manager = manager;

            s.Save(u);
            s.Save(manager);
            u.Manager = manager;

            return u;
        }

        private static void AssertFilterCount(ISession session, int numberOfEnabledFilters)
        {
            Assert.AreEqual(numberOfEnabledFilters, session.GetSessionImplementation().EnabledFilters.Count);
        }
    }
}