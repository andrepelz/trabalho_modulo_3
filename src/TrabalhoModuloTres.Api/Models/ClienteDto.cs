namespace TrabalhoModuloTres.Api.Models;

public class ClienteDto
{
    public int ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
}   