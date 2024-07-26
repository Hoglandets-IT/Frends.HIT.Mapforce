using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.HIT.Mapforce;

/// <summary>
/// Definitions for variables used in the process
/// </summary>
public static class Definitions {
    /// <summary>
    /// Input parameters for running the Mapforce Executable
    /// </summary> 
    public class Input {

        /// <summary>
        /// The path to the Mapforce executable directory (without filename)
        /// </summary>
        [Display(Name = "Mapforce Executable Path")]
        [DefaultValue("D:/Mapforce/")]
        [DisplayFormat(DataFormatString = "Text")]
        public string MapforceExecutablePath { get; set; } = "D:/Mapforce/";

        /// <summary>
        /// The program to run for this execution
        /// </summary>
        [Display(Name = "Mapforce Executable Name")]
        [DefaultValue("executable.exe")]
        public string Program { get; set; } = "executable.exe";

        /// <summary>
        /// The Task ID for this execution
        /// </summary>
        [Display(Name = "Frends Task ID")]
        [DefaultValue("#process.executionId")]
        [DisplayFormat(DataFormatString = "Expression")]
        public string TaskId { get; set; } = "";

        /// <summary>
        /// The data fetched from the source file
        /// </summary>
        [Display(Name = "Input")]
        [DisplayFormat(DataFormatString = "Expression")]
        public byte[] Data { get; set; } = new byte[0];

        /// <summary>
        /// The Raindance company associated with the execution
        /// 100, 200, 300, 400, 500 or 600
        /// </summary>
        /// <value></value>
        [Display(Name = "Randance Company (100-600)")]
        public string RaindanceCompany { get; set; } = "";

        /// <summary>
        /// The path and filename of a variable reference file to be included in the mapforce call as mfm_var_ref_excel
        /// </summary>
        [Display(Name = "Variable Reference Path")]
        [DefaultValue("")]
        public string MapforceVarrefPath { get; set; } = "";

        /// <summary>
        /// The delay in seconds for the message to be pending
        /// </summary>
        /// <value></value>
        [Display(Name = "Message Pending Delay")]
        [DefaultValue(14400)]
        public int MessagePendingDelay { get; set; } = 14400;
    }

    /// <summary>
    /// Output values from the Mapforce execution
    /// </summary>
    public class Output {

        /// <summary>
        /// The contents of the converted file
        /// </summary>
        public byte[]? Data { get; set; }

        /// <summary>
        /// The filename to save the converted file as
        /// </summary>
        public string? Filename { get; set; }

        /// <summary>
        /// The log file from the execution
        /// </summary>
        /// <value></value>
        public string? Log { get; set; }

        /// <summary>
        /// Standard Output from the command
        /// </summary>
        public string? Stdout { get; set; }

        /// <summary>
        /// Standard errors from the command
        /// </summary>
        public string? Stderr { get; set; }
    }
}