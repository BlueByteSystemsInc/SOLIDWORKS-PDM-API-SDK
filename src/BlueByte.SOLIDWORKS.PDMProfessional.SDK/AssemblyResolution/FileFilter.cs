//*********************************************************************
//xToolkit
//Copyright(C) 2023 Xarial Pty Limited
//Product URL: https://xtoolkit.xarial.com
//License: https://xtoolkit.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.XToolkit
{
    /// <summary>
    /// Represents the file fltering helper class
    /// </summary>
    public class FileFilter
    {
        /// <summary>
        /// Combines filters into a string which can be used in file prowsing
        /// </summary>
        /// <param name="filters">Filters</param>
        /// <returns>Combines string</returns>
        public static string BuildFilterString(params FileFilter[] filters)
        {
            return string.Join("|", filters.Select(f =>
            {
                var exts = string.Join(";", f.Extensions);
                return $"{f.Name} ({exts})|{exts}";
            }));
        }

        /// <summary>
        /// Creates new filter
        /// </summary>
        /// <param name="name">User friendly name of the filter</param>
        /// <param name="exts">List of extensions</param>
        /// <returns>Filter</returns>
        public static FileFilter Create(string name, params string[] exts) => new FileFilter(name, exts);

        /// <summary>
        /// All files filter
        /// </summary>
        public static FileFilter AllFiles { get; } = new FileFilter("All Files", "*.*");
        /// <summary>
        /// Image files filter
        /// </summary>
        public static FileFilter ImageFiles { get; } = new FileFilter("Image Files", "*.bmp", "*.png", "*.jpg", "*.jpeg", "*.gif", "*.tif", "*.tiff");

        /// <summary>
        /// Name of the filter
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Filter extensions with wildcards (e.g. *.txt)
        /// </summary>
        public string[] Extensions { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">User friendly name of the filter</param>
        /// <param name="exts">List of extensions</param>
        public FileFilter(string name, params string[] exts)
        {
            Name = name;
            Extensions = exts;
        }
    }
}
