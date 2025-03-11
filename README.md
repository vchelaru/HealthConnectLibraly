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
