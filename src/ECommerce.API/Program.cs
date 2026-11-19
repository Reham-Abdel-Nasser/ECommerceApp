using Asp.Versioning;
using ECommerce.API;
using ECommerce.API.Endpoints;
using ECommerce.API.Extensions;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.Persistence.DbContexts;
using ECommerce.Infrastructure.Persistence.Seeding;
using ECommerce.UseCases;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

const string AngularCorsPolicy = "AngularCorsPolicy";

try
{
    // serilog + seq
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddOutputCache(options =>
    {
        options.AddPolicy("Products", policy =>
        {
            policy.Expire(TimeSpan.FromMinutes(1))
                .SetVaryByQuery("pagesize", "pageNumber");
        });
    });

    // Add services to the container.
    builder.Services.AddPresentation();

    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddApplication();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(AngularCorsPolicy, policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    builder.Services.AddAppRateLimiting();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseExceptionHandler();

    app.UseHttpsRedirection();

    app.UseCors(AngularCorsPolicy);

    app.UseOutputCache();

    app.UseRateLimiter();

    app.UseAuthentication();

    app.UseAuthorization();

    var apiVersionSet = app.NewApiVersionSet()
        .HasApiVersion(new ApiVersion(1, 0))
        .ReportApiVersions()
        .Build();

    app.MapAuthEndpoints(apiVersionSet);
    app.MapUserEndpoints(apiVersionSet);
    app.MapProductEndpoints(apiVersionSet);
    app.MapTypeEndpoints(apiVersionSet);
    app.MapBrandEndpoints(apiVersionSet);
    app.MapBasketEndpoints(apiVersionSet);
    app.MapDeliveryMethodEndpoints(apiVersionSet);
    app.MapOrderEndpoints(apiVersionSet);
    app.MapPaymentEndpoints(apiVersionSet);

    app.UseSwagger(); // runs middleware make documentation file available to use

    app.UseSwaggerUI(options =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger(); // runs middleware make documentation file available to use

        app.UseSwaggerUI(options =>
        {
            foreach (var description in app.DescribeApiVersions())
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }
        });

        await using var scope = app.Services.CreateAsyncScope();
        var sp = scope.ServiceProvider;

        var identityDb = sp.GetRequiredService<IdentityStoreDbContext>();
        await identityDb.Database.MigrateAsync();

        var appDb = sp.GetRequiredService<StoreDbContext>();
        await appDb.Database.MigrateAsync();

        await sp.GetRequiredService<DatabaseSeeder>().SeedAll();
    }

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}