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

// -------
// PROGRAM
// -------
while (true)
{
	var n = await CleanAllAsync();

	if (n == 0)
	{
		break;
	}
}

// ---------
// FUNCTIONS
// ---------
async Task<int> CleanAllAsync()
{
	var listResponse = await apiUrl.AppendPathSegment("fileOperations.list")
		.WithOAuthBearerToken(apiKey)
		.PostJsonAsync(new
		{
			type = "export",
			limit = 100,
		});
	var jobject = JObject.Parse(await listResponse.GetStringAsync());

	// Request deletions
	int i = 0;
	foreach (var obj in jobject["data"])
	{
		await DeleteAsync(obj["id"].ToString());
		i++;
	}

	return i;
}

async Task DeleteAsync(string id)
{
	var deleteResponse = await apiUrl.AppendPathSegment("fileOperations.delete")
		.WithOAuthBearerToken(apiKey)
		.PostJsonAsync(new
		{
			id = id,
		});
	var jobject = JObject.Parse(await deleteResponse.GetStringAsync());
	WriteLine($"{id}: {jobject["success"]}");
}
