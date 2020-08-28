using Microsoft.Extensions.Configuration;
using MoreLinq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Apollo
{
    public class ApolloContext
    {
        private readonly string _dataFileName = ".apollo";

        public IEnumerable<TaggedFile> Files { get; }

        public ObservableCollection<Tag> Tags { get; }

        public ApolloContext(IConfiguration configuration)
        {
            var directories = configuration
                .GetSection("directories")
                .AsEnumerable()
                .Skip(1)
                .Select(f => f.Value);
            Files = GetTaggedFiles(directories)
                .ToList();
            Tags = new ObservableCollection<Tag>(
                Files
                .SelectMany(f => f.Tags)
                .Distinct());
        }

        private IEnumerable<TaggedFile> GetTaggedFiles(IEnumerable<string> folders)
            => folders
                .Where(d => Directory.Exists(d))
                .Select(d => new
                {
                    Directory = d,
                    DataFileName = Path.Combine(d, _dataFileName)
                })
                .GroupBy(d => File.Exists(d.DataFileName))
                .SelectMany(g => g.Key
                    ? g.SelectMany(d => ReadTagDataFromFile(d.Directory, d.DataFileName))
                    : g.SelectMany(d => BuildNewTagData(d.Directory)));

        private IEnumerable<TaggedFile> ReadTagDataFromFile(string directory, string fileName)
            => JsonSerializer.Deserialize<IEnumerable<TaggedFile>>(File.ReadAllText(fileName))
                .Join(
                    BuildNewTagData(directory),
                    f => f,
                    f => f,
                    (read, real) => read.ToDirectory(Path.GetDirectoryName(real.FilePath)),
                    new TaggedFileComparer());

        private IEnumerable<TaggedFile> BuildNewTagData(string directory)
            => Directory.EnumerateFiles(
                    directory,
                    "*",
                    new EnumerationOptions()
                    {
                        RecurseSubdirectories = true
                    })
                .Where(f => Path.GetFileName(f) != _dataFileName)
                .Select(f => new TaggedFile(f, directory));

        public void Save()
        {
            var taggedFileData = GetTagData(Files);

            foreach (var data in taggedFileData)
                WriteDataFile(data.Key, data.Json);
        }

        private IEnumerable<(string Key, string Json)> GetTagData(IEnumerable<TaggedFile> taggedFiles)
            => taggedFiles
                .GroupBy(f => f.TopLevelDirectory)
                .Select(g =>
                (
                    g.Key,
                    Json: JsonSerializer.Serialize(g.ToList())
                ));

        private void WriteDataFile(string folder, string json)
            => File.WriteAllText(
                Path.Combine(folder, _dataFileName),
                json);
    }
}
