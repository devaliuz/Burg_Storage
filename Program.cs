using Burg_Storage.Components;
using Burg_Storage.Components.Account;
using Burg_Storage.Data;
using Burg_Storage.Models;
using Burg_Storage.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/// ------------------------------------------------------------
/// Services
/// ------------------------------------------------------------

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

builder.Services.AddAuthorization(); // // enables [Authorize] attributes

// Database: SQLite provider for user and storage data.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// /// Identity: use default IdentityCore with your ApplicationUser.
// /// EntityFramework stores are backed by ApplicationDbContext.
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    // // optionally tune password/lockout settings here
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// /// Email sender for Identity (no-op is fine for now)
builder.Services.AddSingleton<IEmailSender, IdentityNoOpEmailSender>();

// /// File storage service: store uploads under wwwroot/uploads and write metadata to DB.
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

// Document management service for versioned files.
builder.Services.AddScoped<IDocumentService, DocumentService>();

var app = builder.Build();

/// ------------------------------------------------------------
/// Middleware pipeline
/// ------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();             // // helpful EF error pages + migrations endpoint
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();                         // // serves /wwwroot (incl. /uploads)
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// /// Identity endpoints (/Account/*)
app.MapAdditionalIdentityEndpoints();

app.Run();
