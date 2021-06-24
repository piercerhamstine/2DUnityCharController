using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    // TODO:
    // Implement slopes
    // Implement continuous mode (always check for collisions from every direction.)
    //---------------------------------------------------------------------------------


    #region Collision info and masks
    // Controller Collision Info
    public struct ColInfo
    {
        public bool right, left;
        public bool top, bottom;
        public bool becameGroundedLastFrame;
        public bool groundedLastFrame;

        public void ResetColInfo()
        {
            right = left = false;
            top = bottom = false;
            becameGroundedLastFrame = false;
        }
    }
    public ColInfo colInfo;

    BoxCollider2D col;
    // The layer that the player collides with.
    public LayerMask colMask;
    #endregion

    #region Ray variables
    struct RayOrigins
    {
        public Vector2 botLeft, botRight;
        public Vector2 topLeft;
    }
    RayOrigins rayOrigins;

    // The amount that the ray should be fired from within the controllers own bounds.
    public float insetWidth = 0.02f;
    public float insetTimesFactor = -2;

    // Number of rays fired.
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    // Spacing between each ray.
    private float verticalRaySpacing;
    private float horizontalRaySpacing;
    #endregion

    public Vector2 velocityLastFrame;

    #region Events
    public event Action<RaycastHit2D> onCollide;
    public event Action<Collider2D> onTriggerEnter;
    public event Action<Collider2D> onTriggerExit;
    #endregion

    #region Monobehaviour Overrides
    private void Start()
    {
        col = GetComponent<BoxCollider2D>();
        CalculateRaySpacings();
    }
    #endregion

    #region Event Handling
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(onTriggerEnter != null)
        {
            onTriggerEnter(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(onTriggerExit != null)
        {
            onTriggerExit(collision);
        }
    }
    #endregion

    #region Ray spacing and origins
    private void CalculateRaySpacings()
    {
        Bounds bounds = col.bounds;

        bounds.Expand(insetWidth * insetTimesFactor);

        verticalRaySpacing = (col.size.x - (2*insetWidth)) / (verticalRayCount - 1);
        horizontalRaySpacing = (col.size.y - (2*insetWidth)) / (horizontalRayCount - 1);
    }

    private void CalculateRayOrigins()
    {
        Bounds bounds = col.bounds;

        bounds.Expand(insetWidth* insetTimesFactor);

        rayOrigins.botLeft = new Vector2(bounds.min.x, bounds.min.y);
        rayOrigins.botRight = new Vector2(bounds.max.x, bounds.min.y);
        rayOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
    }
    #endregion

    #region Controller Movement
    public void Move(Vector2 moveVector)
    {
        colInfo.groundedLastFrame = colInfo.bottom;

        CalculateRayOrigins();
        colInfo.ResetColInfo();

        if(moveVector.x != 0)
        {
            HorizontalMove(ref moveVector);
        }

        if (moveVector.y != 0)
        {
            VerticalMove(ref moveVector);
        }

        transform.Translate(moveVector);

        if(!colInfo.groundedLastFrame && colInfo.bottom)
        {
            colInfo.becameGroundedLastFrame = true;
        }

        velocityLastFrame = moveVector;
    }
    #endregion

    #region Controller Collision Detection
    private void HorizontalMove(ref Vector2 moveVector)
    {
        Vector2 rayDir = (Mathf.Sign(moveVector.x) == -1) ? Vector2.left : Vector2.right;
        float rayDist = Mathf.Abs(moveVector.x) + insetWidth;

        Vector2 rayOrigin = (rayDir.x == -1) ? rayOrigins.botLeft : rayOrigins.botRight;

        for (int i = 0; i < horizontalRayCount; ++i)
        {
            Vector2 ray = new Vector2(rayOrigin.x, rayOrigin.y + i * horizontalRaySpacing);

            Debug.DrawRay(ray, rayDir * rayDist, Color.red);

            RaycastHit2D hitInfo = Physics2D.Raycast(ray, rayDir, rayDist, colMask);

            if (hitInfo)
            {
                moveVector.x = hitInfo.point.x - ray.x;
                rayDist = hitInfo.distance;

                if(rayDir.x == -1)
                {
                    // Collision on our left, add back ray inset.
                    moveVector.x += insetWidth;
                    colInfo.left = true;
                }
                else
                {
                    // Collision on right, subtract out ray inset.
                    moveVector.x -= insetWidth;
                    colInfo.right = true;
                }
            }

        }
    }

    private void VerticalMove(ref Vector2 moveVector)
    {
        Vector2 rayDir = (Mathf.Sign(moveVector.y) == -1)? Vector2.down : Vector2.up;
        float rayDist = Mathf.Abs(moveVector.y) + insetWidth;

        Vector2 rayOrigin = (rayDir.y == -1) ? rayOrigins.botLeft : rayOrigins.topLeft;
        
        // The position we will be in.
        rayOrigin.x += moveVector.x;

        for(int i = 0; i < verticalRayCount; ++i)
        {
            Vector2 ray = new Vector2(rayOrigin.x + i * verticalRaySpacing, rayOrigin.y);

            Debug.DrawRay(ray, rayDir*rayDist, Color.red);

            RaycastHit2D hitInfo = Physics2D.Raycast(ray, rayDir, rayDist, colMask);

            if (hitInfo)
            {
                moveVector.y = hitInfo.point.y - ray.y;
                rayDist = hitInfo.distance;

                if(rayDir.y == -1)
                {
                    moveVector.y += insetWidth;

                    colInfo.bottom = true;
                }
                else
                {
                    moveVector.y -= insetWidth;

                    colInfo.top = true;
                }
            }

        }
    }
    #endregion
}
