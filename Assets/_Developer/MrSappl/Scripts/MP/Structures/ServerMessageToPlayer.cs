using Mirror;
public struct ServerMessageToPlayer : NetworkMessage
{
    public StateHub StateHub;
    public string Nickname;
}
