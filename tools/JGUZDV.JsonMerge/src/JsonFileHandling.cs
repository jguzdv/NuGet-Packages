using System.Text.Json;
using System.Text.Json.Nodes;

namespace JGUZDV.JsonMerge
{
    public class JsonFileHandling
    {
        private static JsonNodeOptions DefaultNodeOptions = new() { 
            PropertyNameCaseInsensitive = true
        };
        private static JsonDocumentOptions DefaultDocumentOptions = new() { 
            AllowTrailingCommas = true, 
            CommentHandling = JsonCommentHandling.Skip 
        };

        private static JsonSerializerOptions DefaultWriterOptions = new() {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
        };


        public static async Task RunJsonFileMergeAsync(FileInfo[] inputFiles, FileInfo outputFile)
        {
            var fileStack = new List<FileInfo>(inputFiles);
            var fileContents = new Dictionary<string, JsonNode?>();

            // Read all files and their includes - 
            for (int i = 0; i < fileStack.Count; i++)
            {
                var inputFile = fileStack[i];
                var (fileContent, includeFiles) = await ReadJsonFileAsync(inputFile);

                if (includeFiles != null)
                {
                    // when we finish, we'll read the files in reverse, so were adding them to the stack in reverse order
                    fileStack.AddRange(includeFiles.Reverse());
                }

                fileContents.Add(inputFile.FullName, fileContent);
            }

            // Merge all files together into the empty result node
            var resultNode = new JsonObject();
            for (int i = fileStack.Count -1; i >= 0; i--)
            {
                var filePath = fileStack[i].FullName;
                var fileContent = fileContents[filePath];
                
                if (fileContent != null)
                {
                    MergeJsonNodes(resultNode, fileContent);
                }
            }

            // Write the merged result to the output file
            await WriteJsonFileAsync(outputFile, resultNode);
        }


        public static async Task<(JsonNode? fileContent, FileInfo[]? includeFiles)> ReadJsonFileAsync(FileInfo inputFile)
        {
            if(!inputFile.Exists)
            {
                throw new FileNotFoundException($"The file '{inputFile.FullName}' does not exist.");
            }

            try
            {
                using(var stream = inputFile.OpenRead())
                {
                    var jsonNode = await JsonNode.ParseAsync(stream, DefaultNodeOptions, DefaultDocumentOptions);

                    if(jsonNode is JsonObject jsonObject && jsonObject.ContainsKey("@includes"))
                    {
                        // read all values as string
                        var includes = jsonObject["@includes"]!.AsArray()
                            .Select(x => x!.GetValue<string>())
                            .Select(x => new FileInfo(Path.Combine(inputFile.Directory!.FullName, x)))
                            .ToArray();

                        // remove the @includes property from the json object
                        jsonObject.Remove("@includes");

                        return (jsonObject, includes);
                    }

                    return (jsonNode, null);
                }
            }
            catch (IOException ex)
            {
                throw new IOException($"Error reading the file '{inputFile.FullName}': {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Error parsing the JSON file '{inputFile.FullName}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred while reading the file '{inputFile.FullName}': {ex.Message}", ex);
            }
        }

        public static async Task WriteJsonFileAsync(FileInfo outputFile, JsonNode fileContent)
        {
            try
            {
                using var stream = outputFile.Open(FileMode.Create, FileAccess.Write);
                await JsonSerializer.SerializeAsync(stream, fileContent, DefaultWriterOptions); 
            }
            catch (IOException ex)
            {
                throw new IOException($"Error writing to the file '{outputFile.FullName}': {ex.Message}", ex);
            }
        }


        public static void MergeJsonNodes(JsonNode target, JsonNode? merge)
        {
            // Recursively merge JSON Objects
            if (target is JsonObject targetObject && merge is JsonObject mergeObject)
            {
                foreach (var kvp in mergeObject)
                {
                    if (kvp.Value != null)
                    {
                        if (targetObject.ContainsKey(kvp.Key))
                        {
                            MergeJsonNodes(targetObject[kvp.Key]!, kvp.Value.DeepClone());
                        }
                        else
                        {
                            targetObject[kvp.Key] = kvp.Value.DeepClone();
                        }
                    }
                }
            }
            // Merge JSON Arrays together into a single array
            else if (target is JsonArray targetArray && merge is JsonArray mergeArray)
            {
                foreach (var item in mergeArray)
                {
                    targetArray.Add(item!.DeepClone());
                }
            }
            // Override the target value with the merge value if they are not objects or arrays
            else if (target is JsonValue targetValue && merge is JsonValue mergeValue)
            {
                targetValue.ReplaceWith(mergeValue.DeepClone());
            }
            else
            {
                throw new InvalidOperationException($"Cannot merge from '{merge?.GetPropertyName()}' ({merge?.GetType().Name}) into '{target.GetPropertyName()}' ({target.GetType().Name}");
            }
        }
    }
}
