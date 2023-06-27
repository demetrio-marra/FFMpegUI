using FFMpegUI.Persistence;
using FFMpegUI.Persistence.Mapping;
using FFMpegUI.Persistence.Repositories;
using FFMpegUI.Services;
using FFMpegUI.Services.Configuration;
using FFMpegUI.Services.Middlewares;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents;
using FFMpegUI.Infrastructure.Support;
using FFMpegUI.Infrastructure.Resilience;
using FFMpegUI.Resilience;
using Microsoft.AspNetCore.Http.Features;
using FFMpegUI.Mvc.Hubs;

namespace FFMpegUI.Mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FFMpegDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("SqlDb");

                var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
                {
                    TrustServerCertificate = true // Ignore SSL certificate validation
                                                  //SslMode = SslMode.VerifyCA
                };

                options.UseSqlServer(sqlConnectionStringBuilder.ConnectionString, b => b.MigrationsAssembly("FFMpegUI.Mvc"));
            });

            builder.Services.AddSingleton<IResilientPoliciesLocator, ResilientPoliciesLocator>();

            builder.Services.AddHttpClient("QFileServerApiServiceClient", client =>
            {
                var url = builder.Configuration.GetValue<string>("QFileServerApiUrl");
                if (url == null)
                {
                    throw new Exception("QFileServerApiUrl is null");
                }
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromMinutes(10);
            });

            builder.Services.AddHttpClient("FFMpegApiServiceClient", client =>
            {
                var url = builder.Configuration.GetValue<string>("FFMpegUI:ApiEndpoint");
                if (url == null)
                {
                    throw new Exception("ApiEndpoint is null");
                }
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromMinutes(10);
            });

            builder.Services.AddAutoMapper(
                typeof(Program).Assembly,
                typeof(FFMpegPersistenceMapperProfile).Assembly
            );

            builder.Services.AddScoped<IFFMpegProcessFeaturesRepository, RavenFFMpegProcessFeaturesRepository>();
            builder.Services.AddScoped<IFFMpegProcessRepository, SQLFFMpegProcessRepository>();
            builder.Services.AddScoped<IFFMpegProcessItemsRepository, SQLFFMpegProcessItemsRepository>();

            // raven
            var ravenUrls = builder.Configuration.GetValue<string>("RavenDb:Urls").Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var store = new DocumentStore
            {
                Urls = ravenUrls,
                Database = builder.Configuration.GetValue<string>("RavenDb:Database")
            };

            store.Initialize();

            using (var session = store.OpenSession())
            {
                var databaseNames = session.Advanced.DocumentStore.Maintenance.Server.Send(new Raven.Client.ServerWide.Operations.GetDatabaseNamesOperation(0, int.MaxValue));
                var databaseExists = databaseNames.Contains(store.Database, StringComparer.OrdinalIgnoreCase);

                if (!databaseExists)
                {
                    session.Advanced.DocumentStore.Maintenance.Server.Send(new Raven.Client.ServerWide.Operations.CreateDatabaseOperation(new Raven.Client.ServerWide.DatabaseRecord(store.Database)));
                }

                // Perform any required initialization operations, such as creating indexes or setting up document types
                IndexCreation.CreateIndexes(typeof(Program).Assembly, store);
            }

            builder.Services.AddSingleton<IDocumentStore>(store);

            // Build the FFMpegUIServiceConfiguration object from the configuration section
            var ffmpegUIConfig = builder.Configuration.GetSection("FFMpegUI").Get<FFMpegUIServiceConfiguration>();

            // Register the FFMpegUIServiceConfiguration instance as a singleton
            builder.Services.AddSingleton(ffmpegUIConfig);

            builder.Services.AddScoped<IQFileServerApiService, QFileServerApiService>();
            builder.Services.AddScoped<IFFMpegUIApiService, FFMpegApiService>();

            builder.Services.AddSingleton<BackgroundTaskQueue>();
            builder.Services.AddHostedService<ConvertProcessTaskRunner>();

            builder.Services.AddScoped<IFFMpegManagementService, FFMpegManagementService>();

            builder.Services.AddRazorPages();

            builder.Services.AddSignalR(); // Add SignalR service

            // Configure the maximum request body size before calling UseRouting, UseEndpoints, etc.
            // 1_000_000_000 represents the new size limit in bytes (about 1GB in this example).
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = long.MaxValue;
            });

            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = null; // 1_000_000_000;
            });

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Limits.MaxRequestBodySize = null; // 1_000_000_000;
            });

            // Set URLs
            builder.WebHost.UseUrls("http://*:80");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.MapHub<ReportProgressHub>("/reportprogresshub");

            Task.Run(async () => {
                using (var scope = app.Services.CreateScope())
                {
                    var policyLocator = scope.ServiceProvider.GetRequiredService<IResilientPoliciesLocator>();
                    var sqlPolicy = policyLocator.GetPolicy(ResilientPolicyType.SqlDatabase);

                    await sqlPolicy.ExecuteAsync(async () =>
                    {
                        // Resolve YourDbContext within the scope
                        var dbContext = scope.ServiceProvider.GetRequiredService<FFMpegDbContext>();

                        // Apply migrations
                        await dbContext.Database.MigrateAsync();
                    });
                }
            }).Wait();

            app.Run();
        }
    }
}