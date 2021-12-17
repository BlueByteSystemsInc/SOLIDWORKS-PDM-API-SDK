using System;

namespace BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Attributes
{
    /// <summary>
    /// Minium supported PDM Version.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class RequiredVersionAttribute : Attribute
    {

        /// <summary>
        /// Gets or sets the major.
        /// </summary>
        /// <value>
        /// The major.
        /// </value>
        public int Major { get; set; }
        /// <summary>
        /// Gets or sets the minor.
        /// </summary>
        /// <value>
        /// The minor.
        /// </value>
        public int Minor { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredVersionAttribute"/> class.
        /// </summary>
        /// <param name="major">The major.</param>
        /// <param name="minor">The minor.</param>
        public RequiredVersionAttribute(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredVersionAttribute"/> class.
        /// </summary>
        /// <param name="PDMYear">The PDM year.</param>
        /// <param name="ServicePack">The service pack.</param>
        public RequiredVersionAttribute(Enums.Year_e PDMYear, Enums.ServicePack_e ServicePack)
        {
            Major = (int)PDMYear;
            Minor = (int)ServicePack;
        }
    }
}
