// See https://aka.ms/new-console-template for more information

using System.Net;
using FtpExercise;

Console.WriteLine("Hello, World!");

#region WebRequestFtpUploader

var ftpUploader = new WebRequestFtpUploader(
    "ftp.example.com",
    "username",
    "password"
);

try
{
    await ftpUploader.UploadFileAsync(
        @"C:\local\file.txt",
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
using var uploader = new FluentFtpUploader("ftp.example.com", "name", "password");
await uploader.UploadFileAsync("localfile.txt", "/remote/path/file.txt");
#endregion

