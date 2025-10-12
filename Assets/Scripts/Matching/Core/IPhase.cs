using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public abstract class IPhase : MonoBehaviourPunCallbacks
{
    public PhaseType phaseType;
    protected PhaseRouter _router;

    public void InitializeOnStart(PhaseRouter router)
    {
        _router = router;
        this.gameObject.SetActive(false);
        OnStart();
    }
    protected virtual void OnStart(){}

    public void InitializePhase()
    {
        this.gameObject.SetActive(true);
        OnInitializePhase();
    }
    public void EndPhase(){
        OnEndPhase();
        this.gameObject.SetActive(false);
    }

    
    protected virtual void OnInitializePhase(){ }
    public virtual void OnUpdatePhase(){ }
    protected virtual void OnEndPhase() { }
    
    public override void OnDisconnected(DisconnectCause cause)
    {
        _router.Model.DisconnectCause = cause;
        _router.ChangePhase(PhaseType.Failed);
    }
}

public enum PhaseType
{
    ConnectServer,
    Searching,
    Room,
    Failed
}