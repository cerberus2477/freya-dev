using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FreyaDev.Services
{
    public class ProfileService
    {
        HttpClient httpClient;
        private readonly JsonSerializerOptions jsonOptions;

        public ProfileService()
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

        public async Task<ProfileApiResponse> SaveProfileAsync(string username, string userEmail, string userCity, string userBirthdate, string userDescription)
        {
            User oldUser = JsonSerializer.Deserialize<User>(Preferences.Get("current_user", null));
            var patchData = new Dictionary<string, string>();

            if (username != oldUser.Username)
            {
                patchData["username"] = username;
            }

            if (userEmail != oldUser.Email)
            {
                patchData["email"] = userEmail;
            }

            if (userCity != oldUser.City)
            {
                patchData["city"] = userCity;
            }

            if (userBirthdate != oldUser.Birthdate)
            {
                patchData["birthdate"] = userBirthdate;
            }

            if (userDescription != oldUser.Description)
            {
                patchData["description"] = userDescription;
            }


            var url = $"{AppSettings.ApiBaseUrl}profile";

            var content = new StringContent(JsonSerializer.Serialize(patchData), Encoding.UTF8, "application/json");

            var token = await SecureStorage.GetAsync("auth_token");

            var request = new HttpRequestMessage(HttpMethod.Patch, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            request.Content = content;


            try
            {
                var response = await httpClient.SendAsync(request);
                var responseText = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"\n\nProfile patch request sent to API.\nRaw response: {responseText}");
                var profileApiResponse = JsonSerializer.Deserialize<ProfileApiResponse>(responseText, jsonOptions);
                Debug.WriteLine($"Deseriaolized response: \n\ttype:{profileApiResponse} \n\tcontent:{JsonSerializer.Serialize(profileApiResponse)}");

                if (profileApiResponse != null) return profileApiResponse;
                else return new ProfileApiResponse(500, "Hibás válaszformátum az API-tól");
            }
            catch (JsonException ex)
            {
                return new ProfileApiResponse(500, "Hibás válaszformátum az API-tól");
            }
            catch (Exception ex)
            {
                return new ProfileApiResponse(500, $"Váratlan hiba történt a mentés során. ({ex.Message})");
            }
        }
    }
}
