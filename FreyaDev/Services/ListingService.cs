using System.Net.Http.Headers;
using System.Text;

namespace FreyaDev.Services;

public class ListingService
{
    HttpClient httpClient;
    JsonSerializerOptions jsonOptions;
    ExceptionHandlerUtil exceptionHandlerUtil;
    public ListingService(ExceptionHandlerUtil exceptionHandlerUtil)
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

    List<Listing> listings;

    //TODO: option to get paginated results, e.g only first 4 listings (homepage and profilepage sneak peak of userslistings, or e.g. same city listings)
    public async Task<List<Listing>> SearchListings(string query = "", string username = "")
    {

        //constructing the url
        var url = $"{AppSettings.ApiBaseUrl}listings?pageSize=all";

        if (!string.IsNullOrWhiteSpace(query))
        {
            url += $"&q={Uri.EscapeDataString(query)}";
        }
        if (!string.IsNullOrWhiteSpace(username))
        {
            url += $"&user={Uri.EscapeDataString(username)}";
        }
        //TODO: implement filters. (q will be rewritten probably, because it is handled in a similar way to filters. probably a loop of some kind

        try
        {
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseText = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"\n\nGET Listings request sent to API.\nRaw response: {responseText}");
                var listingsApiResponse = JsonSerializer.Deserialize<ListingsApiResponse>(responseText, jsonOptions);
                Debug.WriteLine($"Deserialized response: \n\tcontent:{JsonSerializer.Serialize(listingsApiResponse)}");

                listings = listingsApiResponse.Data;
            }

            else
            {

                //TODO: ez valamiért breakeli az appot, és a uion nem jelenik meg az üzenet, csak a debug windowban.
                await exceptionHandlerUtil.HandleExceptionAsync(new Exception($"GET Listings request sent to API.\nResponse status: {response.StatusCode}"), "Nem sikerült lekérni a hirdetéseket, mert az API nem 200 (OK) választ adott vissza.");
            }

        }
        catch (JsonException ex)
        {
            await exceptionHandlerUtil.HandleExceptionAsync(ex, "Hibás válaszformátum az API-tól.");
        }
        catch (Exception ex)
        {
            await exceptionHandlerUtil.HandleExceptionAsync(ex, "Váratlan hiba történt a hirdetések lekérése közben.");
        }

        return listings;

    }

    public async Task<PostPatchListingApiResponse> UpdateListingAsync(Listing oldListing, string title, string description, string city, decimal price, List<string> images)
    {
        var patchData = new Dictionary<string, object>();

        if (title != oldListing.Title)
            patchData["title"] = title;

        if (description != oldListing.Description)
            patchData["description"] = description;

        if (city != oldListing.City)
            patchData["city"] = city;

        if (price != oldListing.Price)
            patchData["price"] = price;

        //TODO: handle images
        //todo:  if none are different then skip request
        // Assuming images are compared outside or handled separately
   
        if (images?.Count > 0)
            patchData["images[]"] = images;


        // Skip request if nothing changed
        if (patchData.Count == 0)
        {
            return new PostPatchListingApiResponse(200, "Nem történt változás, frissítés kihagyva.");
        }


        var url = $"{AppSettings.ApiBaseUrl}listings/{oldListing.Id}";
        var token = await SecureStorage.GetAsync("auth_token");

        var request = new HttpRequestMessage(HttpMethod.Patch, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(JsonSerializer.Serialize(patchData), Encoding.UTF8, "application/json");


        try
        {
            var response = await httpClient.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"\n\nhListing patch request sent to API.\nRaw response: {responseText}");
            var profileApiResponse = JsonSerializer.Deserialize<PostPatchListingApiResponse>(responseText, jsonOptions);
            Debug.WriteLine($"Deseriaolized response: \n\ttype:{profileApiResponse} \n\tcontent:{JsonSerializer.Serialize(profileApiResponse)}");

            if (profileApiResponse != null) return profileApiResponse;
            else return new PostPatchListingApiResponse(500, "Hibás válaszformátum az API-tól");
        }
        catch (JsonException ex)
        {
            return new PostPatchListingApiResponse(500, "Hibás válaszformátum az API-tól");
        }
        catch (Exception ex)
        {
            return new PostPatchListingApiResponse(500, $"Váratlan hiba történt a hirdetés módosítása során. ({ex.Message})");
        }
    }

    public async Task<PostPatchListingApiResponse> UpdateUserPlantAsync(Listing oldListing, int plantId, int stageId)
    {
        //check is plantid and stageid are unchanged. only add them to the request if they are different. if none are different then skip
        var patchData = new Dictionary<string, object>();

        if (plantId != oldListing.Plant.Id)
            patchData["plant"] = plantId;

        if (stageId != oldListing.Stage.Id)
            patchData["stage"] = stageId;

        if (patchData.Count == 0)
        {
            return new PostPatchListingApiResponse(200, "Nem történt változás, frissítés kihagyva.");
        }

        //TODO: add userplantsid once in api
        //var url = $"{AppSettings.ApiBaseUrl}profile/plants/{oldListing.userplant.id};
        var url = $"{AppSettings.ApiBaseUrl}profile/plants/17";
        var token = await SecureStorage.GetAsync("auth_token");

        var request = new HttpRequestMessage(HttpMethod.Patch, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(JsonSerializer.Serialize(patchData), Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"\n\nPostPatchListing patch request sent to API.\nRaw response: {responseText}");
            var profileApiResponse = JsonSerializer.Deserialize<PostPatchListingApiResponse>(responseText, jsonOptions);
            Debug.WriteLine($"Deseriaolized response: \n\ttype:{profileApiResponse} \n\tcontent:{JsonSerializer.Serialize(profileApiResponse)}");

            if (profileApiResponse != null) return profileApiResponse;
            else return new PostPatchListingApiResponse(500, "Hibás válaszformátum az API-tól");
        }
        catch (JsonException ex)
        {
            return new PostPatchListingApiResponse(500, "Hibás válaszformátum az API-tól");
        }
        catch (Exception ex)
        {
            return new PostPatchListingApiResponse(500, $"Váratlan hiba történt a hirdetés növény/státusz módosítása során. ({ex.Message})");
        }
    }
}



//TODO: If you want to inject HttpClient properly for testing/DI, you can later refactor it using IHttpClientFactory.