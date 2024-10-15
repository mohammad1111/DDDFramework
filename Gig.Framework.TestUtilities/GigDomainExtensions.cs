using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using FastMember;
namespace Gig.Framework.TestUtilities;

public static class GigDomainExtensions
{
    private static readonly IDictionary<Type, TypeAccessor> Accessors = new ConcurrentDictionary<Type, TypeAccessor>();

    public static T With<T, TFieldOrProperty>(this T instance, Expression<Func<T, TFieldOrProperty>> fieldOrProperty,
        TFieldOrProperty value)
        where T : class
    {
        if (instance == null)
            return null;

        if (!(fieldOrProperty.Body is MemberExpression member))
            throw new ArgumentException($"Expression '{fieldOrProperty}' is not for a property or field.");

        try
        {
            lock (Accessors)
            {
                if (!Accessors.TryGetValue(typeof(T), out var ta))
                    ta = Accessors[typeof(T)] = TypeAccessor.Create(typeof(T), true);

                if (ta[instance, member.Member.Name] != null)
                {
                    ta[instance, member.Member.Name] = value;
                    return instance;
                }
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }

        // fallback to reflection
        var fi = member.Member as FieldInfo;
        fi?.SetValue(instance, value);
        var pi = member.Member as PropertyInfo;
        pi?.SetValue(instance, value);
        return instance;
    }
}

