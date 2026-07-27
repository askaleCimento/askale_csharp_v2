using AskalePortal.BLL.Education;
using AskalePortal.DAL.Education;
using AskalePortal.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AskalePortal.API.Extensions;

public static class EducationModuleExtensions
{
    public static IServiceCollection AddEducationModule(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var mode = Environment.GetEnvironmentVariable("ASKALE_ENVIRONMENT")?.ToLowerInvariant()
                   ?? (environment.IsProduction() ? "server" : environment.IsDevelopment() ? "local" : "test");
        var connectionString = configuration[$"Connectionstrings:{mode}"]
                               ?? configuration.GetConnectionString(mode)
                               ?? throw new InvalidOperationException($"Connectionstrings:{mode} configuration is missing.");

        services.AddDbContext<DBDataContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.UseCompatibilityLevel(120)));

        services.AddScoped<IEducationRepository, EducationRepository>();
        services.AddScoped<IEducationSectionRepository, EducationSectionRepository>();
        services.AddScoped<IEducationVideoRepository, EducationVideoRepository>();
        services.AddScoped<IEgitimSorulariRepository, EgitimSorulariRepository>();
        services.AddScoped<IEducationVideoDurationRepository, EducationVideoDurationRepository>();
        services.AddScoped<IEgitimSoruCevapRepository, EgitimSoruCevapRepository>();
        services.AddScoped<IEducationQuestionRepository, EducationQuestionRepository>();
        services.AddScoped<IEducationQuestionSectionRepository, EducationQuestionSectionRepository>();
        services.AddScoped<IEducationQuestionAnswerRepository, EducationQuestionAnswerRepository>();

        services.AddScoped<IEducationService, EducationService>();
        services.AddScoped<IEducationSectionService, EducationSectionService>();
        services.AddScoped<IEducationVideoService, EducationVideoService>();
        services.AddScoped<IEgitimSorulariService, EgitimSorulariService>();
        services.AddScoped<IEducationVideoDurationService, EducationVideoDurationService>();
        services.AddScoped<IEgitimSoruCevapService, EgitimSoruCevapService>();
        services.AddScoped<IEducationQuestionService, EducationQuestionService>();
        services.AddScoped<IEducationQuestionSectionService, EducationQuestionSectionService>();
        services.AddScoped<IEducationQuestionAnswerService, EducationQuestionAnswerService>();
        return services;
    }
}
