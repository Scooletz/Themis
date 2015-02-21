using System;
using NHibernate.Cfg;
using Themis.NHibernate.Cfg;

// ReSharper disable CheckNamespace
namespace Themis.Cfg
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// The extension methods for <see cref="FluentConfiguration"/>
    /// providing extension points for NHibernate filtering options.
    /// </summary>
    public static class FluentConfigurationExtensionMethods
    {
        /// <summary>
        /// Configures the NHibernate, to allow filtering according to demands defined for
        /// demand service.
        /// </summary>
        /// <param name="fluentConfiguration">The fluent configuration.</param>
        /// <param name="cfg">The nhibernate configuration, just before building the session factory.</param>
        /// <param name="afterService">The action called with a created demand service as its parameter.</param>
        /// <returns>An NHibernate filtering configuration.</returns>
        /// <remarks>
        /// This method should be called after NHibernate configuration finished. 
        /// Do not build <see cref="IDemandService"/> with <see cref="FluentConfiguration.BuildDemandService"/>
        /// because it will throw errors. Pass the action saving a service instance as <paramref name="afterService"/>
        /// instead.
        /// </remarks>
        public static NHibernateFilteringConfiguration ConfigureNHibernate(
            this FluentConfiguration fluentConfiguration, Configuration cfg, Action<IDemandService> afterService)
        {
            if (fluentConfiguration == null)
                throw new ArgumentNullException("fluentConfiguration");
            if (cfg == null)
                throw new ArgumentNullException("cfg");
            if (afterService == null)
                throw new ArgumentNullException("afterService");

            return new NHibernateFilteringConfiguration(fluentConfiguration, cfg, afterService);
        }
    }
}