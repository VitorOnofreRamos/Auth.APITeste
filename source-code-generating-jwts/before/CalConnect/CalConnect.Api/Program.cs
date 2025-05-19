using CalConnect.Api.Database;
using CalConnect.Api.Extensions;
using CalConnect.Api.Users;
using CalConnect.Api.Users.Infrastructure;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.CustomSchemaIds(id => id.FullName!.Replace('+', '-')));

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Database")).UseSnakeCaseNamingConvention());

builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddScoped<EmailVerificationLinkFactory>();

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddFluentEmail(builder.Configuration["Email:SenderEmail"], builder.Configuration["Email:Sender"])
    .AddSmtpSender(builder.Configuration["Email:Host"], builder.Configuration.GetValue<int>("Email:Port"));

builder.Services.AddScoped<RegisterUser>();
builder.Services.AddScoped<LoginUser>();
builder.Services.AddScoped<VerifyEmail>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

UserEndpoints.Map(app);

app.Run();
