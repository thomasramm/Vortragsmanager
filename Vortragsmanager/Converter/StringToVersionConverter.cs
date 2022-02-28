using System;

namespace Vortragsmanager.Converter
{
    internal class StringToVersionConverter
    {
        public static Version Convert(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input == "0")
                return new Version();
            else
                return new Version(input);
        }
    }
}