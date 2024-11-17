using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public int initialSpeed = 5;
    [SerializeField] private float boundaryOffset;

    float CameraHeight;
    float CameraWidth;

    public GameManager gm;
    private QuadTree Quadtree;
    private Boundary boundary;
    private List<GameObject> objectsInRange = new List<GameObject>();

    //float steeringForce;

    public virtual void move()
    {

    }

    public void Start() 
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        Quadtree = gm.QT;
        CameraHeight = gm.CameraHeight;
        CameraWidth = gm.CameraWidth;

        boundary = new Boundary(transform.position, gm.boidPerceptionRegion / 2, gm.boidPerceptionRegion / 2);
        boundaryOffset = 0.01f;

        rb = this.GetComponent<Rigidbody2D>();
        //transform.Rotate(new Vector3(0, 0, Random.Range(-180f, 181f)));
        rb.linearVelocity = new Vector2(Random.Range(-1f,1f),Random.Range(-1,1)) * initialSpeed;

        
    }

    public void FixedUpdate()
    {
        checkBounds();

        // latest perception region size information from editor
        boundary.center = transform.position;
        boundary.halfLength = gm.boidPerceptionRegion/2;
        boundary.halfHeight = gm.boidPerceptionRegion/2;

        objectsInRange = Quadtree.queryRange(boundary);

        rb.linearVelocity += (CohesionSteer().normalized * gm.cohesionForce)
                    + (AlignmentSteer().normalized * gm.alignmentForce)
                    + (SeparationSteer() * gm.separationForce);
        
        if (gm.debugBoids)
            debug();

        rb.linearVelocity = rb.linearVelocity.normalized * gm.particleSpeed;
        Vector3 tempVector = transform.position + (Vector3)rb.linearVelocity;
        Debug.DrawLine(transform.position, tempVector, Color.green);
        Debug.Log("COHESION FORCE:" + gm.cohesionForce);

    }

    public Vector2 CohesionSteer()
    {
        Vector2 averageLocalPos = Vector2.zero;
        Vector2 desiredDirection;
        int total = 0;

        foreach(GameObject go in objectsInRange)
        {
            if (go != this.gameObject)
            {
                averageLocalPos += (Vector2)go.transform.position;
                total++;
            }
        }

        if (total > 0)
        {
            averageLocalPos /= total;
            desiredDirection = averageLocalPos - (Vector2)transform.position;
            desiredDirection -= rb.linearVelocity;
            return desiredDirection;
        }

        return Vector2.zero; 

    }

    public Vector2 AlignmentSteer()
    {
        Vector2 averageLocalVelocity = Vector2.zero;
        Vector2 desiredDirection;
        int total = 0;

        foreach (GameObject go in objectsInRange)
        {
            if (go != this.gameObject)
            {
                averageLocalVelocity += (Vector2)go.GetComponent<Rigidbody2D>().linearVelocity;
                total++;
            }
               
        }

        if (total > 0)
        {
            averageLocalVelocity /= total;
            desiredDirection = averageLocalVelocity - rb.linearVelocity;
            return desiredDirection;
        }

        return Vector2.zero;
    }

    public Vector2 SeparationSteer()
    {
        Vector2 desiredDirection; //separationSteeringForce
        Vector2 separationVector;

        Vector2 total = Vector2.zero;
        foreach (GameObject go in objectsInRange)
        {
            if(go != this.gameObject)
            {
                separationVector = go.transform.position - transform.position;
                float temp = (!go.transform.position.Equals(transform.position)) ? 1 / Mathf.Pow(separationVector.magnitude, 2) : 1;
                total -= separationVector * temp;
            }
        }

        if (!total.Equals(Vector2.zero))
        {
            desiredDirection = total - rb.linearVelocity;
            return desiredDirection;
        }

        else return Vector2.zero;
        
    }
 
    public void debug()
    {
        boundary.drawBoundary(Color.white);
        foreach (GameObject go in objectsInRange)
        {
            if (go != this)
            Debug.DrawLine(transform.position, go.transform.position);
        }
    }

    public void checkBounds()
    {
        if (transform.position.y > CameraHeight / 2)
        {
            transform.position = new Vector2(transform.position.x, -CameraHeight / 2);
        }
        if (transform.position.y < -CameraHeight / 2)
        {
            transform.position = new Vector2(transform.position.x, 0f + CameraHeight / 2);
        }
        if (transform.position.x > CameraWidth / 2)
        {
            transform.position = new Vector2(0f - CameraWidth / 2, transform.position.y);
        }
        if (transform.position.x < -CameraWidth / 2)
        {
            transform.position = new Vector2(0f + CameraWidth / 2, transform.position.y);
        }
    }
}
