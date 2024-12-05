using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using UsuariosApi.Autorization;
using UsuariosApi.Data;
using UsuariosApi.Models;
using UsuariosApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IAuthorizationHandler, IdadeAutorization>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options => {
     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["SymmetricSecurityKey"])
            ),
            ValidateAudience = false,
            ValidateIssuer = false,
            ClockSkew = TimeSpan.Zero,
    };
});
builder.Services.AddAuthorization(options => {
    options.AddPolicy("IdadeMinima", policy => 
    policy.AddRequirements(new IdadeMinima(18))
    );
});


//entity framework
string postgresConnection = builder.Configuration.GetConnectionString("DefaultConnection");
NpgsqlConnection npgsqlConnection;

builder.Services.AddDbContextPool<UsuarioDbContext>(options => options.UseNpgsql(postgresConnection));

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); //para o postgresql entender a data

//identity
builder.Services.AddIdentity<Usuario, IdentityRole>()
.AddEntityFrameworkStores<UsuarioDbContext>()
.AddDefaultTokenProviders();

builder.Services.
    AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//injeçao de dependencia de services criados pelo programador
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<TokenService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
