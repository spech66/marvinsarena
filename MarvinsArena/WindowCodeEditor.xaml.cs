using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;

using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System.Reflection;
using System.CodeDom.Compiler;

namespace MarvinsArena
{
	/// <summary>
	/// Interaction logic for WindowCodeEditor.xaml
	/// </summary>
	public partial class WindowCodeEditor : Window
	{
		private System.Diagnostics.Process battleengineProcess;
		private string filename, assemblyName;
		private string baseTitle;
		private bool lastBuildSucceeded;
		private CodeFile codeFile;

		public WindowCodeEditor()
		{
			InitializeComponent();
			baseTitle = Title;
			filename = "";

			codeFile = new CodeFile(MarvinsArena.Language.CSharp);
			ScintillaEditor.ConfigurationManager.Language = "cs";
			AssingFieldsToControls();
			
			// Show line numbers: http://scintillanet.codeplex.com/wikipage?title=HowToLineNumber&referringTitle=Documentation
			ScintillaEditor.Margins[0].Width = 20;

			lastBuildSucceeded = false;
			UpdateButtons();
		}

		private void UpdateButtons()
		{
			if (filename == "")
			{
				MenuItemBuild.IsEnabled = false;
				ToolBarBuild.IsEnabled = false;
				MenuItemRun2DBattleEngine.IsEnabled = false;
				MenuItemRun3DBattleEngine.IsEnabled = false;
				ToolBarRun2DBattleEngine.IsEnabled = false;
				ToolBarRun3DBattleEngine.IsEnabled = false;
			}
			else if (filename != "" && !lastBuildSucceeded)
			{
				MenuItemBuild.IsEnabled = true;
				ToolBarBuild.IsEnabled = true;
				MenuItemRun2DBattleEngine.IsEnabled = false;
				MenuItemRun3DBattleEngine.IsEnabled = false;
				ToolBarRun2DBattleEngine.IsEnabled = false;
				ToolBarRun3DBattleEngine.IsEnabled = false;
			}
			else
			{
				MenuItemBuild.IsEnabled = true;
				ToolBarBuild.IsEnabled = true;
				MenuItemRun2DBattleEngine.IsEnabled = true;
				MenuItemRun3DBattleEngine.IsEnabled = true;
				ToolBarRun2DBattleEngine.IsEnabled = true;
				ToolBarRun3DBattleEngine.IsEnabled = true;
			}
		}

		private void MenuItemNewCSharp_Click(object sender, RoutedEventArgs e)
		{
			codeFile = new CodeFile(MarvinsArena.Language.CSharp);
			filename = "";
			this.Title = baseTitle;
			
			ScintillaEditor.ConfigurationManager.Language = "cs";
			AssingFieldsToControls();
			lastBuildSucceeded = false;
			UpdateButtons();
		}

		private void MenuItemNewVisualBasic_Click(object sender, RoutedEventArgs e)
		{
			codeFile = new CodeFile(MarvinsArena.Language.VisualBasic);
			filename = "";
			this.Title = baseTitle;

			ScintillaEditor.ConfigurationManager.Language = "vb";
			//ScintillaEditor.ConfigurationManager.Language = "vbscript";
			AssingFieldsToControls();
			lastBuildSucceeded = false;
			UpdateButtons();
		}

		private void MenuItemRun2DBattleEngine_Click(object sender, RoutedEventArgs e)
		{
			RunBattleEngine("2DBattleEngine.exe");
		}

		private void MenuItemRun3DBattleEngine_Click(object sender, RoutedEventArgs e)
		{
			RunBattleEngine("3DBattleEngine.exe");
		}

		private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
				dlg.RestoreDirectory = true;
				dlg.FileName = "Code File";
				dlg.DefaultExt = ".ctml";
				dlg.InitialDirectory = "CodeFiles"; // TODO: Change this
				dlg.Filter = "Code files (.ctml)|*.ctml";

				Nullable<bool> result = dlg.ShowDialog();
				if (result == true)
				{
					filename = dlg.FileName;
					codeFile = CodeFile.ReadFromXml(filename);
					this.Title = baseTitle + " - " + System.IO.Path.GetFileNameWithoutExtension(filename);
					AssingFieldsToControls();
					lastBuildSucceeded = false;
					UpdateButtons();
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show("Error opening file.\n" + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Update control values from code file
		/// </summary>
		private void AssingFieldsToControls()
		{
			ScintillaEditor.Text = codeFile.Code;
			textBoxInfoName.Text = codeFile.Name;
			textBoxInfoVersion.Text = codeFile.Version;
			textBoxInfoAuthor.Text = codeFile.Author;
			textBoxInfoDescription.Text = codeFile.Description;
		}

		/// <summary>
		/// Update code file with control values
		/// </summary>
		private void AssingControlsToFields()
		{
			codeFile.Code = ScintillaEditor.Text;
			codeFile.Name = textBoxInfoName.Text;
			codeFile.Version = textBoxInfoVersion.Text;
			codeFile.Author = textBoxInfoAuthor.Text;
			codeFile.Description = textBoxInfoDescription.Text;
		}

		private void MenuItemSave_Click(object sender, RoutedEventArgs e)
		{
			if (filename == String.Empty)
			{
				MenuItemSaveAs_Click(sender, e);
				return;
			}

			try
			{
				AssingControlsToFields();
				codeFile.SaveToXml(filename);
				UpdateButtons();
				MessageBox.Show("Code saved to file", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception exception)
			{
				MessageBox.Show("Error saving file.\n" + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void MenuItemSaveAs_Click(object sender, RoutedEventArgs e)
		{
			AssingControlsToFields();

			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
			dlg.RestoreDirectory = true;
			dlg.FileName = "Code File";
			dlg.DefaultExt = ".ctml";
			dlg.InitialDirectory = "CodeFiles"; // TODO: Change this
			dlg.Filter = "Code files (.ctml)|*.ctml";

			Nullable<bool> result = dlg.ShowDialog();
			if (result == true)
			{
				filename = dlg.FileName;
				codeFile.SaveToXml(filename);
				this.Title = baseTitle + " - " + System.IO.Path.GetFileNameWithoutExtension(filename);
				UpdateButtons();
			}
		}

		private void MenuItemExit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void MenuItemBuildBuild_Click(object sender, RoutedEventArgs e)
		{			
			ErrorList.Items.Clear();
			
			CodeDomProvider codeProvider;
			string source = String.Empty;
			if (codeFile.CodeLanguage == MarvinsArena.Language.CSharp)
			{
				codeProvider = new CSharpCodeProvider();

				source += "using System.Reflection;\n";
				source += "using System.Runtime.CompilerServices;\n";
				source += "using System.Runtime.InteropServices;\n\n";
				source += "[assembly: AssemblyTitle(\"" + textBoxInfoName.Text + "\")]\n";
				source += "[assembly: AssemblyDescription(\"" + textBoxInfoDescription.Text + "\")]\n";
				source += "[assembly: AssemblyConfiguration(\"\")]\n";
				source += "[assembly: AssemblyCompany(\"" + textBoxInfoAuthor.Text + "\")]\n";
				source += "[assembly: AssemblyProduct(\"" + textBoxInfoName.Text + "\")]\n";
				source += "[assembly: AssemblyVersion(\"" + textBoxInfoVersion.Text + "\")]\n";
				source += "[assembly: AssemblyFileVersion(\"" + textBoxInfoVersion.Text + "\")]\n";
			}
			else if (codeFile.CodeLanguage == MarvinsArena.Language.VisualBasic)
			{
				codeProvider = new VBCodeProvider();

				source += "Imports System\n";
				source += "Imports System.Reflection\n";
				source += "Imports System.Runtime.InteropServices\n\n";
				source += "<Assembly: AssemblyTitle(\"" + textBoxInfoName.Text + "\")>\n";
				source += "<Assembly: AssemblyDescription(\"" + textBoxInfoDescription.Text + "\")> \n";
				source += "<Assembly: AssemblyCompany(\"" + textBoxInfoAuthor.Text + "\")> \n";
				source += "<Assembly: AssemblyProduct(\"" + textBoxInfoName.Text + "\")> \n";
				source += "<Assembly: AssemblyCopyright(\"" + textBoxInfoAuthor.Text + "\")> \n";
				source += "<Assembly: AssemblyVersion(\"" + textBoxInfoVersion.Text + "\")> \n";
				source += "<Assembly: AssemblyFileVersion(\"" + textBoxInfoVersion.Text + "\")> ";
			}
			else
			{
				throw new Exception("Language not supported"); // Should never be reached
			}

			assemblyName = System.IO.Path.GetFileNameWithoutExtension(filename);
			if (String.IsNullOrEmpty(assemblyName)) // TODO: Remove as this is redundant because of diabled build button
				assemblyName = "NoName";
			assemblyName += ".dll";
			CompilerParameters compParams = new CompilerParameters();
			compParams.CompilerOptions = "/target:library /optimize";
			compParams.GenerateExecutable = true;
			compParams.OutputAssembly = "Robots/" + assemblyName;
			compParams.GenerateInMemory = false;
			compParams.IncludeDebugInformation = false;
			compParams.ReferencedAssemblies.Add("System.dll");
			compParams.ReferencedAssemblies.Add("MarvinsArena.Robot.dll");

			//CompilerResults results = compiler.CompileAssemblyFromSource(compParams, script);
			CompilerResults results = codeProvider.CompileAssemblyFromSource(compParams, ScintillaEditor.Text, source);

			if (results.Errors.Count > 0)
			{
				assemblyName = String.Empty;

				foreach (CompilerError error in results.Errors)
					ErrorList.Items.Add(/*error.FileName + " - " +*/error.Line + ": " + error.ErrorText);

				lastBuildSucceeded = false;
				MessageBox.Show("Error during build. See the Error List.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			} else {
				lastBuildSucceeded = true;
				MessageBox.Show("Build succeeded.\nFile: " + assemblyName, "Build", MessageBoxButton.OK, MessageBoxImage.Information);
			}

			UpdateButtons();
		}

		private void MenuItemDocu_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(new ProcessStartInfo("Documentation.chm"));
		}

		private void RunBattleEngine(string file)
		{
			if (String.IsNullOrEmpty(assemblyName))
			{
				MessageBox.Show("No assembly found. Please build the code.", "No Build", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			try
			{
				battleengineProcess = new Process();
				battleengineProcess.StartInfo.FileName = file;
				string args = assemblyName + " -notimeout -debug";
				battleengineProcess.StartInfo.Arguments = args;
				battleengineProcess.Start();
				battleengineProcess.WaitForExit();

				if (battleengineProcess.ExitCode != 0)
				{
					System.IO.StreamReader sr = new System.IO.StreamReader("BattleEngine.log");
					MessageBox.Show(sr.ReadToEnd());
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show("Error starting Battle Engine.\n" + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void MenuItemPrintPreview_Click(object sender, RoutedEventArgs e)
		{
			ScintillaEditor.Printing.PrintPreview();
		}

		private void MenuItemPrint_Click(object sender, RoutedEventArgs e)
		{
			ScintillaEditor.Printing.Print(true);
		}

		private void MenuItemUndo_Click(object sender, RoutedEventArgs e)
		{
			ScintillaEditor.UndoRedo.Undo();
		}

		private void MenuItemRedo_Click(object sender, RoutedEventArgs e)
		{
			ScintillaEditor.UndoRedo.Redo();
		}

		private void MenuItemCut_Click(object sender, RoutedEventArgs e)
		{
			ScintillaEditor.Commands.Execute(ScintillaNet.BindableCommand.Cut);
		}

		private void MenuItemCopy_Click(object sender, RoutedEventArgs e)
		{
			ScintillaEditor.Commands.Execute(ScintillaNet.BindableCommand.Copy);
		}

		private void MenuItemPaste_Click(object sender, RoutedEventArgs e)
		{
			ScintillaEditor.Commands.Execute(ScintillaNet.BindableCommand.Paste);
		}

		private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
		{
			ScintillaEditor.Commands.Execute(ScintillaNet.BindableCommand.DeleteBack);
		}
	}

	/*public class VersionInputRule : ValidationRule
	{
		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			string version;

			try
			{
				if (((string)value).Length > 0)
					version = ((string)value);
				else
					throw new Exception("Field was empty!");
			}
			catch (Exception e)
			{
				return new ValidationResult(false, e.Message);
			}

			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[0-9]+.[0-9]+.[0-9]*[.]*[0-9]*");
			if (!regex.IsMatch(version))
			{
				return new ValidationResult(false,
				  "Please enter a value in the form of major.minor[.build[.revision]]. (e.g. 1.0.0.0 or 1.2)");
			}
			else
			{
				return new ValidationResult(true, null);
			}
		}
	}*/
}
