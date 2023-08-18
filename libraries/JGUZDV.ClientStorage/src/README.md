# JGUZDV.ClientStorage

Simple caching and storage solution with optional background refreshs.

## Setup
To use the ClientStorage you need an implementation of IMemoryCache, IKeyValueStorage and ILifeCycleEvents.

## Using the Storage

### Registration
You can register an entry like so
```csharp
store.Register("testKey", new StoreOptions<string>
{
    LoadFunc = GetTestData, // function which returns the value
    UsesBackgroundRefresh = false, // configure if entry should be refreshed automatically
    ValueExpiry = TimeSpan.FromMinutes(10) // configure expiry time
});
```
and unregister
```csharp
store.Unregister("testKey");
```

### Retrieval
You can load an entry
```csharp
var entry = await store.GetEntry<string>("testKey");
```
you can also load the value directly
```csharp
var value = await store.Get<string>("testKey");
```

### Combining registration and retrieval
you can also combine loading and registration like this
```csharp
var entry = await store.GetOrLoadEntry("schedule", new StoreOptions<Schedule>
{
    ValueExpiry = _defaultExpiry,
    LoadFunc = (ct) => _scheduleService.GetSchedule(ct),
    UsesBackgroundRefresh = true
})
```
and you can also load the value directly
```csharp
var value = await store.GetOrLoad("schedule", new StoreOptions<Schedule>
{
    ValueExpiry = _defaultExpiry,
    LoadFunc = (ct) => _scheduleService.GetSchedule(ct),
    UsesBackgroundRefresh = true
})
```