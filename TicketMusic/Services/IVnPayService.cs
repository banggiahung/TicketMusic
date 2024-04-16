using TicketMusic.Models;

namespace TicketMusic.Services;
public interface IVnPayService
{
    string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
    Task<PaymentResponseModel> PaymentExecute(IQueryCollection collections);
    string GetIpAddress(HttpContext context);
}