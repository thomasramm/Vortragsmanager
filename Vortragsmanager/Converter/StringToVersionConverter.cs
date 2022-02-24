using System;
using DevExpress.Xpf.Core.Native;

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

        public static string ConvertBack(Version version)
        {
            return version.ToString(3);
        }
    }
}