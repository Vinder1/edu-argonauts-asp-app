using Argonauts.Web.Endpoints;
using Argonauts.Web.Extensions;
using Carter;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAutoMapper();
builder.ConfigureAuthorization();
builder.ConfigureSwagger();
builder.ConfigureLogger();
builder.ConfigureHttpLogging();
builder.AddValkeyCache();

builder.AddOptions();
builder.AddRepositories();
builder.AddServices();
builder.AddValidation();
builder.AddCustomExceptionHandler();
builder.AddFastHangfire();
builder.AddGameContent();

builder.Services.AddCarter();

if (builder.Environment.IsDevelopment())
{
    builder.AddFrontendCors();
}

var app = builder.Build();

app.UseExceptionHandler();

app.UseRouting();

app.UseHttpMetrics();
app.MapMetrics().AllowAnonymous();

// app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCorrelationIdGenerator();
app.UseCustomSerilogAndHttpLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/world", () => Results.File("index.html", "text/html"));

app.MapCarter();
app.MapGameHub();

app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        options.RoutePrefix = "swagger";
    });
}

app.Run();
