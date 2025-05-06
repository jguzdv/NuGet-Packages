using System.CommandLine;

using JGUZDV.JsonMerge;

var fileOption = new Option<FileInfo[]>(
    name: "--input",
    description: "The files to read and merge. The order of the files will be preserved");

fileOption.IsRequired = true;
fileOption.AddAlias("-in");


var outputOption = new Option<FileInfo>(
    name: "--output",
    description: "The file to write the merged output to");

outputOption.IsRequired = true;
outputOption.AddAlias("-out");


var rootCommand = new RootCommand("Merges all input files into a single output file.");
rootCommand.AddOption(fileOption);
rootCommand.AddOption(outputOption);

rootCommand.SetHandler(
    JsonFileHandling.RunJsonFileMergeAsync,
    fileOption,
    outputOption);

return await rootCommand.InvokeAsync(args);
