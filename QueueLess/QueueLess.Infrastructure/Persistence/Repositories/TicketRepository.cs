using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QueueLess.Application.Interfaces;
using QueueLess.Domain.Entities;
using QueueLess.Domain.Enums;

namespace QueueLess.Infrastructure.Persistence.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly QueueLessDbContext _context;

        public TicketRepository(QueueLessDbContext context)
        {
            _context = context;
        }

        public async Task<Ticket?> GetActiveTicketByCustomerAsync(Guid customerId)
        {
            return await _context.Tickets
                .Include(t => t.Service)
                .ThenInclude(s => s!.Business)
                .FirstOrDefaultAsync(t => t.CustomerId == customerId &&
                    (t.Status == TicketStatus.Waiting || t.Status == TicketStatus.Serving));
        }

        public async Task<Ticket?> GetByIdAsync(Guid ticketId)
        {
            return await _context.Tickets
                .Include(t => t.Service)
                .ThenInclude(s => s!.Business)
                .FirstOrDefaultAsync(t => t.Id == ticketId);
        }

        public async Task AddAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
        }

        public Task UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            return Task.CompletedTask;
        }

        public async Task<int> GetPeopleAheadAsync(Guid serviceId, int queueNumber)
        {
            return await _context.Tickets
                .CountAsync(t => t.ServiceId == serviceId &&
                                 t.Status == TicketStatus.Waiting &&
                                 t.QueueNumber < queueNumber);
        }

        public async Task<string?> GetCurrentlyServingTicketNumberAsync(Guid serviceId)
        {
            var servingTicket = await _context.Tickets
                .Where(t => t.ServiceId == serviceId && t.Status == TicketStatus.Serving)
                .OrderBy(t => t.QueueNumber)
                .Select(t => t.TicketNumber)
                .FirstOrDefaultAsync();

            return servingTicket;
        }

        public async Task<int> GetNextQueueNumberAsync(Guid serviceId)
        {
            var today = DateTime.UtcNow.Date;
            var maxQueueNumber = await _context.Tickets
                .Where(t => t.ServiceId == serviceId && t.JoinedAt >= today)
                .MaxAsync(t => (int?)t.QueueNumber) ?? 0;

            return maxQueueNumber + 1;
        }

        public async Task<int> GetActiveQueueCountAsync(Guid serviceId)
        {
            return await _context.Tickets
                .CountAsync(t => t.ServiceId == serviceId &&
                                 (t.Status == TicketStatus.Waiting || t.Status == TicketStatus.Serving));
        }

        public async Task<List<Service>> GetServicesByBusinessIdAsync(Guid businessId)
        {
            return await _context.Services
                .Where(s => s.BusinessId == businessId && s.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Service?> GetServiceByIdAsync(Guid serviceId)
        {
            return await _context.Services
                .Include(s => s.Business)
                .FirstOrDefaultAsync(s => s.Id == serviceId && s.IsActive);
        }
    }
}
