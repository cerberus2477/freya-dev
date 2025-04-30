using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace FreyaDev.Services
{
    public class UserService
    {
        HttpClient httpClient;
        JsonSerializerOptions jsonOptions;
        ExceptionHandlerUtil exceptionHandlerUtil;

        public UserService(ExceptionHandlerUtil exceptionHandlerUtil)
        {
            this.httpClient = new HttpClient();
            this.jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,  // Allow flexible casing
                AllowTrailingCommas = false,        // No extra commas allowed
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never // Require all fields
            };
            this.exceptionHandlerUtil = exceptionHandlerUtil;
        }

        List<User> users;

        public async Task<List<User>> GetUsersAsync()
        {
            var url = $"{AppSettings.ApiBaseUrl}users";
            
            var token = await SecureStorage.GetAsync("auth_token");
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            

            try
            {
                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"\n\nGET Users request sent to API.\nRaw response: {responseText}");
                    var userApiResponse = JsonSerializer.Deserialize<UsersApiResponse>(responseText, jsonOptions);
                    Debug.WriteLine($"Deserialized response: \n\tcontent:{JsonSerializer.Serialize(userApiResponse)}");

                    users = userApiResponse.Data;

                }
                else {
                    await exceptionHandlerUtil.HandleExceptionAsync(new Exception($"GET Users Listings request sent to API.\nResponse status: {response.StatusCode}"), "Nem sikerült lekérni a felhasználókat, mert az API nem 200 (OK) választ adott vissza.");
                }
            }
            catch (JsonException ex)
            {
                await exceptionHandlerUtil.HandleExceptionAsync(ex, "Hibás válaszformátum az API-tól.");
            }
            catch (Exception ex)
            {
                await exceptionHandlerUtil.HandleExceptionAsync(ex, "Váratlan hiba történt a felhasználók lekérése közben.");
            }
            return users;
        }


        //TODO finish this (currently just returns the users)
        public async Task<string> DeleteUserAsync(int userId)
        {
            var url = $"{AppSettings.ApiBaseUrl}users";

            var token = await SecureStorage.GetAsync("auth_token");
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);



            try
            {
                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"\n\nGET Users request sent to API.\nRaw response: {responseText}");
                    var userApiResponse = JsonSerializer.Deserialize<UsersApiResponse>(responseText, jsonOptions);
                    Debug.WriteLine($"Deserialized response: \n\tcontent:{JsonSerializer.Serialize(userApiResponse)}");

                    users = userApiResponse.Data;

                }
                else
                {
                    await exceptionHandlerUtil.HandleExceptionAsync(new Exception($"GET Users Listings request sent to API.\nResponse status: {response.StatusCode}"), "Nem sikerült lekérni a felhasználókat, mert az API nem 200 (OK) választ adott vissza.");
                }
            }
            catch (JsonException ex)
            {
                await exceptionHandlerUtil.HandleExceptionAsync(ex, "Hibás válaszformátum az API-tól.");
            }
            catch (Exception ex)
            {
                await exceptionHandlerUtil.HandleExceptionAsync(ex, "Váratlan hiba történt a felhasználók lekérése közben.");
            }
            return "users";
        }
    }
}
