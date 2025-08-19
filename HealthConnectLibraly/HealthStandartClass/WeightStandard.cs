using System.Text.Json.Serialization;

namespace HealthConnectLibraly.HealthStandartClass
{
    public class WeightStandard
    {
        public DateTime Time { get; private set; }
        public float MassGram { get; private set; }
        public WeightStandard( DateTime DringTimeInThisZone, float HydratationInMilitrs, bool thisZone )
        {
            Time = DringTimeInThisZone;
            MassGram = HydratationInMilitrs;
        }
        [JsonConstructor]
        public WeightStandard( DateTime Time, float MassGram )
        {
            this.Time = Time;
            this.MassGram = MassGram;
        }
    }
}
