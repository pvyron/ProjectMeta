using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProMe.Workflow.Models;

[TypeConverter(typeof(ShortGuidTypeConverter))]
[JsonConverter(typeof(ShortGuidJsonConverter))]
public readonly struct ShortGuid
{
    private readonly Guid _value;

    public ShortGuid(Guid value) => _value = value;

    public static implicit operator Guid(ShortGuid shortGuid) => shortGuid._value;
    public static implicit operator ShortGuid(Guid guid) => new(guid);

    public static ShortGuid Parse(string input) => new Guid(WebEncoders.Base64UrlDecode(input));

    public override string ToString()
    {
        Span<byte> bytes = stackalloc byte[16];
        _value.TryWriteBytes(bytes);
        return WebEncoders.Base64UrlEncode(bytes);
    }

    private sealed class ShortGuidTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type? sourceType)
            => sourceType == typeof(string);

        public override object? ConvertFrom(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value)
            => value is string str ? Parse(str) : null;

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
            => destinationType == typeof(string);

        public override object ConvertTo(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type? destinationType)
            => ((ShortGuid?)value)?.ToString() ?? "";
    }

    private sealed class ShortGuidJsonConverter : JsonConverter<ShortGuid>
    {
        public override ShortGuid Read(ref Utf8JsonReader reader, Type? typeToConvert, JsonSerializerOptions? options)
        {
            var str = reader.GetString();
            if (str != null)
                return Parse(str);

            return default;
        }

        public override void Write(Utf8JsonWriter writer, ShortGuid value, JsonSerializerOptions? options)
            => writer.WriteStringValue(value.ToString());
    }
}