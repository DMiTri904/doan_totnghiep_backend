using Microsoft.EntityFrameworkCore;
using project.Application.Dependency;
using project.Infrastructure.Database;
using project.Infrastructure.Depedencies;

namespace project.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddApplicationService();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy("AllowedReactApp", policy =>
                {
                    policy.WithOrigins("https://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("AllowedReactApp");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
