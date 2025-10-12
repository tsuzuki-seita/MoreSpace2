using System.Collections.Generic;
using UnityEngine;

public class PhaseRouter : MonoBehaviour
{
    public PhaseModel Model;
    private IPhase nowPhase;
    [SerializeField] private List<IPhase> phases;
    
    private void Start()
    {
        Model = new PhaseModel() { LocalPlayerName = "testPlayer", RoomName = "room" };
        foreach (var phase in phases)
            phase.InitializeOnStart(this);
        ChangePhase(PhaseType.ConnectServer);
    }

    private void Update()
    {
        nowPhase?.OnUpdatePhase();
    }

    public void ChangePhase(PhaseType phase)
    {
        IPhase toPhase = phases.Find(p => p.phaseType == phase);
        nowPhase?.EndPhase();
        nowPhase = toPhase;
        toPhase?.InitializePhase();
    }
}
