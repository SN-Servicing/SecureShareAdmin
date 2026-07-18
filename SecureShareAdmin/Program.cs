using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Snsc.SecureShareAdmin.Configuration;
using Snsc.SecureShareAdmin.Data;
using Snsc.SecureShareAdmin.Security;
using Snsc.SecureShareAdmin.Users;
using Snsc.SecureShareAdmin.Zones;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SnConfigOptions>(builder.Configuration.GetSection(SnConfigOptions.SectionName));
builder.Services.Configure<SecureShareOptions>(builder.Configuration.GetSection(SecureShareOptions.SectionName));

builder.Services.AddSingleton<SnConfigClient>();
builder.Services.AddSingleton<AppDatabase>();
builder.Services.AddScoped<AmsUserLookup>();
builder.Services.AddScoped<AmsUserContext>();
builder.Services.AddScoped<ZoneCatalog>();
builder.Services.AddScoped<ZoneFileCatalog>();
builder.Services.AddScoped<ExternalUserDirectory>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddRazorPages();

WebApplication app = builder.Build();

app.UseExceptionHandler("/Error");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.Run();
