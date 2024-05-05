>[!WARNING]
> If you are viewing this page directly, we highly recommend you go back to the previous pages in this guide and review any missed steps. 

# Building the project

- Locate the add-in project in the *Solution Explorer* and select it.
- By default, the Debug configuration will be selected. This information will be useful later in locating the output files.
- Right-click on the project and click *Properties*.
- Go the *Build* tab (for csproj ie: C# users)
- Check *Register for COM interop*
- Right-click on the project in the solution explorer and click *Build*
- If the project has built successfully, Visual Studio will print Build succeeded in the lower left side of the window.


<img src="../images/build_succeeded.png"/>

<Enter>

> [!NOTE]
> If you are a VB.NET user, the *Register for com interop* checkbox will be in the *Compile* tab.  

# Output files

- Navigate to the bin/debug folder found in the project's folder. 
- To locate the project folder, right-click on the project in the *Solution Explorer* and click *Open Folder in File Explorer*.
- The bin/debug will contain the: 
    - the dll of the add-in. 
    - the dlls of PDMSDK.
    - Other dependencies you may have installed.

<img src="../images/outputfiles.png" />
 
<Enter>

>[!NOTE]
> We did not list the dll names of PDMSDK, because they are subject to change. We're working on reducing the number of dependencies. The most important dll that you will always have is called BlueByte.SOLIDWORKS.PDM.Professional.SDK.


