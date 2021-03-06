using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using kgh.Signals;
public class line1 : MonoBehaviour
{
    [SerializeField] GameObject Input_line1;
    Camera Camera;
    public LayerMask LayerMask;
    public static line1 instance;
    public Vector3 mousePos;
    public Vector3 RaymousePos;
    public Vector3 RayTouchPos;
    public float Distance;//mouse drag distance
    float MaxDis = 999f;//raycast ray max distance
    public Vector2 TouchPos;Dictionary<int, Vector2> touchStartPos;
    private Vector2 Direction;
    public bool swiped = false;
    public int dragDirection;
    public event Action<int> userInputEvent;
    public float MinMovement;
    public float[] diagonals = { 45, 135, 225, 315 };
    public float windowInDeg = 20f;
    public int lineCount = 0;
    public int SwipeEndCount =0;
    public GameObject String_skel,String_skel1,String_skel2,String_skel3;
    Switch _switch;
    void Start(){
        //Camera = GetComponent<Camera>();
    }
    void Update(){
        processMobileInput();//유니티 모바일일때
        //ProcessInput();//PC
    }
    void Awake(){
        Camera = GetComponent<Camera>();
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        MinMovement = Mathf.Max(screenSize.x, screenSize.y) / 70f;
        _switch = GameManager.instance.sigs.Register("OnMouseBehavior", typeof(Action<int,int>));
        // RayAll();
        enalbeCollider();
        
        instance = this;
    }
    public void ProcessInput()// 유저의 드래그 방향을 알기 위함.
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            mousePos = Input.mousePosition;
            swiped = false;
        }
        else if (Input.GetMouseButton(0) == true)
        {
            Direction = (Input.mousePosition - mousePos).normalized;
            Distance = Vector2.Distance(Input.mousePosition, mousePos);
            RayAll();
        }
        else if (Input.GetMouseButtonUp(0) == true)
        {
            float clockwiseDeg = 360f - Quaternion.FromToRotation(Vector2.up, Direction).eulerAngles.z;
            dragDirection = checkDirection_mouse(clockwiseDeg);

            enalbeCollider();
            swiped = true;
            SwipeEndCount = lineCount;
            lineCount = 0;

            _switch.Invoke(dragDirection, SwipeEndCount);
        }
    }
    void processMobileInput()
    {
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {//터치시작 좌표 저장
                TouchPos = new Vector2(t.position.x, t.position.y);
                swiped = false;
            }
            else if (t.phase == TouchPhase.Moved)
            {
                Vector2 currentTouchPos = new Vector2(t.position.x, t.position.y);
                Direction = (currentTouchPos - TouchPos).normalized;
                Distance = Vector2.Distance(currentTouchPos, TouchPos);
                
                RayAll();
            }
            else if (t.phase == TouchPhase.Ended)
            {
                //Vector2 endPos = Camera.main.ScreenToWorldPoint(t.position);
                //Vector2 dir = (endPos - touchStartPos[t.fingerId]).normalized;
                float clockwiseDeg = 360f - Quaternion.FromToRotation(Vector2.up, Direction).eulerAngles.z;
                int dirCode = checkDirection_mouse(clockwiseDeg);
                touchStartPos.Remove(t.fingerId);
                // if (userInputEvent != null) userInputEvent.Invoke(dirCode);
                _switch.Invoke(dirCode, lineCount);
                swiped = true;
                SwipeEndCount = lineCount;
                lineCount = 0;
                
                _switch.Invoke(SwipeEndCount);
                
            }
        }
    }
    public int checkDirection_mouse(float Deg)
    {
        if (Distance < MinMovement)
        {
            //Debug.Log("Touch");
            return 0;
        }
        else if ((Deg > diagonals[3] + windowInDeg && Deg <= 360) ||
                   (Deg <= diagonals[0] - windowInDeg && Deg >= 0))
        {
            //Debug.Log("UP");
            return 1;
        }
        else if (Deg > diagonals[0] - windowInDeg && Deg <= diagonals[0] + windowInDeg)
        {
            //Debug.Log("UP_RIGHT");
            return 1;
        }
        else if (Deg > diagonals[1] - windowInDeg && Deg <= diagonals[1] + windowInDeg)
        {
            //Debug.Log("DOWN_RIGHT");
            return 2;
        }
        else if (Deg > diagonals[1] + windowInDeg && Deg <= diagonals[2] - windowInDeg)
        {
            //Debug.Log("DOWN");
            return 2;
        }
        else if (Deg > diagonals[2] - windowInDeg && Deg <= diagonals[2] + windowInDeg)
        {
            //Debug.Log("DOWN_LEFT");
            return 2;
        }
        else
        {
            return 1;
            //Debug.Log("UP_LEFT");
        }
    }
    void RayAll(){
        RaycastHit2D[] hits;
        // RaymousePos = Input.mousePosition;
        // RaymousePos = Camera.ScreenToWorldPoint(RaymousePos);
        // hits = Physics2D.RaycastAll(RaymousePos, transform.position, MaxDis, LayerMask);
        RayTouchPos = Input.GetTouch(0).position;
        RayTouchPos = Camera.ScreenToWorldPoint(RayTouchPos);
        Scene_1.instance.Print(TouchPos.x.ToString() + ", " + TouchPos.y.ToString());
        hits = Physics2D.RaycastAll(TouchPos, transform.position, MaxDis);

        for(int i=0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];
                BoxCollider2D Linehit = hit.transform.GetComponent<BoxCollider2D>();

                if(Linehit){
                    Linehit.enabled =false;
                    if(Linehit.name == "String_skel"){
                        StringAnimation.instance.Shake();
                    }
                    if(Linehit.name == "String_skel1"){
                        StringAnimation1.instance.Shake();
                    }
                    if(Linehit.name == "String_skel2"){
                        StringAnimation2.instance.Shake();
                    }
                    if(Linehit.name == "String_skel3"){
                        StringAnimation3.instance.Shake();
                    }
                    lineCount++;
                }
            }
    }
    void enalbeCollider(){
        GameObject[] lines;
        lines = GameObject.FindGameObjectsWithTag("Line");
        foreach(GameObject Line in lines){
            BoxCollider2D collider = Line.transform.GetComponent<BoxCollider2D>();
            collider.enabled = true;
        }
    }
}
