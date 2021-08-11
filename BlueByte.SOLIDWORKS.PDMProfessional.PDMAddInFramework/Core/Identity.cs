namespace BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework
{
    /// <summary>
    /// Add-in identity.
    /// </summary>
    public struct Identity
    {
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
    }
}