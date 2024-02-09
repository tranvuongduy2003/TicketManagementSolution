using TicketManagement.Api.Dtos;

namespace TicketManagement.Api.Contracts;

public interface IPaymentService
{
    Task<ListPaymentObject> GetPayments(PaginationFilter filter);
    Task<ListPaymentObject> GetPaymentsByUserId(string userId, PaginationFilter filter);
    Task<ListPaymentObject> GetPaymentsByEventId(string eventId, PaginationFilter filter);
    Task<PaymentDto> Checkout(CheckoutDto checkoutDto);
    Task<StripeRequestDto> CreateStripeSession(StripeRequestDto stripeRequestDto);
    Task<ValidateStripeResponseDto> ValidateStripeSession(string paymentId);
}