using System.Reflection;

namespace Gig.Framework.Core.Models;

public abstract class GigValueType : IEquatable<GigValueType>
{
    private List<FieldInfo> _fields;
    private List<PropertyInfo> _properties;

    public bool Equals(GigValueType obj)
    {
        return Equals(obj as object);
    }

    public static bool operator ==(GigValueType obj1, GigValueType obj2)
    {
        if (Equals(obj1, null))
        {
            if (Equals(obj2, null)) return true;

            return false;
        }

        return obj1.Equals(obj2);
    }

    public static bool operator !=(GigValueType obj1, GigValueType obj2)
    {
        return !(obj1 == obj2);
    }

    public override bool Equals(object obj)
    {
        // if (ReferenceEquals(this, obj)) return true;
        // if (ReferenceEquals(null, obj)) return false;
        // if (GetType() != obj.GetType()) return false;
        // var other = obj as GigValueType;
        // return other != null && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        
        
        
        if (obj == null || GetType() != obj.GetType()) return false;

        return GetProperties().All(p => PropertiesAreEqual(obj, p))
               && GetFields().All(f => FieldsAreEqual(obj, f));
    }

    private bool PropertiesAreEqual(object obj, PropertyInfo p)
    {
        return Equals(p.GetValue(this, null), p.GetValue(obj, null));
    }

    private bool FieldsAreEqual(object obj, FieldInfo f)
    {
        return Equals(f.GetValue(this), f.GetValue(obj));
    }

    private IEnumerable<PropertyInfo> GetProperties()
    {
        if (_properties == null)
            _properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => !Attribute.IsDefined(p, typeof(IgnoreMemberFromEqualityAttribute))).ToList();

        return _properties;
    }

    private IEnumerable<FieldInfo> GetFields()
    {
        if (_fields == null)
            _fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(f => !Attribute.IsDefined(f, typeof(IgnoreMemberFromEqualityAttribute))).ToList();

        return _fields;
    }

    public override int GetHashCode()
    {
        unchecked //allow overflow
        {
            var hash = 17;
            foreach (var prop in GetProperties())
            {
                var value = prop.GetValue(this, null);
                hash = HashValue(hash, value);
            }

            foreach (var field in GetFields())
            {
                var value = field.GetValue(this);
                hash = HashValue(hash, value);
            }

            return hash;
        }
    }

    private int HashValue(int seed, object value)
    {
        var currentHash = value != null
            ? value.GetHashCode()
            : 0;

        return seed * 23 + currentHash;
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class IgnoreMemberFromEqualityAttribute : Attribute
{
}