using AutoMapper;
using koszalka_api.Profiles;
using koszalka_api.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Swashbuckle.AspNetCore;
using Microsoft.CodeAnalysis.Options;
using koszalka_api.Controllers;
using System.Collections.Generic;
using koszalka_api.Caching;
using koszalka_api.RabbitMQ;
using koszalka_api.Persistence.Model;
using koszalka_api.Persistence.Data;
using koszalka_api.Events.RabbitMQ;
using koszalka_api.Events.Kafka;
using koszalka_api.Persistence.Repository;
using ServiceCollectionAccessorService;
using koszalka_api.Events.RabbitMQ.Interfaces;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddServiceCollectionAccessor();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    option.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});


builder.Services.AddTransient<IBikeRepository, BikeService>();
builder.Services.AddTransient<IShoesRepository, ShoesService>();
builder.Services.AddTransient<IAuthRepository, AuthService>();
builder.Services.AddTransient<IRabitMQProducer, RabbitMQProducer>();
builder.Services.AddTransient<IRabbitMQConsumer, RabbitMQConsumer>();
builder.Services.AddTransient<IRabbitMQConnectionFactory, RabbitMQConnectionFactory>();
builder.Services.AddTransient<ICacheService, CacheService>();
builder.Services.AddTransient<EntityFrameworkConfigurationContext>();
builder.Services.AddTransient<DapperContext>();
builder.Services.AddTransient<KafkaProducer>();
builder.Services.AddTransient<KafkaConsumer>();
builder.Services.AddTransient<BikeController>();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// For Identity  
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(typeof(ShoesProfile));
builder.Services.AddAutoMapper(typeof(BikeProfile));

// Adding Authentication  
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWTKey:ValidAudience"],
        ValidIssuer = builder.Configuration["JWTKey:ValidIssuer"],
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(s: builder.Configuration["JWTKey:Secret"]))
    };
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

static IRabbitMQConsumer GetRabbitMqConsumer() => ServiceCollectionAccessor.Services.BuildServiceProvider().GetRequiredService<IRabbitMQConsumer>();
GetRabbitMqConsumer().CreateRabbitMQConsumer(app);



