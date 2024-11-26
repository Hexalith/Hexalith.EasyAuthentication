// <copyright file="Program.cs" company="Hexalith SAS Paris France">
//     Copyright (c) Hexalith SAS Paris France. All rights reserved.
//     Licensed under the MIT license.
//     See LICENSE file in the project root for full license information.
// </copyright>

using Hexalith.Infrastructure.AspireService.Hosting.Helpers;

using Microsoft.Extensions.Configuration;

HexalithDistributedApplication app = new(args);

app.Builder.AddForwardedHeaders();

if (app.Builder.ExecutionContext.IsRunMode)
{
    Console.WriteLine($"Starting environment {app.Builder.Environment.EnvironmentName}");
    _ = app
        .Builder
        .AddExecutable("dapr-dashboard", "dapr", ".", "dashboard")
        .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "dashboard-http", isProxied: false);
}

app.Builder.Configuration.AddUserSecrets<Program>();

if (app.IsProjectEnabled<Projects.HexalithApp_WebServer>())
{
    _ = app
        .AddProject<Projects.HexalithApp_WebServer>("security-web")
        .WithEnvironmentFromConfiguration("Hexalith__Security__UseMsal")
        .WithEnvironmentFromConfiguration("Hexalith__Security__Enabled")
        .WithEnvironmentFromConfiguration("EmailServer__ApplicationSecret")
        .WithEnvironmentFromConfiguration("EmailServer__FromEmail")
        .WithEnvironmentFromConfiguration("EmailServer__FromName")
        .WithEnvironmentFromConfiguration("AzureAd__Instance")
        .WithEnvironmentFromConfiguration("AzureAd__Domain")
        .WithEnvironmentFromConfiguration("AzureAd__TenantId")
        .WithEnvironmentFromConfiguration("AzureAd__ClientId");
}

if (app.IsProjectEnabled<Projects.HexalithApp_WebServer>())
{
    _ = app
        .AddProject<Projects.HexalithApp_ApiServer>("security-api");
}

await app
    .Builder
    .Build()
    .RunAsync()
    .ConfigureAwait(false);