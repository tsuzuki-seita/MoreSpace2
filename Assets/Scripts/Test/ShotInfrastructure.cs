using System;
using R3;
using UnityEngine;

public interface IShotInfrastructure
{
    Observable<Unit> Shot { get; }
}

public class ShotInfrastructure : MonoBehaviour, IShotInfrastructure
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Subject<Unit> shotSubject = new Subject<Unit>();
    public Observable<Unit> Shot => shotSubject;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            shotSubject.OnNext(Unit.Default);
        }
    }

    public void OnDestroy()
    {
        shotSubject.Dispose();
    }
}
