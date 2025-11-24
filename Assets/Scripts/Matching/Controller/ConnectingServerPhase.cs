using Photon.Pun;

public class ConnectingServerPhase : IPhase
{
    protected override void OnInitializePhase()
    {
        PhotonNetwork.ConnectUsingSettings();
        SoundManager.Instance.PlayBGM(SoundManager.BGMData.BGMTYPE.Mating);
    }

    public override void OnConnectedToMaster()
    {
        _router.ChangePhase(PhaseType.Searching);
    }
}
