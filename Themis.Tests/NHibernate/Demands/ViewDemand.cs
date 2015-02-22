namespace Themis.Tests.NHibernate.Demands
{
    public class ViewDemand<TEntity> : IDemand
    {
        public ViewDemand(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; private set; }
    }
}