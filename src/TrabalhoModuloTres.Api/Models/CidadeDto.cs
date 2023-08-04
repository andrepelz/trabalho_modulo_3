namespace TrabalhoModuloTres.Api.Models;

public class CidadeDto
{
    public int CidadeId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;

    public ICollection<ClienteDto> Clientes { get; set; } = new List<ClienteDto>();
}