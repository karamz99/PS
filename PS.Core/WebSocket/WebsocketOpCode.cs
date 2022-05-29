namespace PS.Core
{
    public enum WebsocketOpCode : byte
    {
        Continuation,
        Text,
        Binary,
        ConnectionClose = 8,
        Ping,
        Pong,
    }
}
