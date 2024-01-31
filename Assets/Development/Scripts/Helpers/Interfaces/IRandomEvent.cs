using Unity.Netcode;

public interface IRandomEvent
{
    public void GetEvent(RandomEventSO randomEventSO);
    public bool IsRandomEven(bool trueOrFalse);
    public NetworkObject GetNetworkObject();

}

