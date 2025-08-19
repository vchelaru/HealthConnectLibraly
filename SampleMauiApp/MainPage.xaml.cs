using Android.Health.Connect.DataTypes;
using HealthConnectLibraly.HealthStandartClass;
using HealthConnectLibraly.Platforms.Android;

namespace SampleMauiApp;

public partial class MainPage : ContentPage
{
    int count = 0;
    HealthService _healthService;

    const int millilitersToAdd = 100;

    public MainPage()
    {
        InitializeComponent();

        AddHydrationButton.Text = $"Add {millilitersToAdd} ml";

        _healthService = new HealthService();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _healthService.InitializeHealthConnect();
    }
    
    private async void HandleAddHydrationClicked(object sender, EventArgs e)
    {
        var hydration = new HydrationStandard(DateTime.Now, millilitersToAdd);

        await _healthService.InsertHydration(hydration);


    }

    private async void HandleReadHydration(object sender, EventArgs e)
    {
#if ANDROID
        var readData = await _healthService.GetRecordsAsync<HydrationRecord>(
            startTime:DateTime.Now.AddDays(-1),
            endTime:DateTime.Now);

        var liters = readData?.Sum(x => x.Volume.InLiters);

        ((Button)sender).Text = $"Total Hydration: {liters} L";
#endif
    }
}
