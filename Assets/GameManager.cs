using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject particle;
    public Boundary initBoundry;
    public Boundary queryBoundary;
    public QuadTree QT;
    private List<GameObject> particles;

    private Vector2 mousePos;

    public float CameraHeight {get;set;}
    public float CameraWidth {get;set;}
    public int NumberOfParticles = 40;

    //Boid stuff
    public float boidPerceptionRegion = 5;
    public float cohesionForce, alignmentForce, separationForce;
    public float particleSpeed;
    public bool debugBoids = false;

    float fps = 0, temp = 0;

    void Start()
    {
        CameraHeight = Camera.main.orthographicSize * 2;
        CameraWidth = (float)Screen.width / (float)Screen.height * CameraHeight;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        particles = new List<GameObject>();
        initBoundry = new Boundary(Vector2.zero, CameraWidth / 2, CameraHeight / 2);
        QT = new QuadTree(initBoundry);
        queryBoundary = new Boundary(mousePos, 1.5f, 1.5f);

        populateParticleList();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        queryBoundary.center = mousePos;
        /*queryBoundary.drawBoundary(Color.green);
        List<GameObject> queryList = QT.queryRange(queryBoundary);

        // Tests QuadTree.query function
        foreach(var g in queryList)
        {
            Debug.DrawLine(g.transform.position, queryBoundary.center);
        }*/

        //inserts new particle in world
        if (Input.GetKey(KeyCode.W))
        {
            GameObject p = Instantiate(particle, mousePos, particle.transform.rotation, transform);
            QT.insertParticle(p);
            particles.Add(p);
            NumberOfParticles++;
        }

        refreshQuadtree(ref QT);

        ShowFPS();

    }

    private void populateParticleList()
    {
        Vector2 randomPos;

        for(int i=0; i<NumberOfParticles; i++)
        {
            randomPos = new Vector2(Random.Range(-CameraWidth / 2, CameraWidth / 2),
                                    Random.Range(-CameraHeight / 2, CameraHeight / 2));
            particles.Add(Instantiate(particle, randomPos, particle.transform.rotation, transform));
            QT.insertParticle(particles[i]);
        }

    }

    private void ShowFPS()
    {
        fps++;
        if (Time.time > temp)
        {
            temp = Time.time + 1;
            Debug.Log(fps);
            fps = 0;
        }
    }

    private void refreshQuadtree(ref QuadTree qt)
    {
        qt = new QuadTree(initBoundry);
        foreach (GameObject g in particles)
        {
            qt.insertParticle(g);
            g.GetComponent<SpriteRenderer>().color = Color.red;
        }

        qt.Draw();
    }

    IEnumerator InsertPoints()
    {
        populateParticleList();
        
        for(int i=0; i<particles.Count; i++)
        {  
            QT.insertParticle(particles[i]);
            QT.Draw();
            yield return new WaitForSeconds(0.8f);
        }
    }

}
