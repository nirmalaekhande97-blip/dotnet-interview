using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using TodoApi.Configuration;
using TodoApi.Repositories;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure database options from appsettings.json
builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection(DatabaseOptions.SectionName));

// Register repository and service dependencies
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();

// Add controllers with improved configuration
builder.Services.AddControllers();

// Configure Swagger/OpenAPI with XML documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TODO API",
        Version = "v1",
        Description = "A RESTful API for managing TODO items",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "TODO API Team"
        }
    });

    // Enable XML comments for better API documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Initialize database on startup
await InitializeDatabaseAsync(app.Services);

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TODO API v1");
        options.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// Initializes the SQLite database with the required schema
/// </summary>
async Task InitializeDatabaseAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var options = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseOptions>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    var connectionString = options.Value.ConnectionString;

    try
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Todos (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Description TEXT,
                IsCompleted INTEGER NOT NULL DEFAULT 0,
                CreatedAt TEXT NOT NULL
            )
        ";
        await command.ExecuteNonQueryAsync();

        logger.LogInformation("Database initialized successfully at {ConnectionString}", connectionString);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to initialize database");
        throw;
    }
}
