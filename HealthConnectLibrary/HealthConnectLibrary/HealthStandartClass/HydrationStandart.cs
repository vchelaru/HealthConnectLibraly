using System.Text.Json.Serialization;

namespace HealthConnectLibrary.HealthStandartClass
{
    public class HydrationStandart
    {
        public DateTime DringTime { get; private set; }
        public float Hydratation { get; private set; }
        public HydrationStandart( DateTime DringTimeInThisZone, float HydratationInMilitrs, bool thisZone )
        {
            DringTime = DringTimeInThisZone;
            Hydratation = HydratationInMilitrs;
        }
        [JsonConstructor]
        public HydrationStandart( DateTime DringTime, float Hydratation )
        {
            this.DringTime = DringTime;
            this.Hydratation = Hydratation;
        }
    }
}
