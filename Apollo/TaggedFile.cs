using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Apollo
{
    [DebuggerDisplay("{FileName}")]
    public class TaggedFile
    {
        public byte[] MD5Hash { get; set; }

        public string FilePath { get; set; }

        public string TopLevelDirectory { get; set; }

        public string FileName
            => Path.GetFileName(FilePath);

        public ObservableCollection<Tag> Tags { get; set; }

        internal TaggedFile() // support deserialization from JSON
        { }

        public TaggedFile(string filePath, string topLevelDirectory)
        {
            // TODO: validate topLevelFolder against filePath

            FilePath = filePath;
            TopLevelDirectory = topLevelDirectory;

            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(FilePath))
                MD5Hash = md5.ComputeHash(stream);

            Tags = new ObservableCollection<Tag>();
        }

        private TaggedFile(string filePath, string topLevelDirectory, byte[] md5Hash, IEnumerable<Tag> tags)
        {
            FilePath = filePath;
            TopLevelDirectory = topLevelDirectory;
            MD5Hash = md5Hash;
            Tags = new ObservableCollection<Tag>(tags);
        }

        public TaggedFile ToDirectory(string directory)
            => new TaggedFile(
                Path.Combine(directory, FileName),
                TopLevelDirectory,
                MD5Hash,
                Tags);
    }
}
