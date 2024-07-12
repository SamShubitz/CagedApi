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

app.MapGet("/", () => "unCAGED Guitar App API");

app.MapGet("/users", async (ProgressionDb db) =>
{
    var allUsers = await db.Users.Include(u => u.ProgressionList).ToListAsync();
    return Results.Ok(allUsers);
});

app.MapPost("/users", async (User user, ProgressionDb db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});

app.MapGet("/progressions", async (ProgressionDb db) =>
{
    var progressions = await db.Progressions.Include(p => p.ChordList).ToListAsync();
    return Results.Ok(progressions);
});

app.MapGet("/{id}/progressions", async (ProgressionDb db, int id, string title) =>
{
    var user = await db.Users
        .Include(u => u.ProgressionList)
        .ThenInclude(p => p.ChordList)
        .FirstOrDefaultAsync(u => u.Id == id);

    if (user == null)
    {
        return Results.NotFound();
    }

    else
    {
        var progression = user.ProgressionList
            .FirstOrDefault(p => p.Title == title);

        return progression != null ? Results.Ok(progression) : Results.NotFound();
    }
});

app.MapGet("/{id}/progressions/titles", async (int id, ProgressionDb db) =>
{
    var user = await db.Users
        .Include(u => u.ProgressionList)
        .FirstOrDefaultAsync(u => u.Id == id);

    if (user == null)
    {
        return Results.NotFound();
    }

    var allTitles = user.ProgressionList.Select(p => p.Title).ToList();
    return Results.Ok(allTitles);
});

app.MapPost("/{id}/progressions", async (int id, Progression progression, ProgressionDb db) =>
{
    var user = await db.Users
        .Include(u => u.ProgressionList)
        .FirstOrDefaultAsync(u => u.Id == id);

    if (user == null)
    {
        return Results.NotFound();
    }

    user.ProgressionList.Add(progression);
    await db.SaveChangesAsync();
    return Results.Created($"/{id}/progressions/{progression.ProgressionId}", progression);
});

app.MapPut("/progressions/", async (Progression inputProgression, ProgressionDb db) =>
{
    var id = inputProgression.ProgressionId;
    var progression = await db.Progressions
        .Include(p => p.ChordList)
        .FirstOrDefaultAsync(p => p.ProgressionId == id);

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
