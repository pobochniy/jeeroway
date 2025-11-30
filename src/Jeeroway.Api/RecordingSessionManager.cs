namespace Jeeroway.Api;

public class RecordingSessionManager
{
    private string? _currentSessionPath;
    private int _frameCounter = 0;
    private string? _currentSessionId;

    public string? CurrentSessionPath => _currentSessionPath;
    public string? CurrentSessionId => _currentSessionId;

    public string StartSession()
    {
        var sessionId = $"session-{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}";
        var sessionPath = Path.Combine("recordings", sessionId);
        Directory.CreateDirectory(sessionPath);
        _currentSessionPath = sessionPath;
        _currentSessionId = sessionId;
        _frameCounter = 0;
        return sessionId;
    }

    public void StopSession()
    {
        _currentSessionPath = null;
        _currentSessionId = null;
        _frameCounter = 0;
    }

    public void SaveFrame(byte[] frame)
    {
        if (_currentSessionPath == null)
            return;

        var filename = $"frame_{_frameCounter++.ToString("D6")}.jpg";
        var path = Path.Combine(_currentSessionPath, filename);
        File.WriteAllBytes(path, frame);
    }

    public IEnumerable<string> ListSessions()
    {
        return Directory.Exists("recordings") 
            ? Directory.GetDirectories("recordings").Select(Path.GetFileName) 
            : Enumerable.Empty<string>();
    }

    public IEnumerable<string> ListFrames(string sessionId)
    {
        var path = Path.Combine("recordings", sessionId);
        if (!Directory.Exists(path)) yield break;
        foreach (var file in Directory.GetFiles(path, "*.jpg"))
            yield return Path.GetFileName(file);
    }

    public string GetFramePath(string sessionId, string frameFile)
    {
        return Path.Combine("recordings", sessionId, frameFile);
    }
}

