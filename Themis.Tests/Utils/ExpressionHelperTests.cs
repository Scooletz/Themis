using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Themis.Expressions;
using Themis.Tests.TestData;

namespace Themis.Tests.Utils
{
    [TestFixture]
    public class ExpressionHelperTests
    {
        [Test]
        public void MapGenericDemandMemberToDemand()
        {
            Expression<Func<Entity, OtherRole, bool>> entityExpression = (e, r) => e.Id == r.EntityId;
            Expression<Func<GenericEntityDemand<Entity>, Entity>> demandToEntity = d => d.Entity;

            var test = ExpressionHelper.MapDemandMemberToDemand(entityExpression, demandToEntity);
            var compiledTest = test.Compile();

            var demandFor1 = new GenericEntityDemand<Entity>(new Entity {Id = 1});
            var demandFor2 = new GenericEntityDemand<Entity>(new Entity {Id = 2});
            var roleFor1 = new OtherRole {EntityId = 1};
            var roleFor2 = new OtherRole {EntityId = 2};

            Assert.True(compiledTest(demandFor1, roleFor1));
            Assert.False(compiledTest(demandFor1, roleFor2));
            Assert.False(compiledTest(demandFor2, roleFor1));
            Assert.True(compiledTest(demandFor2, roleFor2));
        }

        [Test]
        public void MapNonGenericDemandMemberToDemand()
        {
            Expression<Func<Entity, OtherRole, bool>> entityExpression = (e, r) => e.Id == r.EntityId;
            Expression<Func<EntityDemand, Entity>> demandToEntity = d => d.Entity;

            var test = ExpressionHelper.MapDemandMemberToDemand(entityExpression, demandToEntity);
            var compiledTest = test.Compile();

            var demandFor1 = new EntityDemand(new Entity {Id = 1});
            var demandFor2 = new EntityDemand(new Entity {Id = 2});
            var roleFor1 = new OtherRole {EntityId = 1};
            var roleFor2 = new OtherRole {EntityId = 2};

            Assert.True(compiledTest(demandFor1, roleFor1));
            Assert.False(compiledTest(demandFor1, roleFor2));
            Assert.False(compiledTest(demandFor2, roleFor1));
            Assert.True(compiledTest(demandFor2, roleFor2));
        }

        [Test]
        public void TestMemberAccess()
        {
            Expression<Func<EntityDemand, int>> expr = e => e.Entity.Id;
        }
    }
}