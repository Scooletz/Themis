using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace Themis.Tests.NHibernate
{
    [TestFixture]
    public class ExpressionTests
    {
        public class ComplexType
        {
            public IList<string> List { get; set; }
        }

        [Test]
        public void Test()
        {
            Expression<Func<ComplexType, int>> e = c => c.List.Count;

            var converted = Expression.Convert(e.Body, typeof (object));
            var lambda = Expression.Lambda<Func<ComplexType, object>>(converted, e.Parameters.ToArray());

            var func = lambda.Compile();

            var complexType = new ComplexType {List = new List<string> {"test", string.Empty}};

            var val = func(complexType);

            Assert.AreEqual(2, (int) val);
        }
    }
}