using PROJ_P2.Models;
using PROJ_P2.Repositories;

namespace PROJ_P2.Services;

public class CarroService : ICarroService
{
    private readonly ICarroRepository _repositorio;

    public CarroService(ICarroRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<Carro>> ListarAsync()
    {
        return await _repositorio.ListarAsync();
    }

    public async Task<Carro?> ObterAsync(int id)
    {
        return await _repositorio.ObterAsync(id);
    }

    public async Task<Carro> CadastrarAsync(Carro carro)
    {
        await ValidarRegrasAsync(carro, idIgnorado: null);

        carro.Id = 0;
        _repositorio.Adicionar(carro);
        await _repositorio.SalvarAsync();

        return carro;
    }

    public async Task<Carro?> AtualizarAsync(Carro carroAtualizado)
    {
        var carro = await _repositorio.ObterAsync(carroAtualizado.Id);

        if (carro is null)
        {
            return null;
        }

        await ValidarRegrasAsync(carroAtualizado, idIgnorado: carroAtualizado.Id);

        carro.Placa = carroAtualizado.Placa;
        carro.Modelo = carroAtualizado.Modelo;
        carro.Ano = carroAtualizado.Ano;

        await _repositorio.SalvarAsync();

        return carro;
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var carro = await _repositorio.ObterAsync(id);

        if (carro is null)
        {
            return false;
        }

        _repositorio.Remover(carro);
        await _repositorio.SalvarAsync();

        return true;
    }

    private async Task ValidarRegrasAsync(Carro carro, int? idIgnorado)
    {
        // Regra 1: Placa preenchida e sem duplicidade
        if (string.IsNullOrWhiteSpace(carro.Placa))
        {
            throw new RegraDeNegocioException("A placa do carro é obrigatória.");
        }

        if (await _repositorio.ExistePlacaAsync(carro.Placa, idIgnorado))
        {
            throw new RegraDeNegocioException("Já existe um carro cadastrado com esta placa.");
        }

        // Regra 2: Modelo obrigatório
        if (string.IsNullOrWhiteSpace(carro.Modelo))
        {
            throw new RegraDeNegocioException("O modelo do carro é obrigatório.");
        }

        // Regra 3: Ano válido
        if (carro.Ano < 1900 || carro.Ano > DateTime.Now.Year + 1)
        {
            throw new RegraDeNegocioException("O ano do carro é inválido.");
        }
    }
}