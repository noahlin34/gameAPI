using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PlayerDB>(opt => opt.UseInMemoryDatabase("PlayerDB"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config => {
    config.DocumentName="PlayerAPI";
    config.Title = "PlayerAPI v1";
    config.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseOpenApi();
    app.UseSwaggerUi(config => {
        config.DocumentTitle = "Player API";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.MapGet("/players", async (PlayerDB db) => await db.Players.ToListAsync());

app.MapGet("/players/{id}", async (int id, PlayerDB db) => 
    await db.Players.FindAsync(id) 
    is Player player 
    ? Results.Ok(player) 
    : Results.NotFound());



app.MapPost("/players", async (Player player, PlayerDB db) => {
    db.Players.Add(player);
    await db.SaveChangesAsync();

    return Results.Created($"/players/{player.Id}", player);
});

//function to update hiscore for given player
app.MapPut("/players/{id}", async (int id,  int hiScore, PlayerDB db) => {
    var player = await db.Players.FindAsync(id);
    if (player is null) return Results.NotFound();

    player.HiScore = hiScore;
    
    await db.SaveChangesAsync();
    return Results.Ok($"HiScore updated to {hiScore} for player id {player.Id}");
});

app.MapDelete("/players/{id}", async (int id, PlayerDB db) => {
    if (await db.Players.FindAsync(id) is Player player) {
        db.Players.Remove(player);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
});



app.Run();
