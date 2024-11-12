public struct PlayerMessageToServer : Mirror.NetworkMessage
{
    public StateHub StateHub;
    public string Nickname;
}
