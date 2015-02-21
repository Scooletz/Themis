namespace Themis.NHibernate
{
    /// <summary>
    /// The interface of demands specified for an entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public sealed class EntityDemand<TEntity> : IClaim
        where TEntity : class
    {
        public EntityDemand(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; private set; }
    }
}