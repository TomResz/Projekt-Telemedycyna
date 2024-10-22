using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using PressureLogger.UI.Services;

namespace PressureLogger.UI.Layout;

public partial class MainLayout
{
	private bool _isDarkMode;
	private MudThemeProvider _mudThemeProvider;
	private readonly MudTheme _currentTheme = new()
	{
		PaletteLight = new PaletteLight
		{
			Primary = "#0A7BCF",
			Secondary = "#4CAF50",
			Info = "#64a7e2",
			Success = "#2ECC40",
			Warning = "#FFC107",
			Error = "#FF0000",
			AppbarBackground = "#2A72D5",
		},
		PaletteDark = new PaletteDark
		{
			Primary = "#6585e0",
			Secondary = "#607D8B",
			Info = "#a4c2dd",
			Success = "#2ECC40",
			Warning = "#dc2d7e",
			Error = "#de2000",
			AppbarBackground = "#121212",
		}
	};
	public bool IsDarkMode => _isDarkMode;
	[Inject]
	public LocalStorageService LocalStorage { get; set; }

	[Inject]
	public IJSRuntime JSRuntime { get; set; }

	[Inject]
	public NavigationManager Navigation { get; set; }
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			var darkMode = await LocalStorage.GetValueAsync<bool>("isDarkMode");
			if (darkMode)
			{
				_isDarkMode = true;
			}
			StateHasChanged();
		}
	}

	public async Task ThemeToggle()
	{
		_isDarkMode = !_isDarkMode;
		await LocalStorage.SetValueAsync("isDarkMode", _isDarkMode);
		StateHasChanged();
	}
}
