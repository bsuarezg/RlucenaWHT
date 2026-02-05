using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RlucenaWHT.Services;

class Program
{
    static async Task Main(string[] args)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Console.WriteLine("Starting the appointment notification process...");

        // --- Google Calendar ---
        var calendarService = new GoogleCalendarService(config["GoogleCalendar:CredentialFilePath"]);
        var appointments = await calendarService.ObtenerCitasConMovil(config["GoogleCalendar:CalendarId"]);

        // --- WhatsApp ---
        var whatsAppService = new WhatsAppService(config["WhatsApp:ApiUrl"], config["WhatsApp:ApiToken"]);

        foreach (var appointment in appointments)
        {
            Console.WriteLine($"Found appointment: {appointment.Summary} at {appointment.StartTime} with mobile {appointment.MobileNumber}");

            bool success = await whatsAppService.EnviarAvisoCita(
                appointment.MobileNumber,
                appointment.StartTime.ToString("g"),
                appointment.Summary);

            if (success)
            {
                Console.WriteLine("Successfully sent WhatsApp notification.");
            }
            else
            {
                Console.WriteLine("Failed to send WhatsApp notification.");
            }
        }

        Console.WriteLine("Appointment notification process finished.");
    }
}
