using System.Collections.Generic;
using NUnit.Framework;
using Themis.Utils;

namespace Themis.Tests.Utils
{
    [TestFixture]
    public class TypeExtensionsTests
    {
        [Test]
        public void IsGenericDefinition()
        {
            Assert.True(typeof (List<>).IsOpenGeneric());
            Assert.False(typeof (List<int>).IsOpenGeneric());
            Assert.False(typeof (int).IsOpenGeneric());
            Assert.False(typeof (int[]).IsOpenGeneric());
        }
    }
}