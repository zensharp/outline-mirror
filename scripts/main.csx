#!/usr/bin/env dotnet-script

#r "nuget: Flurl.Http, 4.0.2"
#r "nuget: Newtonsoft.Json, 13.0.3"

using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.IO.Compression;

class FileOperation
{
	public string id = null;
	public string type = null;
	public string format = null;
	public string name = null;
	public string state = null;
}

// Environment
var url = Environment.GetEnvironmentVariable("OUTLINE_INSTANCE_URL");
var apiUrl = url + "/api";
var apiKey = Environment.GetEnvironmentVariable("OUTLINE_API_KEY");
var throttle = 5000;

// Begin
var srcFile = await GetExportAsync("outline-markdown", "Markdown");

var destDir = "export";
if (Directory.Exists(destDir))
{
	Directory.Delete(destDir, true);
}
WriteLine($"Extracting export...");
ZipFile.ExtractToDirectory(srcFile, destDir);

async Task<string> GetExportAsync(string format, string displayName)
{
	WriteLine($"Requesting {displayName} export...");
	var fileOperation = await TriggerExportAsync(format);
	Console.ForegroundColor = ConsoleColor.Cyan;
	WriteLine($"Export started for {fileOperation.name} ({fileOperation.id})...");
	Console.ResetColor();
	
	WriteLine($"Waiting for export to finish processing...");
	await WaitForFileToCompleteAsync(fileOperation.id);

	WriteLine($"Downloading export file...");
	var filename = fileOperation.name;
	await DownloadFileAsync(fileOperation.id, filename);

	Console.ForegroundColor = ConsoleColor.Green;
	WriteLine($"Downloaded {filename}!");
	Console.ResetColor();
	WriteLine();

	return filename;
}

async Task<FileOperation> TriggerExportAsync(string format)
{
	var exportAllResponse = await apiUrl.AppendPathSegment("collections.export_all")
		.WithOAuthBearerToken(apiKey)
		.PostJsonAsync(new
		{
			format = format,
		});
	var jobject = JObject.Parse(await exportAllResponse.GetStringAsync());

	// Validate response
	if (!jobject["success"].Value<bool>())
	{
		throw new Exception("The export request (collections.export_all) was not successful.");
	}

	// Read object
	var fileOperation = JsonConvert.DeserializeObject<FileOperation>(jobject["data"]["fileOperation"].ToString());

	return fileOperation;
}

async Task WaitForFileToCompleteAsync(string id)
{
	while (true)
	{
		WriteLine("Sleeping for 5 seconds...");
		await Task.Delay(throttle);

		var infoResponse = await apiUrl.AppendPathSegment("fileOperations.info")
			.WithOAuthBearerToken(apiKey)
			.PostJsonAsync(new
			{
				id = id,
			});
		var jobject = JObject.Parse(await infoResponse.GetStringAsync());

		var temp = JsonConvert.DeserializeObject<FileOperation>(jobject["data"].ToString());
		if (temp.state == "complete")
		{
			break;
		}
	}
}

async Task DownloadFileAsync(string id, string filename)
{
	var redirectResponse = await apiUrl.AppendPathSegment("fileOperations.redirect")
		.WithOAuthBearerToken(apiKey)
		.PostJsonAsync(new
		{
			id = id,
		});

	File.WriteAllBytes(filename, await redirectResponse.GetBytesAsync());
}
