using System.Text.Json;

namespace Quantex.API.Utilities;

public static class QuantextJsonSerializerOptions
{
    private static JsonSerializerOptions _default = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
    };

    private static JsonSerializerOptions _withoutIndentInstance = new(JsonSerializerOptions.Web)
    {
        WriteIndented = false,
    };

    public static JsonSerializerOptions Default => _default;

    public static JsonSerializerOptions WithoutIndentInstance => _withoutIndentInstance;
}
