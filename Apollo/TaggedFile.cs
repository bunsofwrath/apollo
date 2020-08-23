using System.Collections.Generic;
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

        private IEnumerable<Tag> _tags;
        public IEnumerable<Tag> Tags
        {
            get => _tags.ToList();
            set => _tags = value;
        }

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

            _tags = new List<Tag>();
        }

        private TaggedFile(string filePath, string topLevelDirectory, byte[] md5Hash, IEnumerable<Tag> tags)
        {
            FilePath = filePath;
            TopLevelDirectory = topLevelDirectory;
            MD5Hash = md5Hash;
            _tags = tags;
        }

        public TaggedFile AddTag(Tag tag)
            => new TaggedFile(FilePath, TopLevelDirectory, MD5Hash, _tags.Append(tag));

        public TaggedFile ToDirectory(string directory)
            => new TaggedFile(
                Path.Combine(directory, FileName),
                TopLevelDirectory,
                MD5Hash,
                Tags);
    }
}
