// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Globalization;
using System.Text;

namespace PSRule.Monitor.Pipeline;

internal sealed class LogClient : ILogClient
{
    private const string CONTENTTYPE = "application/json";
    private const string TIMESTAMPFIELD = "";
    private const string APIVERSION = "2016-04-01";

    private const string HEADER_ACCEPT = "Accept";
    private const string HEADER_AUTHORIZATION = "Authorization";
    private const string HEADER_LOGTYPE = "Log-Type";
    private const string HEADER_DATE = "x-ms-date";
    private const string HEADER_RESOURCEID = "x-ms-AzureResourceId";
    private const string HEADER_TIMEGENERATED = "time-generated-field";

    private static readonly CultureInfo FormatCulture = new("en-US");

    private readonly HttpClient _HttpClient;
    private readonly Uri _EndpointUri;

    // Track whether Dispose has been called.
    private bool _Disposed;

    public LogClient(string workspaceId, string logName)
    {
        _EndpointUri = new Uri(string.Concat("https://", workspaceId, ".ods.opinsights.azure.com/api/logs?api-version=", APIVERSION));
        _HttpClient = GetClient(logName);
    }

    /// <summary>
    /// Post log data to Azure Monitor endpoint.
    /// </summary>
    public void Post(string signature, DateTime date, string resourceId, string json)
    {
        using var request = PrepareRequest(signature, date, resourceId, json);
        var response = _HttpClient.SendAsync(request);
        response.Wait();
        var result = response.Result.Content.ReadAsStringAsync().Result;
    }

    private static HttpClient GetClient(string logName)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add(HEADER_ACCEPT, CONTENTTYPE);
        client.DefaultRequestHeaders.Add(HEADER_LOGTYPE, logName);
        return client;
    }

    private HttpRequestMessage PrepareRequest(string signature, DateTime date, string resourceId, string json)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _EndpointUri);
        request.Headers.Add(HEADER_AUTHORIZATION, signature);
        request.Headers.Add(HEADER_DATE, date.ToString("r", FormatCulture));
        request.Headers.Add(HEADER_TIMEGENERATED, TIMESTAMPFIELD);
        request.Headers.Add(HEADER_RESOURCEID, resourceId);
        request.Content = new StringContent(json, Encoding.UTF8, CONTENTTYPE);
        return request;
    }

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_Disposed)
        {
            if (disposing)
            {
                _HttpClient.Dispose();
            }
            _Disposed = true;
        }
    }

    #endregion IDisposable
}
