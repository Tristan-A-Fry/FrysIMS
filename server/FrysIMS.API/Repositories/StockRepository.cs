using FrysIMS.API.Data;
using FrysIMS.API.Models;
using Microsoft.EntityFrameworkCore;


public interface IStockRepository
{
  Task<List<Stock>> GetAllAsync();
  Task<Stock> GetByIdAsync(int id);
  Task AddAsync(Stock stock); 
  Task UpdateAsync(Stock stock);
  Task DeleteAsync(Stock stock);
}

public class StockRepository : IStockRepository
{
  private readonly ApplicationDbContext _context;

  public StockRepository(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<List<Stock>> GetAllAsync()=> await _context.Stock.ToListAsync();

  public async Task<Stock> GetByIdAsync(int id)=> await _context.Stock.FindAsync(id);

  public async Task AddAsync(Stock stock)
  {
    _context.Stock.Add(stock);
    await _context.SaveChangesAsync();
  }

  public async Task UpdateAsync(Stock stock)
  {
    _context.Stock.Update(stock);
    await _context.SaveChangesAsync();
  }

  public async Task DeleteAsync(Stock stock)
  {
    _context.Stock.Remove(stock);
    await _context.SaveChangesAsync();
  }



}
