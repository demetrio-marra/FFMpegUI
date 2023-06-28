using FFMpegUI.Infrastructure;
using FFMpegUI.Infrastructure.Support;
using FFMpegUI.Services;
using FFMpegUI.Services.Middlewares;
using Microsoft.AspNetCore.Http.Features;

namespace FFMpegUI.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<ProcessItemBackgroundTaskQueue>();

            // Add services to the container.
            builder.Services.AddHttpClient("QFileServerApiServiceClient", client =>
            {
                var url = builder.Configuration.GetValue<string>("QFileServerApiUrl");
                client.BaseAddress = new Uri(url);
            });

            builder.Services.AddHttpClient(Constants.FFMpegUIMvcProgressMessagesEndpointClientName, client =>
            {
                var url = builder.Configuration.GetValue<string>("FFMpegUIMvcProgressMessagesEndpointUrl");
                client.BaseAddress = new Uri(url);
            });

            builder.Services.AddScoped<IProgressMessagesDispatcher, ProgressMessageDispatcher>();
            builder.Services.AddScoped<IQFileServerApiService, QFileServerApiService>();
            builder.Services.AddScoped<IFFMpegConvertingService, FFMpegConversionService>();

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

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHostedService<ConvertProcessItemTaskRunner>();


            // Set URLs
            builder.WebHost.UseUrls("http://*:80");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            //app.UseAuthorization();

            app.MapControllers();


            app.Run();
        }
    }
}
