using System.Buffers.Text;
using System.Net.Http.Json;
using System.Text;
using System.Net;


namespace FreyaDev.Services;

public class AuthenticationService
{
    HttpClient httpClient;
    JsonSerializerOptions jsonOptions;

    public AuthenticationService()
    {
        this.httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");

        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,  // Allow flexible casing
            AllowTrailingCommas = false,        // No extra commas allowed
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never // Require all fields
        };
    }


    public async Task<LoginApiResponse> LoginAsync(string userEmail, string userPassword)
    {
        var url = $"{AppSettings.ApiBaseUrl}login";
        var requestData = new { email = userEmail, password = userPassword };
        var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync(url, content);
            var responseText = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"\n\nLogin request sent to API.\nRaw response: {responseText}");
            var loginApiResponse = JsonSerializer.Deserialize<LoginApiResponse>(responseText, jsonOptions);
            Debug.WriteLine($"Deseriaolized response: \n\ttype:{loginApiResponse} \n\tcontent:{JsonSerializer.Serialize(loginApiResponse)}");

            if (loginApiResponse != null) return loginApiResponse;
            else return new LoginApiResponse(500, "Hibás válaszformátum az API-tól");
        }
        catch (JsonException ex)
        {
            return new LoginApiResponse(500, $"Hibás válaszformátum az API-tól. ({ex.Message})");
        }
        catch (Exception ex)
        {
            return new LoginApiResponse(500, $"Váratlan hiba történt a bejelentkezés során. ({ex.Message})");
        }
    }
}