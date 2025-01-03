using System.Security.Cryptography;

namespace BlazorMVU.Demo.Components;

public partial class MvuFetchRandomError
{
    // Model
    public record Model(
        WeatherData? WeatherData,
        bool IsLoading,
        string? ErrorMessage)
    {
        public bool HasError
            => !string.IsNullOrEmpty(ErrorMessage);

        public bool HasData
            => WeatherData != null;

        public string ButtonLabel
            => IsLoading ? "Loading..."
                : HasError ? $"Error: {ErrorMessage}"
                : "Fetch Weather";
    }

    public record WeatherData(int Temperature, int Humidity);

    // Messages
    public abstract record Msg
    {
        public record Fetch : Msg;

        public record ReceiveWeather(WeatherData WeatherData) : Msg;

        public record Fail(string ErrorMessage) : Msg;
    }

    // Initialize the model
    protected override Model Init()
        => new(null, false, "");

    // Update the model based on the message
    protected override Model Update(Msg msg, Model model)
        => msg switch
        {
            Msg.Fetch => model with { IsLoading = true, ErrorMessage = null },
            Msg.ReceiveWeather receiveWeather => model with { WeatherData = receiveWeather.WeatherData, IsLoading = false },
            Msg.Fail fail => new Model(null, false, fail.ErrorMessage),
            _ => model
        };

    // Simulate fetching weather data and dispatch a message
    private async Task FetchWeather()
    {
        try
        {
            // Dispatch a message to indicate that we are fetching data
            Dispatch(new Msg.Fetch());

            // Simulate error on 1/3 of the calls
            await Task.Delay(500);
            var willThrowError = RandomNumberGenerator.GetInt32(0, 3) == 0;
            if (willThrowError)
            {
                throw new Exception("Something went wrong");
            }

            // Simulate API call delay and data received
            await Task.Delay(500);
            var weatherData = new WeatherData(25, 60); // Simulate received data

            // Dispatch a message to indicate that we received data
            var msg = new Msg.ReceiveWeather(weatherData);
            Dispatch(msg);
        }
        catch (Exception ex)
        {
            // Dispatch a message to indicate that we received an error
            var msg = new Msg.Fail(ex.Message);
            Dispatch(msg);
        }
    }
}