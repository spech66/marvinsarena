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

namespace MarvinsArena
{
	/// <summary>
	/// Interaktionslogik für WindowAbout.xaml
	/// </summary>
	public partial class WindowAbout : Window
	{
		public WindowAbout()
		{
			InitializeComponent();
		}

		private void HyperlinkRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			string navigateUri = ((Hyperlink)(sender)).NavigateUri.ToString();
			Process.Start(new ProcessStartInfo(navigateUri));
			e.Handled = true;
		}
	}
}
