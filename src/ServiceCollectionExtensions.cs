using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Vertical.Pipelines.DependencyInjection
{
    /// <summary>
    /// Registration methods for pipeline builder.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Composes the tasks within a middleware pipeline and registers the services
        /// required to use a <see cref="PipelineDelegate{TContext}"/> within the application.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configureAction">
        /// A delegate used to configure the order of the pipeline components.
        /// </param>
        /// <param name="serviceLifetime">
        /// The service lifetime in which to register the middleware components and the pipeline
        /// delegate factory.
        /// </param>
        /// <typeparam name="TContext">Contextual data object passed through the pipeline components.</typeparam>
        /// <returns>The given <see cref="IServiceCollection"/></returns>
        public static IServiceCollection ConfigurePipeline<TContext>(
            this IServiceCollection serviceCollection,
            Action<IPipelineBuilder<TContext>> configureAction,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TContext : class
        {
            var builder = new PipelineBuilder<TContext>(serviceCollection, serviceLifetime);

            configureAction(builder);
            
            serviceCollection.Replace(ServiceDescriptor.Describe(
                typeof(IPipelineFactory<TContext>),
                typeof(PipelineFactory<TContext>),
                serviceLifetime));

            serviceCollection.Replace(ServiceDescriptor.Describe(
                typeof(PipelineDelegate<TContext>),
                provider =>
                {
                    var factory = provider.GetRequiredService<IPipelineFactory<TContext>>();
                    return factory.CreatePipeline();
                },
                serviceLifetime));
            
            return serviceCollection;
        }
    }
}