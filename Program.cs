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
    var allUsers = await db.Users.Select(u => u.Id).ToListAsync();
    return Results.Ok(allUsers);
});

app.MapPost("/users", async (User user, ProgressionDb db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});

app.MapGet("/{email}/progressions", async (ProgressionDb db, string email, string? title) =>
{
    var user = await db.Users
        .Include(u => u.ProgressionList)
        .ThenInclude(p => p.ChordList)
        .FirstOrDefaultAsync(u => u.Email == email);

    if (user == null)
    {
        return Results.NotFound();
    }

    if (!string.IsNullOrEmpty(title))
    {
        var progression = user.ProgressionList
            .FirstOrDefault(p => p.Title == title);

        return progression != null ? Results.Ok(progression) : Results.NotFound();
    }
    else
    {
        return Results.Ok(user.ProgressionList);
    }
});

app.MapGet("/{email}/progressions/titles", async (string email, ProgressionDb db) =>
{
    var user = await db.Users
        .Include(u => u.ProgressionList)
        .FirstOrDefaultAsync(u => u.Email == email);

    if (user == null)
    {
        return Results.NotFound();
    }

    var allTitles = user.ProgressionList.Select(p => p.Title).ToList();
    return Results.Ok(allTitles);
});

app.MapPost("/{email}/progressions", async (string email, Progression progression, ProgressionDb db) =>
{
    var user = await db.Users
        .Include(u => u.ProgressionList)
        .FirstOrDefaultAsync(u => u.Email == email);

    if (user == null)
    {
        return Results.NotFound();
    }

    user.ProgressionList.Add(progression);
    await db.SaveChangesAsync();
    return Results.Created($"/{email}/progressions/{progression.ProgressionId}", progression);
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
