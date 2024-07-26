using Frends.HIT.Mapforce;

Test with a sample file for Edlevo
var testdata = File.ReadAllBytes("../Testfiles/file.inc");

var input = new Definitions.Input
{
    MapforceExecutablePath = "../Executables",
    Program = "EdlevoToRaindance.exe",
    TaskId = "123",
    Data = testdata,
    RaindanceCompany = "100",
    MapforceVarrefPath = "../References/123_varref.xlsx",
    MessagePendingDelay = 14400,
};

var output = Main.RunExecutable(input);
Console.WriteLine("Filename: " + output.Filename);
Console.WriteLine("Log: " + output.Log);
Console.WriteLine("Stdout: " + output.Stdout);
Console.WriteLine("Stderr: " + output.Stderr);
Console.WriteLine("Data: " + output.Data);

// Test with a sample file for Speedadmin
var testdata = File.ReadAllBytes("../Testfiles/file.txt");

var input = new Definitions.Input {
    MapforceExecutablePath = "../Executables",
    Program = "SpeedadminToRaindance.exe",
    TaskId = "456",
    Data = testdata,
    RaindanceCompany = "123",
};

var output = Main.RunExecutable(input);
Console.WriteLine("Filename: " + output.Filename);
Console.WriteLine("Log: " + output.Log);
Console.WriteLine("Stdout: " + output.Stdout);
Console.WriteLine("Stderr: " + output.Stderr);
Console.WriteLine("Data: " + output.Data);
