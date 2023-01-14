

using System.Text;
using System.Text.Json.Serialization;
using MedAdvisor.DataAccess.MySql.DataContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

// add db contect 
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseMySql(builder.Configuration.GetConnectionString("dbConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("dbConnection"))));

//
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// For Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();














//using System.Text;
//using MedAdvisor.DataAccess.MySql.Auth;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllers();

//// add db contect 
//builder.Services.AddDbContext<AppDbContext>(
//    options => options.UseMySql(builder.Configuration.GetConnectionString("dbConnection"),
//    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("dbConnection"))));


//// Adding Authentication
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//})

// // Adding Jwt Bearer  it validates the token in authorization bearer
// .AddJwtBearer(options =>
// {
//     options.SaveToken = true;
//     options.RequireHttpsMetadata = false;
//     options.TokenValidationParameters = new TokenValidationParameters()
//     {
//         ValidateIssuer = false, // for dev
//         ValidateAudience = false,  // for dev
//         ValidAudience = builder.Configuration["JWT:ValidAudience"],
//         ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
//         RequireExpirationTime = false, // for dev
//         ValidateLifetime = true

//     };
// });


//// For Identity
//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//    .AddEntityFrameworkStores<AppDbContext>()
//    .AddDefaultTokenProviders();


//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//app.Run();
