using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QueueLess.Application.DTOs;
using QueueLess.Application.Interfaces;
using QueueLess.Domain.Entities;
using QueueLess.Domain.Enums;

namespace QueueLess.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TicketService(ITicketRepository ticketRepository, IUnitOfWork unitOfWork)
        {
            _ticketRepository = ticketRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ServiceDetailDto>> GetServicesByBusinessIdAsync(Guid businessId)
        {
            var services = await _ticketRepository.GetServicesByBusinessIdAsync(businessId);
            var result = new List<ServiceDetailDto>();

            foreach (var s in services)
            {
                var queueCount = await _ticketRepository.GetActiveQueueCountAsync(s.Id);
                var estWait = queueCount * s.AvgServiceTimeMinutes;

                result.Add(new ServiceDetailDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    AvgServiceTimeMinutes = s.AvgServiceTimeMinutes,
                    CountersOpen = s.CountersOpen,
                    PeopleInQueue = queueCount,
                    EstimatedWaitMinutes = estWait
                });
            }

            return result;
        }

        public async Task<TicketResponse> BookTicketAsync(Guid customerId, BookTicketRequest request)
        {
            // 1. Check if customer already has an active ticket (Waiting or Serving)
            var existingTicket = await _ticketRepository.GetActiveTicketByCustomerAsync(customerId);
            if (existingTicket != null)
            {
                throw new InvalidOperationException("You already hold an active ticket in the queue.");
            }

            // 2. Validate Service
            var service = await _ticketRepository.GetServiceByIdAsync(request.ServiceId)
                ?? throw new InvalidOperationException("Requested service was not found or is currently inactive.");

            // 3. Generate Sequence Number and Ticket Code (e.g. C040, G001)
            var nextQueueNumber = await _ticketRepository.GetNextQueueNumberAsync(service.Id);
            var letterPrefix = !string.IsNullOrWhiteSpace(service.Name)
                ? char.ToUpperInvariant(service.Name.Trim()[0])
                : 'G';
            var ticketCode = $"{letterPrefix}{nextQueueNumber:D3}";

            // 4. Create and Save Ticket
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                ServiceId = service.Id,
                CustomerId = customerId,
                QueueNumber = nextQueueNumber,
                TicketNumber = ticketCode,
                QrCodeData = ticketCode,
                Status = TicketStatus.Waiting,
                NotifyByPush = request.NotifyByPush,
                NotifyByEmail = request.NotifyByEmail,
                JoinedAt = DateTime.UtcNow
            };

            await _ticketRepository.AddAsync(ticket);
            await _unitOfWork.SaveChangesAsync();

            // 5. Build live queue calculation
            var peopleAhead = await _ticketRepository.GetPeopleAheadAsync(service.Id, ticket.QueueNumber);
            var currentlyServing = await _ticketRepository.GetCurrentlyServingTicketNumberAsync(service.Id) ?? "None";
            var estWait = peopleAhead * service.AvgServiceTimeMinutes;

            return new TicketResponse
            {
                TicketId = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                QrCodeData = ticket.QrCodeData,
                Status = ticket.Status.ToString(),
                ServiceName = service.Name,
                BusinessName = service.Business?.Name ?? string.Empty,
                BusinessAddress = service.Business?.Address ?? string.Empty,
                BookedAt = ticket.JoinedAt,
                NotifyByPush = ticket.NotifyByPush,
                NotifyByEmail = ticket.NotifyByEmail,
                LiveQueue = new LiveQueueStatusDto
                {
                    PeopleAhead = peopleAhead,
                    EstimatedWaitMinutes = estWait,
                    CurrentlyServingTicket = currentlyServing,
                    Status = ticket.Status.ToString(),
                    IsYourTurn = false
                }
            };
        }

        public async Task<TicketResponse> GetActiveTicketAsync(Guid customerId)
        {
            var ticket = await _ticketRepository.GetActiveTicketByCustomerAsync(customerId)
                ?? throw new InvalidOperationException("No active ticket found for this user.");

            var service = ticket.Service!;
            var peopleAhead = await _ticketRepository.GetPeopleAheadAsync(service.Id, ticket.QueueNumber);
            var currentlyServing = await _ticketRepository.GetCurrentlyServingTicketNumberAsync(service.Id) ?? "None";
            var estWait = peopleAhead * service.AvgServiceTimeMinutes;

            return new TicketResponse
            {
                TicketId = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                QrCodeData = ticket.QrCodeData,
                Status = ticket.Status.ToString(),
                ServiceName = service.Name,
                BusinessName = service.Business?.Name ?? string.Empty,
                BusinessAddress = service.Business?.Address ?? string.Empty,
                BookedAt = ticket.JoinedAt,
                NotifyByPush = ticket.NotifyByPush,
                NotifyByEmail = ticket.NotifyByEmail,
                LiveQueue = new LiveQueueStatusDto
                {
                    PeopleAhead = peopleAhead,
                    EstimatedWaitMinutes = estWait,
                    CurrentlyServingTicket = currentlyServing,
                    Status = ticket.Status.ToString(),
                    IsYourTurn = ticket.Status == TicketStatus.Serving
                }
            };
        }

        public async Task<LiveQueueStatusDto> GetQueueStatusAsync(Guid ticketId, Guid customerId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId)
                ?? throw new InvalidOperationException("Ticket not found.");

            if (ticket.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException("Unauthorized access to ticket information.");
            }

            var service = ticket.Service!;
            var peopleAhead = await _ticketRepository.GetPeopleAheadAsync(service.Id, ticket.QueueNumber);
            var currentlyServing = await _ticketRepository.GetCurrentlyServingTicketNumberAsync(service.Id) ?? "None";
            var estWait = peopleAhead * service.AvgServiceTimeMinutes;

            return new LiveQueueStatusDto
            {
                PeopleAhead = peopleAhead,
                EstimatedWaitMinutes = estWait,
                CurrentlyServingTicket = currentlyServing,
                Status = ticket.Status.ToString(),
                IsYourTurn = ticket.Status == TicketStatus.Serving
            };
        }

        public async Task CancelTicketAsync(Guid ticketId, Guid customerId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId)
                ?? throw new InvalidOperationException("Ticket not found.");

            if (ticket.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException("Unauthorized access to ticket.");
            }

            if (ticket.Status != TicketStatus.Waiting)
            {
                throw new InvalidOperationException($"Cannot cancel ticket that is already {ticket.Status}.");
            }

            // SRS Rule: Cancellation is only allowed while having at least 10 people ahead
            var peopleAhead = await _ticketRepository.GetPeopleAheadAsync(ticket.ServiceId, ticket.QueueNumber);
            if (peopleAhead < 10)
            {
                throw new InvalidOperationException($"Cancellation is not allowed because you have fewer than 10 people ahead ({peopleAhead} ahead). You are almost being served.");
            }

            ticket.Status = TicketStatus.Cancelled;
            await _ticketRepository.UpdateAsync(ticket);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
