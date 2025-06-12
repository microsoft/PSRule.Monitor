// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Globalization;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace PSRule.Monitor.Pipeline;

internal sealed class CollectionHash : IDisposable
{
    private readonly string _WorkspaceId;
    private readonly HMACSHA256 _Algorithm;

    private static readonly CultureInfo FormatCulture = new("en-US");

    internal CollectionHash(string workspaceId, SecureString sharedKey)
    {
        _WorkspaceId = workspaceId;
        _Algorithm = new HMACSHA256(Convert.FromBase64String(new NetworkCredential(string.Empty, sharedKey).Password));
    }

    internal string ComputeSignature(int length, DateTime date, string contentType)
    {
        var challenge = string.Concat("POST\n", length, "\n", contentType, "; charset=utf-8\n", "x-ms-date:", date.ToString("r", FormatCulture), "\n/api/logs");
        return string.Concat("SharedKey ", _WorkspaceId, ":", ComputeHash(challenge));
    }

    private string ComputeHash(string challenge)
    {
        var challengeBytes = Encoding.UTF8.GetBytes(challenge);
        var hash = _Algorithm.ComputeHash(challengeBytes);
        return Convert.ToBase64String(hash);
    }

    #region IDisposable

    private bool _Disposed; // To detect redundant calls

    void Dispose(bool disposing)
    {
        if (!_Disposed)
        {
            if (disposing)
            {
                _Algorithm.Dispose();
            }
            _Disposed = true;
        }
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        Dispose(true);
    }

    #endregion IDisposable
}
