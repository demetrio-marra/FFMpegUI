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
using Microsoft.Extensions.Configuration;

namespace FFMpegUI.Mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // Configure the HTTP request pipeline.

            // Set the certificate validation callback
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

            builder.Services.AddHttpClient("QFileServerApiServiceClient", client =>
            {
                var url = builder.Configuration.GetValue<string>("QFileServerApiUrl");
                if (url == null)
                {
                    throw new Exception("QFileServerApiUrl is null");
                }
                client.BaseAddress = new Uri(url);
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

            builder.Services.AddScoped<IFFMpegConvertingService, FFMpegService>();
            builder.Services.AddScoped<IFFMpegManagementService, FFMpegService>();

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

            // Get an instance of IServiceProvider from the host
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // Resolve YourDbContext within the scope
                var dbContext = services.GetRequiredService<FFMpegDbContext>();

                // Apply migrations
                dbContext.Database.Migrate();
            }

            app.Run();
        }
    }
}