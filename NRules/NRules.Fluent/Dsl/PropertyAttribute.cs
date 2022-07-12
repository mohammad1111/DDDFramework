using System;

namespace NRules.Fluent.Dsl
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PropertyAttribute : Attribute
    {
        public PropertyAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public PropertyAttribute(string name, string description, string value)
        {
            Name = name;
            Value = value;
            Description = description;
        }

        internal string Name { get; }

        internal string Value { get; }

        internal  string Description { get; set; }
    }
}