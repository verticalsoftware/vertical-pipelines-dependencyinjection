using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Vertical.Pipelines.DependencyInjection
{
    /// <summary>
    /// Serves as the base class for pipeline builders.
    /// </summary>
    /// <typeparam name="TContext">Contextual data object passed through the pipeline components.</typeparam>
    public class PipelineBuilder<TContext> : IPipelineBuilder<TContext> where TContext : class
    {
        private readonly ServiceLifetime _middlewareLifetime;
        private readonly Type _pipelineMiddlewareType = typeof(IPipelineMiddleware<TContext>);

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <param name="middlewareLifetime">Service lifetime for the pipeline</param>
        public PipelineBuilder(IServiceCollection serviceCollection, ServiceLifetime middlewareLifetime)
        {
            ApplicationServices = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
            _middlewareLifetime = middlewareLifetime;
        }

        /// <inheritdoc />
        public IServiceCollection ApplicationServices { get; }

        /// <inheritdoc />
        public IPipelineBuilder<TContext> Use(Func<TContext, PipelineDelegate<TContext>, CancellationToken, Task> implementation)
        {
            ApplicationServices.Add(ServiceDescriptor.Describe(
                _pipelineMiddlewareType,
                _ => new MiddlewareAction<TContext>(implementation),
                _middlewareLifetime));
            
            return this;
        }

        /// <inheritdoc />
        public IPipelineBuilder<TContext> UseMiddleware<T>() where T : IPipelineMiddleware<TContext>
        {
            ApplicationServices.Add(ServiceDescriptor.Describe(
                _pipelineMiddlewareType,
                typeof(T),
                _middlewareLifetime));

            return this;
        }

        /// <inheritdoc />
        public IPipelineBuilder<TContext> UseMiddleware<T>(Func<IServiceProvider, T> implementationFactory) where T : IPipelineMiddleware<TContext>
        {
            ApplicationServices.Add(ServiceDescriptor.Describe(
                _pipelineMiddlewareType,
                provider => implementationFactory(provider),
                _middlewareLifetime));

            return this;
        }
    }
}