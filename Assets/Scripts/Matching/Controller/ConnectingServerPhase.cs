using Photon.Pun;

public class ConnectingServerPhase : IPhase
{
    protected override void OnInitializePhase()
    {
        PhotonNetwork.LocalPlayer.NickName = _router.Model.LocalPlayerName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        _router.ChangePhase(PhaseType.Searching);
    }
}
