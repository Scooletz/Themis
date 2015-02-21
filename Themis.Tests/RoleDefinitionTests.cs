using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Themis.Cfg;
using Themis.Impl;
using Themis.Model;
using Themis.Tests.TestData;

namespace Themis.Tests
{
    [TestFixture]
    public class RoleDefinitionTests
    {
        private class EvaluatorFactory : IEvaluatorFactory
        {
            public EvaluatorFactory()
            {
                ProcessedExpressions = new List<TypeTrio>();
            }

            public List<TypeTrio> ProcessedExpressions { get; private set; }

            #region IEvaluatorFactory Members

            public IEvaluator[] GetEvaluators<TDemand, TRole, TResult>(
                Expression<Func<TDemand, TRole, TResult>> expression)
                where TDemand : class, IDemand<TResult>
                where TRole : class
            {
                ProcessedExpressions.Add(new TypeTrio
                                             {
                                                 Permission = typeof (TDemand),
                                                 Role = typeof (TRole),
                                                 Result = typeof (TResult)
                                             });
                return new IEvaluator[0];
            }

            public void EndModelsBuildUp()
            {
                throw new InvalidOperationException("The method should not be called from RoleDefinition");
            }

            #endregion
        }

        private class RoleDefinition1 : RoleDefinition<Role>
        {
            public RoleDefinition1()
            {
                Add<ADemand, object>((p, r) => new object());
                Add<ADemand, object>((p, r) => new object());
                Add<BDemand, IEnumerable<string>>((p, r) => Enumerable.Empty<string>());
            }
        }

        [Test]
        public void RoleDefinition_BuildsAllExpressionWithEvaluatorFactory()
        {
            var factory = new EvaluatorFactory();

            var test = (IModelProvider) new RoleDefinition1();
            test.GetModel(new List<IEvaluatorFactory> {factory});

            var expressions = factory.ProcessedExpressions;
            Assert.AreEqual(3, expressions.Count);
            Assert.AreEqual(2, expressions.Count(t => t.Equals(new TypeTrio
                                                                   {
                                                                       Permission = typeof (ADemand),
                                                                       Result = typeof (object),
                                                                       Role = typeof (Role)
                                                                   })));
            Assert.AreEqual(1, expressions.Count(t => t.Equals(new TypeTrio
                                                                   {
                                                                       Permission = typeof (BDemand),
                                                                       Result = typeof (IEnumerable<string>),
                                                                       Role = typeof (Role)
                                                                   })));
        }
    }
}