using Microsoft.EntityFrameworkCore;
using CagedApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProgressionDb>(options =>
options.UseNpgsql("Host=localhost;Port=5433;Database=CagedDb;"));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddAuthorization();


var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowLocalhost");
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.MapGet("/progressions", async (ProgressionDb db, string? title) =>
{
    if (!string.IsNullOrEmpty(title))
    {
        var progression = await db.Progressions
            .Include(p => p.ChordList)
            .FirstOrDefaultAsync(p => p.Title == title);

        return progression != null ? Results.Ok(progression) : Results.NotFound();
    }
    else
    {
        var progressions = await db.Progressions
            .Include(p => p.ChordList)
            .ToListAsync();

        return Results.Ok(progressions);
    }
});

app.MapGet("/progressions/{title}", async (string title, ProgressionDb db) =>
    await db.Progressions.Include(p => p.ChordList).FirstOrDefaultAsync(p => p.Title == title)
        is Progression progression
            ? Results.Ok(progression)
            : Results.NotFound());

app.MapPost("/progressions", async (Progression progression, ProgressionDb db) =>
{
    db.Progressions.Add(progression);
    await db.SaveChangesAsync();

    return Results.Created($"/progressions/{progression.ProgressionId}", progression);
});

app.MapPut("/progressions/{id}", async (int id, Progression inputProgression, ProgressionDb db) =>
{
    var progression = await db.Progressions.FindAsync(id);

    if (progression is null) return Results.NotFound();

    progression.Title = inputProgression.Title;
    progression.ChordList = inputProgression.ChordList;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/progressions/{id}", async (int id, ProgressionDb db) =>
{
    var progression = await db.Progressions
        .Include(p => p.ChordList)
        .FirstOrDefaultAsync(p => p.ProgressionId == id);

    if (progression == null)
        return Results.NotFound();

    db.Chords.RemoveRange(progression.ChordList);
    db.Progressions.Remove(progression);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
