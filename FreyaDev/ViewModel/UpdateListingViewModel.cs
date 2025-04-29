


//TODO: make this a toast
//await DisplayAlert("Success", "Sikeres módosítás", "OK");

//await Shell.Current.GoToAsync(".."); // Goes back to the previous page in Shell


//TODO: isrefreshing azért nics szerintem, mert nem refreshviewt használunk a viewban.
//(nem ártana, de ha csak egyszer futtatjuk le a háttérben a getstage getplantet, akkor lehet mindegy mert nem is működne a töltődéskerék (pont azért mert a háttérben fut a cucc)
//ha lesz akkor kell ide egy attribute mint a listingviewmodelben pl

namespace FreyaDev.ViewModel;

[QueryProperty(nameof(Listing), "Listing")]
public partial class UpdateListingViewModel : BaseViewModel
{
    [ObservableProperty]
    Listing listing;

    private readonly ListingService listingService;
    private readonly StageService stageService;
    private readonly PlantService plantService;
    private readonly ExceptionHandlerUtil exceptionHandlerUtil;
    private readonly UserSessionService userSessionService;

    //Listing fields
    [ObservableProperty] private string listingTitle;
    [ObservableProperty] private string description;
    [ObservableProperty] private string city;
    [ObservableProperty] private decimal price;
    //image?

    [ObservableProperty] private string titleError;
    [ObservableProperty] private string descriptionError;
    [ObservableProperty] private string cityError;
    [ObservableProperty] private string priceError;
    //[ObservableProperty] private string imageError;

    public bool IsTitleErrorVisible => !string.IsNullOrEmpty(TitleError);
    public bool IsDescriptionErrorVisible => !string.IsNullOrEmpty(DescriptionError);
    private bool IsCityErrorVisible => !string.IsNullOrEmpty(CityError);
    private bool IsPriceErrorVisible => !string.IsNullOrEmpty(PriceError);
    //public bool IsImageErrorVisible => !string.IsNullOrEmpty(ImageError);


    //Userplant fields
    [ObservableProperty] private Plant plant;
    [ObservableProperty] private Stage stage;
    [ObservableProperty] private string count;
    public ObservableRangeCollection<Stage> allStages = new();
    public ObservableRangeCollection<Plant> allPlants = new();

    [ObservableProperty] private string plantError;
    [ObservableProperty] private string stageError;
    [ObservableProperty] private string countError;

    public bool IsPlantErrorVisible => !string.IsNullOrEmpty(PlantError);
    public bool IsStageErrorVisible => !string.IsNullOrEmpty(StageError);
    public bool IsCountErrorVisible => !string.IsNullOrEmpty(CountError);


    public UpdateListingViewModel(ListingService listingService, ExceptionHandlerUtil exceptionHandlerUtil, UserSessionService userSessionService, StageService stageService, PlantService plantService)
    {
        this.listingService = listingService;
        this.exceptionHandlerUtil = exceptionHandlerUtil;
        this.userSessionService = userSessionService;
        this.stageService = stageService;
        this.plantService = plantService;

        //load the stages and plants automatically when navigated to the page
        //Directly runs the methods in a background thread.
        Task.Run(GetStagesAsync);
        Task.Run(GetPlantsAsync);
    }

    partial void OnListingChanged(Listing value)
    {
        if (value == null)
            return;

        ListingTitle = value.Title;
        Description = value.Description;
        City = value.City;
        Price = value.Price;

        plant = value.Plant;
        stage = value.Stage;
        //todo: load all fields ...
        // stb, ha még több mező kell
    }

    [RelayCommand]
    async Task GetStagesAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            var stages = await stageService.GetStages();

            allStages.Clear();
            allStages.AddRange(stages);
            Debug.WriteLine($"📄 Loaded stages.");
        }
        catch (Exception ex)
        {
            await exceptionHandlerUtil.HandleExceptionAsync(new Exception(ex.Message), "Hiba a növények növekedési stádiumainak lekérése során:");
        }
        finally
        {
            IsBusy = false;
            //IsRefreshing = false;
        }
    }


    [RelayCommand]
    async Task GetPlantsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            var plants = await plantService.GetPlants();

            allPlants.Clear();
            allPlants.AddRange(plants);
            Debug.WriteLine($"📄 Loaded plants.");
        }
        catch (Exception ex)
        {
            await exceptionHandlerUtil.HandleExceptionAsync(new Exception(ex.Message), "Hiba a növények lekérése során:");
        }
        finally
        {
            IsBusy = false;
            //IsRefreshing = false;
        }
    }



    //this command is binded to the ui. when the save button is pressed, the userplant or listing or both commands get excecuted.
    [RelayCommand]
    private async Task SendPatchRequests()
    {
        SaveListingCommand.Execute(null);
        SaveUserPlantCommand.Execute(null);
    }


    [RelayCommand]
    private async Task SaveListingAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            TitleError = null;
            CityError = null;
            PriceError = null;
            //ImageError = null;
            //TODO: images


            var result = await listingService.UpdateListingAsync(listing, ListingTitle, Description, City, Price, new List<string>());
            if (result.Data is PostPatchListingSuccessData successData)
            {
                Listing = successData.Listing;
                //Todo: toast sikeres módosítás
            }
            else if (result.Data is PostPatchListingValidationErrorData errorData)
            {
                if (errorData.Errors.TryGetValue("title", out var titleErrors))
                {
                    TitleError = string.Join("\n", titleErrors);
                    OnPropertyChanged(nameof(IsTitleErrorVisible));
                }
                if (errorData.Errors.TryGetValue("description", out var descriptionErrors))
                {
                    DescriptionError = string.Join("\n", descriptionErrors);
                    OnPropertyChanged(nameof(IsDescriptionErrorVisible));
                }
                if (errorData.Errors.TryGetValue("city", out var cityErrors))
                {
                    CityError = string.Join("\n", cityErrors);
                    OnPropertyChanged(nameof(IsCityErrorVisible));
                }
                if (errorData.Errors.TryGetValue("price", out var priceErrors))
                {
                    PriceError = string.Join("\n", priceErrors);
                    OnPropertyChanged(nameof(IsPriceErrorVisible));
                }
                return;
            }
            else
            {
                await exceptionHandlerUtil.HandleExceptionAsync(new Exception(result.Message), "Hirdetés módosítása sikertelen.");
            }
        }
        catch (Exception ex)
        {
            await exceptionHandlerUtil.HandleExceptionAsync(ex, "Hiba adódott a hirdetés módosítása során.");
        }
        finally
        {
            IsBusy = false;
        }
    }


    [RelayCommand]
   async Task SaveUserPlantAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            PlantError = null;
            StageError = null;
            CountError = null;
            //TODO: images


            var result = await listingService.UpdateListingAsync(listing, ListingTitle, Description, City, Price, new List<string>());
            if (result.Data is PostPatchListingSuccessData successData)
            {
                Listing = successData.Listing;
                //Todo: toast sikeres módosítás
            }
            else if (result.Data is PostPatchListingValidationErrorData errorData)
            {
                //todo: modify for plant stage count fields
                if (errorData.Errors.TryGetValue("plant", out var plantErrors))
                {
                    PlantError = string.Join("\n", plantErrors);
                    OnPropertyChanged(nameof(IsPlantErrorVisible));
                }
                if (errorData.Errors.TryGetValue("stage", out var stageErrors))
                {
                    StageError = string.Join("\n", stageErrors);
                    OnPropertyChanged(nameof(IsStageErrorVisible));
                }
                if (errorData.Errors.TryGetValue("count", out var countErrors))
                {
                    CountError = string.Join("\n", countErrors);
                    OnPropertyChanged(nameof(IsCountErrorVisible));
                }
                return;
            }
            else
            {
                await exceptionHandlerUtil.HandleExceptionAsync(new Exception(result.Message), "Hirdetés módosítása sikertelen.");
            }
        }

        catch (Exception ex)
        {
            await exceptionHandlerUtil.HandleExceptionAsync(ex, "Hiba adódott a hirdetés növény/státusz módosítása során.");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
