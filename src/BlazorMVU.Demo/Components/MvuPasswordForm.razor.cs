using Microsoft.AspNetCore.Components;

namespace BlazorMVU.Demo.Components;

public partial class MvuPasswordForm
{
    // Model
    public record Model(string Name, string Password, string PasswordAgain)
    {
        public string? PasswordInvalid
            => Password != "" && PasswordAgain != ""
                ? (Password != PasswordAgain).ToString().ToLower()
                : null;
    }

    // Messages
    public abstract record Msg
    {
        public record NameChange(string Name) : Msg;

        public record PasswordChange(string Password) : Msg;

        public record PasswordAgainChange(string PasswordAgain) : Msg;
    }

    // Initialize the model
    protected override Model Init()
        => new("", "", "");

    // Update the model based on the message
    protected override Model Update(Msg msg, Model model)
        => msg switch
        {
            Msg.NameChange name => model with { Name = name.Name },
            Msg.PasswordChange password => model with { Password = password.Password },
            Msg.PasswordAgainChange passwordAgain => model with { PasswordAgain = passwordAgain.PasswordAgain },
            _ => model
        };

    // Handle the text changes and dispatch messages
    private void HandleNameChange(ChangeEventArgs obj)
    {
        var name = obj.Value?.ToString() ?? "";
        var msg = new Msg.NameChange(name);
        Dispatch(msg);
    }

    private void HandlePasswordChange(ChangeEventArgs obj)
    {
        var password = obj.Value?.ToString() ?? "";
        var msg = new Msg.PasswordChange(password);
        Dispatch(msg);
    }

    private void HandlePasswordAgainChange(ChangeEventArgs obj)
    {
        var passwordAgain = obj.Value?.ToString() ?? "";
        var msg = new Msg.PasswordAgainChange(passwordAgain);
        Dispatch(msg);
    }
}