using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CRM.Models;
using System;
public class ExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ExternalApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<List<GetAllUserDetailsResult>> FetchDataFromExternalApi()
    {
        string apiUrl = _configuration["ExternalApi:Url"];
        string apiKey = _configuration["ExternalApi:Key"];

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            string jsonResponse = await response.Content.ReadAsStringAsync();

            // Deserialize JSON into List<GetAllUserDetailsResult>
            return JsonSerializer.Deserialize<List<GetAllUserDetailsResult>>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<GetAllUserDetailsResult>();
        }
        else
        {
            throw new HttpRequestException($"Error fetching data: {response.StatusCode}");
        }
    }

}
