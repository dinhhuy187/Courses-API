using Microsoft.EntityFrameworkCore;
using courses_buynsell_api.Data;
using courses_buynsell_api.DTOs.VNPAY;
using courses_buynsell_api.Entities;
using courses_buynsell_api.Interfaces;

namespace courses_buynsell_api.Services;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _context;
    private readonly VnPayService _vnPayService;

    public PaymentService(AppDbContext context, VnPayService vnPayService)
    {
        _context = context;
        _vnPayService = vnPayService;
    }

    public async Task<PaymentResponseDto> CreatePaymentAsync(int userId, CreatePaymentRequestDto request, string ipAddress)
    {
        // Validate courses exist and calculate total
        var courses = await _context.Courses
            .Where(c => request.CourseIds.Contains(c.Id))
            .ToListAsync();

        if (courses.Count != request.CourseIds.Count)
        {
            throw new Exception("One or more courses not found");
        }

        decimal totalAmount = courses.Sum(c => c.Price);

        // Generate unique transaction code
        string transactionCode = $"TXN{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";

        // Create transaction record
        var transaction = new Transaction
        {
            TransactionCode = transactionCode,
            PaymentMethod = request.PaymentMethod,
            TotalAmount = totalAmount,
            CreatedAt = DateTime.UtcNow,
            BuyerId = userId,
            TransactionDetails = courses.Select(c => new TransactionDetail
            {
                CourseId = c.Id,
                Price = c.Price
            }).ToList()
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Create payment URL
        string orderInfo = $"Thanh toan khoa hoc - Ma GD: {transactionCode}";
        string paymentUrl = _vnPayService.CreatePaymentUrl(transactionCode, totalAmount, orderInfo, ipAddress);

        return new PaymentResponseDto
        {
            PaymentUrl = paymentUrl,
            TransactionCode = transactionCode
        };
    }

    public async Task<bool> ProcessPaymentCallbackAsync(VnPayCallbackDto callback)
    {
        // Validate signature
        var queryParams = new Dictionary<string, string>
        {
            { "vnp_TmnCode", callback.vnp_TmnCode },
            { "vnp_Amount", callback.vnp_Amount },
            { "vnp_BankCode", callback.vnp_BankCode },
            { "vnp_BankTranNo", callback.vnp_BankTranNo },
            { "vnp_CardType", callback.vnp_CardType },
            { "vnp_OrderInfo", callback.vnp_OrderInfo },
            { "vnp_PayDate", callback.vnp_PayDate },
            { "vnp_ResponseCode", callback.vnp_ResponseCode },
            { "vnp_TxnRef", callback.vnp_TxnRef },
            { "vnp_TransactionNo", callback.vnp_TransactionNo },
            { "vnp_TransactionStatus", callback.vnp_TransactionStatus }
        };

        bool isValidSignature = _vnPayService.ValidateSignature(queryParams, callback.vnp_SecureHash);

        if (!isValidSignature)
        {
            return false;
        }

        // Find transaction
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.TransactionCode == callback.vnp_TxnRef);

        if (transaction == null)
        {
            return false;
        }

        // Update transaction status based on response code
        transaction.UpdatedAt = DateTime.UtcNow;

        // vnp_ResponseCode == "00" means success
        if (callback.vnp_ResponseCode == "00")
        {
            // Payment successful - you can add a Status field to Transaction entity
            // For now, just update UpdatedAt
            await _context.SaveChangesAsync();
            return true;
        }

        // Payment failed
        await _context.SaveChangesAsync();
        return false;
    }

    public async Task<object> GetTransactionByCodeAsync(string transactionCode)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Buyer)
            .Include(t => t.TransactionDetails)
                .ThenInclude(td => td.Course)
            .FirstOrDefaultAsync(t => t.TransactionCode == transactionCode);

        if (transaction == null)
        {
            throw new Exception("Transaction not found");
        }

        return new
        {
            transaction.Id,
            transaction.TransactionCode,
            transaction.PaymentMethod,
            transaction.TotalAmount,
            transaction.CreatedAt,
            transaction.UpdatedAt,
            Buyer = new
            {
                transaction.Buyer?.Id,
                transaction.Buyer?.FullName,
                transaction.Buyer?.Email
            },
            Courses = transaction.TransactionDetails.Select(td => new
            {
                td.Course?.Id,
                td.Course?.Title,
                td.Price
            })
        };
    }
}