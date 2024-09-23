using static Frends.HIT.Mapforce.Definitions;
using static Frends.HIT.Mapforce.Helpers;

namespace Frends.HIT.Mapforce;

/// <summary>
/// The main program class
/// </summary>
public class Main {
    
    /// <summary>
    /// Run MapForce Executable
    /// </summary>
    /// <param name="input">Input Settings</param>
    /// <returns>Output</returns>
    public static Output RunExecutable(Input input)
    {
        var DebugLog = (string ival) => {};
        StreamWriter sw = File.CreateText("C:/Temp/MapforceDebug.log");

        if (input.DebugLogging) {
            DebugLog = (string ival) => {
                sw.WriteLine(ival);
            };
        }

        List<string> createdFiles = new List<string>();

        var ProgramPath = Path.Join(input.MapforceExecutablePath, input.Program);
        ProgramPath = Path.GetFullPath(ProgramPath);

        DebugLog("ProgramPath: " + ProgramPath);

        var InputFilePath = Path.GetTempFileName();
        File.WriteAllBytes(InputFilePath, input.Data);        
        createdFiles.Add(InputFilePath);

        DebugLog("Input File Path: " + InputFilePath);

        var OutputFilePath = Path.GetTempFileName();
        createdFiles.Add(OutputFilePath);

        DebugLog("Output File Path: " + OutputFilePath);

        var LogfilePath = Path.GetTempFileName();
        createdFiles.Add(LogfilePath);

        DebugLog("Log File Path: " + LogfilePath);

        var ProgramBasepath = Path.GetDirectoryName(ProgramPath) ?? throw new Exception("Program path does not contain directory");
        
        DebugLog("Program Base Path: " + ProgramBasepath);
        
        var TempfilePath = Path.Join(ProgramBasepath, "temp");

        DebugLog("Temp File Path: " + TempfilePath);


        if (!Directory.Exists(TempfilePath)) {
            Directory.CreateDirectory(TempfilePath);
            DebugLog("Created Temp Directory: " + TempfilePath);
        }

        var iniConfig = new List<string> {
            "[IntegrationParametersToMapForceBegin]",
            "TaskId=" + input.TaskId,
            "mf_plugin=" + ProgramPath,
            "mf_inputfile=" + InputFilePath,
            "mf_outputfile=" + OutputFilePath,
            "mf_messagependingdelay=" + input.MessagePendingDelay,
            "mfm_foretag=" + input.RaindanceCompany,
        };

        if (input.MapforceVarrefPath != "") {
            var VarrefAbspath = Path.GetFullPath(input.MapforceVarrefPath);
            iniConfig.Add( "mfm_var_ref_excel=" + VarrefAbspath );
        }

        iniConfig.Add("[IntegrationParametersToMapForceEnd]");

        var IniPath = WriteIniFile(iniConfig);
        createdFiles.Add(IniPath);

        DebugLog("Ini File Path: " + IniPath);
        DebugLog("INI File contents: " + String.Join("\r\n", iniConfig));

        var args = new List<string> {
            "/c",
            ProgramPath,
            "/mf_parameters",
            IniPath,
            "2",
            ">",
            LogfilePath,
            "2>&1"    
        };

        DebugLog("Args: " + String.Join(" ", args));

        var procInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe");
        procInfo.UseShellExecute = false;
        procInfo.RedirectStandardOutput = true;
        procInfo.RedirectStandardError = true;
        procInfo.CreateNoWindow = true;
        procInfo.Arguments = string.Join(" ", args);

        var proc = System.Diagnostics.Process.Start(procInfo) ?? throw new Exception("Process failed to start");
        proc.WaitForExit();

        var output = proc.StandardOutput.ReadToEnd();
        var error = proc.StandardError.ReadToEnd();

        DebugLog("Output: " + output);
        DebugLog("Error: " + error);

        proc.Close();

        if (error.Length > 0) {
            CleanFiles(createdFiles, input.DebugLogging);
            throw new Exception(error);
        }

        var log = File.ReadAllText(LogfilePath);

        var responseVar = new Output{
            Stdout = output,
            Stderr = error,
            Log = log
        };
        
        DebugLog("Reading result INI at " + IniPath);

        var resultIni = ReadIniFile(IniPath);
        foreach (var key in resultIni.Keys) {
            DebugLog("ResultINI Key " + key + " Value " + resultIni[key]);
        }

        if (!resultIni.TryGetValue("mf_temp_files", out var mfTempFiles)) {
            CleanFiles(createdFiles, input.DebugLogging);
            throw new Exception("Output file does not contain mf_temp_files: " + error + output + log);
        }
                
        if (!resultIni.TryGetValue("mfr_filename", out var mfrFilename)) {
            CleanFiles(createdFiles, input.DebugLogging);
            throw new Exception("Output file does not contain mfr_filename: " + error + output + log);
        }

        if (mfTempFiles.Length > 0) {
            foreach (var tempFile in mfTempFiles.Split("|")) {
                if (tempFile.Length == 0) {
                    continue;
                }
                if (!File.Exists(tempFile)) {
                    createdFiles.Add(Path.Join(TempfilePath, tempFile));
                    continue;
                }
                createdFiles.Add(Path.Join(tempFile));
            }
        }

        responseVar.Filename = mfrFilename;
        responseVar.Data = File.ReadAllBytes(OutputFilePath);

        CleanFiles(createdFiles, input.DebugLogging);

        if ( responseVar.Data.Length < 10 ) {
            throw new Exception("Output file is empty, conversion likely failed: " + error + output + log);
        }
        
        sw.Flush();
        sw.Close();

        return responseVar;
    }
}