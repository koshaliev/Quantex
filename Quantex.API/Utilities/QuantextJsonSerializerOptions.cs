using System.Text.Json;

namespace Quantex.API.Utilities;

public static class QuantextJsonSerializerOptions
{
    private static JsonSerializerOptions _instance = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
    };

    public static JsonSerializerOptions Instance => _instance;
}
