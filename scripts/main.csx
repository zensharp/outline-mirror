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

// -----------
// ENVIRONMENT
// -----------
var url = Environment.GetEnvironmentVariable("OUTLINE_INSTANCE_URL");
var apiUrl = url + "/api";
var apiKey = Environment.GetEnvironmentVariable("OUTLINE_API_KEY");
var throttle = 5000;

void rm(string path)
{
	if (Directory.Exists(path))
	{
		Directory.Delete(path, true);
	}
}

// -------
// PROGRAM
// -------
await ExportAsync("outline-markdown", "Markdown", "export-markdown");
await ExportAsync("json", "JSON", "export-json");

// ---------
// FUNCTIONS
// ---------
async Task ExportAsync(string format, string displayName, string destDir)
{
	var suffix = $"[{displayName}]".PadRight(12);

	WriteLine($"{suffix}Requesting export...");
	var fileOperation = await TriggerExportAsync(format);
	Console.ForegroundColor = ConsoleColor.Cyan;
	WriteLine($"{suffix}Export started ({fileOperation.id})...");
	Console.ResetColor();
	
	WriteLine($"{suffix}Waiting for export to finish processing...");
	await WaitForFileToCompleteAsync(fileOperation.id);

	WriteLine($"{suffix}Downloading export file...");
	var filename = fileOperation.name;
	await DownloadFileAsync(fileOperation.id, filename);

	WriteLine($"{suffix}Removing export from Outline...");
	await DeleteFileOperationAsync(fileOperation.id);

	WriteLine($"{suffix}Extracting zip...");
	rm(destDir);
	ZipFile.ExtractToDirectory(filename, destDir);
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

async Task DeleteFileOperationAsync(string id)
{
	var deleteResponse = await apiUrl.AppendPathSegment("fileOperations.delete")
		.WithOAuthBearerToken(apiKey)
		.PostJsonAsync(new
		{
			id = id,
		});
	var jobject = JObject.Parse(await deleteResponse.GetStringAsync());
}
