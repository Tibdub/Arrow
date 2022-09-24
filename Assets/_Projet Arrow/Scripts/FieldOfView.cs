using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;

public class FieldOfView : MonoBehaviour
{
    [Header("Paramétrage Fow")]
    public float viewRaduis;
    [Range(0,360)]
    public float viewAngle;
    public float detectionDelay;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

  
    public List<Transform> visibleTargets = new List<Transform>();

    [Header("Affichage Fow")]
    public float meshResolution;
    public int edgeResolveIteration;
    public float edgeDstThreshold;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    [HideInInspector]
    public bool targetIsSpoted;


    public void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mash";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine("FindTargetWithDelay", detectionDelay);
    }

    public void LateUpdate()
    {
        DrawFieldOfView();
    }



    // Detection des targets
    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }


    // Trouve toutes les cibles autours de lui
    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRaduis, targetMask);

        // Detection des cible autour de lui
        for(int i=0; i < targetsInViewRadius.Length; i++)
        {

            // Trouve la direction dans laquelle se trouve la cible
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // La cible se trouve dans le champ de vision ?
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
               
                float disToTarget = Vector3.Distance(transform.position, target.position);

                // La cible ne se trouve pas derrière un obstacle ?
                if (!Physics.Raycast(transform.position, dirToTarget, disToTarget,obstacleMask))
                {
                    visibleTargets.Add(target);

                    // La cible est repérée, faire les actions voulues...
                    // ...
                    // ...
                }
            }
        }

        targetIsSpoted = visibleTargets.Count == 1;
    }

    public void LookAtPlayer()
    {

    }





    // Renvoie une direction (Vector3) en partant d'un angle donné (degrés)
    //
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }


    // Dessine le champ de vision
    //
    void DrawFieldOfView()
    {

        // Défini la précision d'aquisition (nombre de Raycasts)
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;

        // Liste pour les points de collision de chaque raycast
        List<Vector3> viewPoints = new List<Vector3>();

        ViewCastInfo oldViewCast = new ViewCastInfo();

        // Trace un ray tout les X angle
        for (int i = 0; i<= stepCount; i++)
        {
            float angle = transform.transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            // Verifie les raycats au "bord" des obstacles (pour smoothifier)
            if (i > 0) 
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDstThreshold;

                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);

                    if(edge.pointA != Vector3.zero){
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero){
                        viewPoints.Add(edge.pointB);
                    }

                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        // Génération du mesh entre chaque ray
        // (plus d'infos, voir => https://www.youtube.com/watch?v=73Dc5JTCmKI&t=460s)
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];


        vertices[0] = Vector3.zero; // => Vertex intial (position personnage)

        // Génération list des vertex & liste des triangles
        for (int i = 0; i < vertexCount-1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]); // Liste des vertex

            if(i < vertexCount - 2)
            {
                triangles[i * 3] = 0; // => 1er point de chaque triangle = 0;  
                triangles[i * 3 + 1] = i + 1; // => 2èm point de chaque triangle
                triangles[i * 3 + 2] = i + 2; // => 3éme point de chaque triangle
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }


    // Permet de trouver les "bords" des obstacles détectés par le champ de vision
    // Créer un new ray entre les 2 ray qui ont détecté le bord,
    // puis affine jusqu'à un certain point (edgeResolveIteration)
    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for(int i=0; i< edgeResolveIteration; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }



    // Trace un Raycast à l'angle donné % au personnage
    // & stock certaines informations dans un struct ViewCastInfo
    public ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, dir, out hit, viewRaduis, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir* viewRaduis, viewRaduis, globalAngle);
        }
    }



    // -_-_-_-_-_-_-_-_-_-_-_-_-_-_-   STRUCTS     -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_- // 

    // Créer un nouveau type de donnée (Vector3, int, Transform, ...)
    // qui retourne un ensemble de valeurs que l'on définit


    // => Ici, on récupère des données d'un raycats
    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            distance = _dst;
            angle = _angle;
        }
    }


    // Struct pour le smootification des angles lors de la création du mesh de vision
    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
