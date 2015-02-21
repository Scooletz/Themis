namespace Themis.Tests.NHibernate.Demands
{
    public class ViewDemand<TEntity> : IClaim
    {
        public ViewDemand(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; private set; }
    }
}