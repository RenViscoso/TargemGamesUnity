// By Maxim "RenViscoso" Levin

using System;
using System.Collections.Generic;
using UnityEngine;

public class ColoredCube : MonoBehaviour
{
    [SerializeField] protected Material commonMaterial;
    [SerializeField] protected Material hitMaterial;
    [SerializeField] protected List<GameObject> connectPositions;
    protected MeshRenderer Mesh;
    protected Rigidbody RBody;
    protected Construct construct;
    protected Collider _collider;


    public Construct Construct => construct;
    

    public Rigidbody Rigidbody => RBody;


    public Collider Collider => _collider;


    protected void Awake()
    {
        construct = GetComponentInParent<Construct>();
        _collider = GetComponent<Collider>();
        Mesh = GetComponent<MeshRenderer>();
        Mesh.material = commonMaterial;
        RBody = GetComponent<Rigidbody>();
    }


    protected void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Figure"))
        {
            return;
        }
        
        // Change material
        Mesh.material = hitMaterial;
        
        // Send collision to parent
        Construct otherConstruct = collision.gameObject.GetComponent<ColoredCube>().Construct;
        Construct.MakeCollision(otherConstruct);

        // Push away another figure (impulse modifier is temporary)
        Vector3 impulse = Rigidbody.velocity * -5.0f;
        Vector3 impulseNormal = collision.relativeVelocity.normalized;
        collision.rigidbody.AddForce(Vector3.Reflect(impulse, impulseNormal), ForceMode.Impulse);
    }


    public List<Vector3> GetConnectPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        int positionsCount = connectPositions.Count;
        if (positionsCount == 0)
        {
            return null;
        }
        for (int i = 0; i < positionsCount; i += 1)
        {
            positions.Add(connectPositions[i].transform.localPosition);
        }
        return positions;
    }
    
}
