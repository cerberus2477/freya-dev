using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreyaDev.ViewModel;

public partial class UserViewModel : BaseViewModel
{
    public ObservableRangeCollection<User> Users { get; set; } = new ObservableRangeCollection<User>();
    private readonly UserService userService;
    private readonly ExceptionHandlerUtil exceptionHandlerUtil;

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

    [RelayCommand]
    async Task GoToUserUpdate(User user)
    {
        if (user == null)
            return;

        await Shell.Current.GoToAsync("UserDetailsPage", true, new Dictionary<string, object>
        {
            {"User", user }
        });
    }

    [RelayCommand]
    async Task DeleteUser(User user)
    {
        if (user == null) return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Confirm Delete",
            $"Delete user {user.Username}?",
            "Yes", "No");

        if (!confirm) return;

        try
        {
            IsBusy = true;
            await userService.DeleteUserAsync(user.Id);
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
}
