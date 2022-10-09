// By Maxim "RenViscoso" Levin

using System;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class Construct : MonoBehaviour
{
    [SerializeField] protected CollisionController controller;
    [SerializeField] protected ColoredCube cubePrefab;
    [SerializeField] protected int cubesMin = 10;
    [SerializeField] protected int cubesMax = 15;
    [SerializeField] protected GameObject attractionPoint;
    [SerializeField] protected float attractionCoeff = 3.0f;
    [SerializeField] protected float bouncingMultiplier = 5.0f;
    [SerializeField] protected float maxBounce = 30.0f;
    protected Rigidbody RBody;
    // Костыль
    protected bool wasHit = false;


    protected void Awake()
    {
        RBody = GetComponent<Rigidbody>();
        GenerateConstruct();
        SetRandomRotation();
    }


    protected void FixedUpdate()
    {
        // Add attraction
        Vector3 distance = GetDistanceToAttractor(); 
        Vector3 attractionForce = distance * attractionCoeff;
        Vector3 velocity = RBody.velocity;
        
        RBody.AddForce(attractionForce - velocity, ForceMode.Acceleration);

        wasHit = false;
    }


    protected void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Construct"))
        {
            return;
        }

        if (!collision.rigidbody)
        {
            return;
        }

        if (wasHit)
        {
            return;
        }
        
        int collisionPoints = collision.contactCount;
        Vector3 collisionImpulse = (RBody.velocity / collisionPoints) * bouncingMultiplier;
        collisionImpulse = Vector3.ClampMagnitude(collisionImpulse, maxBounce);

        for (int i = 0; i < collisionPoints; i += 1)
        {
            ContactPoint contact = collision.GetContact(i);
            
            // Apply impulse
            Vector3 collisionNormal = contact.normal;
            Vector3 apply = Vector3.Reflect(collisionImpulse, collisionNormal);
            collision.rigidbody.AddForce(apply, ForceMode.Impulse);
            
            // Change cube color
            contact.otherCollider.gameObject.GetComponent<ColoredCube>().Hit();
        }

        wasHit = true;
        controller.AddScore(gameObject.GetInstanceID(), collision.gameObject.GetInstanceID());
    }


    protected void GenerateConstruct()
    {
        List<Vector3> takedPositions = new List<Vector3>();
        
        // First position of construct is always in the center
        List<Vector3> freePositions = new List<Vector3>() { Vector3.zero };
        int cubesCount = Random.Range(cubesMin, cubesMax + 1);

        for (int i = 0; i < cubesCount; i += 1)
        {
            // Get the current position and add it to the list of used positions
            int positionIndex = Random.Range(0, freePositions.Count);
            Vector3 position = freePositions[positionIndex];
            takedPositions.Add(position);
            freePositions.Remove(position);

            // Generate a cube and set its position
            ColoredCube cube = Instantiate(cubePrefab, transform);
            cube.transform.localPosition = position;

            // Add new positions for cubes (but not taken)
            List<Vector3> additionalPositions = cube.GetConnectPositions();
            if (additionalPositions != null)
            {
                foreach (Vector3 newPositionOffset in additionalPositions)
                {
                    Vector3 newPosition = newPositionOffset + position;
                    if (takedPositions.Contains(newPosition))
                    {
                        continue;
                    }

                    freePositions.Add(newPosition);
                }
            }

            // Avoid the probability of getting an exception
            if (freePositions.Count == 0)
            {
                break;
            }
        }
        
        RBody.centerOfMass = Vector3.zero;
    }


    protected void SetRandomRotation()
    {
        Quaternion newRotation = Random.rotation;
        RBody.rotation = newRotation;
    }


    private Vector3 GetDistanceToAttractor()
    {
        if (attractionPoint)
        {
            return attractionPoint.transform.position - RBody.transform.position;
        }
        else
        {
            return Vector3.zero;
        }
    }

}
