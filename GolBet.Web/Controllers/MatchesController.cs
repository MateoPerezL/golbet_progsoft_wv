using GolBet.Entities.Enums;
using GolBet.Repositories.Data;
using GolBet.Services.DTOs;
using GolBet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GolBet.Web.Controllers;

public class MatchesController : Controller
{
    private readonly IMatchService _matchService;
    private readonly ITeamService _teamService;

    public MatchesController(
        IMatchService matchService,
        ITeamService teamService)
    {
        _matchService = matchService;
        _teamService = teamService;
    }

    // Público: cualquier visitante puede ver la cartelera.
    public async Task<IActionResult> Index(
        MatchStatus? status)
    {
        ViewBag.CurrentStatus = status;

        var board =
            await _matchService.GetBoardAsync(status);

        return View(board);
    }

    // Público: cualquier visitante puede ver el detalle.
    public async Task<IActionResult> Detail(int id)
    {
        var match =
            await _matchService.GetDetailAsync(id);

        if (match is null)
        {
            return NotFound();
        }

        return View(match);
    }

    // Solo Admin: mostrar formulario de creación.
    [Authorize(Roles = DbSeeder.AdminRole)]
    public async Task<IActionResult> Create()
    {
        await LoadTeamsAsync();

        return View(new MatchFormDto());
    }

    // Solo Admin: guardar partido.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = DbSeeder.AdminRole)]
    public async Task<IActionResult> Create(
        MatchFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadTeamsAsync();

            return View(dto);
        }

        try
        {
            await _matchService.CreateAsync(dto);

            TempData["Success"] =
                "Partido creado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            await LoadTeamsAsync();

            return View(dto);
        }
    }

    // Solo Admin: mostrar formulario de edición.
    [Authorize(Roles = DbSeeder.AdminRole)]
    public async Task<IActionResult> Edit(int id)
    {
        var dto =
            await _matchService.GetForEditAsync(id);

        if (dto is null)
        {
            return NotFound();
        }

        await LoadTeamsAsync();

        return View(dto);
    }

    // Solo Admin: actualizar partido.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = DbSeeder.AdminRole)]
    public async Task<IActionResult> Edit(
        MatchFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadTeamsAsync();

            return View(dto);
        }

        try
        {
            await _matchService.UpdateAsync(dto);

            TempData["Success"] =
                "Partido actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            await LoadTeamsAsync();

            return View(dto);
        }
    }

    // Solo Admin: desactivar partido.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = DbSeeder.AdminRole)]
    public async Task<IActionResult> Deactivate(int id)
    {
        await _matchService.DeactivateAsync(id);

        TempData["Success"] =
            "Partido desactivado.";

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadTeamsAsync()
    {
        var teams = await _teamService.GetAllAsync();

        ViewBag.Teams = new SelectList(
            teams,
            "Id",
            "Name");
    }
}
