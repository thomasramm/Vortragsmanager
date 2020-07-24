/*

 Code von https://gist.github.com/Larry57/5725301
 NuGet PAket nicht benutzt um Codeänderungen vornehmen zu können,
 so will ich die ini nicht aus einer Datei sondern aus einem string auslesen

  */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Vortragsmanager.Core;

public class Ini
{
    private Dictionary<string, Dictionary<string, string>> ini = new Dictionary<string, Dictionary<string, string>>(StringComparer.InvariantCultureIgnoreCase);
    private string file;

    /// <summary>
    /// Initialize an INI file
    /// Load it if it exists
    /// </summary>
    /// <param name="file">Full path where the INI file has to be read from or written to</param>
    public Ini(string file)
    {
        this.file = file;

        if (!File.Exists(file))
            return;

        var content = File.ReadAllText(file);
        Load(content);
    }

    public Ini()
    {
    }

    /// <summary>
    /// Load the INI file content
    /// </summary>
    public void Load(string content)
    {
        Log.Info(nameof(Load), content);
        if (string.IsNullOrEmpty(content))
            return;

        Dictionary<string, string> currentSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        ini[""] = currentSection;

        foreach (var l in content.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                          .Select((t, i) => new
                          {
                              idx = i,
                              text = t.Trim()
                          }))
        // .Where(t => !string.IsNullOrWhiteSpace(t) && !t.StartsWith(";")))
        {
            var line = l.text;

            if (line.StartsWith(";", StringComparison.InvariantCulture) || string.IsNullOrWhiteSpace(line))
            {
                currentSection.Add(";" + l.idx.ToString(CultureInfo.InvariantCulture), line);
                continue;
            }

            if (line.StartsWith("[", StringComparison.InvariantCulture) && line.EndsWith("]", StringComparison.InvariantCulture))
            {
                currentSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                ini[line.Substring(1, line.Length - 2)] = currentSection;
                continue;
            }

            var idx = line.IndexOf("=", StringComparison.InvariantCulture);
            if (idx == -1)
                currentSection[line] = "";
            else
                currentSection[line.Substring(0, idx)] = line.Substring(idx + 1);
        }
    }

    /// <summary>
    /// Get a parameter value at the root level
    /// </summary>
    /// <param name="key">parameter key</param>
    /// <returns></returns>
    public string GetValue(string key)
    {
        return GetValue(key, "", "");
    }

    /// <summary>
    /// Get a parameter value in the section
    /// </summary>
    /// <param name="key">parameter key</param>
    /// <param name="section">section</param>
    /// <returns></returns>
    public string GetValue(string key, string section)
    {
        return GetValue(key, section, "");
    }

    /// <summary>
    /// Returns a parameter value in the section, with a default value if not found
    /// </summary>
    /// <param name="key">parameter key</param>
    /// <param name="section">section</param>
    /// <param name="default">default value</param>
    /// <returns></returns>
    public string GetValue(string key, string section, string @default)
    {
        Log.Info(nameof(GetValue), $"key={key}, section={section}, default={@default}");
        if (!ini.ContainsKey(section))
            return @default;

        if (!ini[section].ContainsKey(key))
            return @default;

        return ini[section][key];
    }

    /// <summary>
    /// Save the INI file
    /// </summary>
    public void Save()
    {
        Log.Info(nameof(Save));
        var sb = new StringBuilder();
        foreach (var section in ini)
        {
            if (!string.IsNullOrEmpty(section.Key))
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "[{0}]", section.Key);
                sb.AppendLine();
            }

            foreach (var keyValue in section.Value)
            {
                if (keyValue.Key.StartsWith(";", StringComparison.InvariantCulture))
                {
                    sb.Append(keyValue.Value);
                    sb.AppendLine();
                }
                else
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0}={1}", keyValue.Key, keyValue.Value);
                    sb.AppendLine();
                }
            }

            if (!endWithCRLF(sb))
                sb.AppendLine();
        }

        File.WriteAllText(file, sb.ToString());
    }

    private static bool endWithCRLF(StringBuilder sb)
    {
        if (sb.Length < 4)
            return sb[sb.Length - 2] == '\r' &&
                   sb[sb.Length - 1] == '\n';
        else
            return sb[sb.Length - 4] == '\r' &&
                   sb[sb.Length - 3] == '\n' &&
                   sb[sb.Length - 2] == '\r' &&
                   sb[sb.Length - 1] == '\n';
    }

    /// <summary>
    /// Write a parameter value at the root level
    /// </summary>
    /// <param name="key">parameter key</param>
    /// <param name="value">parameter value</param>
    public void WriteValue(string key, string value)
    {
        WriteValue(key, "", value);
    }

    /// <summary>
    /// Write a parameter value in a section
    /// </summary>
    /// <param name="key">parameter key</param>
    /// <param name="section">section</param>
    /// <param name="value">parameter value</param>
    public void WriteValue(string key, string section, string value)
    {
        Log.Info(nameof(WriteValue), $"key={key}, section={section}, value={value}");
        Dictionary<string, string> currentSection;
        if (!ini.ContainsKey(section))
        {
            currentSection = new Dictionary<string, string>();
            ini.Add(section, currentSection);
        }
        else
            currentSection = ini[section];

        currentSection[key] = value;
    }

    /// <summary>
    /// Get all the keys names in a section
    /// </summary>
    /// <param name="section">section</param>
    /// <returns></returns>
    public string[] GetKeys(string section)
    {
        Log.Info(nameof(GetKeys), section);
        if (!ini.ContainsKey(section))
            return Array.Empty<string>();

        return ini[section].Keys.ToArray();
    }

    /// <summary>
    /// Get all the section names of the INI file
    /// </summary>
    /// <returns></returns>
    public string[] GetSections()
    {
        Log.Info(nameof(GetSections));
        return ini.Keys.Where(t => !string.IsNullOrEmpty(t)).ToArray();
    }
}