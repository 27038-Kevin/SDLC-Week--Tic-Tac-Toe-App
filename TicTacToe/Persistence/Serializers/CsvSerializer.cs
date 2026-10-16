using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using TicTacToe.Models.Data;

namespace TicTacToe.Persistence.Serializers
{
    /// <summary>
    /// Serializes and deserializes objects of type <see cref="T"/> to and from a CSV string representation.
    /// </summary>
    /// <typeparam name="T">The type of the object to be serialized/deserialized in CSV.</typeparam>
    public class CsvSerializer<T> : ISerializer<T>
        where T : IHasId, new()
    {
        private static readonly PropertyInfo[] WritableProperties = typeof(T)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(property => property.SetMethod is not null && property.CanWrite)
                        .OrderBy(property => property.Name)
                        .ToArray();

        /// <summary>
        /// Serializes an object to a CSV-formatted string.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <returns>A serialized object formatted as a CSV-compatible string.</returns>
        /// <exception cref="SerializationException">Thrown if serialization fails due to a property value being null.</exception>
        public string Serialize(T obj)
        {
            var serializedProperties = WritableProperties.Select(property => property.GetValue(obj) switch
            {
                string stringValue => this.EscapeString(stringValue),
                decimal decimalValue => decimalValue.ToString(Constants.ProjectCulture),
                DateTime dateValue => dateValue.ToString(Constants.DateTimeFormat, Constants.ProjectCulture),
                IEnumerable<int> intCollection => this.EscapeString(string.Join(',', intCollection)),
                object value => this.EscapeString(value.ToString() ?? string.Empty),
                null => throw new SerializationException("One or more required properties are null"),
            });

            return string.Join(',', serializedProperties);
        }

        /// <summary>
        /// Deserializes a CSV-formatted string into an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="line">The CSV data to be deserialized.</param>
        /// <returns>The deserialized data from the CSV line.</returns>
        /// <exception cref="FormatException">Thrown if a data value cannot be parsed due to corruption.</exception>
        /// <exception cref="SerializationException">Thrown if deserialization fails due to an unknown property type.</exception>
        public T Deserialize(string line)
        {
            List<string> parts = this.ParseCsv(line);
            if (parts.Count != WritableProperties.Length)
            {
                throw new FormatException("Invalid CSV format: Number of parts do not match number of writable properties");
            }

            var obj = new T();
            for (int i = 0; i < WritableProperties.Length; ++i)
            {
                PropertyInfo property = WritableProperties[i];
                string part = parts[i];
                object value = property.PropertyType switch
                {
                    Type tBool when tBool == typeof(bool) => bool.Parse(part),

                    Type tDate when tDate == typeof(DateTime) =>
                        DateTime.ParseExact(
                            part,
                            Constants.DateTimeFormat,
                            Constants.ProjectCulture),

                    Type tEnum when tEnum.IsEnum => Enum.Parse(tEnum, part),
                    Type tGuid when tGuid == typeof(Guid) => Guid.Parse(part),

                    Type tIntArray when tIntArray == typeof(int[]) =>
                        this.ParseCsv(part)
                            .Select(number => int.Parse(number))
                            .ToArray(),

                    Type tPassword when tPassword == typeof(Password) =>
                        Password.FromString(part),

                    Type tString when tString == typeof(string) => part,
                    Type tUInt when tUInt == typeof(uint) => uint.Parse(part),

                    var _ => throw new SerializationException("Cannot parse value when type is unknown"),
                };
                property.SetValue(obj, value);
            }

            return obj;
        }

        /// <summary>
        /// Converts a string into a dot-separated string of byte values, which can be used for serialization.
        /// </summary>
        /// <param name="value">The string to be serialized.</param>
        /// <returns>A dot-separated string containing byte values from the original string.</returns>
        private string EscapeString(string value)
        {
            // Double-quote to escape
            string escapedString = value.Replace("\"", "\"\"");

            return $"\"{escapedString}\"";  // Enclose in double quotes before returning
        }

        /// <summary>
        /// Parses a CSV line and separates the parts of the line after parsing escaped quotes.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>The parts parsed from the line.</returns>
        /// <exception cref="FormatException">Thrown when the line is not in the expected format (is missing a closing quote).</exception>
        private List<string> ParseCsv(string line)
        {
            var parts = new List<string>();

            var part = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; ++i)
            {
                switch (line[i])
                {
                    /**
                     * If currently inside quotes and double quotes are found, add to part and skip the next character.
                     * If currently inside quotes, but not a double quote, then it is the closing quote.
                     * If not inside quotes, then it must be an opening quote.
                     */
                    case '"':
                        if (inQuotes
                              && i + 1 < line.Length
                              && line[i + 1] == '"')
                        {
                            part.Append(line[i]);
                            ++i; // Skip double quote
                        }
                        else
                        {
                            inQuotes = !inQuotes;
                        }
                        break;

                    /**
                     * If currently inside quotes, then add comma to part.
                     * Otherwise, treat it as a delimiter.
                     */
                    case ',':
                        if (inQuotes)
                        {
                            part.Append(line[i]);
                        }
                        else
                        {
                            // Add part to list of parts, and clear the string builder
                            parts.Add(part.ToString());
                            part.Clear();
                        }
                        break;

                    /**
                     * No need to add linefeed character to any parts, since we are just parsing.
                     */
                    case '\r':
                        break;

                    /**
                     * Only append newline if it is in quotes.
                     */
                    case '\n':
                        if (inQuotes)
                        {
                            part.Append(Environment.NewLine);
                        }
                        break;

                    /**
                     * For any other character, simply add it to the part.
                     */
                    default:
                        part.Append(line[i]);
                        break;
                }
            }
            parts.Add(part.ToString());

            if (inQuotes)
            {
                throw new FormatException("Unclosed quote");
            }

            return parts;
        }
    }
}