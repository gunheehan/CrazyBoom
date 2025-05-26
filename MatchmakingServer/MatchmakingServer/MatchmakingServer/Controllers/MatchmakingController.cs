using Microsoft.AspNetCore.Mvc;
using MatchmakingServer.Services;
using MatchmakingServer.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.HttpResults;

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
    public IActionResult CreateRoom([FromQuery] string name, [FromQuery] string playerid,
        [FromQuery] int maxPlayers = 4)
    {
        // 방 생성
        var room = _roomService.CreateRoom(name, maxPlayers);

        // 생성한 방에 방장(Player) 자동 참가
        var joinedRoom = _roomService.JoinRoom(room.Id, playerid);

        if (joinedRoom == null)
            return BadRequest("Failed to join the newly created room.");

        return Ok(joinedRoom);
    }

    [HttpPost("join-room")]
    public IActionResult JoinRoom([FromQuery] string roomId, [FromQuery] string playerId)
    {
        var room = _roomService.JoinRoom(roomId, playerId);
        if (room == null)
            return NotFound("Room not found or full");
        return Ok(room);
    }

    [HttpPost("leave-room")]
    public IActionResult LeaveRoom([FromQuery] string roomId, [FromQuery] string playerid)
    {
        var room = _roomService.LeaveRoom(roomId, playerid);
        if (room == null)
            return NotFound("Room not found or removed");
        return Ok(room);
    }

    [HttpGet("rooms")]
    public IActionResult GetRooms() => Ok(_roomService.GetRooms());
}