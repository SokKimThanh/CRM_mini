using Crm.Web.Components;
using MudBlazor.Services; // 1. Thêm namespace này
using ApexCharts; // 1. Thêm namespace này

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 2. Thêm 2 dịch vụ MudBlazor & ApexCharts vào đây
builder.Services.AddMudServices();
builder.Services.AddApexCharts();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets(); // Giữ nguyên tính năng tối ưu static asset mới của .NET 9/10

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();