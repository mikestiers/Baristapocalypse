using Unity.Netcode;

public interface IRandomEvent
{
    public void SetEvent(RandomEvent randomEvent);
    public bool IsRandomEven(bool trueOrFalse);
    public NetworkObject GetNetworkObject();

}
