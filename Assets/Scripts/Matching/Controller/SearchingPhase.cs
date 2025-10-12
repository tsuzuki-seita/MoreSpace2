using Photon.Pun;
using Photon.Realtime;

public class SearchingPhase : IPhase
{
    protected override void OnInitializePhase()
    {
        PhotonNetwork.JoinRoom(_router.Model.RoomName);
    }

    public override void OnJoinedRoom()
    {
        _router.ChangePhase(PhaseType.Room);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(_router.Model.RoomName, new RoomOptions() { MaxPlayers = 2 });
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _router.Model.DisconnectCause = DisconnectCause.None;
        _router.ChangePhase(PhaseType.Failed);
    }
}
