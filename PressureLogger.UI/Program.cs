using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using PressureLogger.UI;
using PressureLogger.UI.Options;
using PressureLogger.UI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection("api"));


builder.Services.AddHttpClient("api",(sp,client) =>
{
	var opt = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
	client.BaseAddress = new Uri(opt.BaseAddress);
});

builder.Services.AddSingleton(sp=>
	sp.GetRequiredService<IHttpClientFactory>().CreateClient("api"));
builder.Services.AddMudServices();

builder.Services.AddScoped<LocalStorageService>();

await builder.Build().RunAsync();

