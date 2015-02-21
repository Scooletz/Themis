namespace Themis.Cfg
{
    /// <summary>
    /// The start point of fluent configuration.
    /// </summary>
    public static class Fluently
    {
        public static FluentConfiguration Configure()
        {
            return new FluentConfiguration();
        }
    }
}