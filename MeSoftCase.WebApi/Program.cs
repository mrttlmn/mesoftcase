using System.Text;
using MeSoftCase.WebApi.Application;
using MeSoftCase.WebApi.Domain.Entities;
using MeSoftCase.WebApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddControllers();

builder.Services
    .AddMediatR(config =>
    {
        config.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManagerPolicy", policy =>
        policy.RequireRole("Admin", "Manager"));
} );
builder.Services.AddSwaggerGen(c =>
{
    // Swagger UI için JWT token'ını eklemek
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(9090);  
});

var app = builder.Build();
app.UseSwagger(); 
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); 
    c.RoutePrefix = string.Empty;  
});
app.UseAuthentication(); 
app.UseAuthorization();

ApplyDatabaseMigrations(app);
SeedAdminUser(app);

app.UseStaticFiles();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();


app.Run();
void ApplyDatabaseMigrations(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate(); 
    }
}
void SeedAdminUser(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        var roleExist = roleManager.RoleExistsAsync("Admin").Result;
        if (!roleExist)
        {
            var role = new IdentityRole("Admin");
            var roleResult = roleManager.CreateAsync(role).Result;
            if (!roleResult.Succeeded)
            {
                Console.WriteLine("Error creating Admin role");
            }
        }

        var user = userManager.FindByNameAsync("admin@example.com").Result;
        if (user == null)
        {
            user = new AppUser()
            {
                UserName = "admin",
                Email = "admin@example.com",
                ReferalCode = "123456"
            };

            var result = userManager.CreateAsync(user, "Turkuaz007@").Result;
            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(user, "Admin").Wait();
                Console.WriteLine("Admin user created and assigned to Admin role.");
            }
            else
            {
                Console.WriteLine("Error creating default admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}