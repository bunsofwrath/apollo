using System;
using System.Diagnostics;

namespace Apollo
{
    [DebuggerDisplay("{Category}:{Name}")]
    public class Tag
    {
        public string Name { get; set;  }
        public string Category { get; set; }

        internal Tag() // supports deserialization from JSON
        { }

        public Tag(string name)
            : this(name, "")
        { }

        public Tag(string name, string category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tag name cannot be null or white space.");

            Name = name;
            Category = category ?? "";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (obj is Tag tag)
                return Equals(tag);

            return false;
        }

        public bool Equals(Tag other)
        {
            if (other == null)
                return false;

            return Name == other.Name
                && Category == other.Category;
        }

        public override int GetHashCode()
            => (Name, Category).GetHashCode();
    }
}
