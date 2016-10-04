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
using MarvinsArena.Core;

namespace MarvinsArena
{
	/// <summary>
	/// Interaction logic for WindowNew.xaml
	/// </summary>
	public partial class WindowNew : Window
	{
		private Tournament tournament;
		private List<RobotLoader> loaders;

		public Tournament Tournament { get { return tournament; } }

		public WindowNew(TournamentMode mode)
		{
			loaders = new List<RobotLoader>();
			tournament = new Tournament();
			tournament.Rules.Mode = mode;

			InitializeComponent();
			radioButtonEmptyMap.IsChecked = true;
			Title += String.Format(" - {0}", mode.ToString());
			
			// Assign values for robots
			PopulateRobotTree();

			numericTeamsize.MinValue = TournamentRules.TeamsizeMin;
			numericTeamsize.MaxValue = TournamentRules.TeamsizeMax;
			numericTeamsize.Value = TournamentRules.TeamsizeDefault;

			numericRounds.MinValue = TournamentRules.RoundsMin;
			numericRounds.MaxValue = TournamentRules.RoundsMax;
			numericRounds.Value = TournamentRules.RoundsDefault;

			numericHitpoints.MinValue = TournamentRules.HitpointsMin;
			numericHitpoints.MaxValue = TournamentRules.HitpointsMax;
			numericHitpoints.Value = TournamentRules.HitpointsDefault;

			numericRockets.MinValue = TournamentRules.MissilesMin;
			numericRockets.MaxValue = TournamentRules.MissilesMax;
			numericRockets.Value = TournamentRules.MissilesDefault;

			numericMaxHeat.MinValue = TournamentRules.MaxHeatMin;
			numericMaxHeat.MaxValue = TournamentRules.MaxHeatMax;
			numericMaxHeat.Value = TournamentRules.MaxHeatDefault;

			// Assign values for map
			numericMapSize.MinValue = 8;
			numericMapSize.MaxValue = 20;
			numericMapSize.Value = numericMapSize.MinValue;

			// Validation
			ValidateMode();
		}

		private void PopulateRobotTree()
		{
			TreeViewItem tree1Robot = new TreeViewItem();
			tree1Robot.Header = "Base Robot";
			tree1Robot.IsExpanded = true;
			treeView1.Items.Add(tree1Robot);
			TreeViewItem tree1EnhancedRobot = new TreeViewItem();
			tree1EnhancedRobot.Header = "Enhanced Robot";
			tree1EnhancedRobot.IsExpanded = true;
			treeView1.Items.Add(tree1EnhancedRobot);
			TreeViewItem tree1TeamRobot = new TreeViewItem();
			tree1TeamRobot.Header = "Team Robot";
			tree1TeamRobot.IsExpanded = true;
			treeView1.Items.Add(tree1TeamRobot);

			int tempTeamId = 1;
			string[] files = System.IO.Directory.GetFiles("Robots", "*.dll");
			foreach(string file in files)
			{
				// Don't load game files
				if(file.Contains("MarvinsArena"))
					continue;

				try
				{
					RobotLoader loader = new RobotLoader();
					loader.Load(file, 1, tempTeamId, tournament.Map.Map);
					tempTeamId++;
					
					TreeViewItem item = new TreeViewItem();
					item.Header = loader.RobotHost.AssemblyTitle;
					TreeViewRobotInfo tvrInfo = new TreeViewRobotInfo();
					tvrInfo.RobotLoader = loader;
					tvrInfo.FileName = file;
					item.Tag = tvrInfo;

					if(loader.RobotHost.RobotType == LoadedRobotType.Robot)
					{
						tree1Robot.Items.Add(item);
					} else if(loader.RobotHost.RobotType == LoadedRobotType.EnhancedRobot)
					{
						tree1EnhancedRobot.Items.Add(item);
					} else if(loader.RobotHost.RobotType == LoadedRobotType.TeamRobot)
					{
						tree1TeamRobot.Items.Add(item);
					}

					loaders.Add(loader);
				} catch (Exception e) {
					MessageBox.Show(e.Message, "Error loading robot", MessageBoxButton.OK, MessageBoxImage.Information);
					continue;
				}
			}
		}

		/// <summary>
		/// Checks if tournament rules teamsize matches tournament mode
		/// </summary>
		private void ValidateMode()
		{
			if(tournament.Rules.Mode == TournamentMode.LastManStanding ||
				tournament.Rules.Mode == TournamentMode.OneOnOne)
			{
				tournament.Rules.Teamsize = 1;
				numericTeamsize.Value = tournament.Rules.Teamsize;
				numericTeamsize.upButton.IsEnabled = false;
				numericTeamsize.downButton.IsEnabled = false;
			} else if(tournament.Rules.Mode == TournamentMode.TeamTwoOnTwo) {
				numericTeamsize.Value = tournament.Rules.Teamsize; // Internally set to two
				numericTeamsize.upButton.IsEnabled = false;
				numericTeamsize.downButton.IsEnabled = false;
			} else {
				numericTeamsize.Value = tournament.Rules.Teamsize;
				numericTeamsize.upButton.IsEnabled = true;
				numericTeamsize.downButton.IsEnabled = true;
			}
		}

		private void buttonAddRobot_Click(object sender, RoutedEventArgs e)
		{
			TreeViewItem item = (TreeViewItem)treeView1.SelectedItem;
			if(item == null || !item.Items.IsEmpty || item.Tag == null)
				return;

			TreeViewItem item2 = new TreeViewItem();
			item2.Header = item.Header;
			item2.Tag = item.Tag;
			treeView2.Items.Add(item2);
		}

		private void buttonRemoveRobot_Click(object sender, RoutedEventArgs e)
		{
			treeView2.Items.Remove(treeView2.SelectedItem);
		}

		private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			TreeViewItem item = (TreeViewItem)e.NewValue;
			
			// Is top level
			if(!item.Items.IsEmpty || item.Tag == null)
			{
				textBlockInfoName.Text = "";
				textBlockInfoVersion.Text = "";
				textBlockInfoAuthor.Text = "";
				textBlockInfoDescription.Text = "";
				return;
			}

			textBlockInfoName.Text = ((TreeViewRobotInfo)item.Tag).RobotLoader.RobotHost.AssemblyTitle;
			textBlockInfoVersion.Text = ((TreeViewRobotInfo)item.Tag).RobotLoader.RobotHost.AssemblyVersion;
			textBlockInfoAuthor.Text = ((TreeViewRobotInfo)item.Tag).RobotLoader.RobotHost.AssemblyCopyright;
			textBlockInfoDescription.Text = ((TreeViewRobotInfo)item.Tag).RobotLoader.RobotHost.AssemblyDescription;
		}

		private void buttonStart_Click(object sender, RoutedEventArgs e)
		{
			if (treeView2.Items.Count == 0)
			{
				MessageBox.Show("No robot was added. A tournament without robots would be very boring so please select at least two",
					"Not enough Robots", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}
			if(treeView2.Items.Count == 1)
			{
				MessageBox.Show("Only one robot was added. A tournament with just one robot would be very boring so please select at least a second",
					"Not enough Robots", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			foreach(TreeViewItem item in treeView2.Items)
			{
				tournament.RobotManager.AddRobot(((TreeViewRobotInfo)item.Tag).FileName,
														tournament.Rules.Teamsize,
														tournament.Map.Map);
			}

			if(tournament.Rules.Mode == TournamentMode.LastManStanding ||
				tournament.Rules.Mode == TournamentMode.TeamLastTeamStanding)
			{
				tournament.Bracket = new TournamentBracket(tournament.RobotManager.TeamCount, true);
			} else
			{
				tournament.Bracket = new TournamentBracket(tournament.RobotManager.TeamCount);
			}


			// --- Map ---
			byte[][] newMap;
			if(radioButtonMapFile.IsChecked == true)
			{
				try
				{
					WindowEditor.ReadMapFile(textBlockMapFile.Text, out newMap);
				} catch (Exception exception)
				{
					MessageBox.Show("Error opening file.\n" + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
			} else
			{
				int mapSize = (int)numericMapSize.Value;
				newMap = new byte[mapSize][];
				for(int i = 0; i < mapSize; i++)
				{
					newMap[i] = new byte[mapSize];
					for(int j = 0; j < mapSize; j++)
					{
						newMap[i][j] = 0;
					}
				}
				
			}
			tournament.Map = new TournamentMap(newMap);

			int maxRobots = tournament.RobotManager.TeamCount * tournament.Rules.Teamsize;
			if(tournament.Map.FreeFields < maxRobots)
			{
				MessageBox.Show("The map contains " + tournament.Map.FreeFields +
								" free fields but there will be " + maxRobots + " robots.\n" +
								"Please select a different map or reduce the number of robots.",
								"Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			DialogResult = true;
			Close();
		}

		private void buttonSelectMapFile_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.RestoreDirectory = true;
			dlg.DefaultExt = ".xmml";
			dlg.InitialDirectory = "Maps";
			dlg.Filter = "Map files (.mam)|*.mam";

			Nullable<bool> result = dlg.ShowDialog();
			if(result == true)
			{
				textBlockMapFile.Text = dlg.FileName;
				radioButtonMapFile.IsChecked = true;
			}
		}

		private void numericMapSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
		{
			radioButtonEmptyMap.IsChecked = true;
		}

		private void numericTeamsize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
		{
			tournament.Rules.Teamsize = (int)e.NewValue;
		}

		private void numericRounds_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
		{
			tournament.Rules.Rounds = (int)e.NewValue;
		}

		private void numericHitpoints_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
		{
			tournament.Rules.Hitpoints = (int)e.NewValue;
		}

		private void numericMaxHeat_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
		{
			tournament.Rules.MaxHeat = (int)e.NewValue;
		}

		private void numericRockets_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
		{
			tournament.Rules.Missiles = (int)e.NewValue;
		}
	}

	public class TreeViewRobotInfo
	{
		public RobotLoader RobotLoader { get; set; }
		public string FileName { get; set; }
	}

}
