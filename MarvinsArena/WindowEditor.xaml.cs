using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
	/// Interaktionslogik für WindowEditor.xaml
	/// </summary>
	public partial class WindowEditor : Window
	{
		private string filename;
		private string baseTitle;
		private byte[][] tournamentMap;

		private Brush colorNormal, colorBlocked;
		private int iconSize;

		public WindowEditor()
		{
			InitializeComponent();
			baseTitle = Title;
			filename = "";

			colorNormal = Brushes.AliceBlue;
			colorBlocked = Brushes.Tomato;
			iconSize = 16;

			UpdateButtons();
		}

		private void UpdateButtons()
		{
			if (tournamentMap == null)
			{
				MenuItemSave.IsEnabled = false;
				MenuItemSaveAs.IsEnabled = false;
				ToolBarSave.IsEnabled = false;
			}
			else
			{
				MenuItemSave.IsEnabled = true;
				MenuItemSaveAs.IsEnabled = true;
				ToolBarSave.IsEnabled = true;
			}
		}

		public static void ReadMapFile(string filename, out byte[][] tournamentMap)
		{
			Stream stream = new FileStream(filename, System.IO.FileMode.Open);
			IFormatter formatter = new BinaryFormatter();
			tournamentMap = (byte[][])formatter.Deserialize(stream);
			stream.Close();
		}

		private void ReadMapFile(string filename)
		{
			ReadMapFile(filename, out tournamentMap);
		}

		private void WriteMapFile(string filename)
		{
			Stream stream = new FileStream(filename, System.IO.FileMode.Create);
			IFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, tournamentMap);
			stream.Close();
		}

		private Rectangle CreateRectangle(int x, int y, int sizeX, int sizeY, Brush color)
		{
			Rectangle rect = new Rectangle();
			rect.Opacity = 0.75;
			rect.Stroke = System.Windows.Media.Brushes.Black;
			rect.Fill = color;
			Canvas.SetLeft(rect, x);
			Canvas.SetTop(rect, y);
			rect.Width = sizeX;
			rect.Height = sizeY;

			rect.MouseDown += new MouseButtonEventHandler(rect_MouseDown);

			return rect;
		}

		private void rect_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Rectangle rs = (Rectangle)sender;
			Point p = e.GetPosition(Canvas1);

			int indexX = (int)Math.Floor(p.X / iconSize);
			int indexY = (int)Math.Floor(p.Y / iconSize);

			// Keep index in boundaries at the edges
			if(indexX >= tournamentMap.GetLength(0) || indexY >= tournamentMap[0].GetLength(0))
				return;

			if(rs.Fill == colorBlocked)
			{
				rs.Fill = colorNormal;
				tournamentMap[indexX][indexY] = 0;
			} else
			{
				rs.Fill = colorBlocked;
				tournamentMap[indexX][indexY] = 1;
			}
		}

		private void DrawMap()
		{
			Canvas1.Children.Clear();

			int mapWidth = tournamentMap.GetLength(0);
			int mapHeight = tournamentMap[0].GetLength(0);

			Canvas1.Width = mapWidth * iconSize;
			Canvas1.Height = mapHeight * iconSize;

			for(int x = 0; x < mapWidth; x++)
			{
				for(int y = 0; y < mapHeight; y++)
				{
					if(tournamentMap[x][y] == 0)
					{
						Canvas1.Children.Add(CreateRectangle(x * iconSize, y * iconSize, iconSize, iconSize, colorNormal));
					} else
					{
						Canvas1.Children.Add(CreateRectangle(x * iconSize, y * iconSize, iconSize, iconSize, colorBlocked));
					}
				}
			}
		}

		private void MenuItemNew_Click(object sender, RoutedEventArgs e)
		{
			WindowEditorNewDialog dlg = new WindowEditorNewDialog();
			Nullable<bool> result = dlg.ShowDialog();
			if(result == true)
			{
				tournamentMap = new byte[dlg.MapWidth][];
				for(int i = 0; i < dlg.MapWidth; i++)
				{
					tournamentMap[i] = new byte[dlg.MapHeight];
					for(int j = 0; j < dlg.MapHeight; j++)
					{
						tournamentMap[i][j] = 0;
					}
				}

				DrawMap();
				UpdateButtons();
			}
		}

		private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
				dlg.RestoreDirectory = true;
				dlg.DefaultExt = ".xmml";
				dlg.InitialDirectory = "Maps";
				dlg.Filter = "Map files (.mam)|*.mam";

				Nullable<bool> result = dlg.ShowDialog();
				if(result == true)
				{
					filename = dlg.FileName;
					ReadMapFile(filename);
					this.Title = baseTitle + " - " + System.IO.Path.GetFileNameWithoutExtension(filename);
					DrawMap();
					UpdateButtons();
				}
			} catch(Exception exception)
			{
				MessageBox.Show("Error opening file.\n" + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void MenuItemSave_Click(object sender, RoutedEventArgs e)
		{
			if(filename == String.Empty)
			{
				MenuItemSaveAs_Click(sender, e);
				return;
			}

			if(tournamentMap != null)
			{
				try
				{
					WriteMapFile(filename);
					MessageBox.Show("Map saved to file", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
				} catch(Exception exception)
				{
					MessageBox.Show("Error saving file.\n" + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			} else
			{
				MessageBox.Show("No map available!\nPlease create new.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void MenuItemSaveAs_Click(object sender, RoutedEventArgs e)
		{
			if(tournamentMap == null)
			{
				MessageBox.Show("No map available!\nPlease create new.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
			dlg.RestoreDirectory = true;
			dlg.DefaultExt = ".mam";
			dlg.InitialDirectory = "Maps";
			dlg.Filter = "Map files (.mam)|*.mam";

			Nullable<bool> result = dlg.ShowDialog();
			if(result == true)
			{
				filename = dlg.FileName;
				WriteMapFile(filename);
				this.Title = baseTitle + " - " + System.IO.Path.GetFileNameWithoutExtension(filename);
			}
		}

		private void MenuItemExit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
