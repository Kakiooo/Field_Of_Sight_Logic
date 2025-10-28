using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class FOW_Enemy : MonoBehaviour
{
    public float ViewRadius;
    [Range(0,360)]
    public float ViewAngle;
    public int MeshResolution;
    [SerializeField] LayerMask _obstacles, _target;
    public List<Transform> VisibleObjects=new List<Transform> ();
    public MeshFilter FOW_Filter;
    Mesh _viewMesh;
    bool _isLost;
    private void Start()
    {
        _viewMesh=new Mesh ();
        FOW_Filter.mesh = _viewMesh;
        StartCoroutine("LineOfSight", 0.2f);
    }
    private void Update()
    {
        if (_isLost && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }//only use this for testing Delete afterwards 
    }
    private void LateUpdate()
    {
        DrawFieldOfView();
    }
    
    IEnumerator LineOfSight(float Delay)//keep detecting the target after 0.2s delay
    {
        while (true)
        {
            yield return new WaitForSeconds(Delay);
            DetectPlayer();
        }
    }
    public Vector3 AngleDir(float angleDegree,bool IsGlobalAngle)
    {
        if (!IsGlobalAngle)
        {
            angleDegree += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleDegree * Mathf.Deg2Rad), 0, Mathf.Cos(angleDegree * Mathf.Deg2Rad));
    }//universal function for calculate the angle direction from degree

    public void DetectPlayer()
    {
        VisibleObjects.Clear ();
        Collider[] InTheRangeObjects=Physics.OverlapSphere(transform.position,ViewRadius,_target);//obtain all target info inside of the range
        for(int i = 0; i < InTheRangeObjects.Length; i++)
        {
            Transform target= InTheRangeObjects[i].transform;
            Vector3 targetDir=(target.transform.position-transform.position).normalized;//GEt direction of that target 
            float betweenAngle=Vector3.Angle(transform.forward, targetDir);//check if the target is inside the Field of view angle
            if(betweenAngle < (ViewAngle/2))//when the enemy is inside the angle
            {
                float dis = Vector3.Distance(target.transform.position, transform.position);
                if (!Physics.Raycast(transform.position, targetDir, dis, _obstacles))//if there is no obstacles inbetween target and object with vision
                {
                    VisibleObjects.Add(target); //It means the target has been seen
                }
            }
        }
    }

    public void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(ViewAngle * MeshResolution);
        float stepAngle=ViewAngle/stepCount;
        List<Vector3> hitPoints=new List<Vector3>();    
        for(int i = 0;i < stepCount-1; i++)
        {
            float angle = transform.eulerAngles.y - ViewAngle / 2 + stepAngle * i;
            //first shoot ray is starting from all the way to the left. So use object local transform rotation y minus half of degree and plus each degree of each angle between rays.
           // Debug.DrawLine(transform.position, transform.position + AngleDir(angle, true) * ViewRadius, Color.red);
           ViewCastInfo newViewCast=ViewCast(angle);
            hitPoints.Add(newViewCast.HitPoint);
            //remember the vectors here are all local
        }


        //obtaining all data vertices number,vertices local vector3 from player, triangle combination number,
        int verticesCount=hitPoints.Count+1;
        int triangleNum = verticesCount - 2;
        Vector3[] vertices=new Vector3[verticesCount];  
        int[] trangle_Vertices=new int[(triangleNum) *3];//this is how many combination of vertices are for all triangles


        vertices[0] = Vector3.zero;
        for(int i = 0; i < verticesCount-1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(hitPoints[i]);
            if(i < triangleNum)
            {
                trangle_Vertices[i * 3] = 0;
                trangle_Vertices[i * 3 + 1] = i + 1;
                trangle_Vertices[i * 3 + 2] = i + 2;
            }
        }//Generating the customised Mesh as visualization for the field of sight. 

        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = trangle_Vertices;
        _viewMesh.RecalculateNormals();

    }




    ViewCastInfo ViewCast(float globalAngle) //For recording the hitpoint on obstacles from each ray that is projectiled by emitter 
    {
        Vector3 dir = AngleDir(globalAngle, true);
        RaycastHit hit;
        if(Physics.Raycast(transform.position,dir,out hit, ViewRadius, _obstacles))
        {
            return new ViewCastInfo(true,hit.point,globalAngle,hit.distance);//remember to use new sturt to rewrite the value. 
        }
        else
        {
            return new ViewCastInfo(false, transform.position+dir*ViewRadius, globalAngle, ViewRadius);
        }
    }



    public struct ViewCastInfo//all the variables inside cannot be called outside this struct or this constructor. 
    {
        /// <summary>
        ///Save all different data that other function can use it 
        /// </summary>
        public bool Hit;
        public Vector3 HitPoint;
        public float Angle;
        public float Distance;

        public ViewCastInfo(bool _hit,Vector3 _hitPoint,float _angle,float _distance)
        {
            Hit = _hit;
            HitPoint = _hitPoint;
            Angle = _angle;
            Distance = _distance;
        }
    }

}
