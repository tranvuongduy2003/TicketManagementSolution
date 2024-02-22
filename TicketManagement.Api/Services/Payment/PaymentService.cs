using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Data;
using TicketManagement.Api.Dtos;
using TicketManagement.Api.Enums;
using TicketManagement.Api.Messaging.IMessaging;
using TicketManagement.Api.Models;
using Stripe;
using Stripe.Checkout;

namespace TicketManagement.Api.Services;

public class PaymentService : IPaymentService
{
    private readonly IMessageBus _messageBus;
    private readonly AppDbContext _db;
    private readonly ITicketService _ticketService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    protected ResponseDto _response;

    public PaymentService(IMessageBus messageBus, ITicketService ticketService,
        AppDbContext db,
        IMapper mapper, IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        _messageBus = messageBus;
        _ticketService = ticketService;
        _db = db;
        _mapper = mapper;
        _configuration = configuration;
        _userManager = userManager;
        _response = new();
    }

    public async Task<ListPaymentObject> GetPayments(PaginationFilter filter)
    {
        try
        {
            var payments = _db.Payments.ToList();

            // Search by customer's name or email
            if (filter.search != null)
            {
                var lowerCaseSearch = filter.search.ToLower();
                payments = payments.Where(c =>
                    EF.Functions.Contains(c.CustomerEmail, lowerCaseSearch) ||
                    EF.Functions.Contains(c.CustomerName, lowerCaseSearch)).ToList();
            }

            payments = filter.order switch
            {
                PageOrder.ASC => payments.OrderBy(c => c.CreatedAt).ToList(),
                PageOrder.DESC => payments.OrderByDescending(c => c.CreatedAt).ToList(),
                _ => payments
            };

            var metadata = new Metadata(payments.Count(), filter.page, filter.size, filter.takeAll);

            if (filter.takeAll == false)
            {
                payments = payments.Skip((filter.page - 1) * filter.size)
                    .Take(filter.size).Join(_userManager.Users, p => p.UserId, u => u.Id, (p, u) =>
                    {
                        var joinedPayment = p;
                        p.User = new ApplicationUser
                        {
                            Id = u.Id,
                            Name = u.Name,
                            Email = u.Email,
                            PhoneNumber = u.PhoneNumber,
                            Avatar = u.Avatar,
                        };
                        return p;
                    }).ToList();
            }

            var paymentDtos = _mapper.Map<IEnumerable<PaymentDto>>(payments);

            return new ListPaymentObject
            {
                payments = paymentDtos,
                metadata = metadata
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<ListPaymentObject> GetPaymentsByUserId(string userId, PaginationFilter filter)
    {
        try
        {
            var payments = _db.Payments.Where(p => p.UserId == userId).ToList();
            var myTickets = new object();

            // Search by customer's name or email
            if (filter.search != null)
            {
                var lowerCaseSearch = filter.search.ToLower();
                payments = payments.Join(_db.Events.Where(events => EF.Functions.Contains(events.Name, lowerCaseSearch)),
                    p => p.EventId, e => e.Id, (p, e) =>
                    {
                        p.Event = e;
                        return p;
                    }).DistinctBy(mt => mt.Id).ToList();
            }

            payments = filter.order switch
            {
                PageOrder.ASC => payments.OrderBy(c => c.CreatedAt).ToList(),
                PageOrder.DESC => payments.OrderByDescending(c => c.CreatedAt).ToList(),
                _ => payments
            };

            var metadata = new Metadata(payments.Count(), filter.page, filter.size, filter.takeAll);

            if (filter.takeAll == false)
            {
                payments = payments.Skip((filter.page - 1) * filter.size)
                    .Take(filter.size).ToList();
            }

            var paymentDtos = _mapper.Map<IEnumerable<PaymentDto>>(payments);

            return new ListPaymentObject
            {
                payments = paymentDtos,
                metadata = metadata
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<ListPaymentObject> GetPaymentsByEventId(string eventId, PaginationFilter filter)
    {
        try
        {
            var payments = _db.Payments.Where(p => p.EventId == eventId).ToList();

            // Search by customer's name or email
            if (filter.search != null)
            {
                var lowerCaseSearch = filter.search.ToLower();
                payments = payments.Where(c =>
                    EF.Functions.Contains(c.CustomerEmail, lowerCaseSearch) ||
                    EF.Functions.Contains(c.CustomerName, lowerCaseSearch)).ToList();
            }

            payments = filter.order switch
            {
                PageOrder.ASC => payments.OrderBy(c => c.CreatedAt).ToList(),
                PageOrder.DESC => payments.OrderByDescending(c => c.CreatedAt).ToList(),
                _ => payments
            };

            var metadata = new Metadata(payments.Count(), filter.page, filter.size, filter.takeAll);

            if (filter.takeAll == false)
            {
                payments = payments.Skip((filter.page - 1) * filter.size)
                    .Take(filter.size).Join(_userManager.Users, p => p.UserId, u => u.Id, (p, u) =>
                    {
                        var joinedPayment = p;
                        p.User = new ApplicationUser
                        {
                            Id = u.Id,
                            Name = u.Name,
                            Email = u.Email,
                            PhoneNumber = u.PhoneNumber,
                            Avatar = u.Avatar,
                        };
                        return p;
                    }).ToList();
            }

            var paymentDtos = _mapper.Map<IEnumerable<PaymentDto>>(payments);

            return new ListPaymentObject
            {
                payments = paymentDtos,
                metadata = metadata
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<PaymentDto> Checkout(CheckoutDto checkoutDto)
    {
        try
        {
            PaymentDto paymentDto = _mapper.Map<PaymentDto>(checkoutDto);
            paymentDto.CreatedAt = DateTime.Now;
            paymentDto.UpdatedAt = DateTime.Now;
            paymentDto.Status = PaymentStatus.PENDING;
            paymentDto.TotalPrice =
                (decimal)checkoutDto.Tickets.Sum(t => (long)(t.Price) * (100 - checkoutDto.Discount) / 100);

            Payment paymentCreated = _db.Payments.Add(_mapper.Map<Payment>(paymentDto)).Entity;
            await _db.SaveChangesAsync();

            var returnedPaymentDto = _mapper.Map<PaymentDto>(paymentCreated);

            return returnedPaymentDto;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<StripeRequestDto> CreateStripeSession(StripeRequestDto stripeRequestDto)
    {
        try
        {
            var options = new SessionCreateOptions
            {
                SuccessUrl = stripeRequestDto.ApprovedUrl,
                CancelUrl = stripeRequestDto.CancelUrl,
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in stripeRequestDto.Tickets.ToList())
            {
                var sessionItemLine = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price),
                        Currency = "vnd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"{item.OwnerName} - {item.OwnerEmail} - {item.OwnerPhone}",
                        },
                    },
                    Quantity = 1
                };

                options.LineItems.Add(sessionItemLine);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            stripeRequestDto.StripeSessionUrl = session.Url;

            Payment payment =
                _db.Payments.First(u => u.Id == stripeRequestDto.PaymentId);
            payment.StripeSessionId = session.Id;

            await _db.SaveChangesAsync();

            await _ticketService.CreateTickets(payment.Id, new CreateTicketsDto
            {
                Tickets = stripeRequestDto.Tickets
            });

            return stripeRequestDto;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<ValidateStripeResponseDto> ValidateStripeSession(string paymentId)
    {
        try
        {
            Payment payment = _db.Payments.First(u => u.Id == paymentId);

            var service = new SessionService();
            Session session = service.Get(payment.StripeSessionId);

            var paymentDto = new PaymentDto();

            if (payment.Status == PaymentStatus.APPROVED)
            {
                var tickets = await _ticketService.GetTicketsByPaymentId(paymentId);
                paymentDto = _mapper.Map<PaymentDto>(payment);
                paymentDto.Tickets = tickets;
            }
            else if (payment.TotalPrice == 0)
            {
                payment.Status = PaymentStatus.APPROVED;
                payment.UpdatedAt = DateTime.Now;
                var tickets = await _ticketService.ValidateTickets(paymentId, true);
                paymentDto = _mapper.Map<PaymentDto>(payment);
                paymentDto.Tickets = tickets;
                _db.SaveChanges();
            }
            else
            {
                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    // Then payment was successful
                    payment.PaymentIntentId = paymentIntent.Id;
                    payment.Status = PaymentStatus.APPROVED;
                    _db.SaveChanges();
                    var tickets = await _ticketService.ValidateTickets(paymentId, true);
                    paymentDto = _mapper.Map<PaymentDto>(payment);
                    paymentDto.Tickets = tickets;

                    await _messageBus.PublishMessage(new ValidateStripeResponseDto
                        {
                            Quantity = paymentDto.Quantity,
                            TotalPrice = paymentDto.TotalPrice,
                            Discount = paymentDto.Discount,
                            CustomerName = paymentDto.CustomerName,
                            CustomerEmail = paymentDto.CustomerEmail,
                            CustomerPhone = paymentDto.CustomerPhone,
                            Tickets = paymentDto.Tickets,
                        },
                        _configuration.GetValue<string>("TopicAndQueueNames:EmailTicketQueue"));
                }
                else
                {
                    payment.Status = PaymentStatus.CANCELLED;
                    _ticketService.ValidateTickets(paymentId, false);
                    _response.IsSuccess = false;
                }
            }

            var validateStripeResponseDto = new ValidateStripeResponseDto
            {
                Id = paymentDto.Id,
                Quantity = paymentDto.Quantity,
                EventId = paymentDto.Tickets.First().EventId,
                Status = paymentDto.Status,
                Tickets = paymentDto.Tickets,
                Discount = paymentDto.Discount,
                CustomerEmail = paymentDto.CustomerEmail,
                CustomerName = paymentDto.CustomerName,
                CustomerPhone = paymentDto.CustomerPhone,
                TotalPrice = paymentDto.TotalPrice,
                UserId = paymentDto.UserId,
                PaymentIntentId = paymentDto.PaymentIntentId,
                StripeSessionId = paymentDto.StripeSessionId,
                CreatedAt = paymentDto.CreatedAt,
                UpdatedAt = paymentDto.UpdatedAt
            };

            if (payment.TotalPrice != 0)
            {
                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);
                var paymentMethodService = new PaymentMethodService();
                PaymentMethod paymentMethod = paymentMethodService.Get(paymentIntent.PaymentMethodId);
                validateStripeResponseDto.PaymentMethod = new PaymentMethodDto
                {
                    Card = paymentMethod.Card.Brand,
                    Last4 = paymentMethod.Card.Last4
                };
            }

            return validateStripeResponseDto;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}