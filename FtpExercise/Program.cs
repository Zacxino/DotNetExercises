// See https://aka.ms/new-console-template for more information

using System.Net;
using FtpExercise;

Console.WriteLine("Hello, World!");

#region WebRequestFtpUploader

var ftpUploader = new WebRequestFtpUploader(
    "172.18.1.80",
    "EDY",
    "ben"
);

try
{
    await ftpUploader.UploadFileAsync(
        @"E:\test.txt",
        "remote/path/file.txt"
    );
    Console.WriteLine("File uploaded successfully");
}
catch (WebException ex)
{
    var response = (FtpWebResponse)ex.Response;
    Console.WriteLine($"FTP Error: {response.StatusCode} - {response.StatusDescription}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

#endregion

#region FluentFtpUploader

using (var uploader = new FluentFtpUploader("ftp://192.168.132.1", "EDY", "ben"))
{
    await uploader.UploadFileAsync(@"E:\test.txt", Path.GetDirectoryName("remote/path/file/test.txt"));
}


#endregion