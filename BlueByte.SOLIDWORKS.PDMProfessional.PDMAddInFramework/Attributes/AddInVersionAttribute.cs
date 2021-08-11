using System;

namespace BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Attributes
{

    public class AddInVersionAttribute : Attribute
    {
        /// <summary>
        /// Use this option for frequent builds.
        /// </summary>
        public bool UseAssemblyFileRevision { get; set; }

        /// <summary>
        /// Specific version.  
        /// </summary>
        public int Version { get; set; }
        public AddInVersionAttribute(bool useAssemblyFileRevision, int version = 0)
        {
            UseAssemblyFileRevision = useAssemblyFileRevision;
            Version = version;
        }
    }

    /// <summary>
    /// Use this to make an add-in a task
    /// </summary>
    public class IsTaskAttribute : Attribute
    {

        /// <summary>
        /// Gets or sets whether the add-in is a custom task.
        /// </summary>
        public bool IsTask { get; set; }

       
        public IsTaskAttribute(bool isTask)
        {
            IsTask = isTask;

        }
    }
}
