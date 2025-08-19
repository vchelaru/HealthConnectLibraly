using System.Text.Json.Serialization;

namespace HealthConnectLibraly.HealthStandartClass;

/// <summary>
/// Represents a hydration record which can be created used cross-platform. On Android this
/// maps to Android.Health.Connect.DataTypes.HydrationRecord
/// </summary>
public class HydrationStandard
{
    public DateTime DrinkTime { get; private set; }
    public float Hydration { get; private set; }
    public HydrationStandard( DateTime DringTimeInThisZone, float HydratationInMilitrs, bool thisZone )
    {
        DrinkTime = DringTimeInThisZone;
        Hydration = HydratationInMilitrs;
    }
    [JsonConstructor]
    public HydrationStandard( DateTime drinkTime, float hydrationAmountInMilliLiters )
    {
        this.DrinkTime = drinkTime;
        this.Hydration = hydrationAmountInMilliLiters;
    }
}
