using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Jeeroway.Api.controllers;

[ApiController]
[Route("api/video")]
public class VideoStreamController : ControllerBase
{
    private readonly Channel<byte[]> _frameChannel;
    private readonly RecordingSessionManager _recorder;
    private readonly LiveFrameBroadcaster _broadcaster;

    public VideoStreamController(Channel<byte[]> frameChannel, RecordingSessionManager recorder, LiveFrameBroadcaster broadcaster)
    {
        _frameChannel = frameChannel;
        _recorder = recorder;
        _broadcaster = broadcaster;
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

    [HttpGet("current-session")]
    public IActionResult GetCurrentSession()
    {
        return Ok(new { sessionId = _recorder.CurrentSessionId });
    }

    // Live MJPEG stream from in-memory broadcaster
    [HttpGet("live.mjpeg")]
    public async Task LiveMjpeg([FromQuery] Guid roboId)
    {
        Response.StatusCode = 200;
        Response.Headers["Cache-Control"] = "no-cache";
        Response.ContentType = "multipart/x-mixed-replace; boundary=frame";

        var subscription = _broadcaster.Subscribe(roboId);
        try
        {
            await foreach (var frame in subscription.Reader.ReadAllAsync(HttpContext.RequestAborted))
            {
                if (HttpContext.RequestAborted.IsCancellationRequested) break;
                var header = "--frame\r\n" +
                             "Content-Type: image/jpeg\r\n" +
                             $"Content-Length: {frame.Length}\r\n\r\n";
                var headerBytes = Encoding.ASCII.GetBytes(header);
                await Response.Body.WriteAsync(headerBytes, 0, headerBytes.Length, HttpContext.RequestAborted);
                await Response.Body.WriteAsync(frame, 0, frame.Length, HttpContext.RequestAborted);
                var nl = Encoding.ASCII.GetBytes("\r\n");
                await Response.Body.WriteAsync(nl, 0, nl.Length, HttpContext.RequestAborted);
                await Response.Body.FlushAsync(HttpContext.RequestAborted);
            }
        }
        finally
        {
            _broadcaster.Unsubscribe(roboId, subscription);
        }
    }

    // Replay recorded session as MJPEG stream (simple, one pass)
    [HttpGet("sessions/{sessionId}/mjpeg")]
    public async Task StreamSession(string sessionId, [FromQuery] int fps = 15)
    {
        var delay = TimeSpan.FromMilliseconds(Math.Max(1, 1000 / Math.Max(1, fps)));
        Response.StatusCode = 200;
        Response.Headers["Cache-Control"] = "no-cache";
        Response.ContentType = "multipart/x-mixed-replace; boundary=frame";

        var files = _recorder.ListFrames(sessionId).OrderBy(x => x, StringComparer.Ordinal).ToArray();
        foreach (var file in files)
        {
            if (HttpContext.RequestAborted.IsCancellationRequested) break;
            var path = _recorder.GetFramePath(sessionId, file);
            if (!System.IO.File.Exists(path)) continue;
            var frame = await System.IO.File.ReadAllBytesAsync(path, HttpContext.RequestAborted);
            var header = "--frame\r\n" +
                         "Content-Type: image/jpeg\r\n" +
                         $"Content-Length: {frame.Length}\r\n\r\n";
            var headerBytes = Encoding.ASCII.GetBytes(header);
            await Response.Body.WriteAsync(headerBytes, 0, headerBytes.Length, HttpContext.RequestAborted);
            await Response.Body.WriteAsync(frame, 0, frame.Length, HttpContext.RequestAborted);
            var nl = Encoding.ASCII.GetBytes("\r\n");
            await Response.Body.WriteAsync(nl, 0, nl.Length, HttpContext.RequestAborted);
            await Response.Body.FlushAsync(HttpContext.RequestAborted);
            try { await Task.Delay(delay, HttpContext.RequestAborted); } catch { break; }
        }
    }
}
