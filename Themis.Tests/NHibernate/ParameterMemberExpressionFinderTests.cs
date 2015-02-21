using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Themis.NHibernate.Impl;

namespace Themis.Tests.NHibernate
{
    [TestFixture]
    public class ParameterMemberExpressionFinderTests
    {
        public class A1
        {
            public A2 Prop { get; set; }
        }

        public class A2
        {
            public A3 Prop { get; set; }
        }

        public class A3
        {
            public string Prop { get; set; }
        }

        [Test]
        public void FindExpressionsIn_WhenPassedExpressionWithParameterDeepAccess_ThenFindsIt()
        {
            var parameter = Expression.Parameter(typeof (A1), "a1");
            var finder = new ParameterMemberExpressionFinder(parameter);

            Expression<Func<A1, bool>> e = a1 => a1.Prop.Prop.Prop == "p" || a1.Prop.Prop != null;

            var result = finder.FindExpressionsIn(e).ToArray();

            Assert.AreEqual(2, result.Length, "The number of discovered parameters should be 2");
            Assert.True(result.Any(m => m.ToString().Equals("a1.Prop.Prop.Prop")));
            Assert.True(result.Any(m => m.ToString().Equals("a1.Prop.Prop")));
        }
    }
}