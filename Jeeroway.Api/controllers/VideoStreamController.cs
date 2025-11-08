using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;

namespace Jeeroway.Api.controllers;

[ApiController]
[Route("video")]
public class VideoStreamController : ControllerBase
{
    private readonly Channel<byte[]> _frameChannel;
    private readonly RecordingSessionManager _recorder;

    public VideoStreamController(Channel<byte[]> frameChannel, RecordingSessionManager recorder)
    {
        _frameChannel = frameChannel;
        _recorder = recorder;
    }

    [HttpPost("start-recording")]
    public IActionResult StartRecording()
    {
        var sessionId = _recorder.StartSession();
        return Ok(new { sessionId });
    }

    [HttpPost("stop-recording")]
    public IActionResult StopRecording()
    {
        _recorder.StopSession();
        return Ok("Recording stopped");
    }

    [HttpGet("sessions")]
    public IActionResult GetSessions()
    {
        return Ok(_recorder.ListSessions());
    }

    [HttpGet("sessions/{sessionId}")]
    public IActionResult GetSessionFrames(string sessionId)
    {
        return Ok(_recorder.ListFrames(sessionId));
    }

    [HttpGet("sessions/{sessionId}/frames/{fileName}")]
    public IActionResult GetFrame(string sessionId, string fileName)
    {
        var path = _recorder.GetFramePath(sessionId, fileName);
        if (!System.IO.File.Exists(path))
            return NotFound();
        return PhysicalFile(path, "image/jpeg");
    }
}
