using System.Net;
using System.Text;
using System.Text.Json;
using PCSC;

namespace eGKGui;

internal sealed class EgkHttpServer : IDisposable
{
    private readonly HttpListener _listener = new();
    private readonly CancellationTokenSource _cts = new();

    public EgkHttpServer(string prefix = "http://localhost:5000/")
    {
        _listener.Prefixes.Add(prefix);
    }

    public void Start()
    {
        _listener.Start();
        Task.Run(() => RunLoop(_cts.Token));
    }

    private async Task RunLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            HttpListenerContext ctx;
            try { ctx = await _listener.GetContextAsync(); }
            catch (HttpListenerException) { break; }
            catch (ObjectDisposedException) { break; }
            _ = Task.Run(() => HandleRequest(ctx), ct);
        }
    }

    private static void HandleRequest(HttpListenerContext ctx)
    {
        var req = ctx.Request;
        var res = ctx.Response;

        if (req.HttpMethod != "GET" || req.Url?.AbsolutePath != "/egk")
        {
            res.StatusCode = 404;
            res.Close();
            return;
        }

        try
        {
            using var cardCtx = ContextFactory.Instance.Establish(SCardScope.System);
            string[] readerNames = cardCtx.GetReaders();
            if (readerNames.Length == 0) throw new Exception("No reader found.");
            using var reader = cardCtx.ConnectReader(readerNames[0], SCardShareMode.Shared, SCardProtocol.Any);
            var egk = new EgkReader(reader);
            HealthCardData data = egk.GetData();

            WriteJson(res, 200, data);
        }
        catch (Exception ex)
        {
            WriteJson(res, 500, new { error = ex.Message });
        }
        finally
        {
            res.Close();
        }
    }

    private static void WriteJson<T>(HttpListenerResponse res, int statusCode, T value)
    {
        byte[] body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
        res.StatusCode = statusCode;
        res.ContentType = "application/json; charset=utf-8";
        res.ContentLength64 = body.Length;
        res.OutputStream.Write(body, 0, body.Length);
    }

    public void Dispose()
    {
        _cts.Cancel();
        _listener.Stop();
        _listener.Close();
    }
}
