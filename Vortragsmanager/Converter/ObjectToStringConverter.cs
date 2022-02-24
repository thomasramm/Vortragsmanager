namespace Vortragsmanager.Converter
{
    internal class ObjectToStringConverter
    {
        public static string Convert(object input)
        {
            if (input == null)
                return string.Empty;
            return input.ToString().Trim();
        }
    }
}