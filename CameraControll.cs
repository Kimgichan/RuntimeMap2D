using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    public GameObject target;
    public float speed;
    public float zoomSpeed;
    private Camera cam;
    [SerializeField] GameObject mouseCursor;
    [SerializeField] Sprite[] cursorList;
    [SerializeField] Map map;
    private CursorState cursorState = CursorState.NORMER;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }
    // Start is called before the first frame update
    void Start()
    {
        cam = this.gameObject.GetComponent<Camera>();
        Cursor.visible = false;
        mouseCursor.GetComponent<SpriteRenderer>().sprite = cursorList[0];
    }

    private enum CursorState
    {
        NORMER = 0,
        REMOVE,
        INSERT
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = new Vector2(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y);
        pos *= speed;
        transform.position = new Vector3(transform.position.x + pos.x, transform.position.y + pos.y, transform.position.z);
        Vector2 mouseWheel = Input.mouseScrollDelta;
        if(mouseWheel.y > 0)
        {
            float size = cam.orthographicSize + Time.deltaTime * zoomSpeed;
            if(size > 1280f)
            {
                size = 1280f;
            }
            cam.orthographicSize = size;
            float scale = size / 45f;
            mouseCursor.transform.localScale = new Vector3(scale, scale, 1f);
        }
        else if(mouseWheel.y < 0)
        {
            float size = cam.orthographicSize - Time.deltaTime * zoomSpeed;
            if (size < 45f)
            {
                size = 45f;
            }
            cam.orthographicSize = size;
            float scale = size / 45f;
            mouseCursor.transform.localScale = new Vector3(scale, scale, 1f);
        }
        if (Input.GetMouseButton(0))
        {
            if (cursorState != CursorState.INSERT)
            {
                cursorState = CursorState.INSERT;
                mouseCursor.GetComponent<SpriteRenderer>().sprite = cursorList[1];
            }
            map.InsertMatter(new Vector2(mouseCursor.transform.position.x, mouseCursor.transform.position.y));
        }
        else if (Input.GetMouseButton(1))
        {
            if(cursorState != CursorState.REMOVE)
            {
                cursorState = CursorState.REMOVE;
                mouseCursor.GetComponent<SpriteRenderer>().sprite = cursorList[2];
            }
            map.RemoveMatter(new Vector2(mouseCursor.transform.position.x, mouseCursor.transform.position.y));
        }
        else
        {
            if(cursorState != CursorState.NORMER)
            {
                cursorState = CursorState.NORMER;
                mouseCursor.GetComponent<SpriteRenderer>().sprite = cursorList[0];
            }
        }
        {
            var mousePos = Input.mousePosition;
            mousePos = cam.ScreenToWorldPoint(mousePos);
            mousePos.z = -1f;
            mouseCursor.transform.position = mousePos;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
