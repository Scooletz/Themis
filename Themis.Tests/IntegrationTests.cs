using System.Linq;
using NUnit.Framework;
using Themis.Cfg;

namespace Themis.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        private IDemandService _demandService;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _demandService = Fluently.Configure()
                .AddRoleDefinition(new DriverRoleDefinition())
                .AddRoleDefinition(new LogisticsWorkerRoleDefinition())
                .BuildDemandService();
        }

        private bool Can(CanDrive demand, params object[] roles)
        {
            return _demandService.Evaluate<CanDrive, bool>(demand, roles).Any(b => b);
        }

        private class DriverRole
        {
            public DriverRole(int skillLevel)
            {
                SkillLevel = skillLevel;
            }

            public int SkillLevel { get; private set; }
        }

        private class LogisticsWorkerRole
        {
            public LogisticsWorkerRole(int iq)
            {
                Iq = iq;
            }

            public int Iq { get; private set; }
        }

        private class Car
        {
            public Car(int requiredSkillLevel)
            {
                RequiredSkillLevel = requiredSkillLevel;
            }

            public int RequiredSkillLevel { get; private set; }
        }

        private class CanDrive : IDemand<bool>
        {
            public CanDrive(Car car)
            {
                Car = car;
            }

            public Car Car { get; private set; }
        }

        private class DriverRoleDefinition : RoleDefinition<DriverRole>
        {
            public DriverRoleDefinition()
            {
                Add<CanDrive, bool>((d, r) => d.Car.RequiredSkillLevel <= r.SkillLevel);
            }
        }

        private class LogisticsWorkerRoleDefinition : RoleDefinition<LogisticsWorkerRole>
        {
            public LogisticsWorkerRoleDefinition()
            {
                Add<CanDrive, bool>((d, r) => d.Car.RequiredSkillLevel <= r.Iq);
            }
        }

        [Test]
        public void IntegrationTest()
        {
            Assert.True(Can(new CanDrive(new Car(5)), new DriverRole(5)));
            Assert.True(Can(new CanDrive(new Car(5)), new DriverRole(10)));
            Assert.False(Can(new CanDrive(new Car(10)), new DriverRole(1)));
        }
    }
}