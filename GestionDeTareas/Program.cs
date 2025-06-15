using System.Text;
using GestionDeTareas.huds;
using GestionDeTareas.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TuProyecto.Services;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<TaskContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSignalR();
// En Program.cs, después de AddSignalR()
builder.Services.AddScoped<ITaskNotificationService, TaskNotificationService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<TaskQueue>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHub<TaskHub>("/taskHub");

app.UseAuthorization();

app.UseCors();

app.UseAuthentication();

app.MapControllers();

app.Run();
