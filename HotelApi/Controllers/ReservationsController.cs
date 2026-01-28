using HotelApi.Models.DTOs;
using HotelApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    /// <summary>
    /// Create a new reservation
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequest request)
    {
        try
        {
            var reservation = await _reservationService.CreateReservationAsync(request);
            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("not available"))
                return Conflict(ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Cancel a reservation
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelReservation(Guid id)
    {
        try
        {
            await _reservationService.CancelReservationAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get a reservation by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReservation(Guid id)
    {
        var reservation = await _reservationService.GetReservationByIdAsync(id);
        if (reservation == null)
            return NotFound($"Reservation {id} not found.");

        return Ok(reservation);
    }

    /// <summary>
    /// Get a reservation by confirmation number
    /// </summary>
    [HttpGet("confirmation/{confirmationNumber}")]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByConfirmationNumber(string confirmationNumber)
    {
        var reservation = await _reservationService.GetReservationByConfirmationNumberAsync(confirmationNumber);
        if (reservation == null)
            return NotFound($"Reservation with confirmation number {confirmationNumber} not found.");

        return Ok(reservation);
    }

    /// <summary>
    /// Check in a guest
    /// </summary>
    [HttpPatch("{id:guid}/checkin")]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckIn(Guid id)
    {
        try
        {
            var reservation = await _reservationService.CheckInAsync(id);
            return Ok(reservation);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Check out a guest
    /// </summary>
    [HttpPatch("{id:guid}/checkout")]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckOut(Guid id)
    {
        try
        {
            var reservation = await _reservationService.CheckOutAsync(id);
            return Ok(reservation);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
