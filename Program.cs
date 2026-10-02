using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiLab4.Data;
using WebApiLab4.Mapping;
using WebApiLab4.Repositories;
using WebApiLab4.Results;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
            .SelectMany(value => value.Errors)
            .Select(error => error.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message));
        return new BadRequestObjectResult(ReturnResult<object>.Failure(errors));
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddAutoMapper(cfg => { }, typeof(AutoMapperProfile));

var app = builder.Build();

app.UseExceptionHandler(handler => handler.Run(async context =>
{
    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("ApiExceptionHandler");
    var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    logger.LogError(exception, "Unhandled error while processing {Path}", context.Request.Path);
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    context.Response.ContentType = "application/json; charset=utf-8";
    await JsonSerializer.SerializeAsync(context.Response.Body,
        ReturnResult<object>.Failure(new[] { "Внутренняя ошибка сервера. Повторите запрос позже." }),
        new JsonSerializerOptions(JsonSerializerDefaults.Web),
        cancellationToken: context.RequestAborted);
}));

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
