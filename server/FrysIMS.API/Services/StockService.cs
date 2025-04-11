using FrysIMS.API.Models;


public interface IStockService
{
  Task<List<Stock>> GetAllStockAsync();
  Task<Stock> GetStockByIdAsync(int id);
  Task<bool> AddStockAsync(Stock stock);
  Task<bool> UpdateStockAsync(Stock stock);
  Task<bool> DeleteStockAsync(int id);
}

public class StockService : IStockService
{
  private readonly IStockRepository _repository;

  public StockService(IStockRepository repository)
  {
    _repository = repository;
  } 

  public async Task<List<Stock>> GetAllStockAsync()
  {
    return await _repository.GetAllAsync();
  }

  public async Task<Stock> GetStockByIdAsync(int id)
  {
    return await _repository.GetByIdAsync(id);
  }

  public async Task<bool> AddStockAsync(Stock stock)
  {
    var existing = await _repository.GetAllAsync();
    if(existing.Any(s => s.Name.ToLower() == stock.Name.ToLower()))
    {
      Console.WriteLine("Duplicate stock name attempted.");
      return false;
    }

    await _repository.AddAsync(stock);
    return true;
  }

  public async Task<bool> UpdateStockAsync(Stock stock)
  {
    var existing = await _repository.GetByIdAsync(stock.Id);

    if(existing == null) return false;

    existing.Name = stock.Name;
    existing.Quantity = stock.Quantity;
    existing.Unit = stock.Unit;
    existing.OriginalPrice = stock.OriginalPrice;

    await _repository.UpdateAsync(existing);
    return true;
  }

  public async Task<bool> DeleteStockAsync(int id)
  {
    var stock = await _repository.GetByIdAsync(id);

    if(stock == null) return false;

    await _repository.DeleteAsync(stock);
    return true;
  }
}
