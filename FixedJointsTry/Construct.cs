// By Maxim "RenViscoso" Levin

using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Construct : MonoBehaviour
{
    [SerializeField] protected CollisionController controller;
    [SerializeField] protected ColoredCube cubePrefab;
    [SerializeField] protected int cubesMin = 5;
    [SerializeField] protected int cubesMax = 9;
    [SerializeField] protected GameObject attractionPoint;
    [SerializeField] protected float attractionCoeff = 1.0f;
    protected Rigidbody RBody;
    protected List<int> Collisions = new List<int>();


    public Rigidbody Rigidbody => RBody;


    protected void Awake()
    {
        GenerateConstruct();
        SetRandomRotation();
    }


    protected void FixedUpdate()
    {
        // Clear collisions list
        Collisions.Clear();
        
        // Add attraction to attraction point
        Vector3 distance = GetDistanceToAttractor() * attractionCoeff;
        //Vector3 direction = distance.normalized;
        Vector3 velocity = Rigidbody.velocity;
        
        Rigidbody.AddForce(distance - velocity, ForceMode.Acceleration);

    }


    protected void GenerateConstruct()
    {
        List<ColoredCube> cubes = new List<ColoredCube>();
        List<Vector3> takedPositions = new List<Vector3>();
        
        // First position of construct is always in the center
        List<Vector3> freePositions = new List<Vector3>() { Vector3.zero };
        int cubesCount = Random.Range(cubesMin, cubesMax + 1);
        
        for (int i = 0; i < cubesCount; i += 1)
        {
            // Get the current position and add it to the list of used joints
            int positionIndex = Random.Range(0, freePositions.Count);
            Vector3 position = freePositions[positionIndex];
            takedPositions.Add(position);
            freePositions.Remove(position);

            // Generate a cube and set its position
            ColoredCube cube = Instantiate(cubePrefab, transform);
            cube.Rigidbody.Sleep();
            cubes.Add(cube);
            
            // Take first cube RigidBody as construct Rigidbody
            if (i == 0)
            {
                RBody = cube.Rigidbody;
                // This one should remove jittering effect, but it doesn't
                Rigidbody.mass = cubesCount * Rigidbody.mass;
            }
            // Connect the cubes with fixed joints to the first cube
            else
            {
                //HingeJoint joint = cubes[0].gameObject.AddComponent<HingeJoint>();
                
                FixedJoint joint = cubes[0].gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = cube.Rigidbody;
                joint.autoConfigureConnectedAnchor = false;
                joint.anchor = Vector3.zero;
                joint.connectedAnchor = position;
                
                // Settings for HingeJoint
                /*
                joint.useLimits = true;
                JointLimits limits = new JointLimits
                {
                    min = 0.0f,
                    max = 0.0f
                };
                joint.limits = limits;
                */
            }
            
            // Add collision ignore
            foreach (ColoredCube otherCube in cubes)
            {
                Physics.IgnoreCollision(cube.Collider, otherCube.Collider);
            }

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

        Rigidbody.WakeUp();
    }


    protected void SetRandomRotation()
    {
        Quaternion newRotation = Random.rotation;
        Rigidbody.rotation = newRotation;
    }


    public void MakeCollision(Construct otherConstruct)
    {

        int otherID = otherConstruct.gameObject.GetInstanceID();
        
        // Only one message in one frame
        if (Collisions.Contains(otherID))
        {
            return;
        }
        
        Collisions.Add(otherID);
        
        // Sort ID
        int selfID = gameObject.GetInstanceID();
        int firstID = Mathf.Min(selfID, otherID);
        int secondID = Mathf.Max(selfID, otherID);
        
        controller.AddScore(firstID, secondID);
    }


    private Vector3 GetDistanceToAttractor()
    {
        if (attractionPoint)
        {
            return attractionPoint.transform.position - Rigidbody.transform.position;
        }
        else
        {
            return Vector3.zero;
        }
    }

}
