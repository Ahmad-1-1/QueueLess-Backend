using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QueueLess.Domain.Entities;

namespace QueueLess.Application.Interfaces
{
    public interface ITicketRepository
    {
        Task<Ticket?> GetActiveTicketByCustomerAsync(Guid customerId);
        Task<Ticket?> GetByIdAsync(Guid ticketId);
        Task AddAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task<int> GetPeopleAheadAsync(Guid serviceId, int queueNumber);
        Task<string?> GetCurrentlyServingTicketNumberAsync(Guid serviceId);
        Task<int> GetNextQueueNumberAsync(Guid serviceId);
        Task<int> GetActiveQueueCountAsync(Guid serviceId);
        Task<List<Service>> GetServicesByBusinessIdAsync(Guid businessId);
        Task<Service?> GetServiceByIdAsync(Guid serviceId);
    }
}
