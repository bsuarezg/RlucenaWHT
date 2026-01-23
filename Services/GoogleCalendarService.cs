using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RlucenaWHT.Services
{
    public class Appointment
    {
        public string Summary { get; set; }
        public DateTime StartTime { get; set; }
        public string MobileNumber { get; set; }
    }

    public class GoogleCalendarService
    {
        private CalendarService _service;

        public GoogleCalendarService(string credentialFilePath)
        {
            string[] Scopes = { CalendarService.Scope.CalendarReadonly };

            GoogleCredential credential;
            using (var stream = new FileStream(credentialFilePath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            _service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "RlucenaWHT",
            });
        }

        public async Task<List<Appointment>> ObtenerCitasConMovil(string calendarId)
        {
            var appointments = new List<Appointment>();

            var startDate = DateTime.Now.Date.AddDays(4);
            var endDate = startDate.AddDays(1);

            var request = _service.Events.List(calendarId);
            request.TimeMin = startDate;
            request.TimeMax = endDate;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            Events events = await request.ExecuteAsync();

            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    if (eventItem.Description != null)
                    {
                        var mobileNumber = ExtractMobileNumber(eventItem.Description);
                        if (!string.IsNullOrEmpty(mobileNumber))
                        {
                            appointments.Add(new Appointment
                            {
                                Summary = eventItem.Summary,
                                StartTime = eventItem.Start.DateTime.Value,
                                MobileNumber = mobileNumber
                            });
                        }
                    }
                }
            }

            return appointments;
        }

        private string ExtractMobileNumber(string description)
        {
            var regex = new Regex(@"(\+\d{1,3})?\d{10}");
            var match = regex.Match(description);
            return match.Success ? match.Value : null;
        }
    }
}
