using JetBrains.Annotations;
using UnityEngine;

public interface IShotPresentation
{
    public void Shot(Amo amo);
}

public class ShotPresentation : MonoBehaviour, IShotPresentation
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Shot(Amo amo)
    {
        Debug.Log($"Shot! {amo}");
    }
}
