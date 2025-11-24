using Photon.Pun;
using Photon.Realtime;

public class SearchingPhase : IPhase
{
    protected override void OnInitializePhase()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 2) _router.ChangePhase(PhaseType.Room);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _router.ChangePhase(PhaseType.Room);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(_router.Model.RoomName + PhotonNetwork.CountOfRooms, new RoomOptions() { MaxPlayers = 2 });
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _router.Model.DisconnectCause = DisconnectCause.None;
        _router.ChangePhase(PhaseType.Failed);
    }
}
