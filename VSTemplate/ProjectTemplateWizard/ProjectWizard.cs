using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ProjectTemplateWizard
{
    public class ProjectWizard : IWizard
    {
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject,
            Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind,
            object[] customParams)
        {
            try
            {
                var window = new WizardWindow();
                window.ShowDialog();

                replacementsDictionary["$addinname$"] = window.AddInInfo.Name;
                replacementsDictionary["$addindescription$"] = window.AddInInfo.Description;
                replacementsDictionary["$addincompany$"] = window.AddInInfo.Company;
                replacementsDictionary["$addintask$"] = window.AddInInfo.IsTask.ToString().ToLower();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
