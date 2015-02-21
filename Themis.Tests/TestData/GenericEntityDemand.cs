namespace Themis.Tests.TestData
{
    public class GenericEntityDemand<T> : IDemand<bool>
    {
        public GenericEntityDemand(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; private set; }
    }
}