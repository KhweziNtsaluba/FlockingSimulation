using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary
{
    public Vector2 center;
    public float halfLength, halfHeight;

    public Boundary(Vector2 center, float halfWidth, float halfHeight)
    {
        this.center = center;
        this.halfLength = halfWidth;
        this.halfHeight = halfHeight;
    }

    public bool containsParticle(GameObject particle) {
        return Mathf.Abs(particle.transform.position.x - center.x) < halfLength &&
               Mathf.Abs(particle.transform.position.y - center.y) < halfHeight;
    }

    public bool intersectsBoundary(Boundary other)
    {
        //using bottom left and top right vertices for each rectangle
        Vector2 thisLeftVert = new Vector2(center.x - halfLength, center.y - halfHeight);
        Vector2 thisRightVert = new Vector2(center.x + halfLength, center.y + halfHeight);
        Vector2 otherLeftVert = new Vector2(other.center.x - other.halfLength, other.center.y - other.halfHeight);
        Vector2 otherRightVert = new Vector2(other.center.x + other.halfLength, other.center.y + other.halfHeight);

        bool widthPositive = Mathf.Min(thisRightVert.x, otherRightVert.x) > Mathf.Max(thisLeftVert.x, otherLeftVert.x);
        bool heightPositive = Mathf.Min(thisRightVert.y, otherRightVert.y) > Mathf.Max(thisLeftVert.y, otherLeftVert.y);

        return widthPositive && heightPositive;
    }

    public void drawBoundary()
    {
        Vector2 nw, ne, sw, se;
        nw = new Vector2(center.x - halfLength, center.y + halfHeight);
        ne = new Vector2(center.x + halfLength, center.y + halfHeight);
        sw = new Vector2(center.x - halfLength, center.y - halfHeight);
        se = new Vector2(center.x + halfLength, center.y - halfHeight);

        Debug.DrawLine(nw, ne, Color.red);
        Debug.DrawLine(ne, se, Color.red);
        Debug.DrawLine(se, sw, Color.red);
        Debug.DrawLine(sw, nw, Color.red);
 }

    public void drawBoundary(Color c)
    {
        Vector2 nw, ne, sw, se;
        nw = new Vector2(center.x - halfLength, center.y + halfHeight);
        ne = new Vector2(center.x + halfLength, center.y + halfHeight);
        sw = new Vector2(center.x - halfLength, center.y - halfHeight);
        se = new Vector2(center.x + halfLength, center.y - halfHeight);

        Debug.DrawLine(nw, ne, c);
        Debug.DrawLine(ne, se, c);
        Debug.DrawLine(se, sw, c);
        Debug.DrawLine(sw, nw, c);
    }
}
