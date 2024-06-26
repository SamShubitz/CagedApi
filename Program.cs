using Microsoft.EntityFrameworkCore;
using CagedApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ProgressionDb>(opt => opt.UseInMemoryDatabase("ProgressionList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/progressions", async (ProgressionDb db) =>
    await db.Progressions.Include(p => p.ChordList).ToListAsync());

app.MapGet("/progressions/{title}", async (string title, ProgressionDb db) =>
    await db.Progressions.Include(p => p.ChordList).FirstOrDefaultAsync(p => p.Title == title)
        is Progression progression
            ? Results.Ok(progression)
            : Results.NotFound());

app.MapPost("/progressions", async (Progression progression, ProgressionDb db) =>
{
    db.Progressions.Add(progression);
    await db.SaveChangesAsync();

    return Results.Created($"/progressions/{progression.Title}", progression);
});

app.MapPut("/progressions/{title}", async (string title, Progression inputProgression, ProgressionDb db) =>
{
    var progression = await db.Progressions.FindAsync(title);

    if (progression is null) return Results.NotFound();

    progression.Title = inputProgression.Title;
    progression.ChordList = inputProgression.ChordList;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/progressions/{title}", async (string title, ProgressionDb db) =>
{
    if (await db.Progressions.FindAsync(title) is Progression progression)
    {
        db.Progressions.Remove(progression);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();
