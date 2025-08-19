using HealthConnectLibraly.HealthStandartClass;
using HealthConnectLibraly.Platforms.Android;

namespace SampleMauiApp;

public partial class MainPage : ContentPage
{
    int count = 0;
    HealthService _healthService;

    public MainPage()
    {
        InitializeComponent();

        _healthService = new HealthService();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _healthService.InitializeHealthConnect();
    }
    
    private async void HandleAddHydrationClicked(object sender, EventArgs e)
    {
        //var permissions = _healthService.GetPermissions

        var hydration = new HydrationStandard(
            DateTime.Now, 10);

        await _healthService.InsertHydration(hydration);


    }
}
