namespace TrabalhoModuloTres.Api.Entities;

public class Cidade
{
    public int CidadeId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;

    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}