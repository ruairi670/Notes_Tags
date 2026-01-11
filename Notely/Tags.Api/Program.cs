using Microsoft.EntityFrameworkCore;
using Tags.Api.Data;
using Tags.Api.Features.AnalyzeNote;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<TagsDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("notely-tags")));
builder.EnrichSqlServerDbContext<TagsDbContext>();

//builder.Services.AddDbContext<TagsDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("notely-tags")));
//builder.EnrichNpgsqlDbContext<TagsDbContext>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Apply EF Migrations
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<TagsDbContext>();
    //dbContext.Database.Migrate();
}

app.UseHttpsRedirection();

app.MapPost("tags/analyze", AnalyzeNoteEndpoint.AnalyzeNote);

app.Run();
