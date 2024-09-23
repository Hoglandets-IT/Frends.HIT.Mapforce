namespace Frends.HIT.Mapforce;

/// <summary>
/// Helper functions for executing Mapforce processes
/// </summary>
public static class Helpers {

    /// <summary>
    /// Write a list of data to a temporary INI file
    /// </summary>
    /// <param name="data">The lines to write</param>
    /// <returns>The path to the written file</returns> 
    public static string WriteIniFile(List<string> data) {
        var path = Path.GetTempFileName();
        File.WriteAllLines(path, data);
        return path;
    }

    /// <summary>
    /// Read the INI file and parse all rows
    /// </summary>
    /// <param name="path">Path to the INI file</param>
    /// <returns>Contents of INI file as Dictionary</returns>
    public static Dictionary<string, string> ReadIniFile(string path) {
        var data = File.ReadAllLines(path);
        var dict = new Dictionary<string, string>();
        foreach (var line in data) {
            if (line.Contains("=")) {
                var parts = line.Split("=");
                var key = parts.First();
                if (parts.Length > 2) {
                    var value = string.Join("=", parts.Skip(1));
                }
                dict.Add(parts[0], parts[1]);
            }
        }
        
        return dict;
    }

    /// <summary>
    /// Remove old temporary files
    /// </summary>
    /// <param name="files">Paths of files to remove</param>
    /// <param name="DebugLog">Whether to log debug information</param> 
    public static void CleanFiles(List<string> files, bool DebugLog) {
        foreach (var file in files) {
            try {
                if (DebugLog) {
                    try {
                        File.Copy(file, "C:/Temp/MapforceDebugFile-" + Path.GetFileName(file) + ".bak");
                    } catch {}
                }
                File.Delete(file);
            } catch {}
        }
    }
}
