using System;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Sprite[] grounds;
    public GameObject matterPrefab;
    public float ImagePixel = 16.0f; //이미지 픽셀 사이즈 get하는 방법 찾으면 수정
    private int edgeLen; //edgeList 한 변의 길이

    private const byte WATER = 2;
    private const byte GROUND = 1;
    private const byte EMPTY = 0;
    private class QTMatter //Q:quad, T:tree
    {
        public Matter[] matters = new Matter[2];
        public QTMatter[] splitMatter;

        public const int QUAD = 4;
        public const int SOUTH_EAST = 0;
        public const int SOUTH_WEST = 1;
        public const int NORTH_EAST = 2;
        public const int NORTH_WEST = 3;
        public const int WATER = 0;
        public const int GROUND = 1;
    }
    private QTMatter qTMMap;

    //프로그래머스 쿼드 압축 문제 풀어보는 것을 추천.
    //private class QTGround //Q:quad, T:tree
    //{
    //    public object ground= QTGround.emptyGround;
    //    public QuadEdge quadEdge;
    //    public QTGround[] splitGround;
    //    public const int QUAD = 4;

    //    public const int SOUTH_EAST = 0;
    //    public const int SOUTH_WEST = 1;
    //    public const int NORTH_EAST = 2;
    //    public const int NORTH_WEST = 3;
    //    //쓰레기값. ground가 비어있으면 이값 참조
    //    public static object emptyGround = new object();
    //}
    //private QTGround qTGMap;

    //그려질 땅의 종류(리소스 그림의 갯수와 같음), 스프라이트 배열 grounds와 1대1 대응임, 
    //private enum GroundKind{ //edge(x,y)=>Exy , 0은 min, 1은 max
    //    GROUND_E10,
    //    GROUND_E00_AND_E10,
    //    GROUND_E00_AND_E11,
    //    GROUND_E00_AND_E10_AND_E11,
    //    GROUND_ALL,
    //}

    //[Flags]
    //private enum QuadEdge//drawGrounds의 key값으로 이용됨
    //{
    //    NONE = 0,
    //    E00 = 1<<0,
    //    E10 = 1<<1,
    //    E01 = 1<<2,
    //    E11 = 1<<3,
    //    ALL = E00|E10|E01|E11,
    //    MARGE_FAIL = 1<<4
    //}

    

    //그려질 수 있는 땅의 경우
    //edge(x,y)=>EXY , 0은 min, 1은 max
    //private delegate void Draw(GameObject I);
    //private Draw[] drawGrounds;
    //private void DrawGround_E01(GameObject I)
    //{
    //    { 
    //        //        gameObj.transform.eulerAngles = new Vector3(
    //        //    gameObj.transform.eulerAngles.x,
    //        //    gameObj.transform.eulerAngles.y + 180,
    //        //    gameObj.transform.eulerAngles.z
    //        //)
    //    }
    //    DrawGround(I, GroundKind.GROUND_E10, 180f);
    //}
    //private void DrawGround_E11(GameObject I)
    //{
    //    DrawGround(I, GroundKind.GROUND_E10, 90f);
    //}
    //private void DrawGround_E00(GameObject I)
    //{
    //    DrawGround(I, GroundKind.GROUND_E10, 270f);
    //}
    //private void DrawGround_E10(GameObject I)
    //{
    //    DrawGround(I, GroundKind.GROUND_E10);
    //}
    //private void DrawGround_E01AndE11(GameObject I)
    //{
    //    DrawGround(I, GroundKind.GROUND_E00_AND_E10, 180f);
    //}
    //private void DrawGround_E10AndE11(GameObject I)
    //{
    //    DrawGround(I, GroundKind.GROUND_E00_AND_E10, 90f);
    //}
    //private void DrawGround_E00AndE01(GameObject I)
    //{
    //    DrawGround(I, GroundKind.GROUND_E00_AND_E10, 270f);
    //}
    //private void DrawGround_E00AndE10(GameObject I)
    //{
    //    DrawGround(I, GroundKind.GROUND_E00_AND_E10);
    //}
    //private void DrawGround_E10AndE01(GameObject I)
    //{
    //    DrawGround(I, GroundKind.GROUND_E00_AND_E11, 90f);
    //}
    //private void DrawGround_E00AndE11(GameObject I)
    //{
    //    DrawGround(I, GroundKind.GROUND_E00_AND_E11);
    //}
    //private void DrawGround_E00AndE01AndE11(GameObject I)
    //{
    //    DrawGround(I, GroundKind.GROUND_E00_AND_E10_AND_E11, 180f);
    //}
    //private void DrawGround_E10AndE01AndE11(GameObject I)
    //{
    //    DrawGround(I, GroundKind.GROUND_E00_AND_E10_AND_E11, 90f);
    //}
    //private void DrawGround_E00AndE10AndE01(GameObject I)
    //{
    //    DrawGround(I, GroundKind.GROUND_E00_AND_E10_AND_E11, 270f);
    //}
    //private void DrawGround_E00AndE10AndE11(GameObject I)
    //{
    //    DrawGround(I, GroundKind.GROUND_E00_AND_E10_AND_E11);
    //}
    //private void DrawGround_All(GameObject I)
    //{
    //    DrawGround(I, GroundKind.GROUND_ALL);
    //}

    ////직접적으로 그리는 함수
    //private void DrawGround(GameObject I, GroundKind kind, float rot=0f)
    //{
    //    I.transform.rotation = Quaternion.Euler(0f, 0f, rot);
    //    I.GetComponent<SpriteRenderer>().sprite = grounds[(int)kind];
    //}


    ////이제는 사용할 일이 없는 함수
    ////private QuadEdge StringToQuadEdge(string quadEdge)
    ////{
    ////    if (quadEdge == "") return QuadEdge.NONE;
    ////    return (QuadEdge)((int)(quadEdge[0] - '0') * 10 + (int)(quadEdge[1] - '0'));
    ////}

    ////private string QuadEdgeToString(QuadEdge quadEdge)
    ////{
    ////    char[] numberS = new char[2];
    ////    int number = (int)quadEdge;
    ////    numberS[0] = (char)((number / 10)+(int)'0');
    ////    numberS[1] = (char)((number % 10) + (int)'0');
    ////    return new string(numberS);
    ////}

    //private void CreateGround(float x, float y, float size, QuadEdge kind, QTGround qTGMap)
    //{
    //    GroundInit(x, y, size, qTGMap);
    //    drawGrounds[(int)kind]((GameObject)(qTGMap.ground));
    //    ((GameObject)(qTGMap.ground)).GetComponent<SpriteRenderer>().size = new Vector2(ImagePixel * size, ImagePixel * size);
    //}


    // Start is called before the first frame update
    void Start()
    {
        //sin을 이용한 동적 맵 형성 시도 로직
        {
            //var length = 129;
            //var edgeList = new List<List<byte>>(length);
            //for (int y = 0; y < length; y++)
            //{
            //    edgeList.Add(new List<byte>(length));
            //    for (int x = 0; x < length; x++)
            //    {
            //        edgeList[y].Add(EMPTY);
            //    }
            //}
            //int waterHeight = edgeList.Count / 2 - 2;
            //for (int x = 0; x < edgeList.Count; x++)
            //{
            //    int maxY = (int)(UnityEngine.Random.Range(1.2f, 4.6f) * Math.Sin(Math.PI * (double)x * 8.0 / 180.0)) + UnityEngine.Random.Range(edgeList.Count / 2-5,edgeList.Count / 2);
            //    for (int y = 0; y <= maxY; y++)
            //    {
            //        edgeList[y][x] = GROUND;
            //    }
            //    if (maxY < waterHeight)
            //    {
            //        for (int y = maxY + 1; y <= waterHeight; y++)
            //        {
            //            edgeList[y][x] = WATER;
            //        }
            //    }
            //}
        }

        var edgeList = new List<List<byte>>
        {
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 0, 0, 0, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 0, 0, 0, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 0, 0, 0, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
            new List<byte>{ 1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1, 1, 1, 1, 1,1,1,1},
        };
        edgeLen = edgeList.Count;
        int MAX_I = edgeList.Count / 2;
        for (int i = 0; i < MAX_I; i++)
        {
            for (int j = 0; j < edgeList.Count; j++)
            {
                byte cmp = edgeList[i][j];
                edgeList[i][j] = edgeList[edgeList.Count - 1 - i][j];
                edgeList[edgeList.Count - 1 - i][j] = cmp;
            }
        }
        qTMMap = new QTMatter();
        InitQTMatter_DFS(0, edgeList.Count - 2, 0, edgeList.Count - 2, qTMMap, edgeList);
    }

    //private void GroundInit(float x, float y, float size, QTGround qTGMap)
    //{
    //    GameObject ground = Instantiate(groundPrefab, new Vector3(this.gameObject.transform.position.x + (ImagePixel / 2f)*size+x*ImagePixel, this.gameObject.transform.position.y + (ImagePixel / 2f)*size + y*ImagePixel, 1f), Quaternion.identity);
    //    //ground.transform.localScale = new Vector3(1f, 1f, 1f);
    //    ground.SetActive(false);
    //    ground.name = "ground";
    //    ground.transform.parent = this.gameObject.transform;
    //    qTGMap.ground = (object)ground; 
    //}

    private void InitQTMatter_DFS(int minX, int maxX, int minY, int maxY, QTMatter qTMatter, List<List<byte>> edgeList)
    {
        if(minX == maxX) 
        {
            Matter.EMatterEdge water = Matter.EMatterEdge.NULL;
            Matter.EMatterEdge ground = Matter.EMatterEdge.NULL;
            switch(edgeList[minY][minX])
            {
                case GROUND:
                    {
                        ground |= Matter.EMatterEdge.A00;
                        water |= Matter.EMatterEdge.B00;
                    }
                    break;
                case WATER:
                    {
                        water |= Matter.EMatterEdge.A00;
                    }
                    break;
            };
            switch (edgeList[minY][minX + 1])
            {
                case GROUND:
                    {
                        ground |= Matter.EMatterEdge.A01;
                        water |= Matter.EMatterEdge.B01;
                    }
                    break;
                case WATER:
                    {
                        water |= Matter.EMatterEdge.A01;
                    }
                    break;
            }
            switch (edgeList[minY+1][minX])
            {
                case GROUND:
                    {
                        ground |= Matter.EMatterEdge.A10;
                        water |= Matter.EMatterEdge.B10;
                    }
                    break;
                case WATER:
                    {
                        water |= Matter.EMatterEdge.A10;
                    }
                    break;
            }
            switch (edgeList[minY+1][minX+1])
            {
                case GROUND:
                    {
                        ground |= Matter.EMatterEdge.A11;
                        water |= Matter.EMatterEdge.B11;
                    }
                    break;
                case WATER:
                    {
                        water |= Matter.EMatterEdge.A11;
                    }
                    break;
            }

            var pos = new Vector3(this.gameObject.transform.position.x + ImagePixel * (1f / 2f + (float)minX), this.gameObject.transform.position.y + ImagePixel * (1f / 2f + (float)minY), 1f);
            if ((water & Matter.EMatterEdge.A_ALL) != (Matter.EMatterEdge.NULL))
                InitQTMatterMacro(ref pos, water, Matter.EMatterKind.WATER, QTMatter.WATER, ImagePixel, qTMatter);
            else
                InitQTMatterMacro(ref pos, Matter.EMatterEdge.NULL, Matter.EMatterKind.WATER, QTMatter.WATER, ImagePixel, qTMatter);
            InitQTMatterMacro(ref pos, ground, Matter.EMatterKind.GROUND, QTMatter.GROUND, ImagePixel, qTMatter);
            return;
        }

        int midX = (minX + maxX) / 2;
        int midY = (minY + maxY) / 2;
        qTMatter.splitMatter = new QTMatter[QTMatter.QUAD];
        for (int i = 0; i < QTMatter.QUAD; i++)
            qTMatter.splitMatter[i] = new QTMatter();

        //남동
        InitQTMatter_DFS(minX, midX, minY, midY, qTMatter.splitMatter[QTMatter.SOUTH_EAST], edgeList);
        //남서
        InitQTMatter_DFS(midX + 1, maxX, minY, midY, qTMatter.splitMatter[QTMatter.SOUTH_WEST], edgeList);
        //북동
        InitQTMatter_DFS(minX, midX, midY + 1, maxY, qTMatter.splitMatter[QTMatter.NORTH_EAST], edgeList);
        //북서
        InitQTMatter_DFS(midX + 1, maxX, midY + 1, maxY, qTMatter.splitMatter[QTMatter.NORTH_WEST], edgeList);

        Matter.SMatterInfo[,] sMatterInfo = new Matter.SMatterInfo[,]
        {
            {
                qTMatter.splitMatter[QTMatter.SOUTH_EAST].matters[QTMatter.WATER].GetInfo(), 
                qTMatter.splitMatter[QTMatter.SOUTH_EAST].matters[QTMatter.GROUND].GetInfo() 
            },
            {
                qTMatter.splitMatter[QTMatter.SOUTH_WEST].matters[QTMatter.WATER].GetInfo(),
                qTMatter.splitMatter[QTMatter.SOUTH_WEST].matters[QTMatter.GROUND].GetInfo()
            },
            {
                qTMatter.splitMatter[QTMatter.NORTH_EAST].matters[QTMatter.WATER].GetInfo(),
                qTMatter.splitMatter[QTMatter.NORTH_EAST].matters[QTMatter.GROUND].GetInfo()
            },
            {
                qTMatter.splitMatter[QTMatter.NORTH_WEST].matters[QTMatter.WATER].GetInfo(),
                qTMatter.splitMatter[QTMatter.NORTH_WEST].matters[QTMatter.GROUND].GetInfo()
            }
        };

        {
            float size = (float)(maxX - minX) + 1f;
            var pos = new Vector3(this.gameObject.transform.position.x + ImagePixel * (size / 2f + (float)minX), this.gameObject.transform.position.y + ImagePixel * (size / 2f + (float)minY), 1f);
            if (sMatterInfo[QTMatter.SOUTH_EAST, QTMatter.WATER].edge == sMatterInfo[QTMatter.SOUTH_WEST, QTMatter.WATER].edge &&
                sMatterInfo[QTMatter.SOUTH_EAST, QTMatter.WATER].edge == sMatterInfo[QTMatter.NORTH_EAST, QTMatter.WATER].edge &&
                sMatterInfo[QTMatter.SOUTH_EAST, QTMatter.WATER].edge == sMatterInfo[QTMatter.NORTH_WEST, QTMatter.WATER].edge &&
                sMatterInfo[QTMatter.SOUTH_EAST, QTMatter.WATER].kind != Matter.EMatterKind.MergeFail &&
                sMatterInfo[QTMatter.SOUTH_WEST, QTMatter.WATER].kind != Matter.EMatterKind.MergeFail &&
                sMatterInfo[QTMatter.NORTH_EAST, QTMatter.WATER].kind != Matter.EMatterKind.MergeFail &&
                sMatterInfo[QTMatter.NORTH_WEST, QTMatter.WATER].kind != Matter.EMatterKind.MergeFail)
            {
                if (sMatterInfo[QTMatter.SOUTH_EAST, QTMatter.WATER].edge == Matter.EMatterEdge.A_ALL)
                {
                    InitQTMatterMacro(ref pos, Matter.EMatterEdge.A_ALL, Matter.EMatterKind.WATER, QTMatter.WATER, ImagePixel * size, qTMatter);
                    InitQTMatterMacro(ref pos, Matter.EMatterEdge.NULL, Matter.EMatterKind.GROUND, QTMatter.GROUND, ImagePixel * size, qTMatter);
                    if (maxX - minX + 1 == edgeLen - 1)
                        qTMatter.matters[QTMatter.WATER].gameObject.SetActive(true);
                    return;
                }
                else
                {
                    if (sMatterInfo[QTMatter.SOUTH_EAST, QTMatter.GROUND].edge == sMatterInfo[QTMatter.SOUTH_WEST, QTMatter.GROUND].edge &&
                        sMatterInfo[QTMatter.SOUTH_EAST, QTMatter.GROUND].edge == sMatterInfo[QTMatter.NORTH_EAST, QTMatter.GROUND].edge &&
                        sMatterInfo[QTMatter.SOUTH_EAST, QTMatter.GROUND].edge == sMatterInfo[QTMatter.NORTH_WEST, QTMatter.GROUND].edge)
                    {
                        if (sMatterInfo[QTMatter.SOUTH_EAST, QTMatter.GROUND].edge == Matter.EMatterEdge.A_ALL)
                        {
                            InitQTMatterMacro(ref pos, Matter.EMatterEdge.NULL, Matter.EMatterKind.WATER, QTMatter.WATER, ImagePixel * size, qTMatter);
                            InitQTMatterMacro(ref pos, Matter.EMatterEdge.A_ALL, Matter.EMatterKind.GROUND, QTMatter.GROUND, ImagePixel * size, qTMatter);
                            if (maxX - minX + 1 == edgeLen - 1)
                                qTMatter.matters[QTMatter.GROUND].gameObject.SetActive(true);
                        }
                        else if (sMatterInfo[QTMatter.SOUTH_EAST, QTMatter.GROUND].edge == Matter.EMatterEdge.NULL)
                        {
                            InitQTMatterMacro(ref pos, Matter.EMatterEdge.NULL, Matter.EMatterKind.WATER, QTMatter.WATER, ImagePixel * size, qTMatter);
                            InitQTMatterMacro(ref pos, Matter.EMatterEdge.NULL, Matter.EMatterKind.GROUND, QTMatter.GROUND, ImagePixel * size, qTMatter);
                        }
                        return;
                    }
                }
            }
            InitQTMatterMacro(ref pos, Matter.EMatterEdge.NULL, Matter.EMatterKind.MergeFail, QTMatter.WATER, ImagePixel * size, qTMatter);
            InitQTMatterMacro(ref pos, Matter.EMatterEdge.NULL, Matter.EMatterKind.MergeFail, QTMatter.GROUND, ImagePixel * size, qTMatter);
        }
        for (int i = 0; i < 4; i++)
        {
            for(int j=0; j<2; j++)
            {
                if(sMatterInfo[i,j].edge == Matter.EMatterEdge.A_ALL)
                {
                    qTMatter.splitMatter[i].matters[j].gameObject.SetActive(true);
                    break;
                }
            }
        }
    }  
    private void InitQTMatterMacro(ref Vector3 pos, Matter.EMatterEdge eMatterEdge, Matter.EMatterKind eMatterKind, int mattersIndx, float size, QTMatter qTMatter)
    {
        var matter = Instantiate(matterPrefab, pos, Quaternion.identity);
        matter.SetActive(false);
        matter.transform.parent = this.gameObject.transform;
        qTMatter.matters[mattersIndx] = matter.GetComponent<Matter>();
        qTMatter.matters[mattersIndx].StartRun();
        qTMatter.matters[mattersIndx].SetMatter(eMatterKind, eMatterEdge, size);
        if (eMatterEdge != Matter.EMatterEdge.NULL && eMatterEdge != Matter.EMatterEdge.A_ALL)
            qTMatter.matters[mattersIndx].gameObject.SetActive(true);
    }

    // Update is called once per frame
    //public float speed;
    //private float delta;
    //private int dir = QTGround.SOUTH_EAST;
    //private int pivotX = 0;
    //private int pivotY = 0;
    //private int step;
    //private bool remove = true;
    void Update()
    {
        {
            //if (delta > 0f)
            //{
            //    delta -= Time.deltaTime;
            //}
            //else
            //{
            //    delta = speed;
            //    if (remove)
            //        RemoveQTGround_DFS(ref pivotX, ref pivotY, 0, edgeLen - 1, 0, edgeLen - 1, qTGMap);
            //    else
            //        InsertQTGround_DFS(ref pivotX, ref pivotY, 0, edgeLen - 1, 0, edgeLen - 1, qTGMap);
            //    int mid = edgeLen / 2;
            //    if (pivotX != mid || pivotY != mid)
            //    {
            //        switch (dir)
            //        {
            //            case QTGround.SOUTH_EAST:
            //                {
            //                    ++pivotX;
            //                    if (pivotX == step)
            //                        dir = QTGround.SOUTH_WEST;
            //                }
            //                break;
            //            case QTGround.SOUTH_WEST:
            //                {
            //                    ++pivotY;
            //                    if (pivotY == step)
            //                        dir = QTGround.NORTH_EAST;
            //                }
            //                break;
            //            case QTGround.NORTH_EAST:
            //                {
            //                    --pivotX;
            //                    if (pivotX < edgeLen - step)
            //                    {
            //                        dir = QTGround.NORTH_WEST;
            //                    }
            //                }
            //                break;
            //            case QTGround.NORTH_WEST:
            //                {
            //                    --pivotY;
            //                    if (pivotY < edgeLen - step)
            //                    {
            //                        dir = QTGround.SOUTH_EAST;
            //                        ++pivotX;
            //                        ++pivotY;
            //                        --step;
            //                    }
            //                }
            //                break;
            //        }
            //    }
            //    else
            //    {
            //        pivotX = 0;
            //        pivotY = 0;
            //        remove = !remove;
            //        dir = QTGround.SOUTH_EAST;
            //        step = edgeLen - 1;
            //    }
            //}
            /*
            var pos = this.gameObject.transform.position;
            pos.x += Time.deltaTime * 3.0f;
            this.gameObject.transform.position = pos;
            */
        }
    }

    public void CrushCheck(in Vector2 minXY, in Vector2 maxXY, Human human)
    {
        Vector2 mapMinXY = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        float size = (float)(edgeLen - 1) * ImagePixel;
        Vector2 mapMaxXY = new Vector2(mapMinXY.x + size, mapMinXY.y + size);
        CrushCheck_DFS(in minXY, in maxXY, human, mapMinXY, mapMaxXY, qTMMap);
    }

    private void CrushCheck_DFS(in Vector2 minXY, in Vector2 maxXY, Human human, Vector2 mapMinXY, Vector2 mapMaxXY, QTMatter qTMatter)
    {
        //AABB 충돌체크 알고리즘
        if(mapMinXY.x<maxXY.x&&
            minXY.x<mapMaxXY.x&&
            mapMinXY.y<maxXY.y&&
            minXY.y < mapMaxXY.y)
        {
            if (qTMatter.matters[QTMatter.WATER].GetInfo().kind == Matter.EMatterKind.MergeFail)
            {
                Vector2 middle = (mapMinXY + mapMaxXY) * 0.5f;
                CrushCheck_DFS(in minXY, in maxXY, human, mapMinXY, middle, qTMatter.splitMatter[QTMatter.SOUTH_EAST]);
                CrushCheck_DFS(in minXY, in maxXY, human, new Vector2(middle.x,mapMinXY.y), new Vector2(mapMaxXY.x, middle.y), qTMatter.splitMatter[QTMatter.SOUTH_WEST]);
                CrushCheck_DFS(in minXY, in maxXY, human, new Vector2(mapMinXY.x, middle.y), new Vector2(middle.x, mapMaxXY.y), qTMatter.splitMatter[QTMatter.NORTH_EAST]);
                CrushCheck_DFS(in minXY, in maxXY, human, middle, mapMaxXY, qTMatter.splitMatter[QTMatter.NORTH_WEST]);
                return;
            }
            foreach (var matter in qTMatter.matters){
                matter.Contact(human);
            }
        }
    }



    //밀도(점)가 한 번씩 줄어들때마다 한 번씩 호출하는 함수. 큰 그라운드(압축된)는 쪼개면서 진행됨 
    //이미지 출력이 버벅이는 느낌들면 여기 수정 예정. 델리게이트 체인 써서 한꺼번에 수정하는 방향으로
    public void RemoveMatter(Vector2 pos)
    {
        pos = new Vector2(pos.x - transform.position.x, pos.y - transform.position.y)/ImagePixel;
        int pointX = (int)pos.x;
        if( 0.499999f < pos.x - (float)pointX)
        {
            ++pointX;   
        }
        int pointY = (int)pos.y;
        if (0.499999f < pos.y - (float)pointY)
        {
            ++pointY;
        }
        if (pointX < 0) pointX = 0;
        else if (pointX > edgeLen - 1) pointX = edgeLen - 1;
        if (pointY < 0) pointY = 0;
        else if (pointY > edgeLen - 1) pointY = edgeLen - 1;
        RemoveQTMatter_DFS(in pointX, in pointY, 0, 0, edgeLen - 1, edgeLen - 1, qTMMap);
    }
    private void RemoveQTMatter_DFS(in int pointX, in int pointY, int minX, int minY, int maxX, int maxY, QTMatter qTMatter)
    {
        Matter.SMatterInfo groundInfo = qTMatter.matters[QTMatter.GROUND].GetInfo();
        if (groundInfo.kind == Matter.EMatterKind.GROUND&&groundInfo.edge == Matter.EMatterEdge.NULL) return;
        int size = (maxX - minX) / 2;
        if (size > 0)
        {
            float sizeF = ((float)(size*2))*ImagePixel;
            if(groundInfo.kind == Matter.EMatterKind.GROUND) // 꽉 차 있음. 그러니 분열 시켜야 함
            {
                qTMatter.matters[QTMatter.WATER].SetMatter(Matter.EMatterKind.MergeFail, Matter.EMatterEdge.NULL, sizeF);
                qTMatter.matters[QTMatter.GROUND].SetMatter(Matter.EMatterKind.MergeFail, Matter.EMatterEdge.NULL, sizeF);
                sizeF *= 0.5f;
                for(int i =0; i < QTMatter.QUAD; i++)
                {
                    qTMatter.splitMatter[i].matters[QTMatter.WATER].SetMatter(Matter.EMatterKind.WATER, Matter.EMatterEdge.NULL, sizeF);
                    qTMatter.splitMatter[i].matters[QTMatter.GROUND].SetMatter(Matter.EMatterKind.GROUND, Matter.EMatterEdge.A_ALL, sizeF);
                    qTMatter.splitMatter[i].matters[QTMatter.GROUND].gameObject.SetActive(true);
                }
            }
            int midX = (minX + maxX) / 2;
            int midY = (minY + maxY) / 2;
            if (minY <= pointY && pointY <= midY)
            {
                if (minX <= pointX && pointX <= midX)
                {
                    RemoveQTMatter_DFS(pointX, pointY, minX, minY, midX, midY, qTMatter.splitMatter[QTMatter.SOUTH_EAST]);
                }
                if (midX <= pointX && pointX <= maxX)
                {
                    RemoveQTMatter_DFS(pointX, pointY, midX, minY, maxX, midY, qTMatter.splitMatter[QTMatter.SOUTH_WEST]);
                }
            }
            if (midY <= pointY && pointY <= maxY)
            {
                if (minX <= pointX && pointX <= midX)
                {
                    RemoveQTMatter_DFS(pointX, pointY, minX, midY, midX, maxY, qTMatter.splitMatter[QTMatter.NORTH_EAST]);
                }
                if (midX <= pointX && pointX <= maxX)
                {
                    RemoveQTMatter_DFS(pointX, pointY, midX, midY, maxX, maxY, qTMatter.splitMatter[QTMatter.NORTH_WEST]);
                }
            }
            Matter.SMatterInfo[,] sMatterInfo = new Matter.SMatterInfo[2,QTMatter.QUAD];
            for(int i = 0; i< QTMatter.QUAD; i++)
            {
                sMatterInfo[QTMatter.WATER, i] = qTMatter.splitMatter[i].matters[QTMatter.WATER].GetInfo();
                sMatterInfo[QTMatter.GROUND, i] = qTMatter.splitMatter[i].matters[QTMatter.GROUND].GetInfo();
            }
            sizeF *= 2;
            if (sMatterInfo[QTMatter.WATER, QTMatter.SOUTH_EAST].kind == Matter.EMatterKind.EMPTY && 
               sMatterInfo[QTMatter.WATER, QTMatter.SOUTH_WEST].kind == Matter.EMatterKind.EMPTY && 
               sMatterInfo[QTMatter.WATER, QTMatter.NORTH_EAST].kind == Matter.EMatterKind.EMPTY && 
               sMatterInfo[QTMatter.WATER, QTMatter.NORTH_WEST].kind == Matter.EMatterKind.EMPTY &&
               sMatterInfo[QTMatter.GROUND, QTMatter.SOUTH_EAST].kind == Matter.EMatterKind.EMPTY && 
               sMatterInfo[QTMatter.GROUND, QTMatter.SOUTH_WEST].kind == Matter.EMatterKind.EMPTY &&
               sMatterInfo[QTMatter.GROUND, QTMatter.NORTH_EAST].kind == Matter.EMatterKind.EMPTY &&
               sMatterInfo[QTMatter.GROUND, QTMatter.NORTH_WEST].kind == Matter.EMatterKind.EMPTY)
            {
                qTMatter.matters[QTMatter.WATER].SetMatter(Matter.EMatterKind.WATER, Matter.EMatterEdge.NULL, sizeF);
                qTMatter.matters[QTMatter.GROUND].SetMatter(Matter.EMatterKind.GROUND, Matter.EMatterEdge.NULL, sizeF);
            }
        }
        else
        {
            Matter.SMatterInfo[] sMatterInfo = new Matter.SMatterInfo[2]
            {
                qTMatter.matters[QTMatter.WATER].GetInfo(),
                qTMatter.matters[QTMatter.GROUND].GetInfo()
            };

            if (minY == pointY)
            {
                if (minX == pointX)
                {
                    sMatterInfo[QTMatter.WATER].edge &= ~Matter.EMatterEdge.B00;
                    sMatterInfo[QTMatter.GROUND].edge &= ~Matter.EMatterEdge.A00;
                }
                else
                {
                    sMatterInfo[QTMatter.WATER].edge &= ~Matter.EMatterEdge.B01;
                    sMatterInfo[QTMatter.GROUND].edge &= ~Matter.EMatterEdge.A01;
                }
            }
            else
            {
                if (minX == pointX)
                {
                    sMatterInfo[QTMatter.WATER].edge &= ~Matter.EMatterEdge.B10;
                    sMatterInfo[QTMatter.GROUND].edge &= ~Matter.EMatterEdge.A10;
                }
                else
                {
                    sMatterInfo[QTMatter.WATER].edge &= ~Matter.EMatterEdge.B11;
                    sMatterInfo[QTMatter.GROUND].edge &= ~Matter.EMatterEdge.A11;
                }
            }
            qTMatter.matters[QTMatter.WATER].SetMatter(Matter.EMatterKind.WATER, sMatterInfo[QTMatter.WATER].edge, ImagePixel);
            qTMatter.matters[QTMatter.GROUND].SetMatter(Matter.EMatterKind.GROUND, sMatterInfo[QTMatter.GROUND].edge, ImagePixel);
        }
    }
    public void InsertMatter(Vector2 pos)
    {
        pos = new Vector2(pos.x - transform.position.x, pos.y - transform.position.y) / ImagePixel;
        int pointX = (int)pos.x;
        if (0.499999f < pos.x - (float)pointX)
        {
            ++pointX;
        }
        int pointY = (int)pos.y;
        if (0.499999f < pos.y - (float)pointY)
        {
            ++pointY;
        }
        if (pointX < 0) pointX = 0;
        else if (pointX > edgeLen - 1) pointX = edgeLen - 1;
        if (pointY < 0) pointY = 0;
        else if (pointY > edgeLen - 1) pointY = edgeLen - 1;
        InsertQTMatter_DFS(in pointX, in pointY, 0, 0, edgeLen - 1, edgeLen - 1, qTMMap);
    }
    private void InsertQTMatter_DFS(in int pointX, in int pointY, int minX, int minY, int maxX, int maxY, QTMatter qTMatter)
    {
        if (qTMatter.matters[QTMatter.GROUND].GetInfo().edge == Matter.EMatterEdge.A_ALL) return;
        int size = (maxX - minX) / 2;
        if (size > 0)
        {
            int midX = (maxX + minX) / 2;
            int midY = (maxY + minY) / 2;
            if (minY  <= pointY && pointY <= midY)
            {
                if(minX <= pointX && pointX <= midX)
                {
                    InsertQTMatter_DFS(in pointX, in pointY, minX, minY, midX, midY, qTMatter.splitMatter[QTMatter.SOUTH_EAST]);
                }
                if(midX<= pointX && pointX <= maxX)
                {
                    InsertQTMatter_DFS(in pointX, in pointY, midX, minY, maxX, midY, qTMatter.splitMatter[QTMatter.SOUTH_WEST]);
                }
            }
            if(midY <= pointY && pointY <= maxY)
            {
                if (minX <= pointX && pointX <= midX)
                {
                    InsertQTMatter_DFS(in pointX, in pointY, minX, midY, midX, maxY, qTMatter.splitMatter[QTMatter.NORTH_EAST]);
                }
                if (midX <= pointX && pointX <= maxX)
                {
                    InsertQTMatter_DFS(in pointX, in pointY, midX, midY, maxX, maxY, qTMatter.splitMatter[QTMatter.NORTH_WEST]);
                }
            }
            Matter.SMatterInfo[] sMatterInfo = new Matter.SMatterInfo[4];
            for(int i = 0; i<QTMatter.QUAD; i++)
            {
                sMatterInfo[i] = qTMatter.splitMatter[i].matters[QTMatter.GROUND].GetInfo();
            }
            float sizeF = ((float)(size*2))*ImagePixel;
            if (sMatterInfo[QTMatter.SOUTH_EAST].edge == Matter.EMatterEdge.A_ALL &&
               sMatterInfo[QTMatter.SOUTH_WEST].edge == Matter.EMatterEdge.A_ALL &&
               sMatterInfo[QTMatter.NORTH_EAST].edge == Matter.EMatterEdge.A_ALL &&
               sMatterInfo[QTMatter.NORTH_WEST].edge == Matter.EMatterEdge.A_ALL)
            {
                qTMatter.matters[QTMatter.WATER].SetMatter(Matter.EMatterKind.WATER, Matter.EMatterEdge.NULL, sizeF);
                qTMatter.matters[QTMatter.GROUND].SetMatter(Matter.EMatterKind.GROUND, Matter.EMatterEdge.A_ALL, sizeF);
                qTMatter.matters[QTMatter.GROUND].gameObject.SetActive(true);
                for (int i = 0; i < QTMatter.QUAD; i++)
                {
                    qTMatter.splitMatter[i].matters[QTMatter.GROUND].gameObject.SetActive(false);
                }
                return;
            }
            qTMatter.matters[QTMatter.WATER].SetMatter(Matter.EMatterKind.MergeFail, Matter.EMatterEdge.NULL, sizeF);
            qTMatter.matters[QTMatter.GROUND].SetMatter(Matter.EMatterKind.MergeFail, Matter.EMatterEdge.NULL, sizeF);
            sizeF *= 0.5f;
            for(int i = 0; i<QTMatter.QUAD; i++)
            {
                if(sMatterInfo[i].edge == Matter.EMatterEdge.A_ALL)
                {
                    qTMatter.splitMatter[i].matters[QTMatter.WATER].SetMatter(Matter.EMatterKind.WATER, Matter.EMatterEdge.NULL, sizeF);
                    qTMatter.splitMatter[i].matters[QTMatter.GROUND].SetMatter(Matter.EMatterKind.GROUND, Matter.EMatterEdge.A_ALL, sizeF);
                    qTMatter.splitMatter[i].matters[QTMatter.GROUND].gameObject.SetActive(true);
                }
            }
        }
        else
        {
            Matter.SMatterInfo[] sMatterInfo = new Matter.SMatterInfo[2]
            {
                qTMatter.matters[QTMatter.WATER].GetInfo(),
                qTMatter.matters[QTMatter.GROUND].GetInfo()
            };
            if (minY == pointY)
            {
                if (minX == pointX)
                {
                    sMatterInfo[QTMatter.WATER].edge |= Matter.EMatterEdge.B00;
                    sMatterInfo[QTMatter.GROUND].edge |= Matter.EMatterEdge.A00;
                }
                else
                {
                    sMatterInfo[QTMatter.WATER].edge |= Matter.EMatterEdge.B01;
                    sMatterInfo[QTMatter.GROUND].edge |= Matter.EMatterEdge.A01;
                }
            }
            else
            {
                if (minX == pointX)
                {
                    sMatterInfo[QTMatter.WATER].edge |= Matter.EMatterEdge.B10;
                    sMatterInfo[QTMatter.GROUND].edge |= Matter.EMatterEdge.A10;
                }
                else
                {
                    sMatterInfo[QTMatter.WATER].edge |= Matter.EMatterEdge.B11;
                    sMatterInfo[QTMatter.GROUND].edge |= Matter.EMatterEdge.A11;
                }
            }
            if (sMatterInfo[QTMatter.GROUND].edge == Matter.EMatterEdge.A_ALL)
            {
                qTMatter.matters[QTMatter.WATER].SetMatter(Matter.EMatterKind.WATER, Matter.EMatterEdge.NULL, ImagePixel);
                qTMatter.matters[QTMatter.GROUND].SetMatter(Matter.EMatterKind.GROUND, Matter.EMatterEdge.A_ALL, ImagePixel);
                qTMatter.matters[QTMatter.GROUND].gameObject.SetActive(false);
            }
            else
            {
                if (sMatterInfo[QTMatter.WATER].kind != Matter.EMatterKind.EMPTY)
                {
                    qTMatter.matters[QTMatter.WATER].SetMatter(Matter.EMatterKind.WATER, sMatterInfo[QTMatter.WATER].edge, ImagePixel);
                }
                else
                {
                    qTMatter.matters[QTMatter.WATER].SetMatter(Matter.EMatterKind.WATER, Matter.EMatterEdge.NULL, ImagePixel);
                }
                qTMatter.matters[QTMatter.GROUND].SetMatter(Matter.EMatterKind.GROUND, sMatterInfo[QTMatter.GROUND].edge, ImagePixel);
                qTMatter.matters[QTMatter.GROUND].gameObject.SetActive(true);
            }
        }
    }

    //public void CrushCheck(Human human, Vector2 nextPos, Vector2 checkSize)
    //{

    //}
    //private void CrushCheck_DFS(ref Vector2 minPos, ref Vector2 maxPos, Vector2 minMap, Vector2 maxMap, QTMatter qTMatter)
    //{

    //}
}
