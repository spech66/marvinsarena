using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Xml;

namespace MarvinsArena
{
	/// <summary>
	/// Interaktionslogik für WindowConfigureBattleEngine.xaml
	/// </summary>
	public partial class WindowConfigureBattleEngine : Window
	{
		public WindowConfigureBattleEngine()
		{
			InitializeComponent();

			EnumerateDisplay();
			LoadConfiguration("2DBattleEngine.exe.config", BE2DResolution, BE2DFullscreen, BE2DLight, null);
			LoadConfiguration("3DBattleEngine.exe.config", BE3DResolution, BE3DFullscreen, null, BE3DMultiSampling);
		}

		private void LoadConfiguration(string fileName, ComboBox box, CheckBox fullscreen, CheckBox light, CheckBox multiSampling)
		{
			string width = "800";
			string height = "600";
			//string color = "16";

			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);
			XmlNodeList objList = doc.GetElementsByTagName("setting");
			foreach(XmlNode node in objList)
			{
				if(node.Attributes["name"].Value == "PreferredBackBufferWidth")
					width = node.InnerText;
				if(node.Attributes["name"].Value == "PreferredBackBufferHeight")
					height = node.InnerText;
				if(node.Attributes["name"].Value == "IsFullScreen")
					fullscreen.IsChecked = Convert.ToBoolean(node.InnerText);

				if(light != null && node.Attributes["name"].Value == "LightEffects")
					light.IsChecked = Convert.ToBoolean(node.InnerText);
				if(multiSampling != null && node.Attributes["name"].Value == "PreferMultiSampling")
					multiSampling.IsChecked = Convert.ToBoolean(node.InnerText);
			}

			string resEntry = String.Format("{0}x{1}", width, height);// + "x" + color;
			int sel = -1;
			if(-1 != (sel = box.Items.IndexOf(resEntry)))
			{
				box.SelectedIndex = sel;
			} else {
				box.SelectedIndex = 1;
			}
		}

		private void SaveConfiguration(string fileName, ComboBox box, CheckBox fullscreen, CheckBox light, CheckBox multiSampling)
		{
			string[] resSplit = box.SelectedValue.ToString().Split(new char[] { 'x' });
			string width = resSplit[0];
			string height = resSplit[1];
			//string color = resSplit[2];

			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);

			XmlNodeList objList = doc.GetElementsByTagName("setting");
			foreach(XmlNode node in objList)
			{
				if(node.Attributes["name"].Value == "PreferredBackBufferWidth")
					node.InnerXml = String.Format("<value>{0}</value>", width);
				if(node.Attributes["name"].Value == "PreferredBackBufferHeight")
					node.InnerXml = String.Format("<value>{0}</value>", height);
				if(node.Attributes["name"].Value == "IsFullScreen")
					node.InnerXml = "<value>" + fullscreen.IsChecked + "</value>";

				if(light != null && node.Attributes["name"].Value == "LightEffects")
					node.InnerXml = "<value>" + light.IsChecked + "</value>";
				if(multiSampling != null && node.Attributes["name"].Value == "PreferMultiSampling")
					node.InnerXml = "<value>" + multiSampling.IsChecked + "</value>";
			}

			doc.Save(fileName);
		}

		[DllImport("user32.dll")]
		private static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);
		[DllImport("user32.dll")]
		private static extern bool EnumDisplaySettingsEx(string lpszDeviceName, uint iModeNum, out DEVMODE lpDevMode, uint dwFlags);

		private void EnumerateDisplay()
		{
			BE2DResolution.Items.Clear();
			BE3DResolution.Items.Clear();

			DEVMODE vDevMode = new DEVMODE();
			int i = 0;
			while(EnumDisplaySettings(null, i, ref vDevMode))
			{
				i++;

				if(vDevMode.dmBitsPerPel < 16)
					continue;

				StringBuilder sb = new StringBuilder();
				/*sb.AppendFormat("{0}x{1}x{2}", vDevMode.dmPelsWidth,
					vDevMode.dmPelsHeight, vDevMode.dmBitsPerPel);*/
				sb.AppendFormat("{0}x{1}", vDevMode.dmPelsWidth,
					vDevMode.dmPelsHeight);

				if(!BE2DResolution.Items.Contains(sb.ToString()))
				{
					BE2DResolution.Items.Add(sb.ToString());
					BE3DResolution.Items.Add(sb.ToString());
				}
			}

			BE2DResolution.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription());
			BE3DResolution.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription());
		}

		private void buttonSave_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				SaveConfiguration("2DBattleEngine.exe.config", BE2DResolution, BE2DFullscreen, BE2DLight, null);
				SaveConfiguration("3DBattleEngine.exe.config", BE3DResolution, BE3DFullscreen, null, BE3DMultiSampling);
				MessageBox.Show("Configuration saved", "Complete", MessageBoxButton.OK, MessageBoxImage.Information);
				Close();
			} catch(Exception exc)
			{
				MessageBox.Show(exc.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}			
		}
	}

	[StructLayout(LayoutKind.Sequential)]

	public struct DEVMODE
	{
		private const int CCHDEVICENAME = 0x20;
		private const int CCHFORMNAME = 0x20;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
		public string dmDeviceName;
		public short dmSpecVersion;
		public short dmDriverVersion;
		public short dmSize;
		public short dmDriverExtra;
		public int dmFields;
		public int dmPositionX;
		public int dmPositionY;
		public short dmDisplayOrientation;
		public int dmDisplayFixedOutput;
		public short dmColor;
		public short dmDuplex;
		public short dmYResolution;
		public short dmTTOption;
		public short dmCollate;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
		public string dmFormName;
		public short dmLogPixels;
		public int dmBitsPerPel;
		public int dmPelsWidth;
		public int dmPelsHeight;
		public int dmDisplayFlags;
		public int dmDisplayFrequency;
		public int dmICMMethod;
		public int dmICMIntent;
		public int dmMediaType;
		public int dmDitherType;
		public int dmReserved1;
		public int dmReserved2;
		public int dmPanningWidth;
		public int dmPanningHeight;
	}
}
