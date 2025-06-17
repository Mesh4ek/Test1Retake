using Microsoft.AspNetCore.Mvc;
using Test1Retake.Contracts.Requests;
using Test1Retake.Contracts.Responses;
using Test1Retake.Exceptions;
using Test1Retake.Services.Abstractions;

namespace Test1Retake.Controllers;

[ApiController]
[Route("api/movies")]
public class MovieController(IMovieService movieService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<MovieWithDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMovies([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken token = default)
    {
        try
        {
            var filter = new MovieFilterDto { From = from, To = to };
            var response = await movieService.GetMoviesAsync(filter, token);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    
    [HttpPost("assign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignActorToMovie([FromBody] AssignActorRequest request, CancellationToken token = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await movieService.AssignActorToMovieAsync(request, token);
            return NoContent();
        }
        catch (MovieExceptions.MovieNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (MovieExceptions.ActorNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (MovieExceptions.ActorAlreadyAssignedException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}