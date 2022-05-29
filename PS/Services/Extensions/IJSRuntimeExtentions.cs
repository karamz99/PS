using Microsoft.JSInterop;

namespace PS
{
    public static class IJSRuntimeExtentions
    {
        public static ValueTask DisplayMessage(this IJSRuntime js, string message, SweetAlertMessageTypes type = SweetAlertMessageTypes.Info)
             => js.InvokeVoidAsync("fire", message, ((SweetAlertMessageTypes)type).ToString().ToLower());

        public static ValueTask<bool> DisplayConfirm(this IJSRuntime js, string title, string message, SweetAlertMessageTypes type = SweetAlertMessageTypes.Question, string confirmText = "Ok", string cancelText = "Cancel", string confirmButtonColor = "#3085d6", string cancelButtonColor = "#6e7d88")
            => js.InvokeAsync<bool>("sweetConfirm", title, message, ((SweetAlertMessageTypes)type).ToString().ToLower(), confirmText, cancelText, confirmButtonColor, cancelButtonColor);

        public enum SweetAlertMessageTypes { Success, Error, Warning, Info, Question, }
    }
}
