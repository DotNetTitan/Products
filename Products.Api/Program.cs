using Products.Api.Filters;
using Products.Api.Middlewares;
using Products.Application;
using Products.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication();

builder.Services.AddScoped(typeof(ValidationFilter<>));

builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI();

app.UseGlobalExceptionHandling();

app.MapControllers();

app.Run();