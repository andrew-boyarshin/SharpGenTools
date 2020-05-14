#nullable enable

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using SharpGen.Doc;
using static SharpGenTools.Sdk.Documentation.DocConverterUtilities;

namespace SharpGenTools.Sdk.Documentation
{
    internal sealed class DocSubItemConverter : JsonConverter<IDocSubItem>
    {
        public override bool CanConvert(Type typeToConvert) => typeof(IDocSubItem).IsAssignableFrom(typeToConvert);

        public override IDocSubItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var subItem = new DocSubItem();

            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            subItem.Term = Read<string>(ref reader, nameof(IDocSubItem.Term));
            subItem.Description = Read<string>(ref reader, nameof(IDocSubItem.Description));
            AssignSet(subItem.Attributes, Read<HashSet<string>>(ref reader, nameof(IDocSubItem.Attributes)));

            if (!reader.Read())
                throw new JsonException();

            if (reader.TokenType != JsonTokenType.EndObject)
                throw new JsonException();

            subItem.IsDirty = false;

            return subItem;

            T Read<T>(ref Utf8JsonReader reader, string expectedPropertyName) =>
                ReadProperty<T>(ref reader, expectedPropertyName, options);
        }

        public override void Write(Utf8JsonWriter writer, IDocSubItem value, JsonSerializerOptions options)
        {
            JsonConverter<ISet<string>>? setStringConverter = null;

            writer.WriteStartObject();

            writer.WriteString(nameof(value.Term), value.Term);
            writer.WriteString(nameof(value.Description), value.Description);
            Write(ref setStringConverter, nameof(value.Attributes), value.Attributes);

            writer.WriteEndObject();

            value.IsDirty = false;

            void Write<T>(ref JsonConverter<T>? converter, string name, T value) =>
                WriteProperty(writer, options, ref converter, name, value);
        }
    }
}