namespace Themis.Tests.TestData
{
    public class EntityDemand : IDemand<bool>
    {
        public EntityDemand(Entity entity)
        {
            Entity = entity;
        }

        public Entity Entity { get; private set; }
    }
}