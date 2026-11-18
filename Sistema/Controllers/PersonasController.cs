using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema.Dominio;
using System.ComponentModel.DataAnnotations;

[Route("api/[controller]")]
[ApiController]
public class PersonasController : ControllerBase
{
    private readonly SistemaContext _context;

    public PersonasController(SistemaContext context)
    {
        _context = context;
    }

    // GET: api/personas/listar
    [HttpGet("listar")]
    public async Task<ActionResult<IEnumerable<Persona>>> GetPersonas()
    {
        return await _context.Persona
            .AsNoTracking()
            .OrderBy(persona => persona.Id)
            .ToListAsync();
    }

    // POST: api/personas/crear
    [HttpPost("crear")]
    public async Task<ActionResult<Persona>> PostPersona([FromBody] PersonaDatos datos)
    {
        // Validar si ya existe una persona con el mismo CI
        if (await _context.Persona.AnyAsync(persona => persona.Ci == datos.Ci))
        {
            return BadRequest("Ya existe una persona con ese CI");
        }

        var persona = new Persona
        {
            Nombre = datos.Nombre,
            Ci = datos.Ci,
            Estado = datos.Estado ?? "Activo"
        };

        _context.Persona.Add(persona);
        await _context.SaveChangesAsync();

        return Ok(persona);
    }

    public record PersonaDatos(
        [Required] string Nombre,
        [Required] string Ci,
        string? Estado = null);
}