using FFMpegUI.Persistence.Mapping;
using FFMpegUI.Services;
using FFMpegUI.Services.Middlewares;

namespace FFMpegUI.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHttpClient("QFileServerApiServiceClient", client =>
            {
                var url = builder.Configuration.GetValue<string>("QFileServerApiUrl");
                if (url == null)
                {
                    throw new Exception("QFileServerApiUrl is null");
                }
                client.BaseAddress = new Uri(url);
            });

            builder.Services.AddScoped<IQFileServerApiService, QFileServerApiService>();
            builder.Services.AddScoped<IFFMpegConvertingService, FFMpegConversionService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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