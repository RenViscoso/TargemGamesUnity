// By Maxim "RenViscoso" Levin

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionController : MonoBehaviour
{
    [SerializeField] protected int score = 0;
    [SerializeField] protected float pastTime = 0.0f;
    [SerializeField] protected Text textScore;
    [SerializeField] protected Text textTime;
    protected List<List<int>> Collisions = new List<List<int>>();


    public int Score
    {
        get => score;
        set
        {
            score = value;
            if (textScore)
            {
                textScore.text = $"Столкновений произошло: {Mathf.Floor(score)}";
            }
        }
    }


    public float PastTime
    {
        get => pastTime;
        set
        {
            pastTime = value;
            if (textTime)
            {
                textTime.text = $"Времени прошло: {Mathf.Floor(pastTime)}";
            }
        }
    }

    
    private void Awake()
    {
        Clear();
    }
    

    private void Update()
    {
        PastTime += Time.deltaTime;
    }


    private void FixedUpdate()
    {
        Collisions.Clear();
    }


    public void AddScore(int firstID, int secondID)
    {
        // Only one collision with two same objects in one frame
        List<int> currentCollision = new List<int>()
        {
            firstID,
            secondID
        };
        foreach (List<int> collision in Collisions)
        {
            if (collision.Equals(currentCollision))
            {
                return;
            }
        }
        
        Collisions.Add(currentCollision);
        Score += 1;
    }

    
    public void Clear()
    {
        Score = 0;
        PastTime = 0.0f;
    }
}
