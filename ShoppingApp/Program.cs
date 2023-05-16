using Microsoft.AspNetCore.Diagnostics;
using ShoppingApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ShoppingService>();
builder.Services.AddHttpClient<ShoppingService>(client => client.BaseAddress = new Uri("https://fakestoreapi.com/"));

var app = builder.Build();
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        if (context.Response.StatusCode == StatusCodes.Status404NotFound)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
        }

        var exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature?.Error is ApplicationException)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    });
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
