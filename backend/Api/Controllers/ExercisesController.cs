using Application.Common.Interfaces.Repository;
using Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/exercises")]
[ApiController]
public class ExercisesController(IExerciseRepository exerciseRepository) : ControllerBase
{
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetExercises()
    {
        var exercises = await _exerciseRepository.GetAllAsync();
        var dtos = exercises.Select(e => new ExerciseDto
        {
            Id = e.Id,
            Name = e.Name,
            MuscleGroup = e.MuscleGroup.ToString(),
            Difficulty = e.Difficulty,
            Description = e.Description
        }).ToList();
        return Ok(dtos);
    }
}
