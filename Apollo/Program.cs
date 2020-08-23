using Microsoft.Extensions.Configuration;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Apollo
{
    class Program
    {
        private static readonly string _dataFileName = ".apollo";

        static IConfigurationRoot Configuration
            => new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        static void Main(string[] args)
        {
            var folders = GetDirectories();
            var taggedFiles = GetTaggedFiles(folders);


            
            var taggedFileData = GetTagData(taggedFiles);

            foreach (var data in taggedFileData)
                WriteDataFile(data.Key, data.Json);
        }

        private static IEnumerable<string> GetDirectories()
            => Configuration
                .GetSection("directories")
                .AsEnumerable()
                .Skip(1)
                .Select(f => f.Value);

        private static IEnumerable<TaggedFile> GetTaggedFiles(IEnumerable<string> folders)
            => folders
                .Where(d => Directory.Exists(d))
                .Select(d => new
                {
                    Directory = d,
                    DataFileName = Path.Combine(d, _dataFileName)
                })
                .GroupBy(d => File.Exists(d.DataFileName))
                .SelectMany(g => g.Key
                    ? g.SelectMany(d => ReadTagDataFromFile(d.DataFileName))
                    : g.SelectMany(d => BuildNewTagData(d.Directory)));

        private static IEnumerable<TaggedFile> ReadTagDataFromFile(string fileName)
            => JsonSerializer.Deserialize<IEnumerable<TaggedFile>>(File.ReadAllText(fileName));

        private static IEnumerable<TaggedFile> BuildNewTagData(string directory)
            => Directory.EnumerateFiles(
                    directory,
                    "*",
                    new EnumerationOptions()
                    {
                        RecurseSubdirectories = true
                    })
                .Select(f => new TaggedFile(f, directory));

        private static IEnumerable<(string Key, string Json)> GetTagData(IEnumerable<TaggedFile> taggedFiles)
            => taggedFiles
                .GroupBy(f => f.TopLevelDirectory)
                .Select(g =>
                (
                    g.Key,
                    Json: JsonSerializer.Serialize(g.ToList())
                ));

        private static void WriteDataFile(string folder, string json)
            => File.WriteAllText(
                Path.Combine(folder, _dataFileName),
                json);
    }
}
