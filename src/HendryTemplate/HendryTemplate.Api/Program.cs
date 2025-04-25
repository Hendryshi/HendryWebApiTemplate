using Serilog;
using HendryTemplate.Application;
using HendryTemplate.Infrastructure;
using ZymLabs.NSwag.FluentValidation;
using HendryTemplate.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Serilog.Log.Logger = Common.Application.Serilog.SerilogConfiguration.CreateSerilogLogger(builder.Configuration);
builder.Host.UseSerilog(Serilog.Log.Logger);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureSqlServices(builder.Configuration);
builder.Services.AddAPIServices(builder.Configuration);

builder.Services.AddScoped<FluentValidationSchemaProcessor>(provider =>
{
    var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
    var loggerFactory = provider.GetService<ILoggerFactory>();

    return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
    app.UseOpenApi();

    app.UseSwaggerUi(settings =>
    {

    });

}

app.UseRouting();
if(app.Configuration.GetSection("Cors:AllowAll").Get<bool>())
{
    app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
}
else
{
    app.UseCors(options => options.AllowAnyHeader().WithOrigins([.. app.Configuration.GetSection("Cors:AllowSpecific").Get<List<string>>()]));
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
