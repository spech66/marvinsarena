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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using MarvinsArena.Core;

namespace MarvinsArena
{
	/// <summary>
	/// Interaction logic for WindowMain.xaml
	/// </summary>
	public partial class WindowMain : Window
	{
		private Tournament tournament;
		private string filename;
		private string baseTitle;
		private System.Diagnostics.Process battleengineProcess;

		public WindowMain()
		{
			//string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
			//System.IO.Directory.SetCurrentDirectory(path);

			InitializeComponent();
			baseTitle = Title;
			filename = "";
			UpdateButtons();
		}

		private void DrawTourney()
		{
			Canvas1.Children.Clear();

			if(tournament.Rules.Mode == TournamentMode.LastManStanding ||
				tournament.Rules.Mode == TournamentMode.TeamLastTeamStanding)
			{
				DrawTourneyFlat();
			} else
			{
				DrawTourneyTree();
			}
		}

		private void DrawTourneyFlat()
		{
			int player = tournament.Bracket.Root.Count;

			for(int i = 0; i < player; i++)
			{
				DrawTourneyTreeBrackets(tournament.Bracket.Root[i], 20, i*20, 260, 20);
			}

			Canvas1.Width = 200;
			Canvas1.Height = player * 20;
		}

		private void DrawTourneyTree()
		{
			if (tournament == null)
				return;
			
			int sizeX = 200;
			int sizeY = tournament.Bracket.Teams * 10;
			int width = tournament.Bracket.Rounds * sizeX - 180; // 200 - 20px margin
			int height = 2 * sizeY;
			DrawTourneyTreeBrackets(tournament.Bracket.FirstRoot, width, height, sizeX, sizeY);

			Canvas1.Width = tournament.Bracket.Rounds * sizeX - 20;
			Canvas1.Height = 4 * sizeY + 20;
		}

		private Rectangle CreateRectangle(int x, int y, int sizeX, int sizeY)
		{
			Rectangle rect = new Rectangle();
			rect.Opacity = 0.75;
			rect.Stroke = System.Windows.Media.Brushes.Black;
			rect.Fill = System.Windows.Media.Brushes.SkyBlue;
			Canvas.SetLeft(rect, x);
			Canvas.SetTop(rect, y);
			rect.Width = sizeX;
			rect.Height = sizeY;

			return rect;
		}

		private Line CreateLine(int x1, int y1, int x2, int y2)
		{
			Line line = new Line();
			line.Stroke = System.Windows.Media.Brushes.White;
			line.Opacity = 0.75;
			line.X1 = x1;
			line.Y1 = y1;
			line.X2 = x2;
			line.Y2 = y2;
			line.StrokeThickness = 2;

			return line;
		}

		private void DrawTourneyTreeBrackets(Bracket b, int x, int y, int sizeX, int sizeY)
		{
			Canvas1.Children.Add(CreateRectangle(x, y, sizeX - 60, 20));

			TextBlock textBlock = new TextBlock();
			textBlock.Foreground = Brushes.Black;
			if(b.Current - 1 < tournament.RobotManager.Teams.Count && b.Current > 0)
			{
				textBlock.Text = tournament.RobotManager.Teams[b.Current - 1].Robots[0].RobotHost.AssemblyTitle;
			} else
			{
				textBlock.Text = "";
			}
			textBlock.FontSize = 12;
			Canvas.SetLeft(textBlock, x + 10);
			Canvas.SetTop(textBlock, y + 2);
			Canvas1.Children.Add(textBlock);

			if(b.Left != null)
			{
				DrawTourneyTreeBrackets(b.Left, x - sizeX, y - sizeY, sizeX, sizeY / 2);
				Canvas1.Children.Add(CreateLine(x - (sizeX - 80) / 2, y - sizeY + 10, x - sizeX / 4 + 20, y - sizeY + 10)); //-+ left
				Canvas1.Children.Add(CreateLine(x - sizeX / 4 + 20, y + 10, x - sizeX / 4 + 20, y - sizeY + 10)); // | up
			}
			if(b.Left != null || b.Right != null)
			{
				Canvas1.Children.Add(CreateLine(x, y + 10, x - sizeX / 4 + 20, y + 10)); // +-----
			}
			if(b.Right != null)
			{
				DrawTourneyTreeBrackets(b.Right, x - sizeX, y + sizeY, sizeX, sizeY / 2);
				Canvas1.Children.Add(CreateLine(x - sizeX / 4 + 20, y + 10, x - sizeX / 4 + 20, y + sizeY + 10)); // | up
				Canvas1.Children.Add(CreateLine(x - (sizeX - 80) / 2, y + sizeY + 10, x - sizeX / 4 + 20, y + sizeY + 10)); //-+ left
			}
		}

		private void UpdateButtons()
		{
			if (filename == "" && tournament == null)
			{
				MenuItemSave.IsEnabled = false;
				MenuItemSaveAs.IsEnabled = false;
				MenuItemRun2DBattleEngine.IsEnabled = false;
				MenuItemRun3DBattleEngine.IsEnabled = false;
				ToolBarSave.IsEnabled = false;
				ToolBarRun2DBattleEngine.IsEnabled = false;
				ToolBarRun3DBattleEngine.IsEnabled = false;
			}
			else if (filename == "")
			{
				MenuItemSave.IsEnabled = true;
				MenuItemSaveAs.IsEnabled = true;
				ToolBarSave.IsEnabled = true;

				MenuItemRun2DBattleEngine.IsEnabled = false;
				MenuItemRun3DBattleEngine.IsEnabled = false;				
				ToolBarRun2DBattleEngine.IsEnabled = false;
				ToolBarRun3DBattleEngine.IsEnabled = false;
			}
			else
			{
				MenuItemSave.IsEnabled = true;
				MenuItemSaveAs.IsEnabled = true;
				MenuItemRun2DBattleEngine.IsEnabled = true;
				MenuItemRun3DBattleEngine.IsEnabled = true;
				ToolBarSave.IsEnabled = true;
				ToolBarRun2DBattleEngine.IsEnabled = true;
				ToolBarRun3DBattleEngine.IsEnabled = true;
			}
		}

		private void MenuItemExit_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void MenuItemNewTournament_Click(object sender, RoutedEventArgs e)
		{
			WindowNew windowNew;
			if (sender is System.Windows.Controls.Button)
			{
				windowNew = new WindowNew(TournamentMode.OneOnOne);
			}
			else
			{
				if (((MenuItem)sender).Name == OneOnOne.Name)
				{
					windowNew = new WindowNew(TournamentMode.OneOnOne);
				}
				else if (((MenuItem)sender).Name == LastManStanding.Name)
				{
					windowNew = new WindowNew(TournamentMode.LastManStanding);
				}
				else if (((MenuItem)sender).Name == TeamOnTeam.Name)
				{
					windowNew = new WindowNew(TournamentMode.TeamTwoOnTwo);
				}
				else if (((MenuItem)sender).Name == LastTeamStanding.Name)
				{
					windowNew = new WindowNew(TournamentMode.TeamLastTeamStanding);
				}
				else
				{
					throw new MarvinsArenaException("TournamentMode not defined!");
				}
			}

			if(true == windowNew.ShowDialog())
			{
				tournament = windowNew.Tournament;
				filename = "";
				this.Title = baseTitle;
				DrawTourney();
				UpdateButtons();
			}
		}

		private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
				dlg.RestoreDirectory = true;
				dlg.FileName = "Tournament";
				dlg.DefaultExt = ".xtml";
				dlg.InitialDirectory = "Tournaments";
				dlg.Filter = "Tournament files (.xtml)|*.xtml";

				Nullable<bool> result = dlg.ShowDialog();
				if (result == true)
				{
					filename = dlg.FileName;
					tournament = Tournament.ReadFromXml(filename);
					this.Title = baseTitle + " - " + System.IO.Path.GetFileNameWithoutExtension(filename);
					DrawTourney();
					UpdateButtons();
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show("Error opening file.\n" + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void MenuItemSave_Click(object sender, RoutedEventArgs e)
		{
			if (filename == String.Empty)
			{
				MenuItemSaveAs_Click(sender, e);
				return;
			}

			if (tournament != null)
			{
				try
				{
					tournament.SaveToXml(filename);
					UpdateButtons();
					MessageBox.Show("Tournament saved to file", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				catch (Exception exception)
				{
					MessageBox.Show("Error saving file.\n" + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			else
			{
				MessageBox.Show("No tournament available!\nPlease create new.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void MenuItemSaveAs_Click(object sender, RoutedEventArgs e)
		{
			if (tournament == null)
			{
				MessageBox.Show("No tournament available!\nPlease create new.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
			dlg.RestoreDirectory = true;
			dlg.FileName = "Tournament";
			dlg.DefaultExt = ".xtml";
			dlg.InitialDirectory = "Tournaments";
			dlg.Filter = "Tournament files (.xtml)|*.xtml";
			
			Nullable<bool> result = dlg.ShowDialog();
			if (result == true)
			{
				filename = dlg.FileName;
				tournament.SaveToXml(filename);
				this.Title = baseTitle + " - " + System.IO.Path.GetFileNameWithoutExtension(filename);
				UpdateButtons();
			}
		}

		private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
		{
			WindowAbout about = new WindowAbout();
			about.ShowDialog();
		}

		private void MenuItemDocu_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(new ProcessStartInfo("Documentation.chm"));
		}

		private void MenuItemConfigureBattleEngine_Click(object sender, RoutedEventArgs e)
		{
			WindowConfigureBattleEngine conf = new WindowConfigureBattleEngine();
			conf.ShowDialog();
		}

		private void MenuItemMapEditor_Click(object sender, RoutedEventArgs e)
		{
			WindowEditor editor = new WindowEditor();
			editor.ShowDialog();
		}

		private void MenuItemRun2DBattleEngine_Click(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			RunBattleEngine("2DBattleEngine.exe");
		}

		private void MenuItemRun3DBattleEngine_Click(object sender, RoutedEventArgs e)
		{
			RunBattleEngine("3DBattleEngine.exe");
		}

		private void RunBattleEngine(string file)
		{
			if(filename == String.Empty)
			{
				MessageBox.Show("No tournament available!\nPlease create new and save it.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			try
			{
				if(tournament.Bracket.Root.Count == 1 && tournament.Bracket.FirstRoot.NextBattle() == null)
				{
					MessageBox.Show("There is already a winner!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				battleengineProcess = new System.Diagnostics.Process();
				battleengineProcess.StartInfo.FileName = file;
				battleengineProcess.StartInfo.Arguments = "\"" + filename + "\"";
				battleengineProcess.Start();
				battleengineProcess.WaitForExit();
				DrawTourney();

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

		private void MenuItemCodeEditor_Click(object sender, RoutedEventArgs e)
		{
			WindowCodeEditor editor = new WindowCodeEditor();
			editor.ShowDialog();
		}
	}
}
