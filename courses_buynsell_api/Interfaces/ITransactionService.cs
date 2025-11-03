using courses_buynsell_api.DTOs.Transaction;
namespace courses_buynsell_api.Interfaces;

public interface ITransactionService
{
    Task<List<TransactionListDto>> GetAllAsync();
    Task<TransactionDetailDto?> GetByCodeAsync(string transactionCode);
    Task<List<StudentTransactionStatDto>> GetStudentStatsAsync();
    Task<List<CourseTransactionStatDto>> GetCourseStatsAsync();
    Task<List<MonthlyRevenueDto>> GetLast12MonthsRevenueAsync();

}
