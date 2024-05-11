using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    private CubePos nowCube = new CubePos(0, 1, 0);
    public float cubeChangePlaceSpeed = 0.5f;
    public Transform cubeToPlace;
    public GameObject cubeToCreate, allCubes;
    public GameObject[] canvasStartPage;
    private Rigidbody allCubesRb;
    private bool IsLose, firstCube;
    private float camMoveToYPosihion, camMoveSpeed = 2f;
    public Color[] bgcolors;

    private List<Vector3> allCubesPositions = new List<Vector3>
    {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, -1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, -1),
    };
    private int prevCountMaxHorisontal = 0;
    private Transform mainCam;
    private Coroutine showCubePlace;
   

    private List<Vector3> positions = new List<Vector3>(); // Объявляем positions как список здесь

    private void Start()
    {

        mainCam = Camera.main.transform;
        camMoveToYPosihion = 5.9f + nowCube.y - 1f;


        allCubesRb = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace());
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null && allCubes != null)
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;
#endif
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (!firstCube)
            {
                firstCube = true; foreach (GameObject obj in canvasStartPage)
                    Destroy(obj);

            }

            GameObject newCube = Instantiate(cubeToCreate, cubeToPlace.position, Quaternion.identity) as GameObject;
            newCube.transform.SetParent(allCubes.transform);
            nowCube.SetVector(cubeToPlace.position);
            allCubesPositions.Add(nowCube.GetVector());
            allCubesRb.isKinematic = true;
            allCubesRb.isKinematic = false;
            SpawnPositions();
            MoveCameraChangeBg();
        }
        mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition,
        new Vector3(mainCam.localPosition.x, camMoveToYPosihion, mainCam.localPosition.z),
        camMoveSpeed * Time.deltaTime);
        if (IsLose || allCubesRb.velocity.magnitude <= 0.1f)
        {
            return;

        }

        Destroy(cubeToPlace.gameObject);
        IsLose = true;
        StopCoroutine(showCubePlace);



    }
    

    
    


    IEnumerator ShowCubePlace()
    {
        while (true)
        {
            SpawnPositions();
            yield return new WaitForSeconds(cubeChangePlaceSpeed);
        }
    }

    private void SpawnPositions()
    {
        positions.Clear(); // Очищаем positions перед использованием

        AddPositionIfEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        AddPositionIfEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        AddPositionIfEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        AddPositionIfEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));
        AddPositionIfEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));
        AddPositionIfEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));

        if (positions.Count > 1)
        
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
            else if(positions.Count == 0)
                IsLose = true;
            else
             cubeToPlace.position = positions[0];
            
        
    }

    private void AddPositionIfEmpty(Vector3 targetPos)
    {
        if (IsPositionEmpty(targetPos) && targetPos != cubeToPlace.position)
        {
            positions.Add(targetPos);
        }
    }

    private bool IsPositionEmpty(Vector3 targetPos)
    {
        foreach (Vector3 pos in allCubesPositions)
        {
            if (pos == targetPos)
            {
                return false;
            }
        }
        return true;

    }
    private void MoveCameraChangeBg()
    {
        int maxX = 0, maxY = 0, maxZ = 0, maxHor;

        foreach(Vector3 pos in allCubesPositions)
        {
            if (Math.Abs(Convert.ToInt32(pos.x)) > maxX)
                maxX = Convert.ToInt32(pos.x);

            if ((Convert.ToInt32(pos.y)) > maxY)
                maxY = Convert.ToInt32(pos.y);

            if (Math.Abs(Convert.ToInt32(pos.z)) > maxZ)
                maxZ = Convert.ToInt32(pos.z);
        }
        camMoveToYPosihion = 5.9f + nowCube.y - 1f;
        maxHor = maxX > maxZ ? maxX : maxZ;
        if (maxHor % 3 == 0 && prevCountMaxHorisontal != maxHor)
        {
            mainCam.localPosition -= new Vector3(0, 0, 2f);
            prevCountMaxHorisontal = maxHor;
        }
        if (maxY >= 15)
            Camera.main.backgroundColor = bgcolors[2];
        else if (maxY >= 10)
            Camera.main.backgroundColor = bgcolors[1];
        else if (maxY >= 3)
            Camera.main.backgroundColor = bgcolors[0];
    }
       
           
    }

    struct CubePos
    {
        public int x, y, z;

        public CubePos(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 GetVector()
        {
            return new Vector3(x, y, z);
        }

        public void SetVector(Vector3 pos)
        {
            x = Mathf.RoundToInt(pos.x);
            y = Mathf.RoundToInt(pos.y);
            z = Mathf.RoundToInt(pos.z);
        }
    }


