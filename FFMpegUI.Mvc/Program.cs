using FFMpegUI.Models;
using FFMpegUI.Persistence;
using FFMpegUI.Persistence.Entities;
using FFMpegUI.Persistence.Mapping;
using FFMpegUI.Persistence.Repositories;
using FFMpegUI.Services;
using FFMpegUI.Services.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Raven.DependencyInjection;

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
            if (!builder.Environment.IsDevelopment())
            {
                builder.Services.AddDbContext<FFMpegDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("FFMpegUI")));
            }
            else
            {
                builder.Services.AddDbContext<FFMpegDbContext>(options =>
                    options.UseInMemoryDatabase("FFMpegUI")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                );
            }

           

            // RavenDB Configuration
            builder.Services.AddRavenDbDocStore(options =>
            {
                options.Settings = new RavenSettings
                {
                    Urls = builder.Configuration.GetValue<string[]>("RavenDb:Urls") ,
                    DatabaseName = builder.Configuration.GetValue<string>("RavenDb:Database")
                    // Configure any additional settings here...
                };
                options.SectionName = "RavenDb";
                options.GetConfiguration = builder.Configuration;

                // If using a certificate
                // options.Certificate = new X509Certificate2("path-to-certificate", "optional-password");

                // BeforeInitializeDocStore and AfterInitializeDocStore can be used to execute code before and after the document store is initialized, respectively.
                // For example, to set conventions or register indexes.
            });

            builder.Services.AddAutoMapper(
                typeof(Program).Assembly, 
                typeof(FFMpegPersistenceMapperProfile).Assembly
            );

            builder.Services.AddScoped<IFFMpegProcessFeaturesRepository, RavenFFMpegProcessFeaturesRepository>();
            builder.Services.AddScoped<IFFMpegProcessRepository, SQLFFMpegProcessRepository>();
            builder.Services.AddScoped<IFFMpegProcessItemsRepository, SQLFFMpegProcessItemsRepository>();

            // Build the FFMpegUIServiceConfiguration object from the configuration section
            var ffmpegUIConfig = builder.Configuration.GetSection("FFMpegUI").Get<FFMpegUIServiceConfiguration>();

            // Register the FFMpegUIServiceConfiguration instance as a singleton
            builder.Services.AddSingleton(ffmpegUIConfig);
            builder.Services.AddScoped<IFFMpegConvertingService, FFMpegService>();
            builder.Services.AddScoped<IFFMpegManagementService, FFMpegService>();

            var app = builder.Build();


            // Add dummy data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<FFMpegDbContext>();

                // Ensure the database is created
                dbContext.Database.EnsureCreated();

                // Check if any data already exist
                if (!dbContext.Processes.Any())
                {
                    // Creating dummy data
                    var dummyData = Enumerable.Range(1, 50).Select(index => new FFMpegPersistedProcess
                    {
                        Id = index,
                        SubmissionDate = DateTime.Now,
                        StartDate = DateTime.Now.AddHours(-index),
                        EndDate = DateTime.Now.AddHours(index),
                         Items = Enumerable.Range(1, 3).Select(index2 => new FFMpegPersistedProcessItem
                         {
                              Id = (index - 1) * 50 + index2,
                               DestFileFullPath = "/var/ffmpegdata/converted",
                                SourceFileFullPath = "/var/ffmpegdata/sourcefiles",
                                 StartDate = DateTime.Now,
                                  ProcessId = index,
                                   Successfull = true,
                                   EndDate = DateTime.Now.AddHours(3)
                         }).ToList()
                    }).ToList();

                    dbContext.Processes.AddRange(dummyData);
                    dbContext.SaveChanges();
                }
            }

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


            if (!app.Environment.IsDevelopment())
            {
                // Auto migrate database
                using (var serviceScope = app.Services.CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<FFMpegDbContext>();
                    context!.Database.Migrate();
                }
            }
            app.Run();
        }
    }
}