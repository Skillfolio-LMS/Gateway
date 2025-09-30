using Gateway.Api.Middleware;
using Gateway.Application.Services;
using Gateway.Infrastructure.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IAuthValidationService, AuthValidationService>();

builder.Services.AddReverseProxy()
    .LoadFromMemory([
            // Routes
            new Yarp.ReverseProxy.Configuration.RouteConfig
            {
                RouteId = "auth-route",
                ClusterId = "auth-cluster",
                Match = new Yarp.ReverseProxy.Configuration.RouteMatch
                {
                    Path = "/auth/{**catch-all}"
                }
            },
            new Yarp.ReverseProxy.Configuration.RouteConfig
            {
                RouteId = "skills-route",
                ClusterId = "skills-cluster",
                Match = new Yarp.ReverseProxy.Configuration.RouteMatch
                {
                    Path = "/skills/{**catch-all}"
                }
            }
        ], [
            // Clusters
            new Yarp.ReverseProxy.Configuration.ClusterConfig
            {
                ClusterId = "auth-cluster",
                Destinations = new Dictionary<string, Yarp.ReverseProxy.Configuration.DestinationConfig>
                {
                    { "auth-destination", new Yarp.ReverseProxy.Configuration.DestinationConfig { Address = "http://localhost:3000" } }
                }
            },
            new Yarp.ReverseProxy.Configuration.ClusterConfig
            {
                ClusterId = "skills-cluster",
                Destinations = new Dictionary<string, Yarp.ReverseProxy.Configuration.DestinationConfig>
                {
                    { "skills-destination", new Yarp.ReverseProxy.Configuration.DestinationConfig { Address = "http://localhost:3001" } }
                }
            }
        ]
    );

var app = builder.Build();

app.UseMiddleware<AuthMiddleware>();

app.MapReverseProxy();

app.Run();
