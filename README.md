This code is an example of how to use Health Connect in MAUI.

I am giving permission to use this code in your project.

Example of using hydration:
```
public async Task InsertHydration(HydrationStandard hydrationStandard)
{
    TimeZoneInfo localZone = TimeZoneInfo.Local;
    DateTimeOffset dto = new DateTimeOffset(hydrationStandard.DrinkTime, localZone.GetUtcOffset(hydrationStandard.DrinkTime));

    PrepareForRecord(out Metadata metadata);

    NewRecord(typeof(HydrationRecord), hydrationStandard.Hydration, metadata, 
        hydrationStandard.DrinkTime.ToUniversalTime(), 
        hydrationStandard.DrinkTime.ToUniversalTime());

    await InsertDataIntoHealth();
}
```
For create new record, you can use method chaining

```
 NewRecord(typeof(HydrationRecord), hydrationStandard.Hydration, metadata, 
    hydrationStandard.DrinkTime.ToUniversalTime(), 
    hydrationStandard.DrinkTime.ToUniversalTime());

NewRecord(typeof(HydrationRecord), hydrationStandard.Hydration, metadata, 
    hydrationStandard.DrinkTime.ToUniversalTime(), 
    hydrationStandard.DrinkTime.ToUniversalTime());

await InsertDataIntoHealth();
```

you need create activity and permision in android manifest
```
	<application android:allowBackup="true" android:icon="@mipmap/appicon" android:supportsRtl="true">
		<activity android:name=".ViewPermissionUsageActivity" android:exported="true" android:permission="android.permission.START_VIEW_PERMISSION_USAGE">
			<intent-filter>
				<action android:name="android.intent.action.VIEW_PERMISSION_USAGE" />
				<category android:name="android.intent.category.HEALTH_PERMISSIONS" />
			</intent-filter>
		</activity>
	</application>
	<uses-permission android:name="android.permission.health.READ_HYDRATION" />
	<uses-permission android:name="android.permission.health.READ_WEIGHT" />
	<uses-permission android:name="android.permission.health.WRITE_HYDRATION" />
	<uses-permission android:name="android.permission.health.WRITE_WEIGHT" />
	
```
