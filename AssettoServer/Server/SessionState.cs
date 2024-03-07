﻿using System.Collections.Generic;
using AssettoServer.Server.Configuration.Kunos;
using AssettoServer.Shared.Model;

namespace AssettoServer.Server;

public class SessionState
{
    public SessionConfiguration Configuration { get; }
    public long EndTimeMilliseconds { get; set; }
    public long StartTimeMilliseconds { get; set; }
    public int TimeLeftMilliseconds => Configuration.Infinite ? Configuration.Time * 60_000 : (int)(StartTimeMilliseconds + Configuration.Time * 60_000 - _timeSource.ServerTimeMilliseconds);
    public long SessionTimeMilliseconds => _timeSource.ServerTimeMilliseconds - StartTimeMilliseconds;
    public uint TargetLap { get; set; } = 0;
    public uint LeaderLapCount { get; set; } = 0;
    public bool LeaderHasCompletedLastLap { get; set; } = false;
    public bool IsStarted { get; set; } = false;
    public bool SessionOver
    {
        get
        {
            if (Configuration is { Type: SessionType.Practice, Infinite: true }) 
                return true;
            if (Configuration.Type == SessionType.Race)
                return EndTimeMilliseconds == 0 
                       && _timeSource.ServerTimeMilliseconds - StartTimeMilliseconds > Configuration.Time * 60_000;
            return TimeLeftMilliseconds < 0;
        }
    }

    public long OverTimeMilliseconds { get; set; } = 0;
    public bool HasSentRaceOverPacket { get; set; } = false;
    public long LastRaceStartUpdateMilliseconds { get; set; }
    public Dictionary<byte, EntryCarResult>? Results { get; set; }
    public IEnumerable<IEntryCar<IClient>>? Grid { get; set; }

    private readonly SessionManager _timeSource;

    public SessionState(SessionConfiguration configuration, SessionManager timeSource)
    {
        Configuration = configuration;
        _timeSource = timeSource;
    }
}
