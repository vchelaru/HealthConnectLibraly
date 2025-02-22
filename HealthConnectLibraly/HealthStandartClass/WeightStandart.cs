using System.Text.Json.Serialization;

namespace HealthConnectLibraly.HealthStandartClass
{
    public class WeightStandart
    {
        public DateTime Time { get; private set; }
        public float MassGram { get; private set; }
        public WeightStandart( DateTime DringTimeInThisZone, float HydratationInMilitrs, bool thisZone )
        {
            Time = DringTimeInThisZone;
            MassGram = HydratationInMilitrs;
        }
        [JsonConstructor]
        public WeightStandart( DateTime Time, float MassGram )
        {
            this.Time = Time;
            this.MassGram = MassGram;
        }
    }
}
}
