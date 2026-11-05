using System.CommandLine;

using JGUZDV.JsonMerge;

var fileOption = new Option<FileInfo[]>("--input", "-in")
{
    Description = "The files to read and merge. The order of the files will be preserved",
    Required = true
};

var outputOption = new Option<FileInfo>("--output", "-out")
{
    Description = "The file to write the merged output to",
    Required = true
};

var rootCommand = new RootCommand("Merges all input files into a single output file.")
{
    Options = { fileOption, outputOption },
};

rootCommand.SetAction(async parseResult =>
{
    await JsonFileHandling.RunJsonFileMergeAsync(
        parseResult.GetRequiredValue(fileOption),
        parseResult.GetRequiredValue(outputOption));
});

return await rootCommand.Parse(args).InvokeAsync();
