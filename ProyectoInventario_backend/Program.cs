using System.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using ProyectoInventario.Application.Interfaces;
using ProyectoInventario.Application.Services;
using ProyectoInventario.Domain.Interfaces;
using ProyectoInventario.Infrastructure.Repositories;
using ProyectoInventario.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Repositorios y servicios de autenticación
builder.Services.AddScoped<IUsuariosRepo, UsuarioRepository>();
builder.Services.AddScoped<IProductosRepo, ProductoRepository>();
builder.Services.AddScoped<ILotesRepo, LoteRepository>();
builder.Services.AddScoped<IProductoLotesRepo, ProductoLoteRepository>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ILoteService, LoteService>();
builder.Services.AddScoped<IProductoLoteService, ProductoLoteService>();

var conexion = builder.Configuration.GetConnectionString("BaseConexion");
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(conexion));

builder.Services.AddOpenApi();
//Swagger con bearer token
builder.Services.AddSwaggerGen(c=>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT"
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

//CORS Angular 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularConexion", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Autenticación JWT
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AngularConexion");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
