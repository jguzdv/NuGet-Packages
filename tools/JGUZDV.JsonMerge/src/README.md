# JGUZDV.JsonMerge

This tool will take all input JSON files and merge them into a single output file.
Keys existing in multiple files will be merged into a single value - last one wins.

Also it possible to include other files into the JSON file using the `@includes` array in your json file.
Inclusions will be read first and might be overriden by your input files.

Install this tool via Nuget: 
```pwsh
dotnet tool install JGUZDV.JsonMerge -g
```

Use it like this:
```pwsh
dotnet jsonmerge -in input.json -out output.json
```

