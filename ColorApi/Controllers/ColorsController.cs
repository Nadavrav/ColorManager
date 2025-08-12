using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ColorsController : ControllerBase
{
    private static List<Color> _colors = new List<Color>
    {
        new Color { Id = 1, ColorName = "ורוד", ColorHex = "#ffc7ff", Price = 35, IsInStock = true, DisplayOrder = 1 },
        new Color { Id = 2, ColorName = "צהוב", ColorHex = "#f8ff21", Price = 42, IsInStock = true, DisplayOrder = 2 },
        new Color { Id = 3, ColorName = "כחול", ColorHex = "#0073ff", Price = 28, IsInStock = false, DisplayOrder = 3 }
    };

    [HttpGet]
    public ActionResult<List<Color>> GetColors()
    {
        return Ok(_colors.OrderBy(c => c.DisplayOrder).ToList());
    }

        // POST: api/colors
    // Adds a new color. The color data is sent in the request body.
    [HttpPost]
    public ActionResult<Color> AddColor([FromBody] Color newColor)
    {
        // In a real database, the ID would be generated automatically.
        // Here, we'll simulate it by finding the max ID and adding 1.
        newColor.Id = _colors.Max(c => c.Id) + 1;
        _colors.Add(newColor);
        
        // Return the newly created color with its new ID.
        return CreatedAtAction(nameof(GetColors), new { id = newColor.Id }, newColor);
    }

    // PUT: api/colors/{id}
    // Updates an existing color. The ID is in the URL, and new data is in the body.
    [HttpPut("{id}")]
    public IActionResult UpdateColor(int id, [FromBody] Color updatedColor)
    {
        var existingColor = _colors.FirstOrDefault(c => c.Id == id);
        if (existingColor == null)
        {
            return NotFound(); // Return 404 if the color doesn't exist.
        }

        // Update the properties of the existing color.
        existingColor.ColorName = updatedColor.ColorName;
        existingColor.ColorHex = updatedColor.ColorHex;
        existingColor.Price = updatedColor.Price;
        existingColor.IsInStock = updatedColor.IsInStock;
        // We don't update DisplayOrder here; that will be a separate bonus function.
        
        return NoContent(); // Return 204 No Content to indicate success.
    }

    // DELETE: api/colors/{id}
    // Deletes a color by its ID from the URL.
    [HttpDelete("{id}")]
    public IActionResult DeleteColor(int id)
    {
        var colorToRemove = _colors.FirstOrDefault(c => c.Id == id);
        if (colorToRemove == null)
        {
            return NotFound();
        }

        _colors.Remove(colorToRemove);
        
        return NoContent(); // Return 204 No Content to indicate success.
    }

    // POST: api/colors/updateorder
// Receives a list of IDs in their new order and updates the DisplayOrder property.
[HttpPost("updateorder")]
public IActionResult UpdateOrder([FromBody] List<int> ids)
{
    for (int i = 0; i < ids.Count; i++)
    {
        var color = _colors.FirstOrDefault(c => c.Id == ids[i]);
        if (color != null)
        {
            color.DisplayOrder = i + 1;
        }
    }
    return Ok();
}
}