using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ColorsController : ControllerBase
{
    private readonly DataContext _context;

    // The database context is "injected" by the framework, giving us a connection.
    public ColorsController(DataContext context)
    {
        _context = context;
    }

    // GET: api/colors
    // Asynchronously gets all colors from the database, ordered by DisplayOrder.
    [HttpGet]
    public async Task<ActionResult<List<Color>>> GetColors()
    {
        var colors = await _context.Colors.OrderBy(c => c.DisplayOrder).ToListAsync();
        return Ok(colors);
    }

    // POST: api/colors
    // Asynchronously adds a new color to the database.
    [HttpPost]
    public async Task<ActionResult<Color>> AddColor([FromBody] Color newColor)
    {
        // Automatically set the display order for the new item.
        var maxOrder = await _context.Colors.AnyAsync() ? await _context.Colors.MaxAsync(c => c.DisplayOrder) : 0;
        newColor.DisplayOrder = maxOrder + 1;

        _context.Colors.Add(newColor);
        await _context.SaveChangesAsync(); // Saves the new color to the DB.

        return CreatedAtAction(nameof(GetColors), new { id = newColor.Id }, newColor);
    }

    // PUT: api/colors/{id}
    // Asynchronously finds a color by ID and updates its properties.
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateColor(int id, [FromBody] Color updatedColor)
    {
        var existingColor = await _context.Colors.FindAsync(id);
        if (existingColor == null)
        {
            return NotFound();
        }

        existingColor.ColorName = updatedColor.ColorName;
        existingColor.ColorHex = updatedColor.ColorHex;
        existingColor.Price = updatedColor.Price;
        existingColor.IsInStock = updatedColor.IsInStock;
        
        await _context.SaveChangesAsync(); // Saves the changes to the DB.
        return NoContent();
    }

    // DELETE: api/colors/{id}
    // Asynchronously finds a color by ID and removes it.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteColor(int id)
    {
        var colorToRemove = await _context.Colors.FindAsync(id);
        if (colorToRemove == null)
        {
            return NotFound();
        }

        _context.Colors.Remove(colorToRemove);
        await _context.SaveChangesAsync(); // Saves the deletion to the DB.
        
        return NoContent();
    }

    // POST: api/colors/updateorder
    // Asynchronously updates the DisplayOrder for a list of colors.
    [HttpPost("updateorder")]
    public async Task<IActionResult> UpdateOrder([FromBody] List<int> ids)
    {
        var colors = await _context.Colors.ToListAsync();
        
        for (int i = 0; i < ids.Count; i++)
        {
            // Find the color from the database that matches the current ID in the list.
            var color = colors.FirstOrDefault(c => c.Id == ids[i]);
            if (color != null)
            {
                // Update its order to match its new position.
                color.DisplayOrder = i + 1;
            }
        }

        await _context.SaveChangesAsync(); // Saves all the reordering changes at once.
        return Ok();
    }
}