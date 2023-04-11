using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Product.Api.Data;
using Product.Api.Middleware;
using Product.Api.Services;
using Serilog;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Product.Api
{
    public static class Program
    {
        private static readonly string Env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Env}.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {
            Console.Title = "Demo Product API";

            string serviceName = "mx-local-service";
            string serviceVersion = "1.0.0";
            string sourceName = "mx-local-source";
            ActivitySource MyActivitySource = new(serviceName);

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            ResourceBuilder appResourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName: serviceName, serviceVersion: serviceVersion);

            using Meter meter = new(serviceName);
            Counter<long> counter = meter.CreateCounter<long>("app.mx.request-counter");

            // Configure important OpenTelemetry settings, the console exporter, and instrumentation library
            _ = builder.Services.AddOpenTelemetry().WithTracing(b =>
            {
                _ = b.AddConsoleExporter()
                .AddSource(sourceName).SetResourceBuilder(resourceBuilder: ResourceBuilder.CreateDefault().AddService(serviceName: serviceName, serviceVersion: serviceVersion))
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation();

            }).WithMetrics(b =>
            {
                _ = b
                .AddConsoleExporter()
                .AddMeter(meter.Name)
                .SetResourceBuilder(appResourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();
            });

            _ = builder.Host.UseSerilog((context, services, configuration) =>
            {
                // string outputTemplate = "{NewLine}[{Timestamp:HH:mm:ss} {Level:u3} {Properties}]{NewLine}{Message:lj}{NewLine}{Exception}";

                _ = configuration
                    .ReadFrom.Configuration(builder.Configuration);
                // .WriteTo.DatadogLogs(new DatadogJsonNoTemplateFormatter());
            });

            _ = builder.Services
                .AddDbContextPool<ProductDbContext>(o => o.UseInMemoryDatabase(databaseName: "ProductDb"))
                .AddScoped<IProductService, ProductService>()
                .AddScoped<IRepository<Entities.Product>, GenericRepository<Entities.Product, ProductDbContext>>();

            // Add services to the container.
            _ = builder.Services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = ctx =>
                {
                    if (ctx.ProblemDetails.Status >= 500)
                    {
                        ctx.ProblemDetails.Detail = "Contact app support team with the 'traceId' value.";
                    }

                    // ctx.ProblemDetails.Instance = problemCorrelationId; // Guid
                    // _logger.LogError(problemCorrelationId);
                };
            });

            _ = builder.Services.AddHttpContextAccessor();
            _ = builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            _ = builder.Services.AddEndpointsApiExplorer();
            _ = builder.Services.AddSwaggerGen();

            WebApplication app = builder.Build();

            // More on ".UseHttpLogging": https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-logging/?view=aspnetcore-6.0

            /*_ = app.Environment.IsDevelopment() ?
                app.UseExceptionHandler("/error-details") :
                app.UseExceptionHandler("/error"); */

            /* _ = app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async httpContext =>
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        httpContext.Response.ContentType =
                            "application/problem+json"; // MediaTypeNames.Application.Json;

                        IExceptionHandlerFeature? exceptionHandlerFeatureFeature = httpContext.Features.Get<IExceptionHandlerFeature>();

                        if (exceptionHandlerFeatureFeature != null)
                        {
                            await httpContext.Response.WriteAsync(new ProblemDetails()
                            {
                                Status = httpContext.Response.StatusCode,
                                Title = "Internal Server Error",
                                Detail = exceptionHandlerFeatureFeature.Error.InnerException?.Message

                            }.ToString());
                        }
                    }
                );
            }); */

            /*
            _ = app.UseHttpLogging();
            _ = app.UseW3CLogging();
            */

            // Without 'UseExceptionHandler' we will get full stacktrace in the ProblemDetails
            _ = app.UseExceptionHandler();

            /*_ = app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    // string body = httpContext.Request.Body.;
                    // diagnosticContext.Set("Body", body);
                };

                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} {Body} responded {StatusCode} in {Elapsed:0.0000}";

            }); */

            // ... instead of running migration command from CLI
            using (IServiceScope scope = app.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;
                ProductDbContext context = services.GetRequiredService<ProductDbContext>();
                _ = context.Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                _ = app.UseSwagger();
                _ = app.UseSwaggerUI();
                _ = app.MapFallback(() => Results.Redirect("/swagger"));
            }

            _ = app.UseHttpsRedirection();
            _ = app.UseAuthorization();
            _ = app.UseMiddleware<UserScopeMiddleware>();
            _ = app.MapControllers();

            app.Run();
        }
    }
}