
using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Persistence;
using Persistence.Data;
using Persistence.Repositories;
using Service;
using Service.MappingProfiles;
using ServiceAbstraction;
using System.Threading.Tasks;

namespace NGUWep.api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add Services To The Container 
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<StoreDbContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddScoped<IDataSeeding, DataSeeding>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(typeof(Service.AssemblyReference).Assembly);
            builder.Services.AddScoped<IServiceManager, ServiceManager>();
            #endregion

            var app = builder.Build();
            var Scope = app.Services.CreateScope();
            var ObjectDataSeed = Scope.ServiceProvider.GetRequiredService<IDataSeeding>();
            await ObjectDataSeed.DataSeedAsync();


            #region Configure The HTTP Request Pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}
