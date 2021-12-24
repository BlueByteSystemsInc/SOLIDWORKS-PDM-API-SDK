# VS Project Template Extension

## Solution projects
**ProjectTemplateWizard** - WPF project that contains a window to display before creating a new project. Please see [here](https://docs.microsoft.com/en-us/visualstudio/extensibility/how-to-use-wizards-with-project-templates) and [here](https://docs.microsoft.com/en-us/visualstudio/sharepoint/walkthrough-creating-a-site-column-project-item-with-a-project-template-part-2) to get know more.

**SOLIDWORKSPDMAddIn** - sample project for add-in. Project template is based on this project.

**VSExtension** - VSIX project to package Visual Studio extension. Build the project in Release mode and publish `Release/VSExtension.vsix`.

## VSIX Manifest

The `source.extension.vsixmanifest` in the `VSExtension` project describes VS extension.

**Metadata** tab contains such fields as Product Name, Version, Icon and so on.

**Install Targets** is set up to support both VS 2019 and VS 2022.

**Assets** specifies the following:
- `SOLIDWORKSPDMAddIn.zip` project template
- `SOLIDWORKSPDMTaskSetupPage.zip`item template
- `ProjectTemplateWizard` assembly to show a window after the user clicks on create project

## How to create Visual Studio Project Template

1. Open `VSTemplate/SOLIDWORKSPDMAddIn.sln` solution
2. Click on **Project** - **Export Template** and generate project template file of `SOLIDWORKSPDMAddIn` project
3. Extract the generated .zip file to a folder
4. Replace the following values in `AddIn.cs` file:
    - `Name` attribute -> `[Name("$addinname$")]` to substitute a value that user specifies in project wizard
    - `Description` attribute -> `[Description("$addindescription$")]`
    - `CompanyName` attribute -> `[CompanyName("$addincompany$")]`
    - `IsTask` attribute -> `[IsTask($addintask$)]`
    - `Guid` attribute -> `[Guid("$guid1$")]` to always generate a new GUID when new project is created
5. Modify `MyTemplate.vstemplate` file:
    - Set appropriate values in `TemplateData` section
    - Add the next code after `TemplateContent` section, to show a UI for the user before creating a new project:
        ```
      <WizardExtension>
            <Assembly>ProjectTemplateWizard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=77753319f51656db</Assembly>
            <FullClassName>ProjectTemplateWizard.ProjectWizard</FullClassName>
      </WizardExtension>
        ```
6. Compress all the folder to archive `SOLIDWORKSPDMAddIn.zip`
7. Move the archive to the top directory of the repository
 