using Microsoft.EntityFrameworkCore;
using Notes.Api.Data;
using Notes.Api.Data.Features.CreateNote;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<NotesDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("notely-notes")));
builder.EnrichSqlServerDbContext<NotesDbContext>();

//builder.Services.AddDbContext<NotesDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("notely-notes")));
//builder.EnrichNpgsqlDbContext<NotesDbContext>();

builder.Services.AddHttpClient("TagsApi", client =>
{
    client.BaseAddress = new Uri("https+http://tags-api"); // As named in AppHost.cs
});

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
    var dbContext = scope.ServiceProvider.GetRequiredService<NotesDbContext>();
    //dbContext.Database.Migrate();
}

app.UseHttpsRedirection();

app.MapPost("notes", CreateNoteEndpoint.CreateNote);

app.Run();
