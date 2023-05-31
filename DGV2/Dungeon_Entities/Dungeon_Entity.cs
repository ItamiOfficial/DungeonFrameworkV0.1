using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Dungeon_Entity : MonoBehaviour
{
    // ==== Location & Movement ==== //
    [SerializeField]
    private Vector2Int Location;
    private const float WalkSpeed = 4.0f;
    public bool IsMoving { get; private set; }
    public Coroutine MovementCoroutine;

    private void Awake()
    {
        Dungeon_Manager.AddEntity(this);
    }
    public Vector2Int GetLocation()
    {
        return Location;
    }
    public void SetLocation(Vector2Int newLocation, bool UpdateInstantly)
    {
        Location = newLocation;
        if (UpdateInstantly)
        {
            if(MovementCoroutine != null)
                StopCoroutine(MovementCoroutine);
            
            transform.position = VecConversionLibrary.Vec2To3(newLocation);
            IsMoving = false;
        }
        else
        {
            MovementCoroutine = StartCoroutine(AnimateMovement(newLocation));    
        }
    }
    
    private IEnumerator AnimateMovement(Vector2Int newLocation)
    {
        Vector3 start = transform.position;
        Vector3 target = VecConversionLibrary.Vec2To3(newLocation);

        IsMoving = true;

        float alpha = 0.0f;

        while (alpha < 1)
        {
            alpha += Time.deltaTime * WalkSpeed;
            transform.position = Vector3.Lerp(start, target, alpha);
            yield return null;
        }

        transform.position = target;
        IsMoving = false;

        // Invoke some Event Here
    }
    public virtual void RemoveEntity()
    {
        Destroy(gameObject);
    }

    public virtual void InitEntity(Vector2Int spawnPosition)
    {
        Dungeon_Manager.AddEntity(this);
        SetLocation(spawnPosition,true);
    }
}
