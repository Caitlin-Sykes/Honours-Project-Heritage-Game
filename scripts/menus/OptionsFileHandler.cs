using Godot;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public partial class OptionsFileHandler : Node2D
{

	public readonly static string PROJECT_PATH = System.Environment.CurrentDirectory + "\\config\\";
	public readonly static string CONFIG_PATH = System.Environment.CurrentDirectory + "\\config\\config.txt";

	/**
	* ----------------------------------------------------------------
	* File Handling
	* ----------------------------------------------------------------
	**/

	 // Returns whether the file is present
	public static bool IsOptionsPresent() {
		if (Directory.Exists(OptionsFileHandler.PROJECT_PATH)) {
			return true;
		}

		return false;
	}

	
	// Checks whether the config file is present
	public static bool IsConfigFilePresent() {
		if (File.Exists(OptionsFileHandler.CONFIG_PATH)) {
			return true;
		}
			return false;
	}

	// Creates the options directory - 
	// in C#, only does it if it already exists, hence the lack of validation
	public static void CreateOptionsDirectory() {
        Directory.CreateDirectory(OptionsFileHandler.PROJECT_PATH);
	}

	// Creates the options file
	public static StreamWriter CreateConfigFile() {
		return File.CreateText(OptionsFileHandler.CONFIG_PATH);
	}

	// Closes the config file
	public static void CloseConfigFile(StreamWriter sw) {
		sw.Close();
	}

	// Reads the configuration
	public static void InitConfigFile() {
		OptionsFileHandler.CreateOptionsDirectory();

		//If the file doesn't exist or is empty, will initialise it to defaults
		if (!File.Exists(OptionsFileHandler.CONFIG_PATH) || (new FileInfo(CONFIG_PATH).Length == 0)) {
			StreamWriter sw = OptionsFileHandler.CreateConfigFile();

			//Writes Visual Default Settings
			sw.WriteLine("[VISUAL]");
			sw.WriteLine("[FONT] : CASCADIA");
			sw.WriteLine("[FONT_SIZE] : DEFAULT");
			sw.WriteLine("[FULL_SCREEN] : FALSE");

			CloseConfigFile(sw);

		}
	}

	// A method to replace the setting in a file
	// Borrowed from here: https://www.csharp411.com/searchreplace-in-files/
	// Accessed on the 10/11/2023
	public static void ReplaceInFile(string searchText, string replaceText )
{
    StreamReader reader = new StreamReader(CONFIG_PATH);
    string content = reader.ReadToEnd();
    reader.Close();

    content = Regex.Replace( content, searchText, replaceText );

    StreamWriter writer = new StreamWriter(CONFIG_PATH);
    writer.Write( content );
    writer.Close();
}
	
	// A method to find the line in a file
	public static bool FindInFile(string match)
{
    StreamReader sr = new StreamReader(CONFIG_PATH);
    string content = sr.ReadToEnd();
    sr.Close();

    if (Regex.IsMatch(content, match)) {
		return true;
	}

	return false;
}
}