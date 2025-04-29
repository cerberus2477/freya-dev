using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreyaDev.Utils
{
    public class ExceptionHandlerUtil
    {
        // Helper method for common error handling
        //debugging to output window and showing user the error in a uniform popup
        public async Task HandleExceptionAsync(Exception ex, string message = "A kért kérés teljesítése közben hiba állt fel", string title = "Sikertelen művelet", bool displayExMessage = true, [CallerMemberName] string caller = null)
        {
            Debug.WriteLine($"Error in {caller}: {ex.Message}\nException:{ex}");
            if (displayExMessage)
            {
                message += ex.Message;
            }
            try
            {
                //UI modifications can only be done on the main thread
                if (MainThread.IsMainThread)
                {
                    await Shell.Current.DisplayAlert(title, message, "OK");
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Shell.Current.DisplayAlert(title, message, "OK");
                    });
                }
            }
            catch (Exception innerEx)
            {
                Debug.WriteLine($"Failed to show DisplayAlert: {innerEx.Message}");
                // Even if displaying alert fails, don't crash the app
            }
        }
    }
}
