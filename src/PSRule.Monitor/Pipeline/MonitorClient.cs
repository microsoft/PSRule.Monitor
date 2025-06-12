// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Management.Automation;
using System.Reflection;

namespace PSRule.Monitor.Pipeline;

internal abstract class MonitorClient : IDisposable
{
    // Track whether Dispose has been called.
    private bool _Disposed;

    protected static string GetPropertyValue(PSObject obj, string propertyName)
    {
        return obj.Properties[propertyName] == null || obj.Properties[propertyName].Value == null ? null : obj.Properties[propertyName].Value.ToString();
    }

    protected static Guid? GetPropertyGuid(PSObject obj, string propertyName)
    {
        var result = GetPropertyValue(obj, propertyName);
        if (result == null)
            return null;

        return Guid.Parse(result);
    }

    protected static T GetProperty<T>(PSObject obj, string propertyName)
    {
        return obj.Properties[propertyName] == null ? default : (T)obj.Properties[propertyName].Value;
    }

    protected static object GetProperty(object obj, string propertyName)
    {
        return TryProperty(obj, propertyName, out var value) ? value : null;
    }

    private static bool TryProperty(object obj, string propertyName, out object value)
    {
        value = null;
        var typeInfo = obj.GetType();
        if (obj is PSObject o && o.Properties[propertyName] != null)
        {
            value = o.Properties[propertyName].Value;
            return true;
        }
        else
        {
            var propertyInfo = typeInfo.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            if (propertyInfo != null)
            {
                value = propertyInfo.GetValue(obj);
                return true;
            }
        }
        return false;
    }

    protected static Hashtable GetPropertyMap(object o)
    {
        if (o == null)
            return null;

        var result = new Hashtable();
        if (o is IDictionary dictionary)
        {
            foreach (DictionaryEntry kv in dictionary)
            {
                if (HasValue(kv.Value))
                    result[kv.Key] = kv.Value;
            }
        }
        else if (o is PSObject pso)
        {
            foreach (var p in pso.Properties)
            {
                if (p.MemberType == PSMemberTypes.NoteProperty && HasValue(p.Value))
                    result[p.Name] = p.Value;
            }
        }
        return result.Count == 0 ? null : result;
    }

    private static bool HasValue(object value)
    {
        return !(value == null || (value is string s && string.IsNullOrEmpty(s)));
    }

    protected static string GetField(object o, string propertyName)
    {
        if (o is IDictionary dictionary && TryDictionary(dictionary, propertyName, out var value) && value != null)
            return value.ToString();

        if (o is PSObject pso)
            return GetPropertyValue(pso, propertyName);

        return null;
    }

    protected static bool TryDictionary(IDictionary dictionary, string key, out object value)
    {
        value = null;
        var comparer = StringComparer.OrdinalIgnoreCase;
        foreach (var k in dictionary.Keys)
        {
            if (comparer.Equals(key, k))
            {
                value = dictionary[k];
                return true;
            }
        }
        return false;
    }

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_Disposed)
        {
            if (disposing)
            {
                // Do nothing yet
            }
            _Disposed = true;
        }
    }

    #endregion IDisposable
}
