using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    // Implement slopes
    
    // 
    struct RayOrigins
    {
        public Vector2 botLeft, botRight;
        public Vector2 topLeft;
    }

    // Controller Collision Info
    public struct ColInfo
    {
        public bool right, left;
        public bool isGrounded;

        public void ResetColInfo()
        {
            right = left = false;
            isGrounded = false;
        }
    }

    public ColInfo colInfo;

    BoxCollider2D col;
    // The layer that the player collides with.
    public LayerMask colMask;
    // The amount that the ray should be fired from within the controllers own bounds.
    float insetWidth = 0.02f;

    RayOrigins rayOrigins;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;
    private float verticalRaySpacing;
    private float horizontalRaySpacing;

    private void Start()
    {
        col = GetComponent<BoxCollider2D>();
        CalculateRaySpacings();
    }

    private void CalculateRaySpacings()
    {
        Bounds bounds = col.bounds;

        bounds.Expand(insetWidth * -2f);

        verticalRaySpacing = (col.size.x - (2*insetWidth)) / (verticalRayCount - 1);
        horizontalRaySpacing = (col.size.y - (2*insetWidth)) / (horizontalRayCount - 1);
    }

    private void CalculateRayOrigins()
    {
        Bounds bounds = col.bounds;

        bounds.Expand(insetWidth * -2f);

        rayOrigins.botLeft = new Vector2(bounds.min.x, bounds.min.y);
        rayOrigins.botRight = new Vector2(bounds.max.x, bounds.min.y);
        rayOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
    }

    public void Move(Vector2 moveVector)
    {
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

        this.transform.Translate(moveVector);
    }

    private void HorizontalMove(ref Vector2 moveVector)
    {
        float rayDir = Mathf.Sign(moveVector.x);
        float rayDist = Mathf.Abs(moveVector.x) + insetWidth;

        if(Mathf.Abs(moveVector.x) < insetWidth)
        {
            rayDist = 2 * insetWidth;
        }

        Vector2 rayOrigin = (rayDir == -1) ? rayOrigins.botLeft : rayOrigins.botRight;

        for (int i = 0; i < horizontalRayCount; ++i)
        {
            Vector2 ray = new Vector2(rayOrigin.x, rayOrigin.y + i * horizontalRaySpacing);

            Debug.DrawRay(ray, rayDir * Vector2.right * rayDist, Color.red);

            RaycastHit2D hitInfo = Physics2D.Raycast(ray, Vector2.right * rayDir, rayDist, colMask);

            if (hitInfo)
            {
                moveVector.x = hitInfo.point.x - ray.x;

                rayDist = hitInfo.distance;

                if(rayDir == -1)
                {
                    moveVector.x += insetWidth;
                    colInfo.left = true;
                }
                else
                {
                    moveVector.x -= insetWidth;
                    colInfo.right = true;
                }
            }

        }
    }

    private void VerticalMove(ref Vector2 moveVector)
    {
        float rayDir = Mathf.Sign(moveVector.y);
        float rayDist = Mathf.Abs(moveVector.y) + insetWidth;

        Vector2 rayOrigin = (rayDir == -1) ? rayOrigins.botLeft : rayOrigins.topLeft;

        rayOrigin.x += moveVector.x;

        for(int i = 0; i < verticalRayCount; ++i)
        {
            Vector2 ray = new Vector2(rayOrigin.x + i * verticalRaySpacing, rayOrigin.y);

            Debug.DrawRay(ray, rayDir * Vector2.up * rayDist, Color.red);

            RaycastHit2D hitInfo = Physics2D.Raycast(ray, Vector2.up * rayDir, rayDist, colMask);

            if (hitInfo)
            {
                moveVector.y = hitInfo.point.y - ray.y;

                rayDist = hitInfo.distance;

                moveVector.y += insetWidth;

                colInfo.isGrounded = true;

            }

        }
    }

    private void DrawDebugRay(Vector2 ray, Vector2 dir, float length, Color color)
    {
        Debug.DrawRay(ray, dir * length, color);
    }
}
