using Microsoft.EntityFrameworkCore;
using StudyPlanerSamPlusEFGame.Data;
using StudyPlanerSamPlusEFGame.Endpoints_Controller_;
using StudyPlanner.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StudyPlannerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    
    c.RoutePrefix = "swagger";
});

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapSubjectsEndpoint();


// фронт, swager, web server, xnu, эндпоинты создание, пот пост и д.р запросы

app.Run();