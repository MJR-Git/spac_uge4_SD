using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CshScript.Tests.TestUtilitys
{
    public class SlnPath
    {
        /// <summary>
        /// Taken from "https://stackoverflow.com/questions/19001423/getting-path-to-the-parent-folder-of-the-solution-file-using-c-sharp"
        /// </summary>
        /// <param name="currentPath"></param>
        /// <returns></returns>
        public static DirectoryInfo TryGetSolutionDirectoryInfo(string? currentPath = null)
        {
            var directory = new DirectoryInfo(
                currentPath ?? Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }
            return directory!;
        }
    }
}