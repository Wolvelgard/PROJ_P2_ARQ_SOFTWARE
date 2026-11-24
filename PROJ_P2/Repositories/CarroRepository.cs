using Microsoft.EntityFrameworkCore;
using PROJ_P2.Data;
using PROJ_P2.Models;

namespace PROJ_P2.Repositories;

public class CarroRepository : ICarroRepository
{
    private readonly AppDbContext _context;

    public CarroRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Carro>> ListarAsync()
    {
        return await _context.Carros
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Carro?> ObterAsync(int id)
    {
        return await _context.Carros
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> ExistePlacaAsync(string placa, int? idIgnorado)
    {
        return await _context.Carros
            .AnyAsync(c => c.Placa.ToLower() == placa.ToLower() && c.Id != idIgnorado);
    }

    public void Adicionar(Carro carro)
    {
        _context.Carros.Add(carro);
    }

    public void Remover(Carro carro)
    {
        _context.Carros.Remove(carro);
    }

    public async Task SalvarAsync()
    {
        await _context.SaveChangesAsync();
    }
}