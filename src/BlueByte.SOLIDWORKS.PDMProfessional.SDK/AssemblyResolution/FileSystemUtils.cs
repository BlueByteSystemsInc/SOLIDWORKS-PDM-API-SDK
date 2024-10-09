//*********************************************************************
//xToolkit
//Copyright(C) 2023 Xarial Pty Limited
//Product URL: https://xtoolkit.xarial.com
//License: https://xtoolkit.xarial.com/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Xarial.XToolkit
{
    public static class FileSystemUtils
    {
        private static readonly Lazy<char[]> m_IllegalChars = new Lazy<char[]>(() => Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars()).ToArray());

        /// <summary>
        /// Combines the directory paths
        /// </summary>
        /// <param name="srcPath">Start path</param>
        /// <param name="additionalPaths">Additional path parts</param>
        /// <returns>Combined path</returns>
        /// <remarks>This method works with relative path, including moving the upper folders via ..</remarks>
        public static string CombinePaths(string srcPath, params string[] additionalPaths) 
        {
            var pathParts = new List<string>();

            var addedRoot = "";

            if (!Path.IsPathRooted(srcPath))
            {
                addedRoot = @"C:\";
                pathParts.Add(Path.Combine(addedRoot, srcPath));
            }
            else 
            {
                pathParts.Add(srcPath);
            }

            foreach (var path in additionalPaths) 
            {
                pathParts.Add(path.TrimStart('\\'));
            }

            var combinedPath = new Uri(Path.Combine(pathParts.ToArray())).LocalPath;

            if (!string.IsNullOrEmpty(addedRoot))
            {
                combinedPath = combinedPath.Substring(addedRoot.Length);
            }

            return combinedPath;
        }

        /// <summary>
        /// Excludes all sub level folders and only returns top level folders
        /// </summary>
        /// <param name="paths">Input directory paths</param>
        /// <returns>Top level folders paths</returns>
        public static string[] GetTopFolders(IEnumerable<string> paths)
        {
            bool IsSameOrInDirectory(string thisDir, string parentDir)
                => NormalizeDirectoryPath(thisDir).StartsWith(NormalizeDirectoryPath(parentDir),
                    StringComparison.CurrentCultureIgnoreCase);

            var result = new List<string>();

            foreach (var path in paths.OrderBy(p => p))
            {
                if (!result.Any(r => IsSameOrInDirectory(path, r)))
                {
                    result.Add(path);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Checks if the specified path is in the other directory
        /// </summary>
        /// <param name="thisPath">Path to check</param>
        /// <param name="parentDir">Directory to check agains</param>
        /// <returns>True of directory is within another directory</returns>
        public static bool IsInDirectory(string thisPath, string parentDir)
        {
            return thisPath.StartsWith(NormalizeDirectoryPath(parentDir),
                    StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Finds the relative path
        /// </summary>
        /// <param name="thisPath">Path to get relative path for</param>
        /// <param name="relativeToDir">Relative directory</param>
        /// <returns>Relative path</returns>
        /// <exception cref="Exception"></exception>
        public static string GetRelativePath(string thisPath, string relativeToDir) 
        {
            relativeToDir = NormalizeDirectoryPath(relativeToDir);

            if (IsInDirectory(thisPath, relativeToDir))
            {
                return thisPath.Substring(relativeToDir.Length);
            }
            else 
            {
                throw new Exception($"'{relativeToDir}' is not in the '{thisPath}' directory");
            }
        }

        /// <summary>
        /// Opens file explorer at the specified folder
        /// </summary>
        /// <param name="path"></param>
        public static void BrowseFolderInExplorer(string path) 
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        /// <summary>
        /// Opens file explorer and selects specified file
        /// </summary>
        /// <param name="path"></param>
        public static void BrowseFileInExplorer(string path)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "explorer.exe",
                UseShellExecute = true,
                Arguments = $"/select, \"{path}\""
            });
        }

        /// <summary>
        /// Replaces illegal characters in the relative file path (rooted path is not supported)
        /// </summary>
        /// <param name="path">Input path</param>
        /// <param name="replacer">Illegal characters replacer</param>
        /// <returns>Legal file path</returns>
        public static string ReplaceIllegalRelativePathCharacters(string path, Func<char, char> replacer) 
        {
            if (string.IsNullOrEmpty(path)) 
            {
                throw new ArgumentNullException(nameof(path));
            }

            var res = new StringBuilder();

            foreach (var pathChar in path) 
            {
                if (pathChar != Path.DirectorySeparatorChar && m_IllegalChars.Value.Contains(pathChar))
                {
                    res.Append(replacer.Invoke(pathChar));
                }
                else
                {
                    res.Append(pathChar);
                }
            }

            return res.ToString();
        }

        private static string NormalizeDirectoryPath(string path)
        {
            if (!path.EndsWith("\\"))
            {
                return path + "\\";
            }
            else
            {
                return path;
            }
        }
    }
}
