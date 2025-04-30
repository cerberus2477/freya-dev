using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreyaDev.ViewModel;

public partial class UserViewModel : BaseViewModel
{
    [ObservableProperty]
    private User originalUser;
    public ObservableRangeCollection<User> Users { get; set; } = new ObservableRangeCollection<User>();
    private readonly UserService userService;
    private readonly ExceptionHandlerUtil exceptionHandlerUtil;
    [ObservableProperty] private string userUsername;
    [ObservableProperty] private string userEmail;
    [ObservableProperty] private string userCity;
    [ObservableProperty] private string userBirthdate;
    [ObservableProperty] private string userDescription;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(IsUserUsernameErrorVisible))] private string userUsernameError;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(IsUserEmailErrorVisible))] private string userEmailError;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(IsUserCityErrorVisible))] private string userCityError;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(IsUserBirthdateErrorVisible))] private string userBirthdateError;

    public bool IsUserUsernameErrorVisible => !string.IsNullOrEmpty(UserUsernameError);
    public bool IsUserEmailErrorVisible => !string.IsNullOrEmpty(UserEmailError);
    public bool IsUserCityErrorVisible => !string.IsNullOrEmpty(UserCityError);
    public bool IsUserBirthdateErrorVisible => !string.IsNullOrEmpty(UserBirthdateError);

    public UserViewModel(UserService userService, ExceptionHandlerUtil exceptionHandlerUtil)
    {
        Title = "Felhasználók";
        this.userService = userService;
        this.exceptionHandlerUtil = exceptionHandlerUtil;
        //load the listings automatically when navigated to the page
        Task.Run(GetUsersAsync);
    }

    [RelayCommand]
    async Task GetUsersAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {

            var users = await userService.GetUsersAsync();
            Users.Clear();
            Users.AddRange(users);
        }
        catch (Exception ex)
        {
            await exceptionHandlerUtil.HandleExceptionAsync(ex, "Nem sikerült lekérni a felhasználókat.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Add this property to hold the current user
    [ObservableProperty]
    private User _currentUser;

    // Replace GoToUserUpdate with this version
    [RelayCommand]
    async Task GoToUserUpdate(User user)
    {
        if (user == null) return;
        await Shell.Current.GoToAsync($"UserUpdatePage?userId={user.Id}");
    }

    // Add this command to load user details
    [RelayCommand]
    private async Task LoadUserDetails(int userId)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var response = await userService.GetUserByIdAsync(userId);

            if (response?.Data is UserSuccessData successData)
            {
                CurrentUser = successData.User;
                UserUsername = CurrentUser.Username;
                UserEmail = CurrentUser.Email;
                UserCity = CurrentUser.City;
                //UserBirthdate = CurrentUser.Birthdate;
                UserDescription = CurrentUser.Description;
                OriginalUser = CurrentUser; // Keep a copy for comparison
            }
            else
            {
                await exceptionHandlerUtil.HandleExceptionAsync(
                    new Exception(response?.Message ?? "Unknown error"),
                    "Failed to load user details");
            }
        }
        catch (Exception ex)
        {
            await exceptionHandlerUtil.HandleExceptionAsync(ex, "Error loading user");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task DeleteUser(User user)
    {
        if (user == null) return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Törlés",
            $"Biztosan törli {user.Username} felhasználót?",
            "Törlés", "Mégsem");

        if (!confirm) return;

        try
        {
            IsBusy = true;
            await userService.DeleteUserAsync(user);
            Users.Remove(user);
        }
        catch (Exception ex)
        {
            await exceptionHandlerUtil.HandleExceptionAsync(ex, "Failed to delete user");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveUserAsync(User user)
    {
        if (user == null || IsBusy) return;
        try
        {
            IsBusy = true;
            userEmailError = null;
            userCityError = null;
            userUsernameError = null;
            userBirthdateError = null;

            var result = await this.userService.UpdateUserAsync(user, UserUsername, UserEmail, UserCity, UserBirthdate, UserDescription);
            if (result.Data is UserSuccessData successData)
            {
                await Shell.Current.DisplayAlert(
                    "Sikeres módosítás",
                    $"{user.Username} módosítva",
                    "Ok");
            }
            else if (result.Data is EmptyUserData)
            {
                await exceptionHandlerUtil.HandleExceptionAsync(new Exception(result.Message), "Hiba adódott profiladatok mentése során.");
            }
            else if (result.Data is UserValidationErrorData errorData)
            {

                if (errorData.Errors.ContainsKey("email"))
                {
                    UserEmailError = string.Join("\n", errorData.Errors["email"]);
                    OnPropertyChanged(nameof(IsUserEmailErrorVisible));
                }

                if (errorData.Errors.ContainsKey("city"))
                {
                    UserCityError = string.Join("\n", errorData.Errors["city"]);
                    OnPropertyChanged(nameof(IsUserCityErrorVisible));

                }

                if (errorData.Errors.ContainsKey("birthdate"))
                {
                    UserBirthdateError = string.Join("\n", errorData.Errors["birthdate"]);
                    OnPropertyChanged(nameof(IsUserBirthdateErrorVisible));

                }

                if (errorData.Errors.ContainsKey("username"))
                {
                    UserCityError = string.Join("\n", errorData.Errors["username"]);
                    OnPropertyChanged(nameof(IsUserCityErrorVisible));

                }
            }
            else
            {
                await exceptionHandlerUtil.HandleExceptionAsync(new Exception(result.Message), "Hiba adódott profiladatok mentése során.");
            }
        }
        catch (Exception ex)
        {
            await exceptionHandlerUtil.HandleExceptionAsync(ex, "Hiba adódott a profiladatok mentése során.");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
