using System;
using System.Collections.Generic;

namespace QueueLess.Application.DTOs
{
    public class ServiceDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int AvgServiceTimeMinutes { get; set; }
        public int CountersOpen { get; set; }
        public int PeopleInQueue { get; set; }
        public int EstimatedWaitMinutes { get; set; }
    }

    public class BookTicketRequest
    {
        public Guid ServiceId { get; set; }
        public bool NotifyByPush { get; set; } = true;
        public bool NotifyByEmail { get; set; } = false;
    }

    public class LiveQueueStatusDto
    {
        public int PeopleAhead { get; set; }
        public int EstimatedWaitMinutes { get; set; }
        public string CurrentlyServingTicket { get; set; } = string.Empty;
        public string Status { get; set; } = "Waiting";
        public bool IsYourTurn { get; set; }
    }

    public class TicketResponse
    {
        public Guid TicketId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string QrCodeData { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string BusinessAddress { get; set; } = string.Empty;
        public DateTime BookedAt { get; set; }
        public bool NotifyByPush { get; set; }
        public bool NotifyByEmail { get; set; }
        public LiveQueueStatusDto LiveQueue { get; set; } = new();
    }
}
