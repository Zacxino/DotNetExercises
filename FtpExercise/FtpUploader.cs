using System.Net;
using FluentFTP;

namespace FtpExercise;

public class WebRequestFtpUploader : IDisposable
{
    private readonly string _host;
    private readonly NetworkCredential _credentials;
    private bool _disposed;

    public WebRequestFtpUploader(string host, string username, string password)
    {
        if (!host.StartsWith("ftp://"))
            host = "ftp://" + host;

        _host = host.TrimEnd('/');
        _credentials = new NetworkCredential(username, password);
    }

    public async Task UploadFileAsync(string localPath, string remotePath, bool overwrite = true)
    {
        await CreateDirectoryRecursive(remotePath);

        var request = CreateRequest(CombinePaths(_host, remotePath),
            overwrite ? WebRequestMethods.Ftp.UploadFile : WebRequestMethods.Ftp.AppendFile);

        using var fileStream = File.OpenRead(localPath);
        using var requestStream = await request.GetRequestStreamAsync();

        await fileStream.CopyToAsync(requestStream);
    }

    public async Task CreateDirectoryRecursive(string remotePath)
    {
        var pathParts = remotePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        var currentPath = _host;

        foreach (var part in pathParts)
        {
            currentPath = CombinePaths(currentPath, part);

            if (!await DirectoryExists(currentPath))
            {
                var createRequest = CreateRequest(currentPath, WebRequestMethods.Ftp.MakeDirectory);
                using (await createRequest.GetResponseAsync()) { }
            }
        }
    }

    private async Task<bool> DirectoryExists(string path)
    {
        try
        {
            var request = CreateRequest(Path.GetDirectoryName(path), WebRequestMethods.Ftp.ListDirectory);
            using (await request.GetResponseAsync()) { }
            return true;
        }
        catch (WebException ex) when (ex.Response is FtpWebResponse response &&
                                     response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
        {
            return false;
        }
    }

    private FtpWebRequest CreateRequest(string url, string method)
    {
        var request = (FtpWebRequest)WebRequest.Create(url);
        request.Credentials = _credentials;
        request.Method = method;
        request.UseBinary = true;
        request.EnableSsl = false;  // 根据需要修改
        request.UsePassive = true; // 大多数现代 FTP 服务器需要被动模式
        return request;
    }

    private static string CombinePaths(string basePath, string relativePath)
    {
        return $"{basePath}/{relativePath.TrimStart('/')}";
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            // 这里可以添加资源清理代码
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    // 添加其他辅助方法...
}

public class FluentFtpUploader : IDisposable
{
    private readonly AsyncFtpClient _ftpClient;
    private bool _disposed;

    public FluentFtpUploader(string host, string name, string password)
    {
        _ftpClient = new AsyncFtpClient(host, name, password);
        ConfigureClient();
    }

    private void ConfigureClient()
    {
        _ftpClient.Config.EncryptionMode = FtpEncryptionMode.None; // 根据需要修改为显式/隐式TLS
        _ftpClient.Config.ValidateAnyCertificate = true; // 仅用于测试环境
        _ftpClient.Config.DataConnectionType = FtpDataConnectionType.PASV; // 被动模式
    }

    public async Task ConnectAsync()
    {
        if (!_ftpClient.IsConnected)
        {
            await _ftpClient.Connect();
        }
    }

    public async Task UploadFileAsync(string localPath, string remotePath, bool overwrite = true)
    {
        await ConnectAsync();
        await _ftpClient.UploadFile(localPath, remotePath, overwrite ? FtpRemoteExists.Overwrite : FtpRemoteExists.AddToEnd);
    }

    public async Task CreateDirectoryRecursive(string remotePath)
    {
        await ConnectAsync();
        await _ftpClient.CreateDirectory(remotePath);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (_ftpClient.IsConnected)
            {
                _ftpClient.Disconnect();
            }
            _ftpClient.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}

