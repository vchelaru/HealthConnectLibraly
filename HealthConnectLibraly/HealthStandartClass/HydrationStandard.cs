using System.Text.Json.Serialization;

namespace HealthConnectLibraly.HealthStandartClass;

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
