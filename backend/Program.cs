using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Walks.API.Data;
using Walks.API.Mapping;
using Walks.API.Middleware;
using Walks.API.Options;
using Walks.API.Repositories;
using Walks.API.Seeds;
using Serilog;

var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
Directory.CreateDirectory(uploadsPath);

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "uploads"
});

// Serilog configuration
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(allowIntegerValues: false));
    });

// AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfiles>());

// Identity
builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("Walks")
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// JWT Options
builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .Validate(options => !string.IsNullOrWhiteSpace(options.Key), "Missing configuration: Jwt:Key")
    .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer), "Missing configuration: Jwt:Issuer")
    .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), "Missing configuration: Jwt:Audience")
    .Validate(options => options.DurationInMinutes > 0, "Invalid configuration: Jwt:DurationInMinutes must be greater than 0")
    .ValidateOnStart();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services
    .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtOptions>>((options, jwtOptionsAccessor) =>
    {
        var jwtOptions = jwtOptionsAccessor.Value;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
        };
    });

// Swagger
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Walks API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter JWT token. Example: eyJhbGciOiJIUzI1NiIs..."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document, null),
            new List<string>()
        }
    });
});

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

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthWalksConnectionString")));

// Repositories
builder.Services.AddScoped<IRegionRepository, RegionRepository>();
builder.Services.AddScoped<IWalkRepository, WalkRepository>();
builder.Services.AddScoped<IImageRepository, LocalImageRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WalksDbContext>();
    dbContext.Database.Migrate();

    var authDbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    authDbContext.Database.Migrate();
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

app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
