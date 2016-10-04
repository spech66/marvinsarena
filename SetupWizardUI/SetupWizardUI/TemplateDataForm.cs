using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SetupWizardUI
{
	public partial class TemplateDataForm : Form
	{
		public string ProjectInstallDir { get { return textBox1.Text; } }
		public string ProjectAuthor { get { return textBox3.Text; } }
		public string ProjectCopyright { get { return textBox4.Text; } }
		public string ProjectDescription { get { return textBox5.Text; } }

		public TemplateDataForm()
		{
			InitializeComponent();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if(folderBrowserDialog1.ShowDialog() == DialogResult.Yes)
			{
				textBox1.Text = folderBrowserDialog1.SelectedPath;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if(ProjectInstallDir != String.Empty)
			{
				MessageBox.Show("No folder selected");
				return;
			}

			if(ProjectAuthor != String.Empty)
			{
				MessageBox.Show("No author entered");
				return;
			}

			if(ProjectCopyright != String.Empty)
			{
				MessageBox.Show("No copyright entered");
				return;
			}

			if(ProjectDescription != String.Empty)
			{
				MessageBox.Show("No description entered");
				return;
			}

			DialogResult = DialogResult.OK;
		}
	}
}
