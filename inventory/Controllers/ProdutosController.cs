using Microsoft.AspNetCore.Mvc;
using Inventory.Api.Services;
using Inventory.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProdutosController : ControllerBase
{
    private readonly IEstoqueService _estoque;

    public ProdutosController(IEstoqueService estoque)
    {
        _estoque = estoque;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Produto produto)
    {
        var produtoAdionado = await _estoque.AdicionarProdutoAsync(produto);
        return CreatedAtAction(nameof(GetById), new { id = produtoAdionado.Id }, produtoAdionado);
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _estoque.ObterProdutosAsync());


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var produto = await _estoque.ObterPorIdAsync(id);
        if (produto == null) return NotFound();
        return Ok(produto);
    }

    [AllowAnonymous]
    [HttpPost("reduzir")]
    public async Task<IActionResult> Reduzir([FromBody] ReduzirEstoqueDto reduzirEstoqueDto)
    {
        var ok = await _estoque.ReduzirEstoqueAsync(reduzirEstoqueDto.ProdutoId, reduzirEstoqueDto.Quantidade);
        if (!ok) return BadRequest(new { message = "Estoque insuficiente ou produto não encontrado" });
        return Ok();
    }

    public record ReduzirEstoqueDto(int ProdutoId, int Quantidade);
}