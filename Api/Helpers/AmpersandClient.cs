namespace Api.Helpers;
using Api.Models;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public interface IAmpersandClient
{
    Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body);
    Task<TResponse> GetAsync<TResponse>(string path, object bodyForSignature = null);
}

public class AmpersandClient : IAmpersandClient
{
    private readonly HttpClient _http;
    private readonly AmpersandOptions _opts;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = null };

    public AmpersandClient(HttpClient http, IOptions<AmpersandOptions> opts)
    {
        _http = http;
        _opts = opts.Value;
        _http.BaseAddress = new Uri(_opts.BaseUrl);
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body)
    {
        var requestJson = JsonSerializer.Serialize(body, _jsonOptions);
        var signature = SignatureHelper.Sign(requestJson, _opts.SecretKey);

        using var req = new HttpRequestMessage(HttpMethod.Post, path);
        req.Headers.Add("signature", signature);
        req.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        var res = await _http.SendAsync(req);
        var content = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(content, _jsonOptions);
    }

    public async Task<TResponse> GetAsync<TResponse>(string path, object bodyForSignature = null)
    {
        // For /tx/channel the spec expects signature header and merchantId in body,
        // but endpoint is GET — we'll send merchantId in query and signature of body or "{}"
        string bodyJson = bodyForSignature == null ? "{}" : JsonSerializer.Serialize(bodyForSignature, _jsonOptions);
        var signature = SignatureHelper.Sign(bodyJson, _opts.SecretKey);
        var uri = path;
        using var req = new HttpRequestMessage(HttpMethod.Get, uri);
        req.Headers.Add("signature", signature);
        var res = await _http.SendAsync(req);
        var content = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(content, _jsonOptions);
    }
}
