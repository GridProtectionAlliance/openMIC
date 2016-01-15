using System.Collections.Generic;
using System.IO;
using System.Linq;
using GSF;
using GSF.IO;

namespace WixFolderGen
{
    class Program
    {
        static void Main(string[] args)
        {
            const string SourceFolder = "..\\..\\Web\\";
            const string DestinationFile = "WixCode.wxs";

            List<string> folderList = GetFolderList(SourceFolder);
            List<string> componentGroupRefTags = GetComponentRefTags(folderList);
            List<string> directoryTags = GetDirectoryTags(folderList);
            List<string> componentGroupTags = GetComponentGroupTags(SourceFolder, folderList);

            using (FileStream stream = File.Create(DestinationFile))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine("<Feature Id=\"WebFilesFeature\" Title=\"Web Files\" Description=\"Web Files\">");

                foreach (string tag in componentGroupRefTags)
                    writer.WriteLine("  " + tag);

                writer.WriteLine("</Feature>");
                writer.WriteLine();

                writer.WriteLine("<Directory Id=\"ROOTFOLDER\" Name=\"WebFiles\">");

                foreach (string tag in directoryTags)
                    writer.WriteLine("  " + tag);

                writer.WriteLine("</Directory>");
                writer.WriteLine();

                foreach (string tag in componentGroupTags)
                    writer.WriteLine(tag);
            }
        }

        private static List<string> GetFolderList(string path)
        {
            List<string> folderList = new List<string>();
            BuildFolderList(folderList, path, string.Empty);
            return folderList;
        }

        private static List<string> GetComponentRefTags(List<string> folderList)
        {
            return new string[] { "root" }
                .Concat(folderList)
                .Select(folder => folder.Replace(Path.DirectorySeparatorChar, '_').Replace('-', '_'))
                .Select(folder => string.Format("<ComponentGroupRef Id=\"{0}Components\" />", folder))
                .ToList();
        }

        private static List<string> GetDirectoryTags(List<string> folderList)
        {
            List<string> directoryTags = new List<string>();

            List<string[]> brokenFolderList = folderList
                .Select(FilePath.RemovePathSuffix)
                .Select(folder => folder.Split(Path.DirectorySeparatorChar))
                .ToList();

            BuildDirectoryTags(directoryTags, brokenFolderList, 0);

            return directoryTags;
        }

        private static List<string> GetComponentGroupTags(string path, List<string> folderList)
        {
            List<string> componentGroupTags = new List<string>();

            componentGroupTags.Add("<ComponentGroup Id=\"rootComponents\" Directory=\"ROOTFOLDER\">");

            foreach (string file in Directory.EnumerateFiles(path))
            {
                string fileName = FilePath.GetFileName(file);
                string fileID = fileName.RemoveWhiteSpace().Replace('-', '_').Replace("(", "").Replace(")", "");
                string fileSource = Path.Combine("$(var.SolutionDir)", "WebFiles", fileName);
                string componentID = fileID.Replace('.', '_');

                componentGroupTags.Add(string.Format("  <Component Id=\"{0}\">", componentID));
                componentGroupTags.Add(string.Format("    <File Id=\"{0}\" Name=\"{1}\" Source=\"{2}\" />", fileID, fileName, fileSource));
                componentGroupTags.Add("  </Component>");
            }

            componentGroupTags.Add("</ComponentGroup>");

            foreach (string folder in folderList)
            {
                string groupID = folder.Replace(Path.DirectorySeparatorChar, '_').Replace('-', '_');
                string directory = folder.Replace(Path.DirectorySeparatorChar.ToString(), "").Replace('-', '_').ToUpper();

                componentGroupTags.Add(string.Format("<ComponentGroup Id=\"{0}Components\" Directory=\"{1}FOLDER\">", groupID, directory));

                foreach (string file in Directory.EnumerateFiles(Path.Combine(path, folder)))
                {
                    string fileName = FilePath.GetFileName(file);

                    if (!fileName.Equals("thumbs.db", System.StringComparison.OrdinalIgnoreCase))
                    {
                        string fileID = (folder.Replace(Path.DirectorySeparatorChar, '_') + "_" + fileName).RemoveWhiteSpace().Replace('-', '_').Replace("(", "").Replace(")", "");
                        string fileSource = Path.Combine("$(var.SolutionDir)", "WebFiles", folder, fileName);
                        string componentID = fileID.Replace('.', '_');

                        componentGroupTags.Add(string.Format("  <Component Id=\"{0}\">", componentID));
                        componentGroupTags.Add(string.Format("    <File Id=\"{0}\" Name=\"{1}\" Source=\"{2}\" />", fileID, fileName, fileSource));
                        componentGroupTags.Add("  </Component>");
                    }
                }

                componentGroupTags.Add("</ComponentGroup>");
            }

            return componentGroupTags;
        }

        private static void BuildFolderList(List<string> folderList, string path, string rootPath)
        {
            string name;

            foreach (string folder in Directory.EnumerateDirectories(path))
            {
                name = FilePath.AddPathSuffix(rootPath + FilePath.GetLastDirectoryName(FilePath.AddPathSuffix(folder)));
                folderList.Add(name);
                BuildFolderList(folderList, folder, name);
            }
        }

        private static void BuildDirectoryTags(List<string> directoryTags, List<string[]> folderList, int level)
        {
            List<IGrouping<string, string[]>> groupings = folderList
                .Where(folder => folder.Length > level)
                .GroupBy(folder => string.Join(Path.DirectorySeparatorChar.ToString(), folder.Take(level + 1)))
                .OrderBy(grouping => grouping.Key)
                .ToList();

            foreach (IGrouping<string, string[]> grouping in groupings)
            {
                if (grouping.Count() == 1)
                {
                    string[] folder = grouping.First();
                    string id = string.Join("", folder).Replace('-', '_').ToUpper();
                    string name = FilePath.GetLastDirectoryName(FilePath.AddPathSuffix(string.Join(Path.DirectorySeparatorChar.ToString(), folder)));

                    directoryTags.Add(string.Format("{0}<Directory Id=\"{1}FOLDER\" Name=\"{2}\" />", new string(' ', level * 2), id, name));
                }
                else
                {
                    List<string[]> subfolderList = grouping
                        .Where(folder => folder.Length > level + 1)
                        .ToList();

                    string id = grouping.Key.Replace(Path.DirectorySeparatorChar.ToString(), "").Replace('-', '_').ToUpper();
                    string name = FilePath.GetLastDirectoryName(FilePath.AddPathSuffix(grouping.Key));

                    directoryTags.Add(string.Format("{0}<Directory Id=\"{1}FOLDER\" Name=\"{2}\">", new string(' ', level * 2), id, name));
                    BuildDirectoryTags(directoryTags, subfolderList, level + 1);
                    directoryTags.Add(string.Format("{0}</Directory>", new string(' ', level * 2)));
                }
            }
        }
    }
}
