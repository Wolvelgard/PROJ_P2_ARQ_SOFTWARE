using PROJ_P2.Models;

namespace PROJ_P2.Repositories;

public interface ICarroRepository
{
    Task<List<Carro>> ListarAsync();
    Task<Carro?> ObterAsync(int id);
    Task<bool> ExistePlacaAsync(string placa, int? idIgnorado);
    void Adicionar(Carro carro);
    void Remover(Carro carro);
    Task SalvarAsync();
}