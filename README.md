# vertical-pipelines-dependencyinjection

Generic "middleware" pipelines.

![.net](https://img.shields.io/badge/Frameworks-.netstandard2.0-purple)
![GitHub](https://img.shields.io/github/license/verticalsoftware/vertical-pipelines-dependencyinjection)
![Package info](https://img.shields.io/nuget/v/vertical-pipelines-dependencyinjection.svg)

[![Dev build](https://github.com/verticalsoftware/vertical-pipelines-dependencyinjection/actions/workflows/dev-build.yml/badge.svg)](https://github.com/verticalsoftware/vertical-pipelines-dependencyinjection/actions/workflows/dev-build.yml)
[![Release](https://github.com/verticalsoftware/vertical-pipelines-dependencyinjection/actions/workflows/release.yml/badge.svg)](https://github.com/verticalsoftware/vertical-pipelines-dependencyinjection/actions/workflows/release.yml)

## Usage

### Install

```
$ dotnet add package vertical-pipelines-dependencyinjection -v 1.0.0
```

### Configure In Application Setup

```csharp
var serviceCollection = new ServiceCollection();

serviceCollection.ConfigurePipeline<MyAppContext>(app =>
{
    app.Use((list, next, ct) =>
    {
        // Implementation
        
        return next(list, ct);
    });

    // Register using function
    app.UseMiddleware(serviceProvider => new MiddlewareA());

    // Register using type
    app.UseMiddleware<MiddlewareB>();
});

var provider = serviceCollection.BuildServiceProvider();
using var scope = provider.CreateScope();
var context = new List<string>();
var pipeline = scope.ServiceProvider.GetRequiredService<PipelineDelegate<List<string>>>();

await pipeline(context, CancellationToken.None);
```

## Issues, feedback, feature requests

File an issue [here](https://github.com/verticalsoftware/vertical-pipelines-dependencyinjection/issues).
