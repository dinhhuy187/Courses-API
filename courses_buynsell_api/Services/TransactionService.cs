using Microsoft.EntityFrameworkCore;
using courses_buynsell_api.Data;
using courses_buynsell_api.DTOs.Transaction;
using courses_buynsell_api.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using courses_buynsell_api.Exceptions;
namespace courses_buynsell_api.Services;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;

    public TransactionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TransactionListDto>> GetAllAsync()
    {
        var result = await _context.Transactions
            .Include(t => t.Buyer)
            .Select(t => new TransactionListDto
            {
                TransactionCode = t.TransactionCode,
                BuyerName = t.Buyer!.FullName ?? "(Unknown buyer)",
                TotalAmount = t.TotalAmount,
                PaymentMethod = t.PaymentMethod,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();

        if (!result.Any())
            throw new NotFoundException("No transactions found.");

        return result;
    }


    public async Task<TransactionDetailDto?> GetByCodeAsync(string transactionCode)
    {
        if (string.IsNullOrWhiteSpace(transactionCode))
            throw new ArgumentException("Transaction code cannot be empty.", nameof(transactionCode));

        var transaction = await _context.Transactions
            .Include(t => t.Buyer)
            .Include(t => t.TransactionDetails)
                .ThenInclude(td => td.Course)
            .Where(t => t.TransactionCode == transactionCode)
            .Select(t => new TransactionDetailDto
            {
                TransactionCode = t.TransactionCode,
                CreatedAt = t.CreatedAt,
                BuyerName = t.Buyer!.FullName ?? "(Unknown buyer)",
                TotalAmount = t.TotalAmount,
                Courses = t.TransactionDetails.Select(td => new TransactionDetailCourseDto
                {
                    CourseName = td.Course!.Title ?? "(Unknown course)",
                    Price = td.Price
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (transaction is null)
            throw new NotFoundException($"Transaction with code '{transactionCode}' not found.");

        return transaction;
    }


    public async Task<List<StudentTransactionStatDto>> GetStudentStatsAsync()
    {
        var stats = await _context.Users
            .Where(u => u.Transactions.Any())
            .Select(u => new StudentTransactionStatDto
            {
                StudentId = u.Id,
                FullName = u.FullName,
                PurchaseCount = u.Transactions.Count(),
                TotalRevenue = u.Transactions.Sum(t => t.TotalAmount),
                LastTransactionDate = u.Transactions
                    .OrderByDescending(t => t.CreatedAt)
                    .Select(t => t.CreatedAt)
                    .FirstOrDefault()
            })
            .ToListAsync();

        if (!stats.Any())
            throw new NotFoundException("No student transaction data available.");

        return stats;
    }


    public async Task<List<CourseTransactionStatDto>> GetCourseStatsAsync()
    {
        var stats = await _context.TransactionDetails
            .Include(td => td.Course)
            .Include(td => td.Transaction)
            .GroupBy(td => new { td.CourseId, td.Course!.Title })
            .Select(g => new CourseTransactionStatDto
            {
                CourseId = g.Key.CourseId,
                CourseTitle = g.Key.Title,
                PurchaseCount = g.Count(),
                TotalRevenue = g.Sum(x => x.Price),
                LastTransactionDate = g.Max(x => x.Transaction!.CreatedAt)
            })
            .ToListAsync();

        if (!stats.Any())
            throw new NotFoundException("No course transaction data available.");

        return stats;
    }
}
