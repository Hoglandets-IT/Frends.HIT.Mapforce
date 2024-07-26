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
        List<string> createdFiles = new List<string>();

        var ProgramPath = Path.Join(input.MapforceExecutablePath, input.Program);
        ProgramPath = Path.GetFullPath(ProgramPath);

        var InputFilePath = Path.GetTempFileName();
        File.WriteAllBytes(InputFilePath, input.Data);        
        createdFiles.Add(InputFilePath);

        var OutputFilePath = Path.GetTempFileName();
        createdFiles.Add(OutputFilePath);

        var LogfilePath = Path.GetTempFileName();
        createdFiles.Add(LogfilePath);

        var ProgramBasepath = Path.GetDirectoryName(ProgramPath) ?? throw new Exception("Program path does not contain directory");
        var TempfilePath = Path.Join(ProgramBasepath, "temp");

        if (!Directory.Exists(TempfilePath)) {
            Directory.CreateDirectory(TempfilePath);
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

        proc.Close();

        if (error.Length > 0) {
            CleanFiles(createdFiles);
            throw new Exception(error);
        }

        var log = File.ReadAllText(LogfilePath);

        var responseVar = new Output{
            Stdout = output,
            Stderr = error,
            Log = log
        };
        
        
        var resultIni = ReadIniFile(IniPath);
        if (!resultIni.TryGetValue("mf_temp_files", out var mfTempFiles)) {
            CleanFiles(createdFiles);
            throw new Exception("Output file does not contain mf_temp_files: " + error + output + log);
        }

        if (!resultIni.TryGetValue("mfr_filename", out var mfrFilename)) {
            CleanFiles(createdFiles);
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

        CleanFiles(createdFiles);

        if ( responseVar.Data.Length < 10 ) {
            throw new Exception("Output file is empty, conversion likely failed: " + error + output + log);
        }

        return responseVar;
    }
}