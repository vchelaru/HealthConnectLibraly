using Android.Content;
using Android.Health.Connect;
using Android.Health.Connect.DataTypes;
using Android.Health.Connect.DataTypes.Units;
using HealthConnectLibrary.HealthStandartClass;
using Java.Util.Concurrent;
using UnitConverterLibrary;
using UnitConverterLibrary.UnitsTypes;
using HydrationRecord = Android.Health.Connect.DataTypes.HydrationRecord;
using Metadata = Android.Health.Connect.DataTypes.Metadata;


namespace HealthConnectLibrary.Platforms.Android
{


    public partial class HealthService
    {
        private HealthConnectManager HealthConnectManager { get; set; }

        public void InitializeHealthConnect()
        {
            try
            {
                var context = Platform.CurrentActivity?.ApplicationContext;
                if( context == null )
                {
                    Console.WriteLine( "Chyba: context je null." );
                    return;
                }

                // Použití konstanty Context.HEALTH_CONNECT_SERVICE
                HealthConnectManager = context.GetSystemService( Context.HealthconnectService ) as HealthConnectManager;
                if( HealthConnectManager == null )
                {
                    Console.WriteLine( "Nepodařilo se získat HealthConnectManager" );
                    return;
                }

                Console.WriteLine( "HealthConnectManager byl úspěšně inicializován." );
            }
            catch( Exception ex )
            {
                Console.WriteLine( $"Chyba při inicializaci: {ex.Message}" );
                Console.WriteLine( $"Stack trace: {ex.StackTrace}" );
                if( ex.InnerException != null )
                {
                    Console.WriteLine( $"Inner exception: {ex.InnerException.Message}" );
                }
            }
        }
        #region Permision
        async partial void GetPermisionParticular()
        {

            try
            {
                //get activity
                var activity = Platform.CurrentActivity;

                try
                {
                    var intent = new Intent("androidx.health.ACTION_HEALTH_CONNECT_SETTINGS");
                    //get permissions for health connect
                    activity.StartActivity( intent );

                }
                catch( ActivityNotFoundException ex )
                {
                    // Health Connect ist installed
                    // open Google Play Store
                    var playStoreIntent = new Intent(Intent.ActionView);
                    playStoreIntent.SetData( Android.Net.Uri.Parse( "market://details?id=com.google.android.apps.healthdata" ) );
                    activity.StartActivity( playStoreIntent );
                }
            }
            catch( Exception )
            {
            }
        }

        #endregion
        #region CreateRecord
        //list of all recors for saving
        private IList<Record> Records { get; set; } = new List<Record>();
        //function for create record by type
        void NewRecord( Type type, double value, Metadata metadata, DateTime dateTimeStart, DateTime dateTimeEnd )
        {
            Record record;
            Java.Time.Instant instantStart = Java.Time.Instant.Parse(dateTimeStart.ToString($"yyyy-MM-ddTHH:mm:ssZ"));
            Java.Time.Instant instantEnd = Java.Time.Instant.Parse(dateTimeEnd.ToString($"yyyy-MM-ddTHH:mm:ssZ"));
            if( type == typeof( HydrationRecord ) )
            {
                double liters = UnitConverter.Convert(value,VolumeUnit.MlToL );
                record = new HydrationRecord.Builder(
                            metadata,
                          instantStart, //start activity
                          instantEnd,  //end activity
                            Volume.FromLiters( liters )
                ).Build();


            }
            else
            {
                record = new WeightRecord.Builder(
            metadata,
          instantStart,
            Mass.FromGrams( UnitConverter.Convert( value, WeightUnit.KgToG ) )
        ).Build();
            }
            Records.Add( record );
        }
        //priper metadata
        public void PripereForRecord( out Metadata metadata )
        {

            // create metadata with inicialize
            metadata = new Metadata.Builder()
            .Build();

            if( metadata == null )
            {
                throw new InvalidOperationException( "metadata arent created" );
            }
        }
        #endregion


        public async partial void InsertDataIntoHealth()
        {
            try
            {
                bool hasPermission = await CheckAndRequestHealthPermissions();
                if( !hasPermission )
                {
                    Console.WriteLine( "permision not getted." );
                    return;
                }
                try
                {
                    InitializeHealthConnect();
                }
                catch( Exception )
                {

                    throw;
                }
                // check HealthConnectManager
                if( HealthConnectManager == null )
                {
                    throw new InvalidOperationException( "HealthConnectManager není inicializován" );
                }


                // make executor and check
                var executor = Executors.NewSingleThreadExecutor();
                if( executor == null )
                {
                    throw new InvalidOperationException( "Nepodařilo se vytvořit executor" );
                }
                // make receiver and check
                var outcomeReceiver = new HydrationOutcomeReceiver(result =>
                {
                    //callback after inser data

                    if (result != null)
                    {
                        // if result is null Code insert data into observable collection and call INotifiPropertyChange
                        if (result is InsertRecordsResponse collection)
                        {
                            var filteredRecords = collection.Records.OfType<HydrationRecord>();

                            foreach (var record in filteredRecords)
                            {
                                HydrationRecords.Add(new HydrationStandart(DateTime.Parse(record.StartTime.ToString()),(float)UnitConverter.Convert(record.Volume.InLiters,VolumeUnit.LToMl)));
                            }

                            Records.Clear();
                        }

                    }
                    else
                    {
                        Console.WriteLine("Result was null");
                    }
                });
                //check instance
                if( outcomeReceiver == null )
                {
                    throw new InvalidOperationException( "Nepodařilo se vytvořit receiver" );
                }

                // call InsertRecords and save data into health connect
                HealthConnectManager.InsertRecords( Records, executor, outcomeReceiver );
            }
            catch( Exception ex )
            {
                Console.WriteLine( $"Error inserting hydration record: {ex.Message}" );
                throw;
            }
        }

        async partial void GetFromHealth( Type type )
        {
            try
            {
                bool hasPermission = await CheckAndRequestHealthPermissions();
                if( !hasPermission )
                {
                    Console.WriteLine( "Oprávnění nebylo uděleno." );
                    return;
                }
                try
                {
                    InitializeHealthConnect();
                }
                catch( Exception )
                {
                    throw;
                }

                if( HealthConnectManager == null )
                {
                    throw new InvalidOperationException( "HealthConnectManager není inicializován" );
                }

                // create times for this day
                var now = DateTime.Now;
                var startTimeStr = now.Date.ToString("yyyy-MM-ddTHH:mm:ssZ");
                var endTimeStr = now.Date.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssZ");

                var startTime = Java.Time.Instant.Parse(startTimeStr);
                var endTime = Java.Time.Instant.Parse(endTimeStr);

                // Vytvoříme metadata pro request
                var metadata = new Metadata.Builder().Build();

                // Vytvoříme executor
                var executor = Executors.NewSingleThreadExecutor();
                if( executor == null )
                {
                    throw new InvalidOperationException( "Nepodařilo se vytvořit executor" );
                }

                // Vytvoříme receiver jako Java objekt implementující IOutcomeReceiver
                HydrationOutcomeReceiver  outcomeReceiver = new HydrationOutcomeReceiver( result =>
                {

                    if( result != null )
                    {
                        if (result is ReadRecordsResponse collection)
                        {
                            HydrationRecords.Clear();
                            //take only hydratacionreqest into variable and give them into my array
                            var hydratationRecords = collection.Records.OfType<HydrationRecord>();

                            foreach (var record in hydratationRecords)
                            {
                                HydrationRecords.Add(new HydrationStandart(DateTime.Parse(record.StartTime.ToString()),(float)UnitConverter.Convert(record.Volume.InLiters,VolumeUnit.LToMl)));
                            }
                            //take last weight
                            var weightRecord = collection.Records.OfType<WeightRecord>();
                            if (weightRecord.Count()>0)
                            {
                                LastWeight = ( (WeightRecord) ( weightRecord.First() ) ).Weight.InGrams;
                            }

                        }
                    }
                    else
                    {
                        Console.WriteLine( "Result was null" );
                    }
                } );


                var recordClass = Java.Lang.Class.FromType(type);
                var builder = new ReadRecordsRequestUsingFilters.Builder(recordClass);
                var readRequest = builder.Build() as ReadRecordsRequest;

                if( readRequest == null )
                {
                    throw new InvalidOperationException( "Nepodařilo se vytvořit ReadRecordsRequest" );
                }

                // Pokračování s vytvořeným requestem
                HealthConnectManager.ReadRecords( readRequest, executor, outcomeReceiver );


            }
            catch( Exception ex )
            {
                Console.WriteLine( $"Error reading hydration records: {ex.Message}" );
                throw;
            }
        }
    }

}
