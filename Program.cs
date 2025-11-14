using System;
using System.Threading.Tasks;
using RlucenaWHT.Services;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting the appointment notification process...");

        // --- Google Calendar ---
        // Replace with your actual credential file and calendar ID
        var calendarService = new GoogleCalendarService("path/to/your/credential.json");
        var appointments = await calendarService.ObtenerCitasConMovil("YOUR_CALENDAR_ID");

        // --- WhatsApp ---
        var whatsAppService = new WhatsAppService();

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
