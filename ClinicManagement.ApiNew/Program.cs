using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models; // Required for OpenApiSecurityScheme and OpenApiReference
using ClinicManagement.Data.Context;
using ClinicManagement.ApiNew.Services; // Ensure this is present if AuthService is in this namespace
using Microsoft.AspNetCore.Identity; // For IdentityUser, IdentityRole if used

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Configure DbContext
builder.Services.AddDbContext<ClinicManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ClinicManagementDbConnection")));

// 2. Add Controllers and enable JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure JSON serialization for DateOnly
        options.JsonSerializerOptions.Converters.Add(new ClinicManagement.ApiNew.DTOs.Patients.DateOnlyJsonConverter());
    });

// 3. Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// --- START: Swagger/OpenAPI Configuration for JWT Authorization ---
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Clinic Management API", Version = "v1" });

    // Define the security scheme for JWT Bearer authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Add a security requirement for all operations (optional, can be applied per endpoint too)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    // Optional: Include XML comments for Swagger UI (for endpoint descriptions and DTOs)
    // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // c.IncludeXmlComments(xmlPath);
});
// --- END: Swagger/OpenAPI Configuration for JWT Authorization ---


// 4. Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            // Updated JWT Key
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured.")))
        };
    });

// 5. Configure Authorization Policies (if you have specific role-based policies)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ClinicStaff", policy => policy.RequireRole("Admin", "HR", "Receptionist", "Doctor", "Nurse", "InventoryManager"));
    options.AddPolicy("PatientAccess", policy => policy.RequireRole("Patient"));
    // Add more policies as needed
});

// 6. Register custom services
builder.Services.AddScoped<AuthService>(); // Register AuthService for dependency injection

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // --- START: Swagger UI Configuration for JWT Authorization ---
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinic Management API v1");
        // Removed c.EnableRethinkRequest(); as it's not a recognized method.
        // The security definition above already enables the 'Authorize' button.
    });
    // --- END: Swagger UI Configuration for JWT Authorization ---
}

app.UseHttpsRedirection();

// 7. Use Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
