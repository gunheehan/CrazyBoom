using Microsoft.AspNetCore.Mvc;
using MatchmakingServer.Services;
using MatchmakingServer.Models;
using System.Collections.Generic;

namespace MatchmakingServer.Controllers;

[ApiController]
[Route("[controller]")]
public class MatchmakingController : ControllerBase
{
    private readonly RoomService _roomService;

    public MatchmakingController(RoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpPost("create-room")]
    public IActionResult CreateRoom([FromQuery] string name, [FromQuery] int maxPlayers = 4)
    {
        var room = _roomService.CreateRoom(name, maxPlayers);
        return Ok(room);
    }

    [HttpPost("join-room")]
    public IActionResult JoinRoom([FromQuery] string roomId, [FromQuery] string playerId)
    {
        var room = _roomService.JoinRoom(roomId, playerId);
        if (room == null)
            return NotFound("Room not found or full");
        return Ok(room);
    }

    [HttpGet("rooms")]
    public IActionResult GetRooms() => Ok(_roomService.GetRooms());
}