using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using System.Collections.Generic;
using System.IO;

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
            var window = new WizardWindow();

            if (window.ShowDialog() == true)
            {
                replacementsDictionary["$addinname$"] = window.AddInInfo.Name;
                replacementsDictionary["$addindescription$"] = window.AddInInfo.Description;
                replacementsDictionary["$addincompany$"] = window.AddInInfo.Company;
                replacementsDictionary["$addintask$"] = window.AddInInfo.IsTask.ToString().ToLower();
            }
            else
            {
                Directory.Delete(replacementsDictionary["$solutiondirectory$"], true);
                throw new WizardCancelledException();
            }
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
