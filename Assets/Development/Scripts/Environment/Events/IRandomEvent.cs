using Unity.Netcode;

public interface IRandomEvent
{
    public void SetEvent(RandomEvent randomEvent);
    public bool IsRandomEven();
    public NetworkObject GetNetworkObject();
}
