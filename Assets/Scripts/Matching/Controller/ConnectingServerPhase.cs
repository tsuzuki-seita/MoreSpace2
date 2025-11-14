using Photon.Pun;

public class ConnectingServerPhase : IPhase
{
    protected override void OnInitializePhase()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        _router.ChangePhase(PhaseType.Searching);
    }
}
