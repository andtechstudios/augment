using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Andtech.Augment
{

    public class AssemblyDefinitionAsset
    {
        public string Guid { get; set; }
        public string Path { get; set; }

        public string name;
        public string[] references = new string[0];

        public static AssemblyDefinitionAsset GUIDToPackage(string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var assembly = Read(path);
            assembly.Guid = guid;

            return assembly;
        }

        public static AssemblyDefinitionAsset Read(string path)
		{
            var text = File.ReadAllText(path);
            var assembly = JsonUtility.FromJson<AssemblyDefinitionAsset>(text);
            assembly.Path = path;
            return assembly;
        }
    }

    public class PackageManifest
	{
        public string Path { get; set; }

        public string version;
        public string name;
        public string displayName;
        public Dictionary<string, string> dependencies = new Dictionary<string, string>();

        public static PackageManifest Read(string path)
        {
            var text = File.ReadAllText(path);
            var packageManifest = JsonUtility.FromJson<PackageManifest>(text);
            var dependenciesMatch = Regex.Match(text, @"\""dependencies\"":\s*{(?<content>[^}]*)}");
            var referenceMatches = Regex.Matches(dependenciesMatch.Value, @"\""(?<key>.+)?\"":\s*\""(?<value>.+?)\""");
            packageManifest.Path = path;
            packageManifest.dependencies = referenceMatches.Cast<Match>().ToDictionary(x => x.Groups["key"].Value, x => x.Groups["value"].Value);

            return packageManifest;
		}
    }

    public class Package
    {
        public List<AssemblyDefinitionAsset> AssemblyDefinitions { get; set; }
        public PackageManifest Manifest { get; set; }

        public Package(PackageManifest manifest)
		{
            Manifest = manifest;
            var packageRoot = Path.GetDirectoryName(manifest.Path);
            AssemblyDefinitions = Directory.EnumerateFiles(packageRoot, "*.asmdef", SearchOption.AllDirectories)
                .Select(AssemblyDefinitionAsset.Read)
                .ToList();
            foreach (var assemblyDefinition in AssemblyDefinitions)
			{
                assemblyDefinition.Guid = AssetDatabase.AssetPathToGUID(assemblyDefinition.Path);
			}
		}
    }

    public static class Tools
    {

        [MenuItem("Custom/Tools/Synchronize Manifests")]
        public static void SynchronizeManifests()
		{
            var packageManifests = AssetDatabase.FindAssets("package")
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Where(x => Path.GetFileName(x) == "package.json")
                .Select(x => PackageManifest.Read(x))
                .ToList();
            var packages = packageManifests.Select(x => new Package(x)).ToList();

            var assemblies = packages
                .SelectMany(x => x.AssemblyDefinitions)
                .ToList();
            var developmentPackages = packages
                .Where(x => GetRootDirectory(x.Manifest.Path) == "Assets");

            foreach (var developmentPackage in developmentPackages)
            {
                var referencedAssemblies = developmentPackage.AssemblyDefinitions
                    .SelectMany(x => x.references)
                    .Select(ExtractGuid)
                    .Distinct()
                    .Select(x => assemblies.First(y => y.Guid == x));
                var referencedPackages = packages
                    .Where(x => x.AssemblyDefinitions.Any(y => referencedAssemblies.Any(z => z.Guid == y.Guid)));

                var lines = new List<string>();
                foreach (var package in referencedPackages)
                {
                    var targetPackage = packageManifests.First(x => x.name == package.Manifest.name);
                    var line = $"\"{targetPackage.name}\": \"{targetPackage.version}\"";
                    lines.Add(line);
                }

                var packageRoot = Path.GetDirectoryName(developmentPackage.Manifest.Path);
                var manifestPath = Path.Combine(packageRoot, "package.json");
                Rewrite(manifestPath, lines);

                Debug.Log($"Package dependencies updated for {manifestPath}!");
            }

            string ExtractGuid(string value)
			{
                return value.Replace("GUID:", string.Empty);
			}
        }

        private static string GetRootDirectory(string value)
		{
            var match = Regex.Match(value, @"^(?<directory>[^/\\]+)(/|\\)");
            if (!match.Success)
			{
                return string.Empty;
			}

            return match.Groups["directory"].Value;
		}

        private static void Rewrite(string manifestPath, List<string> dependencies)
        {
            var dependenciesRegex = new Regex(@"\""dependencies\"":\s*{(?<content>[^}]*)}");

            if (dependencies.Count == 0)
			{
                return;
			}

            var content = File.ReadAllText(manifestPath);
            var dependenciesMatch = dependenciesRegex.Match(content);
            if (dependenciesMatch.Success)
            {
                content = dependenciesRegex.Replace(content, GetJson("\t", string.Empty, string.Empty));
            }
            else
            {
                content = Regex.Replace(content, @"\s*}\s*$", @$",{Environment.NewLine}{GetJson("\t", "\t", "\n")}}}");
			}

            File.WriteAllText(manifestPath, content);

            string GetJson(string whitespace, string firstWhitespace = "\t", string finalWhitespace = "\n")
            {
                var writer = new StringWriter();
                writer.WriteLine($"{firstWhitespace}\"dependencies\": {{");
                writer.WriteLine($"{string.Join(", \n", dependencies.Select(x => $"{whitespace}\t{x}"))}");
                writer.Write($"{whitespace}}}{finalWhitespace}");

                return writer.ToString();
            }
        }
    }
}
