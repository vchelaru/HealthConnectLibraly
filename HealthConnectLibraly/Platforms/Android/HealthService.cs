using Android.Content;
using Android.Health.Connect;
using Android.Health.Connect.DataTypes;
using Android.Health.Connect.DataTypes.Units;
using HealthConnectLibraly.HealthStandartClass;
using Java.Util.Concurrent;
using UnitConverterLibrary;
using UnitConverterLibrary.UnitsTypes;
using HydrationRecord = Android.Health.Connect.DataTypes.HydrationRecord;
using Metadata = Android.Health.Connect.DataTypes.Metadata;


namespace HealthConnectLibraly.Platforms.Android;



public partial class HealthService
{
    private HealthConnectManager HealthConnectManager { get; set; }

    public void InitializeHealthConnect()
    {
        try
        {
            var context = Platform.CurrentActivity?.ApplicationContext;
            if (context == null)
            {
                Console.WriteLine("Chyba: context je null.");
                return;
            }

            // Použití konstanty Context.HEALTH_CONNECT_SERVICE
            HealthConnectManager = context.GetSystemService(Context.HealthconnectService) as HealthConnectManager;
            if (HealthConnectManager == null)
            {
                Console.WriteLine("Nepodařilo se získat HealthConnectManager");
                return;
            }

            Console.WriteLine("HealthConnectManager byl úspěšně inicializován.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Chyba při inicializaci: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
        }
    }

    #region Permision
    private partial HealthService GetPermisionParticular()
    {

        try
        {
            //get activity
            var activity = Platform.CurrentActivity;

            try
            {
                var intent = new Intent("androidx.health.ACTION_HEALTH_CONNECT_SETTINGS");
                //get permissions for health connect
                activity.StartActivity(intent);
                return this;

            }
            catch (ActivityNotFoundException ex)
            {
                // Health Connect isn't installed
                // open Google Play Store
                var playStoreIntent = new Intent(Intent.ActionView);
                playStoreIntent.SetData(global::Android.Net.Uri.Parse("market://details?id=com.google.android.apps.healthdata"));
                activity.StartActivity(playStoreIntent);
                throw;
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    #endregion

    #region CreateRecord
    //list of all recors for saving
    private IList<Record> Records { get; set; } = new List<Record>();
    //function for create record by type
    HealthService NewRecord(Type type, double value, Metadata metadata, DateTime dateTimeStart, DateTime dateTimeEnd)
    {
        Record record;
        Java.Time.Instant instantStart = Java.Time.Instant.Parse(dateTimeStart.ToString($"yyyy-MM-ddTHH:mm:ssZ"));
        Java.Time.Instant instantEnd = Java.Time.Instant.Parse(dateTimeEnd.ToString($"yyyy-MM-ddTHH:mm:ssZ"));
        if (type == typeof(HydrationRecord))
        {
            double liters = UnitConverter.Convert(value, VolumeUnit.MlToL);
            record = new HydrationRecord.Builder(
                        metadata,
                      instantStart, //start activity
                      instantEnd,  //end activity
                        Volume.FromLiters(liters)
            ).Build();


        }
        else
        {
            record = new WeightRecord.Builder(
        metadata,
      instantStart,
        Mass.FromGrams(UnitConverter.Convert(value, WeightUnit.KgToG))
    ).Build();
        }
        Records.Add(record);
        return this;
    }
    //Prepare metadata
    public void PrepareForRecord(out Metadata metadata)
    {

        // create metadata with inicialize
        metadata = new Metadata.Builder()
        .Build();

        if (metadata == null)
        {
            throw new InvalidOperationException("metadata arent created");
        }
    }
    #endregion

    public async partial void InsertDataIntoHealth()
    {
        try
        {
            bool hasPermission = await CheckAndRequestHealthPermissions();
            if (!hasPermission)
            {
                Console.WriteLine("permision not getted.");
                return;
            }
            try
            {
                InitializeHealthConnect();
            }
            catch (Exception)
            {

                throw;
            }
            // check HealthConnectManager
            if (HealthConnectManager == null)
            {
                throw new InvalidOperationException("HealthConnectManager není inicializován");
            }


            // make executor and check
            var executor = Executors.NewSingleThreadExecutor();
            if (executor == null)
            {
                throw new InvalidOperationException("Nepodařilo se vytvořit executor");
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
                            var insertedHydration =
                                new HydrationStandard(DateTime.Parse(record.StartTime.ToString()), (float)UnitConverter.Convert(record.Volume.InLiters, VolumeUnit.LToMl));
                            HydrationRecords.Add(insertedHydration);
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
            if (outcomeReceiver == null)
            {
                throw new InvalidOperationException("Nepodařilo se vytvořit receiver");
            }

            // call InsertRecords and save data into health connect
            HealthConnectManager.InsertRecords(Records, executor, outcomeReceiver);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inserting hydration record: {ex.Message}");
            throw;
        }
    }

    public async Task<T[]?> GetRecordsAsync<T>(DateTime startTime, DateTime endTime)
    {
        try
        {
            bool hasPermission = await CheckAndRequestHealthPermissions();
            if (!hasPermission)
            {
                Console.WriteLine("Oprávnění nebylo uděleno.");
                return null;
            }
            try
            {
                InitializeHealthConnect();
            }
            catch (Exception)
            {
                throw;
            }

            if (HealthConnectManager == null)
            {
                throw new InvalidOperationException("HealthConnectManager není inicializován");
            }

            // create times for this day
            var now = DateTime.Now;
            var startTimeStr = startTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var endTimeStr = endTime.ToString("yyyy-MM-ddTHH:mm:ssZ");

            var startTimeJava = Java.Time.Instant.Parse(startTimeStr);
            var endTimeJava = Java.Time.Instant.Parse(endTimeStr);

            // Vytvoříme metadata pro request
            var metadata = new Metadata.Builder().Build();

            // Vytvoříme executor
            var executor = Executors.NewSingleThreadExecutor();
            if (executor == null)
            {
                throw new InvalidOperationException("Nepodařilo se vytvořit executor");
            }

            TaskCompletionSource<T[]?> tcs = new ();


            // Vytvoříme receiver jako Java objekt implementující IOutcomeReceiver
            HydrationOutcomeReceiver outcomeReceiver = new HydrationOutcomeReceiver(result =>
            {

                if (result != null)
                {
                    if (result is ReadRecordsResponse collection)
                    {
                        HydrationRecords.Clear();
                        //take only hydratacionreqest into variable and give them into my array
                        tcs.TrySetResult(collection?.Records.OfType<T>().ToArray());
                    }
                    else
                    {
                        tcs.TrySetResult(null);
                    }
                }
                else
                {
                    Console.WriteLine("Result was null");
                    tcs.TrySetResult(null);
                }
            });


            var recordClass = Java.Lang.Class.FromType(typeof(T));
            var builder = new ReadRecordsRequestUsingFilters.Builder(recordClass);
            var readRequest = builder.Build() as ReadRecordsRequest;

            if (readRequest == null)
            {
                throw new InvalidOperationException("Nepodařilo se vytvořit ReadRecordsRequest");
            }

            // Pokračování s vytvořeným requestem
            HealthConnectManager.ReadRecords(readRequest, executor, outcomeReceiver);

            var result = await tcs.Task;

            return result ?? null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading hydration records: {ex.Message}");
            throw;
        }
    }

    async partial void UpdateRecordsFromHealth(Type type)
    {
        if(type == typeof(HydrationRecord))
        {
            var hydrationRecords = await GetRecordsAsync<HydrationRecord>(
                startTime: DateTime.Now.AddDays(-1),
                endTime: DateTime.Now);

            HydrationRecords.Clear();

            foreach (var hydrationRecord in hydrationRecords)
            {
                HydrationRecords.Add(new HydrationStandard(DateTime.Parse(hydrationRecord.StartTime.ToString()), (float)UnitConverter.Convert(hydrationRecord.Volume.InLiters, VolumeUnit.LToMl)));
            }
        }
        else if(type == typeof(WeightRecord))
        {
            var weightRecord = await GetRecordsAsync<WeightRecord>(
                startTime: DateTime.Now.AddDays(-1),
                endTime: DateTime.Now);

            //take last weight

            if (weightRecord.Count() > 0)
            {
                LastWeight = (weightRecord.First()).Weight.InGrams;
            }
        }
    }
}
