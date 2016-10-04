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

namespace MarvinsArena
{
	/// <summary>
	/// Interaktionslogik für WindowEditorNewDialog.xaml
	/// </summary>
	public partial class WindowEditorNewDialog : Window
	{
		public int MapHeight;
		public int MapWidth;

		public WindowEditorNewDialog()
		{
			InitializeComponent();

			numericSizeHeight.MinValue = 8;
			numericSizeHeight.MaxValue = 20;
			numericSizeHeight.Value = numericSizeHeight.MinValue;

			numericSizeWidth.MinValue = 8;
			numericSizeWidth.MaxValue = 20;
			numericSizeWidth.Value = numericSizeWidth.MinValue;
		}

		private void buttonNew_Click(object sender, RoutedEventArgs e)
		{
			MapHeight = (int)numericSizeHeight.Value;
			MapWidth = (int)numericSizeWidth.Value;
			DialogResult = true;
		}
	}
}
