using System;
using UnityEngine;


//TODO:改名，这个现在也包括区域设置了
public class InputManager : MonoBehaviour
{
    public static Vector2 CameraDrift; //因拖动引起的摄像头移动，会一直变，谨慎修改
    public static readonly Vector2 CameraDriftStart = new(0.5f, 0.0f); //常量 随意修改 摄像头开机的偏移，用于将人物初始化的时候放在画幅正中间
    public static readonly Vector2 CameraDriftCheck= new(1.4f, 0.0f); //常量 随意修改 摄像头检测的偏移，用于微调切换的位置，消除黑边 TODO：去掉
    private Camera c;

    private Vector2 dragStartMousePosition;
    private Vector2 dragStartCameraPosition;

    private float zoomSpeed = ConfigParam.CameraZoomSpeed;
    private float zoomLowerBound = ConfigParam.CameraZoomLowerBound;
    private float zoomUpperBound = ConfigParam.CameraZoomUpperBound;
    
    private bool isDraging = false;
    
    //android 的两根手指头拖拽
    private Vector2 androidStartP1;
    private Vector2 androidStartP2;

    private Vector2 androidStartCameraPosition;
    private Vector2 androidStartPosition;
    private float androidStartCameraZoom;
    private float androidStartZoom;

    private bool isZooming = false;

    private void Start()
    {
        c = Camera.main;
        CirclePrefeb = ResManager.LoadPrefab("CircleArea");
    }

    //TODO:这个为啥Update会来回跳
    private void LateUpdate()
    {
        if (!GameMain.battleFlag && !GameMain.configFlag)
        {
            return;
        }
        
        if(hit.collider != null)
        {
            return;
        }
#if UNITY_EDITOR
        HandleDragMouse();
        HandleZoomMouse();
#elif UNITY_ANDROID
        AndroidHandle();
#else
        HandleDragMouse();
        HandleZoomMouse();
#endif
        
    }
    
    void HandleDragMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDraging = true;
            dragStartMousePosition = Input.mousePosition;
            dragStartCameraPosition = CameraDrift;
            // Debug.Log(dragStartCameraPosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDraging = false;
        }
        if (isDraging)
        {
            Vector2 dragCurrentMousePosition = Input.mousePosition;
            Vector2 distanceInWorldSpace = c.ScreenToWorldPoint(dragStartMousePosition) -
                                           c.ScreenToWorldPoint(dragCurrentMousePosition);
            Vector2 newPos = dragStartCameraPosition + distanceInWorldSpace;
            CameraDrift = newPos;
        }
    }

    void HandleZoomMouse()
    {
        float size = c.orthographicSize;
        //Zoom out
        if (Input.mouseScrollDelta.y < 0 && size < zoomUpperBound)
        {
            Vector2 screenP0 = Input.mousePosition;
            Vector2 worldP0 = c.ScreenToWorldPoint(screenP0);
            c.orthographicSize += zoomSpeed;
            c.orthographicSize = c.orthographicSize > zoomUpperBound ? zoomUpperBound : c.orthographicSize;
            Vector2 worldP1 = c.ScreenToWorldPoint(screenP0);
            CameraDrift += worldP0 - worldP1;
        }
        //Zoom in
        if (Input.mouseScrollDelta.y > 0 && size > zoomLowerBound )
        {
            Vector2 screenP0 = Input.mousePosition;
            Vector2 worldP0 = c.ScreenToWorldPoint(screenP0);
            c.orthographicSize -= zoomSpeed;
            c.orthographicSize = c.orthographicSize < zoomLowerBound ? zoomLowerBound : c.orthographicSize;
            Vector2 worldP1 = c.ScreenToWorldPoint(screenP0);
            CameraDrift += worldP0 - worldP1;
        }
    }
    
    void AndroidHandle()
    {
        if (Input.touchCount <= 0)
            return;
        if (Input.touchCount == 1) //单点触碰移动摄像机
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDraging = true;
                dragStartMousePosition = Input.mousePosition;
                dragStartCameraPosition = CameraDrift;
                // Debug.Log(dragStartCameraPosition);
            }
            if (Input.GetMouseButtonUp(0))
            {
                isDraging = false;
            }
            if (isDraging)
            {
                Vector2 dragCurrentMousePosition = Input.mousePosition;
                Vector2 distanceInWorldSpace = c.ScreenToWorldPoint(dragStartMousePosition) -
                                               c.ScreenToWorldPoint(dragCurrentMousePosition);
                CameraDrift = dragStartCameraPosition + distanceInWorldSpace;
            }

        }
        else if (Input.touchCount == 2)
        {
            //注：手机的touch依然可与使用GetMouseButtonDown 函数获取，其中0指的是第一根手指， 1指的是第二根手指。
            //第一根手指抬起或放下的时候不会进这个分支，所以只需要处理第二根手指
            if (Input.GetMouseButtonDown(1))
            {
                //记录两根手指的初始位置
                isZooming = true;
                androidStartP1 = Input.touches[0].position;
                androidStartP2 = Input.touches[1].position;
                androidStartPosition = (androidStartP1 + androidStartP2)/2;
                androidStartCameraPosition = CameraDrift;

                androidStartZoom = Vector2.Distance(androidStartP1,androidStartP2);
                androidStartCameraZoom = c.orthographicSize;
                
                // Debug.Log(dragStartCameraPosition);
                Debug.Log("1 down");
            }
            if (Input.GetMouseButtonUp(1))
            {
                isZooming = false;
                
                Debug.Log("1 up");
            }
            if (isZooming)
            {
                Vector2 androidCurrentP1 = Input.touches[0].position;
                Vector2 androidCurrentP2 = Input.touches[1].position;
                Vector2 androidCurrentPosition = (androidCurrentP1 + androidCurrentP2)/2;

                float androidCurrentZoom = Vector2.Distance(androidCurrentP1,androidCurrentP2);

                Vector2 distanceInWorldSpace = c.ScreenToWorldPoint(androidStartPosition) -
                                               c.ScreenToWorldPoint(androidCurrentPosition);
                CameraDrift = androidStartCameraPosition + distanceInWorldSpace;
                
                
                c.orthographicSize = androidStartCameraZoom * androidStartZoom / androidCurrentZoom;
                c.orthographicSize = c.orthographicSize < zoomLowerBound ? zoomLowerBound : c.orthographicSize;
                c.orthographicSize = c.orthographicSize > zoomUpperBound ? zoomUpperBound : c.orthographicSize;
            }
        }
    }
    
    
    
    /////////////////////////////////////////////////
    /// 关于区域设置的内容
    //////////////////////////////////////////////////
    public Vector2 mousepos;
    public static GameObject CirclePrefeb;
    Ray ray;
    public static RaycastHit2D hit;
    

    // Update is called once per frame
    void Update()
    {
        if (!GameMain.configFlag)
        {
            return;
        }
        
        //每帧更新鼠标世界位置
        mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        //鼠标按下时，获取位置差与碰撞信息
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            hit = Physics2D.Raycast(ray.origin,
                                    ray.direction);
            
            //拿到向量差值，拖动的圆变暗
            if (hit.collider != null)
            {
                hit.collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(
                    hit.collider.gameObject.GetComponent<SpriteRenderer>().color.r *0.8f,
                    hit.collider.gameObject.GetComponent<SpriteRenderer>().color.g *0.8f, 
                    hit.collider.gameObject.GetComponent<SpriteRenderer>().color.b *0.8f, 
                    hit.collider.gameObject.GetComponent<SpriteRenderer>().color.a) ;
            }
        }

        //鼠标移动时，完成被碰撞物体位置的更新
        if (Input.GetMouseButton(0))
        {
            if (hit.collider != null)
            {
                GameObject circle = hit.collider.gameObject.transform.parent.gameObject;
                
                if (hit.collider.gameObject.name == "CircleHeart")
                {
                    circle.transform.position = (Vector3)mousepos + new Vector3(0,0,-0.5f);
                }

                if (hit.collider.gameObject.name == "CircleEdge")
                {
                    GameObject circleEdge = circle.transform.GetChild(0).gameObject;
                    GameObject circleShow = circle.transform.GetChild(2).gameObject;
                    
                    //这里直接设置circleEdge的绝对坐标即可
                    circleEdge.transform.position = (Vector3)mousepos + new Vector3(0,0,-0.5f);
                    float radius = ((Vector2)circleEdge.transform.localPosition).magnitude;
                    circleShow.transform.localScale =  new Vector3(radius * 2, radius * 2, 1) ;
                }
            }
        }

        //抬起的时候把颜色复原
        if (Input.GetMouseButtonUp(0))
        {
            if (hit.collider != null)
            {
                hit.collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(
                    hit.collider.gameObject.GetComponent<SpriteRenderer>().color.r / 0.8f,
                    hit.collider.gameObject.GetComponent<SpriteRenderer>().color.g / 0.8f,
                    hit.collider.gameObject.GetComponent<SpriteRenderer>().color.b / 0.8f,
                    hit.collider.gameObject.GetComponent<SpriteRenderer>().color.a);
            }
        }
    }

    public static GameObject InstantiateCircle(string areaName)
    {
        GameObject ciecle = Instantiate(CirclePrefeb, new Vector3(0, 0, (float)-0.5), Quaternion.identity);
        ciecle.name = areaName;
        return ciecle;
    }
    
    //TODO：跟随AreaInfo修改圆的位置和大小
}
