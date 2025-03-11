using System.Text.Json.Serialization;

namespace HealthConnectLibraly.HealthStandartClass
{
    public class HydrationStandart
    {
        public DateTime DrinkTime { get; private set; }
        public float Hydration { get; private set; }
        public HydrationStandart( DateTime DringTimeInThisZone, float HydratationInMilitrs, bool thisZone )
        {
            DrinkTime = DringTimeInThisZone;
            Hydration = HydratationInMilitrs;
        }
        [JsonConstructor]
        public HydrationStandart( DateTime DringTime, float Hydratation )
        {
            this.DrinkTime = DringTime;
            this.Hydration = Hydratation;
        }
    }
}
