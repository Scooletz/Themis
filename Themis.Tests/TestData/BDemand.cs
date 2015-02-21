using System.Collections.Generic;

namespace Themis.Tests.TestData
{
    /// <summary>
    /// The B permission.
    /// </summary>
    public class BDemand : IDemand<IEnumerable<string>>
    {
        public string Context { get; set; }
    }
}