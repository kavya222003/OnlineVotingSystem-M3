using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnlineVoting_API.Data;
using OnlineVoting_API.Helpers;
using OnlineVoting_API.Repositories;
using OnlineVoting_API.Services;
using OnlineVoting_API.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -------------------- DATABASE --------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// -------------------- DEPENDENCY INJECTION --------------------
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<PollService>();
builder.Services.AddScoped<VoteService>();

// -------------------- CONTROLLERS --------------------
builder.Services.AddControllers();

// -------------------- JWT CONFIG --------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secret = jwtSettings["Secret"] ?? throw new Exception("JWT Secret missing!");
var key = Encoding.UTF8.GetBytes(secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// -------------------- SWAGGER CONFIG --------------------
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Online Voting API",
        Version = "v1"
    });

    // JWT Authentication in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter: Bearer {your token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();
// -------------------- SEED POLL TEMPLATES --------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.PollTemplates.Any())
    {
        db.PollTemplates.AddRange(
            new PollTemplate
            {
                TemplateName = "Programming Languages",
                DefaultQuestion = "Which programming language do you prefer?",
                DefaultOptions = "[\"C#\", \"Java\", \"Python\", \"JavaScript\"]"
            },
            new PollTemplate
            {
                TemplateName = "Lunch Poll",
                DefaultQuestion = "What should we eat today?",
                DefaultOptions = "[\"Pizza\", \"Burger\", \"Biryani\", \"Salad\"]"
            },
            new PollTemplate
            {
                TemplateName = "Event Date",
                DefaultQuestion = "Choose preferred event date",
                DefaultOptions = "[\"Friday\", \"Saturday\", \"Sunday\"]"
            }
        );

        db.SaveChanges();
    }
}

// -------------------- MIDDLEWARE --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// IMPORTANT ORDER
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();