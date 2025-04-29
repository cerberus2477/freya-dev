namespace FreyaDev.Services;

public class PlantService
{
    HttpClient httpClient;
    JsonSerializerOptions jsonOptions;
    ExceptionHandlerUtil exceptionHandlerUtil;
    public PlantService(ExceptionHandlerUtil exceptionHandlerUtil)
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

    List<Plant> plants;

    //TODO: lehet kell jogosultság (tbh minden kéréshez kb)
    public async Task<List<Plant>> GetPlants()
    {
        var url = $"{AppSettings.ApiBaseUrl}plants";
        try
        {
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseText = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"\n\nGET Plants request sent to API.\nRaw response: {responseText}");
                var plantsApiResponse = JsonSerializer.Deserialize<PlantsApiResponse>(responseText, jsonOptions);
                Debug.WriteLine($"Deserialized response: \n\tcontent:{JsonSerializer.Serialize(plantsApiResponse)}");

                plants = plantsApiResponse.Data;
            }

            else
            {

                //TODO: ez valamiért breakeli az appot, és a uion nem jelenik meg az üzenet, csak a debug windowban.
                await exceptionHandlerUtil.HandleExceptionAsync(new Exception($"GET Plants request sent to API.\nResponse status: {response.StatusCode}"), "Nem sikerült lekérni a növényeket, mert az API nem 200 (OK) választ adott vissza.");
            }

        }
        catch (JsonException ex)
        {
            await exceptionHandlerUtil.HandleExceptionAsync(ex, "Hibás válaszformátum az API-tól.");
        }
        catch (Exception ex)
        {
            await exceptionHandlerUtil.HandleExceptionAsync(ex, "Váratlan hiba történt a növények lekérése közben.");
        }

        return plants;

    }

}
    //TODO: If you want to inject HttpClient properly for testing/DI, you can later refactor it using IHttpClientFactory.}

