using PROJ_P2.Models;
using PROJ_P2.Repositories;

namespace PROJ_P2.Services;

public class RegraDeNegocioException : Exception
{
    public RegraDeNegocioException(string mensagem) : base(mensagem)
    {
    }
}

/// <summary>
/// O "cozinheiro": conhece as regras do negócio.
/// Não sabe nada de HTTP nem de Entity Framework.
/// </summary>
public class OrdemServicoService : IOrdemServicoService
{
    private readonly IOrdemServicoRepository _repositorio;

    public OrdemServicoService(IOrdemServicoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<OrdemServico>> ListarAsync()
    {
        return await _repositorio.ListarAsync();
    }

    // Devolve null quando a OrdemServico não existe
    public async Task<OrdemServico?> ObterAsync(int id)
    {
        return await _repositorio.ObterAsync(id);
    }

    public async Task<OrdemServico> AbrirAsync(OrdemServico ordemServico)
    {
        await ValidarRegrasAsync(ordemServico, idIgnorado: null);

        ordemServico.Id = 0;
        _repositorio.Adicionar(ordemServico);
        await _repositorio.SalvarAsync();

        return ordemServico;
    }

    // Devolve null quando a OrdemServico não existe
    public async Task<OrdemServico?> FecharOrdemAsync(OrdemServico ordemServicoAtualizado)
    {
        var ordemServico = await _repositorio.ObterAsync(ordemServicoAtualizado.Id);

        if (ordemServico is null)
        {
            return null;
        }

        await ValidarRegrasAsync(ordemServicoAtualizado, idIgnorado: ordemServicoAtualizado.Id);

        ordemServico.NumeroOrdem = ordemServicoAtualizado.NumeroOrdem;
        ordemServico.Valor = ordemServicoAtualizado.Valor;
        ordemServico.Inicio = ordemServicoAtualizado.Inicio;
        ordemServico.Termino = ordemServicoAtualizado.Termino;
        ordemServico.MecanicoId = ordemServicoAtualizado.MecanicoId;
        ordemServico.CarroId = ordemServicoAtualizado.CarroId;

        await _repositorio.SalvarAsync();

        return ordemServico;
    }

    // Devolve false quando a OrdemServico não existe
    public async Task<bool> RemoverAsync(int id)
    {
        var ordemServico = await _repositorio.ObterAsync(id);

        if (ordemServico is null)
        {
            return false;
        }

        _repositorio.Remover(ordemServico);
        await _repositorio.SalvarAsync();

        return true;
    }

    // As regras de negócio moram aqui, num lugar só.
    // idIgnorado: no fechamento/atualização, a própria OrdemServico não conta como duplicada.
    private async Task ValidarRegrasAsync(OrdemServico ordemServico, int? idIgnorado)
    {
        // Regra 1: não permitir número de ordem duplicado
        if (await _repositorio.ExisteAsync(ordemServico.NumeroOrdem, idIgnorado))
        {
            throw new RegraDeNegocioException("Já existe uma Ordem de Serviço com esse número.");
        }

        // Regra 2: o valor não pode ser negativo
        if (ordemServico.Valor < 0)
        {
            throw new RegraDeNegocioException("O valor da Ordem de Serviço não pode ser negativo.");
        }

        // Regra 3: a data de término não pode ser anterior à data de início
        if (ordemServico.Termino is not null && ordemServico.Termino < ordemServico.Inicio)
        {
            throw new RegraDeNegocioException("A data de término não pode ser anterior à data de início.");
        }

        // Regra 4: mecânico e carro obrigatórios
        if (ordemServico.MecanicoId <= 0 || ordemServico.CarroId <= 0)
        {
            throw new RegraDeNegocioException("É obrigatório informar um mecânico e um carro válidos.");
        }
    }
}