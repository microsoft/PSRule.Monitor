// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Management.Automation;
using System.Net;
using System.Security;

namespace PSRule.Monitor
{
    /// <summary>
    /// A parameter transformation attribute for converting a string to a secure string.
    /// </summary>
    public sealed class SecureStringAttribute : ArgumentTransformationAttribute
    {
        public SecureStringAttribute()
            : base() { }

        public override bool TransformNullOptionalParameters => false;

        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            return TrySecureString(inputData, out SecureString s) || (inputData is PSObject pso && TrySecureString(pso, out s)) ? s : null;
        }

        private static bool TrySecureString(object o, out SecureString value)
        {
            value = null;
            if (o is string s)
            {
                value = new NetworkCredential("na", s).SecurePassword;
                return true;
            }
            return false;
        }
    }
}
