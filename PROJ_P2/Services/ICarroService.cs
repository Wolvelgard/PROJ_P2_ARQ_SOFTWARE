using PROJ_P2.Models;

namespace PROJ_P2.Services;

public interface ICarroService
{
    Task<List<Carro>> ListarAsync();
    Task<Carro?> ObterAsync(int id);
    Task<Carro> CadastrarAsync(Carro carro);
    Task<Carro?> AtualizarAsync(Carro carroAtualizado);
    Task<bool> RemoverAsync(int id);
}