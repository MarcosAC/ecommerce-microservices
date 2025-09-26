using Microsoft.AspNetCore.Mvc;
using Sales.Api.Services;
using Sales.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace Sales.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly PedidoService _service;

    public PedidosController(PedidoService service)
    {
        _service = service;
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Pedido pedido)
    {
        try
        {
            if (pedido == null || pedido.Itens == null || !pedido.Itens.Any())
                return BadRequest(new { message = "Pedido inválido ou sem itens." });

            var criado = await _service.CriarPedidoAsync(pedido);
            return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch
        {
            return StatusCode(500, new { message = "Erro interno ao criar pedido." });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var pedido = await _service.ObterPedidoPorIdAsync(id);
            if (pedido == null)
                return NotFound(new { message = $"Pedido {id} não encontrado." });

            return Ok(pedido);
        }
        catch
        {
            return StatusCode(500, new { message = "Erro interno ao buscar pedido." });
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var pedidos = await _service.ObterTodosPedidosAsync();
            return Ok(pedidos);
        }
        catch
        {
            return StatusCode(500, new { message = "Erro interno ao listar pedidos." });
        }
    }
}
