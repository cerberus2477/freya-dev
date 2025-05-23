﻿using FreyaDev.Services;
using Microsoft.Maui.ApplicationModel.Communication;
using System.Reflection.Metadata;
using System.Windows.Input;

namespace FreyaDev.ViewModel;

public partial class AuthViewModel : BaseViewModel
{
    private readonly AuthenticationService authService;
    private readonly ExceptionHandlerUtil exceptionHandlerUtil;
    private readonly UserSessionService userSessionService;

    [ObservableProperty] private string username;
    [ObservableProperty] private string userEmail;
    [ObservableProperty] private string userPassword;
    [ObservableProperty] private string userPasswordConfirmation;

    [ObservableProperty] private string title;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(IsUsernameErrorVisible))] private string usernameError;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(IsEmailErrorVisible))] private string emailError;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(IsPasswordErrorVisible))] private string passwordError;

    public bool IsUsernameErrorVisible => !string.IsNullOrEmpty(UsernameError);
    public bool IsEmailErrorVisible => !string.IsNullOrEmpty(EmailError);
    public bool IsPasswordErrorVisible => !string.IsNullOrEmpty(PasswordError);


    public AuthViewModel(AuthenticationService authService, ExceptionHandlerUtil exceptionHandlerUtil, UserSessionService userSessionService)
    {
        this.authService = authService;
        this.exceptionHandlerUtil = exceptionHandlerUtil;
        this.userSessionService = userSessionService;
    }


    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            EmailError = null;
            PasswordError = null;

            var result = await authService.LoginAsync(UserEmail, UserPassword);
            if (result == null) return;
            if (result.Data is LoginSuccessData successData)
            {
                if (successData.User.RoleId == 3) //check if user has the right role
                {
                    await exceptionHandlerUtil.HandleExceptionAsync(new Exception(), "Nincs megfelelő jogosultságod az app használatához, kérjük fordulj a fejlesztőkhöz, hogy közreműködő lehess!");
                }
                else
                {
                    await userSessionService.SetAuthTokenAsync(successData.Token);
                    userSessionService.SetCurrentUser(successData.User);

                    await Shell.Current.Navigation.PopToRootAsync();
                    await Shell.Current.GoToAsync("///HomePage");
                }
            }
            else if (result.Data is EmptyLoginData)
            {
                await exceptionHandlerUtil.HandleExceptionAsync(new Exception(result.Message), "");
            }
            else if (result.Data is LoginValidationErrorData errorData)
            {

                if (errorData.Errors.ContainsKey("email"))
                {
                    EmailError = string.Join("\n", errorData.Errors["email"]);
                    OnPropertyChanged(nameof(IsEmailErrorVisible));
                }

                if (errorData.Errors.ContainsKey("password"))
                {
                    PasswordError = string.Join("\n", errorData.Errors["password"]);
                    OnPropertyChanged(nameof(IsPasswordErrorVisible));

                }
            }
            else
            {
                await exceptionHandlerUtil.HandleExceptionAsync(new Exception(result.Message), "Hiba adódott bejelentkezés során.");
            }
        }
        catch (Exception ex)
        {
            await exceptionHandlerUtil.HandleExceptionAsync(ex, "Hiba adódott a bejelentkezés során.");
        }
        finally
        {
            IsBusy = false;
        }
    }
}