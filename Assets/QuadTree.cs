using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree
{
    private const int MaxParticles = 4;
    private List<GameObject> particleList = new List<GameObject>();
    private QuadTree nw, ne, sw, se;
    private Boundary currBoundary;
    Color RandomColour;

    /// <include file='Documentation/QuadTreeDoc.xml' path='Class/Member[@name="QuadTree"]/*' />
    public QuadTree(Boundary boundary)
    {
        currBoundary = boundary;
    }

    /// <include file='Documentation/QuadTreeDoc.xml' path='Class/Member[@name="subdivide"]/*' />
    private void subdivide()
    {
        Vector2 nwCenter = new Vector2(currBoundary.center.x - currBoundary.halfLength / 2, currBoundary.center.y + currBoundary.halfHeight /2);
        Vector2 neCenter = new Vector2(currBoundary.center.x + currBoundary.halfLength / 2, currBoundary.center.y + currBoundary.halfHeight / 2);
        Vector2 swCenter = new Vector2(currBoundary.center.x - currBoundary.halfLength / 2, currBoundary.center.y - currBoundary.halfHeight / 2);
        Vector2 seCenter = new Vector2(currBoundary.center.x + currBoundary.halfLength / 2, currBoundary.center.y - currBoundary.halfHeight / 2);

        nw = new QuadTree(new Boundary(nwCenter, currBoundary.halfLength / 2, currBoundary.halfHeight / 2));
        ne = new QuadTree(new Boundary(neCenter, currBoundary.halfLength / 2, currBoundary.halfHeight / 2));
        sw = new QuadTree(new Boundary(swCenter, currBoundary.halfLength / 2, currBoundary.halfHeight / 2));
        se = new QuadTree(new Boundary(seCenter, currBoundary.halfLength / 2, currBoundary.halfHeight / 2));
    }

    public bool insertParticle(GameObject particle)
    {
        if (!currBoundary.containsParticle(particle)) return false;
        
        if(particleList.Count < MaxParticles && nw == null)
        {
            //particle.GetComponent<SpriteRenderer>().color = RandomColour;
            particleList.Add(particle);
            return true;
        }

        //if we haven't subdivided yet
        if (nw == null)
            subdivide();

        if (nw.insertParticle(particle)) return true;
        if (ne.insertParticle(particle)) return true;
        if (sw.insertParticle(particle)) return true;
        if (se.insertParticle(particle)) return true;

        return false;

    }

    public List<GameObject> queryRange(Boundary boundary)
    {
        List<GameObject> queriedParticles = new List<GameObject>();
        if (!this.currBoundary.intersectsBoundary(boundary)) return queriedParticles;

        foreach(GameObject g in particleList)
        {
            if (boundary.containsParticle(g))
                queriedParticles.Add(g);
        }

        if (nw == null) return queriedParticles;

        queriedParticles.AddRange(nw.queryRange(boundary));
        queriedParticles.AddRange(ne.queryRange(boundary));
        queriedParticles.AddRange(se.queryRange(boundary));
        queriedParticles.AddRange(sw.queryRange(boundary));
        return queriedParticles;

    }

    public void Draw()
    {
        currBoundary.drawBoundary();

        if (nw == null) return;

        nw.Draw();
        ne.Draw();
        sw.Draw();
        se.Draw();
    }
}
