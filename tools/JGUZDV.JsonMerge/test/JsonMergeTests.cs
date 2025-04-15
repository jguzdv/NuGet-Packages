using System.Text.Json.Nodes;

namespace JGUZDV.JsonMerge.Tests
{
    public class JsonMergeTests
    {
        [Fact]
        public void Two_Objects_Are_Merged()
        {
            var json1 = JsonNode.Parse("""
                {
                    "prop1": "value1",
                    "prop2": "value2",
                    "prop3": {
                        "subprop1": "subvalue1",
                        "subprop2": "subvalue2"
                    },
                    "prop4": "value4"
                }
                """)!;

            var json2 = JsonNode.Parse("""
                {
                    "prop1": "value3",
                    "prop2": "value4",
                    "prop3": {
                        "subprop1": "subvalue3",
                        "subprop3": "subvalue4"
                    }
                }
                """);

            JsonFileHandling.MergeJsonNodes(json1, json2);

            Assert.Equal("value3", json1["prop1"].GetValue<string>());
            Assert.Equal("value4", json1["prop2"].GetValue<string>());
            Assert.Equal("value4", json1["prop4"].GetValue<string>());
            Assert.Equal("subvalue3", json1["prop3"]["subprop1"].GetValue<string>());
            Assert.Equal("subvalue2", json1["prop3"]["subprop2"].GetValue<string>());
            Assert.Equal("subvalue4", json1["prop3"]["subprop3"].GetValue<string>());
        }

        [Fact]
        public void Merge_Two_String_Arrays()
        {
            var json1 = JsonNode.Parse("""
                {
                    "prop1": ["value1", "value2"],
                    "prop2": ["value3", "value4"]
                }
                """)!;
            var json2 = JsonNode.Parse("""
                {
                    "prop1": ["value5", "value6"],
                    "prop2": ["value3", "value8"]
                }
                """);
            JsonFileHandling.MergeJsonNodes(json1, json2);
            Assert.Equal(4, json1["prop1"].AsArray().Count);
            Assert.Equal(4, json1["prop2"].AsArray().Count);
        }
    }
}
