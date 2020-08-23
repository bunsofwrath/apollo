using System.Diagnostics;

namespace Apollo
{
    [DebuggerDisplay("{_category}:{_name}")]
    internal class Tag
    {
        public string Name { get; set;  }
        public string Category { get; set; }

        public Tag() // supports deserialization from JSON
        { }

        public Tag(string name, string category)
        {
            Name = name;
            Category = category;
        }
    }
}
