using System;
using System.Collections;
using System.Collections.Generic;
using Seed.DecalProjector;
using UnityEngine;

[ExecuteInEditMode]
public class ProjectorPositioner : MonoBehaviour {
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private SDFCanvasController canvas;
    
    [SerializeField]
    private Vector3 bottomLeft;
    
    [SerializeField]
    private Vector3 topRight;

    [SerializeField]
    private float groundY = 0f;
    
    private void LateUpdate()
    {
        // Get the bottom left and top right corners of a rectangle which always fully contains the camera's view.
        bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));
    }

    private void OnDrawGizmos()
    {
        var corner1 = GetWorldPositionFromViewport(new(0, 0, 0), groundY);
        var corner2 = GetWorldPositionFromViewport(new(1, 0, 0), groundY);
        var corner3 = GetWorldPositionFromViewport(new(1, 1, 0), groundY);
        var corner4 = GetWorldPositionFromViewport(new(0, 1, 0), groundY);
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(corner1, 10f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(corner2, 10f);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(corner3, 10f);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(corner4, 10f);

        var bounds = new Bounds();
        bounds.Encapsulate(corner1);
        bounds.Encapsulate(corner2);
        bounds.Encapsulate(corner3);
        bounds.Encapsulate(corner4);
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
    
    private Vector3 GetWorldPositionFromViewport(Vector3 viewportPosition, float groundY) {
        var ray = mainCamera.ViewportPointToRay(viewportPosition);

        if (ray.direction.y != 0) {
            var t = (groundY - ray.origin.y) / ray.direction.y;
            return ray.origin + ray.direction * t;
        }

        return Vector3.zero;
    }
}
