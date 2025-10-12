using Photon.Realtime;

public class PhaseModel
{
    public string LocalPlayerName;
    public string RoomName;
    public bool[] IsPlayerReady = new bool[2];
    public DisconnectCause DisconnectCause;
}