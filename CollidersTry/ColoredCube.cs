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
    protected Collider _collider;


    public Collider Collider => _collider;


    protected void Awake()
    {
        _collider = GetComponent<Collider>();
        Mesh = GetComponent<MeshRenderer>();
        Mesh.material = commonMaterial;
    }


    public void Hit()
    {
        Mesh.material = hitMaterial;
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
