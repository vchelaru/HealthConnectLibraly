using HealthConnectLibrary.HealthStandartClass;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HealthConnectLibrary.Platforms.Android
{
    public partial class HealthService : INotifyPropertyChanged
    {
        #region Properties
        public DateOnly ShowDay { get; set; } = DateOnly.FromDateTime( DateTime.Now );
        private ObservableCollection<HydrationStandart> hydratationsRecords= new ObservableCollection<HydrationStandart>();
        public ObservableCollection<HydrationStandart> HydrationRecords
        {
            get => hydratationsRecords;
            set
            {
                hydratationsRecords = value;
                OnPropertyChanged( nameof( HydrationRecords ) );
            }
        }
        private double lastWeight =0;
        public double LastWeight
        {
            get => lastWeight;
            set
            {
                lastWeight = value;
                OnPropertyChanged( nameof( LastWeight ) );
            }
        }


        #endregion
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }
        #endregion


        #region Permision
        public void GetPermision()
        {
            GetPermisionParticular();
        }
        partial void GetPermisionParticular();
        public async Task<bool> CheckAndRequestHealthPermissions()
        {
#if ANDROID
            string READ_HYDRATION = "android.permission.health.READ_HYDRATION";
            string READ_WEIGHT = "android.permission.health.READ_WEIGHT";
            string WRITE_HYDRATION = "android.permission.health.WRITE_HYDRATION";
            string WRITE_WEIGHT = "android.permission.health.WRITE_WEIGHT";
            var permissions = new[] { READ_HYDRATION, WRITE_WEIGHT, WRITE_HYDRATION, READ_WEIGHT };


            // Získej aktivitu
            var activity = Platform.CurrentActivity;  // Správný způsob, jak získat aktivitu v MAUI

            // Požádáme o oprávnění
           // PermissionHelper.RequestPermission( activity, permissions, 100 );

            for( int i = 0 ; i < 50 ; i++ )
            {
                foreach( var permission in permissions )
                {
                    /*if( PermissionHelper.HasPermission( permission ) )
                    {
                        i = 50;
                    }*/
                    await Task.Delay( 200 );
                }
            }

#endif
            return true;
        }
        #endregion
        #region Insert
        public async Task InsertHydratacion( HydrationStandart hydrationStandart )
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
        public void GetHydratacion()
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
