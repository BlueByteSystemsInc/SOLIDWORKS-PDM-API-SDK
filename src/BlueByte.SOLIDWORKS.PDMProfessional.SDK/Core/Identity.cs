using EPDM.Interop.epdm;

namespace BlueByte.SOLIDWORKS.PDMProfessional.SDK
{
    /// <summary>
    /// Add-in identity.
    /// </summary>
    public struct Identity
    {

        /// <summary>
        /// Converts to caption.
        /// </summary>
        public string ToCaption()
        {
            return $"{this.Name} V{this.Version}";
            
        }

        /// <summary>
        /// Converts to caption.
        /// </summary>
        /// <param name="specificWindowTitle">The specific window title.</param>
        public string ToCaption(string specificWindowTitle = "")
        {

            return $"{this.Name} V{this.Version} - {specificWindowTitle}";

        }


        /// <summary>
        /// Name
        /// </summary>
        public string Name;

        /// <summary>
        /// Description
        /// </summary>
        public string Description;

        /// <summary>
        /// Company name
        /// </summary>
        public string CompanyName;

        /// <summary>
        /// Add-in version
        /// </summary>
        public int Version;

        /// <summary>
        /// Required major version.
        /// </summary>
        public int RequiredMajorVersion;

        /// <summary>
        /// Required minor.
        /// </summary>
        public int RequiredMinorVersion;



        /// <summary>
        /// Gets or sets the vault.
        /// </summary>
        /// <value>
        /// The vault.
        /// </value>
        public IEdmVault5 Vault { get; set; }
    }
}