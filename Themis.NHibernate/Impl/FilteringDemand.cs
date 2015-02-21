namespace Themis.NHibernate.Impl
{
    /// <summary>
    /// The markup demand, used only to retrieve the filter scope from <see cref="IDemandService"/>.
    /// </summary>
    internal sealed class FilteringDemand : IDemand<FilterApplier>
    {
        public static readonly FilteringDemand Instance = new FilteringDemand();

        private FilteringDemand()
        {
        }
    }
}