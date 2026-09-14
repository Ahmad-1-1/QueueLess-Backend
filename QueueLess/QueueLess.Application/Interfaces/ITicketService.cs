using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QueueLess.Application.DTOs;

namespace QueueLess.Application.Interfaces
{
    public interface ITicketService
    {
        Task<List<ServiceDetailDto>> GetServicesByBusinessIdAsync(Guid businessId);
        Task<TicketResponse> BookTicketAsync(Guid customerId, BookTicketRequest request);
        Task<TicketResponse> GetActiveTicketAsync(Guid customerId);
        Task<LiveQueueStatusDto> GetQueueStatusAsync(Guid ticketId, Guid customerId);
        Task CancelTicketAsync(Guid ticketId, Guid customerId);
    }
}
