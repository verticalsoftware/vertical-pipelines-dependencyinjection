using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Vertical.Pipelines.DependencyInjection.Tests
{
    public class Tests
    {
        private sealed class MiddlewareGreen : IPipelineMiddleware<List<string>>
        {
            /// <inheritdoc />
            public Task InvokeAsync(List<string> context, PipelineDelegate<List<string>> next, CancellationToken cancellationToken)
            {
                context.Add("green");
                return next(context, cancellationToken);
            }
        }
        
        private sealed class MiddlewareBlue : IPipelineMiddleware<List<string>>
        {
            /// <inheritdoc />
            public Task InvokeAsync(List<string> context, PipelineDelegate<List<string>> next, CancellationToken cancellationToken)
            {
                context.Add("blue");
                return next(context, cancellationToken);
            }
        }

        [Fact]
        public async Task ConfigureRegistersExpectedServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.ConfigurePipeline<List<string>>(app =>
            {
                app.Use((list, next, ct) =>
                {
                    list.Add("red");
                    return next(list, ct);
                });

                app.UseMiddleware(_ => new MiddlewareGreen());

                app.UseMiddleware<MiddlewareBlue>();

            }, ServiceLifetime.Scoped);

            var provider = serviceCollection.BuildServiceProvider();

            using var scope = provider.CreateScope();

            var context = new List<string>();
            var pipeline = scope.ServiceProvider.GetRequiredService<PipelineDelegate<List<string>>>();

            await pipeline(context, CancellationToken.None);

            Assert.Equal("red",context[0]);
            Assert.Equal("green",context[1]);
            Assert.Equal("blue",context[2]);
        }
    }
}