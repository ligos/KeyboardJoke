using System;
using Microsoft.SPOT;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Micro Framework doesn't define Extension attribute for extension methods. So define it ourselves!
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class ExtensionAttribute : Attribute
    {
        public ExtensionAttribute()
        {
        }
    }
}
