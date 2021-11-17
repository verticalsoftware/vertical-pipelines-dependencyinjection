using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Vertical.Pipelines.DependencyInjection
{
    /// <summary>
    /// Represents a builder object used to register the services required to compose
    /// a task pipeline.
    /// </summary>
    /// <typeparam name="TContext">Contextual data object passed through the pipeline components.</typeparam>
    public interface IPipelineBuilder<TContext> where TContext : class
    {
        /// <summary>
        /// Gets the application services instance.
        /// </summary>
        IServiceCollection ApplicationServices { get; }
        
        /// <summary>
        /// Registers services that wrap the provided handling delegate into a task composed
        /// within a pipeline.
        /// </summary>
        /// <param name="implementation">
        /// The delegate that contains the implementation of the pipeline task.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        IPipelineBuilder<TContext> Use(Func<TContext, PipelineDelegate<TContext>, CancellationToken, Task> implementation);

        /// <summary>
        /// Registers the given service implementation type as a pipeline component.
        /// </summary>
        /// <typeparam name="T">Middleware component type.</typeparam>
        /// <returns>A reference to this instance.</returns>
        IPipelineBuilder<TContext> UseMiddleware<T>() where T : IPipelineMiddleware<TContext>;

        /// <summary>
        /// Registers the given service instance as a pipeline component.
        /// </summary>
        /// <param name="implementationFactory">A delegate that provides the component instance.</param>
        /// <typeparam name="T">Middleware component type.</typeparam>
        /// <returns>A reference to this instance.</returns>
        IPipelineBuilder<TContext> UseMiddleware<T>(Func<IServiceProvider, T> implementationFactory) where T : IPipelineMiddleware<TContext>;
    }
}