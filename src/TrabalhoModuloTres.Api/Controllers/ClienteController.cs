using System.Text.Json;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using TrabalhoModuloTres.Api.DbContexts;
using TrabalhoModuloTres.Api.Entities;
using TrabalhoModuloTres.Api.Models;
using TrabalhoModuloTres.Shared;

namespace TrabalhoModuloTres.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClienteController : ControllerBase
{
    private readonly ClienteContext _context;
    private readonly IConnectionMultiplexer _redis;
    private readonly IPublishEndpoint _endpoint;

    public ClienteController(ClienteContext context, IConnectionMultiplexer redis, IPublishEndpoint endpoint)
    {
        _context = context;
        _redis = redis;
        _endpoint = endpoint;
    }

    [HttpGet("{cidadeId}")]
    public async Task<IActionResult> RetornaClientesCidadeId(int cidadeId)
    {
        var cache = _redis.GetDatabase();

        var resultFromCache = await cache.StringGetAsync($"clientes_cidade_{cidadeId}");

        if (!resultFromCache.IsNull)
        {
            // return Ok(
            //     JsonSerializer.Deserialize<IEnumerable<string>>(resultFromCache.ToString())
            // );
            return Ok(resultFromCache.ToString());
        }

        var result = _context.Clientes.Where(x => x.CidadeId == cidadeId).ToList();

        if (result.Count == 0) return NotFound();

        var nomesClientes = result.Select(cliente => cliente.Nome);

        var cacheResultSet = JsonSerializer.Serialize(nomesClientes);
        await cache.StringSetAsync($"clientes_cidade_{cidadeId}", cacheResultSet);
        await cache.KeyExpireAsync($"clientes_cidade_{cidadeId}", TimeSpan.FromSeconds(120));

        return Ok(nomesClientes);
    }

	[HttpGet("uf/{uf}")]
    public async Task<IActionResult> RetornaClientesUF(string uf)
    {
        int soma;
        List<Cliente>? clientes;
        List<ClienteDto>? clientesDto;

        var cache = _redis.GetDatabase();

        var resultFromCache = await cache.StringGetAsync($"clientes_uf_{uf}");

        if(!resultFromCache.IsNull)
        {
            clientesDto = JsonSerializer.Deserialize<List<ClienteDto>>(resultFromCache.ToString());
            soma = clientesDto!.Count;

            if(soma > 10) await Notify(clientesDto);

            return Ok(soma);
            // return Ok(-1);
        }

        var cidades = _context.Cidades.Include(c => c.Clientes).Where(x => x.UF == uf).ToList();

        if(cidades.Count == 0) return NotFound();

        clientes = cidades.SelectMany(cidade => cidade.Clientes).ToList();

        soma = clientes.Count;

        clientesDto = clientes.Select(c => new ClienteDto { 
            ClienteId = c.ClienteId, 
            Nome = c.Nome, 
            Telefone = c.Telefone, 
            Endereco = c.Endereco
        }).ToList();

        if(soma > 10) await Notify(clientesDto);

        await cache.StringSetAsync($"clientes_uf_{uf}", JsonSerializer.Serialize(clientesDto));
        await cache.KeyExpireAsync($"clientes_uf_{uf}", TimeSpan.FromSeconds(120));
        
        return Ok(soma);
    }

    private async Task Notify(List<ClienteDto> clientes)
    {
        var notificationDto = new List<NotificationDto>();

        notificationDto.AddRange(clientes.Select(x => new NotificationDto {Nome = x.Nome, Endereco = x.Endereco}));

        await _endpoint.Publish<IMessage>(new {
            Date = DateTime.Now,
            Message = notificationDto,
            Author = "TrabalhoModuloTres"
        });
    }
}