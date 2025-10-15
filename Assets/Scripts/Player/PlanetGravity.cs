using System;
using System.Linq;
using UnityEngine;

public class PlanetGravity : MonoBehaviour
{
    [SerializeField] private Rigidbody thisRigid;
    [SerializeField] private Transform[] planets;

    private const float GRAVITATION = 5f;

    public void SetPlanets(Transform[] pList)
    {
        planets = pList.ToArray();
    }
    
    private void Update()
    {
        var gravity = CalcGravity();
        Move(gravity);
    }

    Vector3 CalcGravity()
    {
        Vector3 tempGravity = Vector3.zero;
        
        foreach (var planet in planets)
        {
            var direction = planet.transform.position - this.transform.position;

            if (tempGravity.magnitude == 0 || direction.magnitude < tempGravity.magnitude)
            {
                tempGravity = direction;
            }
        }

        return tempGravity.normalized * GRAVITATION;
    }
    
    void Move(Vector3 gravityDirection)
    {
        thisRigid.AddForce(gravityDirection,ForceMode.Acceleration);
    }
}
