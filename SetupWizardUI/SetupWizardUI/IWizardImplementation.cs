using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TemplateWizard;
using System.Windows.Forms;
using EnvDTE;

namespace SetupWizardUI
{
	public class IWizardImplementation : IWizard
	{
		private TemplateDataForm templateDataForm;

		// This method is called before opening any item that 
		// has the OpenInEditor attribute.
		public void BeforeOpeningFile(ProjectItem projectItem)
		{
		}

		public void ProjectFinishedGenerating(Project project)
		{
		}

		// This method is only called for item templates,
		// not for project templates.
		public void ProjectItemFinishedGenerating(ProjectItem projectItem)
		{
		}

		// This method is called after the project is created.
		public void RunFinished()
		{
		}

		public void RunStarted(object automationObject, 
			Dictionary<string, string> replacementsDictionary,
			WizardRunKind runKind, object[] customParams)
		{
			try
			{
				templateDataForm = new TemplateDataForm();
				if(templateDataForm.ShowDialog() == DialogResult.OK)
				{
					replacementsDictionary.Add("$projectinstalldir$", templateDataForm.ProjectInstallDir);
					replacementsDictionary.Add("$projectauthor$", templateDataForm.ProjectAuthor);
					replacementsDictionary.Add("$projectcopyright$", templateDataForm.ProjectCopyright);
					replacementsDictionary.Add("$projectdescription$", templateDataForm.ProjectDescription);
				}

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		// This method is only called for item templates,
		// not for project templates.
		public bool ShouldAddProjectItem(string filePath)
		{
			return true;
		}        
	}
}
