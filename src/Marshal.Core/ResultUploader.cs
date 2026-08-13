using System.Net.Http.Json;
using Marshal.Abstractions;

namespace Marshal.Core;

/// <summary>
/// POSTs reconciled results to the scrutineering ingest service. Reads its settings from the
/// [upload] section of the local config. The service account is ingest-only, so this is a
/// one-way push — there's nothing interactive behind it.
/// </summary>
public sealed class ResultUploader
{
    private readonly Uri _endpoint;
    private readonly string _user;
    private readonly string _password;
    private readonly bool _verifyTls;

    public ResultUploader(string endpoint, string user, string password, bool verifyTls = true)
    {
        _endpoint = new Uri(endpoint);
        _user = user;
        _password = password;
        _verifyTls = verifyTls;
    }

    public async Task<bool> UploadAsync(IReadOnlyList<Finding> findings, CancellationToken ct = default)
    {
        var handler = new HttpClientHandler();
        if (!_verifyTls)
            handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

        using var http = new HttpClient(handler);
        var basic = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"{_user}:{_password}"));
        http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", basic);

        var resp = await http.PostAsJsonAsync(_endpoint, findings, ct);
        return resp.IsSuccessStatusCode;
    }
}
