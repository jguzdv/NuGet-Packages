# JGUZDV.Blazor.Components.Map

Blazor-Komponentenbibliothek zur Darstellung einer Karte auf Basis von `maplibre-gl`.

## Installation

NuGet-Paket referenzieren:
- `JGUZDV.Blazor.Components.Map`

Namespace importieren:
- `@using JGUZDV.Blazor.Components.Map`

## Komponente

`BlazorMap`

### Parameter

- `Center` (**required**): Startkoordinate als `LngLat`
- `Zoom` (default: `15`): Start-Zoom
- `MaxBounds`: optionale Bounding-Box
- `BaseLayerStyleUrl`: URL der Basis-Style-Definition
- `AdditionalLayerStyleUrls`: zusätzliche Layer-Style-URLs
- `AdditionalSourceData`: zusätzliche GeoJSON-Quellen
- `PathPrefix`: Präfix für relative Pfade (Sprites/Bilder/Sources)
- `IsServerRendered`: serverseitige Initialisierung aktivieren
- `MapClicked`: Callback für Kartenklick
- `MapIsInitialized`: Callback nach Initialisierung (`true`/`false`)

### Öffentliche Methoden (`@ref`)

- `PanToPolygon(List<LngLat> bounds)`
- `AddStyleLayerFromUrl(string url)`
- `SetSourceDataFromUrl(string sourceId, string data)`
- `SetSourceData(string sourceId, FeatureCollection data)`
- `AddLayer(object data)`
- `RemoveLayer(string layerId)`

Hinweis: Diese Methoden sind nur im interaktiven Modus verfügbar (`IsServerRendered == false`).

## Beispiel (interaktiv)

```razor
<div style="height:300px;">
    <BlazorMap Center="new LngLat(8.2377, 49.9934)"
               Zoom="15"
               MapClicked="OnMapClicked"
               MapIsInitialized="OnMapInitialized" />
</div>

@code {
    private void OnMapClicked(MapMouseEventArgs args)
    {
        Console.WriteLine($"Klick bei {args.LngLat?.Lng}, {args.LngLat?.Lat}");
    }

    private Task OnMapInitialized(bool initialized)
    {
        return Task.CompletedTask;
    }
}
```

## Beispiel (server-rendered)

```razor
<div style="height:300px;">
    <BlazorMap Center="new LngLat(8.2377, 49.9934)"
               IsServerRendered="true"
               AdditionalSourceData="_sourceData" />
</div>
```

## Frontend-Build (TypeScript)

Manuell:

```powershell
cd libraries/JGUZDV.Blazor.Components.Map/src/typescript
npm ci
npm run build:Debug
```

`dotnet build` des Projekts führt den TypeScript-Build ebenfalls über die Targets in der `.csproj` aus.
