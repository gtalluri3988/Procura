using Api.Helpers;
using Api.Models;
using Api.Workers;
using BusinessLogic;
using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using BusinessLogic.Services;
using DB;
using DB.EFModel;
using DB.Model;
using DB.Repositories;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Text;
using System.Text.Json.Serialization;
using YourNamespace.Services;

var builder = WebApplication.CreateBuilder(args);




builder.Services.Configure<AmpersandOptions>(builder.Configuration.GetSection("Ampersand"));

// Add services
builder.Services.AddHttpClient<IAmpersandClient, AmpersandClient>();
builder.Services.AddSingleton<IOrderStore, InMemoryOrderStore>();



builder.Services.AddHttpClient();
var connectionString = Environment.GetEnvironmentVariable("ProcuraConnection");
builder.Services.AddDbContext<ProcuraDbContext>(options =>
    options.UseSqlServer(connectionString),
    ServiceLifetime.Scoped);
// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
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
            ValidIssuer = jwtSettings["Issuer"], // Ensure this matches `iss` in token
            ValidAudience = jwtSettings["Audience"], // Ensure this matches `aud` in token
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
        };
    });

builder.Services.AddBusinessLogic().AddDBProject(builder.Configuration);
// Register Services
//builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<IPasswordPolicyService, PasswordPolicyService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IRoleMenuPermissionService, RoleMenuPermissionService>();
builder.Services.AddScoped<IContentService, ContentService>();

builder.Services.AddScoped<IPaymentService, PaymentService>();


builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IVendorService, VendorService>();

builder.Services.AddScoped<ISAPServices, SAPServices>();
builder.Services.AddScoped<IMasterDataService, MasterDataService>();
builder.Services.AddScoped<ITenderService, TenderService>();
builder.Services.AddScoped<IBiddingService, BiddingService>();
builder.Services.AddScoped<IBiddingRepository, BiddingRepository>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
builder.Services.AddScoped<ICategoryCodeApprovalService, CategoryCodeApprovalService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<EmailOutboxWorker>();
builder.Services.AddHostedService<VendorReminderWorker>();


bool useSimulator = true;
builder.Services.AddScoped<IZBAPI_CREATEVENDORClient>(sp =>
    useSimulator
        ? new ZBAPI_CREATEVENDORClient()
        : new RealZBAPI_CREATEVENDORClient()
);


//builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Keys"))
    .SetApplicationName("MyApp");
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Procura API", Version = "v1" });

    // Enable the Authorize button
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {your-token}'"
    });

    // Apply the security globally
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ProcuraDbContext>(options =>
    options.UseSqlServer(connectionString)
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information));
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Log to Console
    .WriteTo.File("Logs/app_log.txt", rollingInterval: RollingInterval.Day) // Log to File
    .CreateLogger();

// Use Serilog instead of default logging
builder.Host.UseSerilog();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()  // Allows all origins
                  .AllowAnyMethod()  // Allows all HTTP methods (GET, POST, etc.)
                  .AllowAnyHeader(); // Allows all headers
        });
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsProduction() || app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
//app.UseHttpsRedirection();
//app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(@"C:\VendorUploads"),
    RequestPath = "/VendorUploads"
});
app.UseCors("AllowAll");
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthorization();

app.MapControllers();

app.Run();
