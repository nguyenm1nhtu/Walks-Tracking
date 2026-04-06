using Microsoft.EntityFrameworkCore;
using Walks.API.Data;
using Walks.API.Mapping;
using Walks.API.Seeds;
using Walks.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfiles>());

// Swagger
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<WalksDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("WalksConnectionString"))
    .UseSeeding((context, _) =>
    {
        var dbContext = (WalksDbContext)context;

        DifficultySeed.Seed(dbContext);
        RegionSeed.Seed(dbContext);
        dbContext.SaveChanges();

        WalkSeed.Seed(dbContext);
        dbContext.SaveChanges();
    })
    .UseAsyncSeeding(async (context, _, cancellationToken) =>
    {
        var dbContext = (WalksDbContext)context;

        await DifficultySeed.SeedAsync(dbContext, cancellationToken);
        await RegionSeed.SeedAsync(dbContext, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await WalkSeed.SeedAsync(dbContext, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }));

// Repositories
builder.Services.AddScoped<IRegionRepository, RegionRepository>();
builder.Services.AddScoped<IWalkRepository, WalkRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WalksDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        foreach (var url in app.Urls)
        {
            var baseUrl = url.TrimEnd('/');
            Console.WriteLine($"Swagger UI: {baseUrl}/swagger");
            Console.WriteLine($"OpenAPI JSON: {baseUrl}/openapi/v1.json");
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
