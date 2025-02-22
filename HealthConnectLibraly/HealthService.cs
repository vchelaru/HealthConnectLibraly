using HealthConnectLibraly.HealthStandartClass;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HealthConnectLibraly
{

    public partial class HealthService : INotifyPropertyChanged
    {
        #region Properties
        public DateOnly ShowDay { get; set; } = DateOnly.FromDateTime( DateTime.Now );
        private readonly Dictionary<Type, object> dataCollections = new();

        public ObservableCollection<T> GetCollection<T>() where T : class
        {
            if( !dataCollections.ContainsKey( typeof( T ) ) )
            {
                dataCollections[typeof( T )] = new ObservableCollection<T>();
            }
            return (ObservableCollection<T>) dataCollections[typeof( T )];
        }

        public static ObservableCollection<HydrationStandart> HealthCollection { get; private set; }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }
        #endregion

        #region Permission
        public void GetPermision()
        {
            GetPermisionParticular();
        }
        partial void GetPermisionParticular();
        public async Task<bool> CheckAndRequestHealthPermissions()
        {
#if ANDROID
            string[] permissions =
            {
                "android.permission.health.READ_HYDRATION",
                "android.permission.health.WRITE_HYDRATION",
                "android.permission.health.READ_WEIGHT",
                "android.permission.health.WRITE_WEIGHT"
            };

            var activity = Platform.CurrentActivity;
            //PermissionHelper.RequestPermission(activity, permissions, 100);

            for (int i = 0; i < 50; i++)
            {
               /* if (permissions.All(PermissionHelper.HasPermission))
                {
                    break;
                }*/
                await Task.Delay(200);
            }
#endif
            return true;
        }
        #endregion

        #region Insert
        public async Task InsertHydration( HydrationStandart hydrationStandart )
        {
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            DateTimeOffset dto = new DateTimeOffset( hydrationStandart.DringTime, localZone.GetUtcOffset( hydrationStandart.DringTime ) );
            PripereForRecord( out Metadata metadata );
            NewRecord( typeof( HydrationRecord ), hydrationStandart.Hydratation, metadata, hydrationStandart.DringTime.ToUniversalTime(), hydrationStandart.DringTime.ToUniversalTime() );
            await Task.Run( () => InsertDataIntoHealth() );
        }
        public async Task InsertWeight( double weight )
        {
            DateTimeOffset dto = HealthFunctionService.GetDateTimeOffsetFromDatetime(DateTime.Now);
            PripereForRecord( out Metadata metadata );
            NewRecord( typeof( WeightRecord ), weight, metadata, dto.UtcDateTime, dto.UtcDateTime );
            await Task.Run( () => InsertDataIntoHealth() );
            GetWeight();
        }
        public partial void InsertDataIntoHealth();
        #endregion

        #region Get
        public void GetHydration()
        {
            GetFromHealth( typeof( HydrationRecord ) );
        }
        public void GetWeight()
        {
            GetFromHealth( typeof( WeightRecord ) );
        }
        partial void GetFromHealth( Type type );
        #endregion
    }
}
