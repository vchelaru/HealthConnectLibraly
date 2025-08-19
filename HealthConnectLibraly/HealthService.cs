using Android.Health.Connect;
using Android.Health.Connect.DataTypes;
using HealthConnectLibraly.HealthStandartClass;
using Microsoft.Maui.ApplicationModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HealthConnectLibraly.Platforms.Android
{
    public partial class HealthService : INotifyPropertyChanged
    {
        #region Properties

        public DateOnly ShowDay { get; set; } = DateOnly.FromDateTime( DateTime.Now );
        private ObservableCollection<HydrationStandard> hydratationRecords= new ObservableCollection<HydrationStandard>();
        public ObservableCollection<HydrationStandard> HydrationRecords
        {
            get => hydratationRecords;
            set
            {
                hydratationRecords = value;
                OnPropertyChanged( nameof( HydrationRecords ) );
            }
        }
        private double lastWeight = 0;
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
        public void GetPermissions()
        {
            GetPermisionParticular();
        }
        private partial HealthService GetPermisionParticular();
        public async Task<bool> CheckAndRequestHealthPermissions()
        {

#if ANDROID
            var permissionStatus = await Permissions.CheckStatusAsync<HealthPermissions>();

            if(permissionStatus != PermissionStatus.Granted)
            {
                permissionStatus = await Permissions.RequestAsync<HealthPermissions>();

            }

            return permissionStatus == PermissionStatus.Granted;
#else
            return false;
#endif
        }
        #endregion

        #region Insert

        public async Task InsertHydration( HydrationStandard hydrationStandard )
        {
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            DateTimeOffset dto = new DateTimeOffset( hydrationStandard.DrinkTime, localZone.GetUtcOffset( hydrationStandard.DrinkTime ) );
            PrepareForRecord( out Metadata metadata );
            NewRecord( typeof( HydrationRecord ), hydrationStandard.Hydration, metadata, hydrationStandard.DrinkTime.ToUniversalTime(), hydrationStandard.DrinkTime.ToUniversalTime() );
            await Task.Run( () => InsertDataIntoHealth() );
        }

        public async Task InsertWeight( double weight )
        {
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            DateTime dateTime = DateTime.Now;
            DateTimeOffset dto = new DateTimeOffset( dateTime, localZone.GetUtcOffset( dateTime ) );
            PrepareForRecord( out Metadata metadata );
            NewRecord( typeof( WeightRecord ), weight, metadata, dto.UtcDateTime, dto.UtcDateTime );
            await Task.Run( () => InsertDataIntoHealth() );
            GetWeight();
        }

        public partial void InsertDataIntoHealth();

        #endregion

        #region Get
        public void GetHydration()
        {
            UpdateRecordsFromHealth( typeof( HydrationRecord ) );
        }
        public void GetWeight()
        {
            UpdateRecordsFromHealth( typeof( WeightRecord ) );
        }
        partial void UpdateRecordsFromHealth( Type type );
        #endregion
    }
}
