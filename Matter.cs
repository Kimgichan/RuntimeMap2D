using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matter : MonoBehaviour
{
    [SerializeField]
    private MatterList matterTable;
    private Action<Matter, Human> contactAction;
    private Func<SMatterInfo> getInfo;
    // Start is called before the first frame update
    public void StartRun()
    {
        contactAction = new Action<Matter, Human>(Matter.ContactNull);
        getInfo = new Func<SMatterInfo>(Matter.MergeFailMatter);
    }

    // Update is called once per frame

    public void SetMatter(EMatterKind kind, EMatterEdge edge, float size)
    {
        matterCreateDic[kind][edge](this);
        if (edge == EMatterEdge.NULL)
            gameObject.SetActive(false);
        this.gameObject.GetComponent<SpriteRenderer>().size = new Vector2(size, size);
    }

    public SMatterInfo GetInfo()
    {
        return this.getInfo();
    }
    public void Contact(Human human)
    {
        contactAction(this, human);
    }

    [Flags]
    public enum EMatterEdge//A는 물질 종류, B는 다른 특정할 수 없는 물질, A00은 minY, minX 좌표에 A가 존재함을 의미
    {
        NULL = 0,
        A00 = 1 << 0,
        A01 = 1 << 1,
        A10 = 1 << 2,
        A11 = 1 << 3,
        A_ALL = A00 | A01 | A10 | A11,
        B00 = 1 << 4,
        B01 = 1 << 5,
        B10 = 1 << 6,
        B11 = 1 << 7
    }
    public enum EMatterKind
    {
        EMPTY = 0,
        GROUND,
        WATER,
        MergeFail
    }
    public struct SMatterInfo
    {
        public EMatterKind kind;
        public EMatterEdge edge;
        public SMatterInfo(EMatterKind kind, EMatterEdge edge)
        {
            this.kind = kind;
            this.edge = edge;
        }
    }

    private static Dictionary<EMatterKind, Dictionary<EMatterEdge, Action<Matter>>> matterCreateDic = new Dictionary<EMatterKind, Dictionary<EMatterEdge, Action<Matter>>>
    {
        {EMatterKind.GROUND, new Dictionary<EMatterEdge, Action<Matter>>
            {
                {EMatterEdge.NULL, Matter.CreateEmptyGround},
                {EMatterEdge.A00, Matter.CreateGroundA00},
                {EMatterEdge.A01, Matter.CreateGroundA01},
                {EMatterEdge.A10, Matter.CreateGroundA10},
                {EMatterEdge.A11, Matter.CreateGroundA11},
                {EMatterEdge.A00|EMatterEdge.A01, Matter.CreateGroundA00A01},
                {EMatterEdge.A01|EMatterEdge.A11, Matter.CreateGroundA01A11},
                {EMatterEdge.A10|EMatterEdge.A11, Matter.CreateGroundA10A11},
                {EMatterEdge.A00|EMatterEdge.A10, Matter.CreateGroundA00A10},
                {EMatterEdge.A00|EMatterEdge.A11, Matter.CreateGroundA00A11},
                {EMatterEdge.A01|EMatterEdge.A10, Matter.CreateGroundA01A10},
                {EMatterEdge.A00|EMatterEdge.A01|EMatterEdge.A11, Matter.CreateGroundA00A01A11},
                {EMatterEdge.A01|EMatterEdge.A10|EMatterEdge.A11, Matter.CreateGroundA01A10A11},
                {EMatterEdge.A00|EMatterEdge.A10|EMatterEdge.A11, Matter.CreateGroundA00A10A11},
                {EMatterEdge.A00|EMatterEdge.A01|EMatterEdge.A10, Matter.CreateGroundA00A01A10 },
                {EMatterEdge.A_ALL, Matter.CreateGroundALL}
            }
        },
        {EMatterKind.WATER, new Dictionary<EMatterEdge, Action<Matter>>
            {
                {EMatterEdge.NULL, Matter.CreateEmptyWater},
                {EMatterEdge.A00, Matter.CreateWaterA00},
                {EMatterEdge.A01, Matter.CreateWaterA01},
                {EMatterEdge.A10, Matter.CreateWaterA10},
                {EMatterEdge.A11, Matter.CreateWaterA11},
                {EMatterEdge.A00|EMatterEdge.B11, Matter.CreateWaterA00B11},
                {EMatterEdge.A01|EMatterEdge.B10, Matter.CreateWaterA01B10},
                {EMatterEdge.A10|EMatterEdge.B01, Matter.CreateWaterA10B01},
                {EMatterEdge.A11|EMatterEdge.B00, Matter.CreateWaterA11B00},
                {EMatterEdge.A00|EMatterEdge.B01, Matter.CreateWaterA00B01},
                {EMatterEdge.A01|EMatterEdge.B11, Matter.CreateWaterA01B11},
                {EMatterEdge.A11|EMatterEdge.B10, Matter.CreateWaterA11B10},
                {EMatterEdge.A10|EMatterEdge.B00, Matter.CreateWaterA10B00},
                {EMatterEdge.A01|EMatterEdge.B00, Matter.CreateWaterA01B00},
                {EMatterEdge.A11|EMatterEdge.B01, Matter.CreateWaterA11B01},
                {EMatterEdge.A10|EMatterEdge.B11, Matter.CreateWaterA10B11},
                {EMatterEdge.A00|EMatterEdge.B10, Matter.CreateWaterA00B10},
                {EMatterEdge.A00|EMatterEdge.B01|EMatterEdge.B11, Matter.CreateWaterA00B01B11},
                {EMatterEdge.A01|EMatterEdge.B10|EMatterEdge.B11, Matter.CreateWaterA01B10B11},
                {EMatterEdge.A11|EMatterEdge.B00|EMatterEdge.B10, Matter.CreateWaterA11B00B10},
                {EMatterEdge.A10|EMatterEdge.B00|EMatterEdge.B01, Matter.CreateWaterA10B00B01},
                {EMatterEdge.A01|EMatterEdge.B00|EMatterEdge.B10, Matter.CreateWaterA01B00B10},
                {EMatterEdge.A11|EMatterEdge.B00|EMatterEdge.B01, Matter.CreateWaterA11B00B01},
                {EMatterEdge.A10|EMatterEdge.B01|EMatterEdge.B11, Matter.CreateWaterA10B01B11},
                {EMatterEdge.A00|EMatterEdge.B10|EMatterEdge.B11, Matter.CreateWaterA00B10B11},
                {EMatterEdge.A00|EMatterEdge.B01|EMatterEdge.B10, Matter.CreateWaterA00B01B10},
                {EMatterEdge.A01|EMatterEdge.B00|EMatterEdge.B11, Matter.CreateWaterA01B00B11},
                {EMatterEdge.A11|EMatterEdge.B01|EMatterEdge.B10, Matter.CreateWaterA11B01B10},
                {EMatterEdge.A10|EMatterEdge.B00|EMatterEdge.B11, Matter.CreateWaterA10B00B11},
                {EMatterEdge.A00|EMatterEdge.B01|EMatterEdge.B10|EMatterEdge.B11, Matter.CreateWaterA00B01B10B11},
                {EMatterEdge.A01|EMatterEdge.B00|EMatterEdge.B10|EMatterEdge.B11, Matter.CreateWaterA01B00B10B11},
                {EMatterEdge.A11|EMatterEdge.B00|EMatterEdge.B01|EMatterEdge.B10, Matter.CreateWaterA11B00B01B10},
                {EMatterEdge.A10|EMatterEdge.B00|EMatterEdge.B01|EMatterEdge.B11, Matter.CreateWaterA10B00B01B11},
                {EMatterEdge.A00|EMatterEdge.A01, Matter.CreateWaterA00A01},
                {EMatterEdge.A01|EMatterEdge.A11, Matter.CreateWaterA01A11},
                {EMatterEdge.A10|EMatterEdge.A11, Matter.CreateWaterA10A11},
                {EMatterEdge.A00|EMatterEdge.A10, Matter.CreateWaterA00A10},
                {EMatterEdge.A00|EMatterEdge.A01|EMatterEdge.B10, Matter.CreateWaterA00A01B10},
                {EMatterEdge.A01|EMatterEdge.A11|EMatterEdge.B00, Matter.CreateWaterA01A11B00},
                {EMatterEdge.A10|EMatterEdge.A11|EMatterEdge.B01, Matter.CreateWaterA10A11B01},
                {EMatterEdge.A00|EMatterEdge.A10|EMatterEdge.B11, Matter.CreateWaterA00A10B11},
                {EMatterEdge.A00|EMatterEdge.A01|EMatterEdge.B11, Matter.CreateWaterA00A01B11},
                {EMatterEdge.A01|EMatterEdge.A11|EMatterEdge.B10, Matter.CreateWaterA01A11B10},
                {EMatterEdge.A10|EMatterEdge.A11|EMatterEdge.B00, Matter.CreateWaterA10A11B00},
                {EMatterEdge.A00|EMatterEdge.A10|EMatterEdge.B01, Matter.CreateWaterA00A10B01},
                {EMatterEdge.A00|EMatterEdge.A01|EMatterEdge.B10|EMatterEdge.B11, Matter.CreateWaterA00A01B10B11},
                {EMatterEdge.A01|EMatterEdge.A11|EMatterEdge.B00|EMatterEdge.B10, Matter.CreateWaterA01A11B00B10},
                {EMatterEdge.A10|EMatterEdge.A11|EMatterEdge.B00|EMatterEdge.B01, Matter.CreateWaterA10A11B00B01},
                {EMatterEdge.A00|EMatterEdge.A10|EMatterEdge.B01|EMatterEdge.B11, Matter.CreateWaterA00A10B01B11},
                {EMatterEdge.A00|EMatterEdge.A11, Matter.CreateWaterA00A11},
                {EMatterEdge.A01|EMatterEdge.A10, Matter.CreateWaterA01A10},
                {EMatterEdge.A01|EMatterEdge.A10|EMatterEdge.B00, Matter.CreateWaterA01A10B00},
                {EMatterEdge.A00|EMatterEdge.A11|EMatterEdge.B01, Matter.CreateWaterA00A11B01},
                {EMatterEdge.A01|EMatterEdge.A10|EMatterEdge.B11, Matter.CreateWaterA01A10B11},
                {EMatterEdge.A00|EMatterEdge.A11|EMatterEdge.B10, Matter.CreateWaterA00A11B10},
                {EMatterEdge.A00|EMatterEdge.A11|EMatterEdge.B01|EMatterEdge.B10, Matter.CreateWaterA00A11B01B10},
                {EMatterEdge.A01|EMatterEdge.A10|EMatterEdge.B00|EMatterEdge.B11, Matter.CreateWaterA01A10B00B11},
                {EMatterEdge.A00|EMatterEdge.A01|EMatterEdge.A10, Matter.CreateWaterA00A01A10},
                {EMatterEdge.A00|EMatterEdge.A01|EMatterEdge.A11, Matter.CreateWaterA00A01A11},
                {EMatterEdge.A01|EMatterEdge.A10|EMatterEdge.A11, Matter.CreateWaterA01A10A11},
                {EMatterEdge.A00|EMatterEdge.A10|EMatterEdge.A11, Matter.CreateWaterA00A10A11},
                {EMatterEdge.A00|EMatterEdge.A01|EMatterEdge.A10|EMatterEdge.B11, Matter.CreateWaterA00A01A10B11},
                {EMatterEdge.A00|EMatterEdge.A01|EMatterEdge.A11|EMatterEdge.B10, Matter.CreateWaterA00A01A11B10},
                {EMatterEdge.A01|EMatterEdge.A10|EMatterEdge.A11|EMatterEdge.B00, Matter.CreateWaterA01A10A11B00},
                {EMatterEdge.A00|EMatterEdge.A10|EMatterEdge.A11|EMatterEdge.B01, Matter.CreateWaterA00A10A11B01},
                {EMatterEdge.A_ALL, Matter.CreateWaterALL},
            }
        },
        {EMatterKind.MergeFail, new Dictionary<EMatterEdge, Action<Matter>>{ { EMatterEdge.NULL, Matter.CreateMergeFail} } }
    };
    private static void CreateEmptyGround(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.GROUND_ALL, 0f, Matter.ContactEmptyGround, Matter.EmptyGround);
    }
    private static void ContactEmptyGround(Matter matter, Human human)
    {
        //human.NotContact();
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.EmptyGround(in pos.x, in pos.y, in size);
    }
    private static SMatterInfo EmptyGround()
    {
        return new SMatterInfo(EMatterKind.EMPTY, EMatterEdge.NULL);
    }
    private static void CreateEmptyWater(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_ALL, 0f, Matter.ContactEmptyWater, Matter.EmptyWater);
    }
    private static void ContactEmptyWater(Matter matter, Human human)
    {
        //human.NotContact();
    }
    private static SMatterInfo EmptyWater()
    {
        return new SMatterInfo(EMatterKind.EMPTY, EMatterEdge.NULL);
    }
    private static void CreateMergeFail(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.GROUND_ALL, 0f, Matter.ContactNull, Matter.MergeFailMatter);
    }
    private static void ContactNull(Matter matter, Human human)
    {

    }
    private static SMatterInfo MergeFailMatter()
    {
        return new SMatterInfo(EMatterKind.MergeFail, EMatterEdge.NULL);
    }
    private static void CreateMatterMacro(Matter matter, bool flipX, MatterList.EMatterTable eMatterTable, float rot, Action<Matter, Human> contact, Func<SMatterInfo> info)
    {
        matter.gameObject.GetComponent<SpriteRenderer>().flipX = flipX;
        matter.gameObject.GetComponent<SpriteRenderer>().sprite = matter.matterTable.SerchSprite(eMatterTable);
        matter.gameObject.GetComponent<Transform>().rotation = Quaternion.Euler(0f, 0f, rot);
        matter.contactAction = contact;
        matter.getInfo = info;
    }
    private static void CreateGroundA00(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.GROUND_A01, 270f, Matter.ContactGA00, Matter.MatterGA00);
    }
    private static void ContactGA00(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactG00(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterGA00()
    {
        return new SMatterInfo(EMatterKind.GROUND, EMatterEdge.A00);
    }
    private static void CreateGroundA01(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.GROUND_A01, 0f, Matter.ContactGA01, Matter.MatterGA01);
    }
    private static void ContactGA01(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactG01(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterGA01()
    {
        return new SMatterInfo(EMatterKind.GROUND, EMatterEdge.A01);
    }
    private static void CreateGroundA10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.GROUND_A01, 180f, Matter.ContactGA10, Matter.MatterGA10);
    }
    private static void ContactGA10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactG10(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterGA10()
    {
        return new SMatterInfo(EMatterKind.GROUND, EMatterEdge.A10);
    }
    private static void CreateGroundA11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.GROUND_A01, 90f, Matter.ContactGA11, Matter.MatterGA11);
    }
    private static void ContactGA11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactG11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterGA11()
    {
        return new SMatterInfo(EMatterKind.GROUND, EMatterEdge.A11);
    }
    private static void CreateGroundA00A01(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.GROUND_A00A01, 0f, Matter.ContactGA00A01, Matter.MatterGA00A01);
    }
    private static void ContactGA00A01(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactG00G01(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterGA00A01()
    {
        return new SMatterInfo(EMatterKind.GROUND, EMatterEdge.A00 | EMatterEdge.A01);
    }
    private static void CreateGroundA01A11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.GROUND_A00A01, 90f, Matter.ContactGA01A11, Matter.MatterGA01A11);
    }
    private static void ContactGA01A11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactG01G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterGA01A11()
    {
        return new SMatterInfo(EMatterKind.GROUND, EMatterEdge.A01 | EMatterEdge.A11);
    }
    private static void CreateGroundA10A11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.GROUND_A00A01, 180f, Matter.ContactGA10A11, Matter.MatterGA10A11);
    }
    private static void ContactGA10A11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactG10G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterGA10A11()
    {
        return new SMatterInfo(EMatterKind.GROUND, EMatterEdge.A10 | EMatterEdge.A11);
    }
    private static void CreateGroundA00A10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.GROUND_A00A01, 270f, Matter.ContactGA00A10, Matter.MatterGA00A10);
    }
    private static void ContactGA00A10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactG00G10(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterGA00A10()
    {
        return new SMatterInfo(EMatterKind.GROUND, EMatterEdge.A00 | EMatterEdge.A10);
    }
    private static void CreateGroundA00A11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.GROUND_A00A11, 0f, Matter.ContactGA00A11, Matter.MatterGA00A11);
    }
    private static void ContactGA00A11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactG00G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterGA00A11()
    {
        return new SMatterInfo(EMatterKind.GROUND, EMatterEdge.A00 | EMatterEdge.A11);
    }
    private static void CreateGroundA01A10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.GROUND_A00A11, 90f, Matter.ContactGA01A10, Matter.MatterGA01A10);
    }
    private static void ContactGA01A10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactG01G10(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterGA01A10()
    {
        return new SMatterInfo(EMatterKind.GROUND, EMatterEdge.A01 | EMatterEdge.A10);
    }
    private static void CreateGroundA00A01A11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.GROUND_A00A01A11, 0f, Matter.ContactGA00A01A11, Matter.MatterGA00A01A11);
    }
    private static void ContactGA00A01A11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactG00G01G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterGA00A01A11()
    {
        return new SMatterInfo(EMatterKind.GROUND, EMatterEdge.A00 | EMatterEdge.A01 | EMatterEdge.A11);
    }
    private static void CreateGroundA01A10A11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.GROUND_A00A01A11, 90f, Matter.ContactGA01A10A11, Matter.MatterGA01A10A11);
    }
    private static void ContactGA01A10A11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactG01G10G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterGA01A10A11()
    {
        return new SMatterInfo(EMatterKind.GROUND, EMatterEdge.A01 | EMatterEdge.A10 | EMatterEdge.A11);
    }
    private static void CreateGroundA00A10A11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.GROUND_A00A01A11, 180f, Matter.ContactGA00A10A11, Matter.MatterGA00A10A11);
    }
    private static void ContactGA00A10A11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactG00G10G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterGA00A10A11()
    {
        return new SMatterInfo(EMatterKind.GROUND, EMatterEdge.A00 | EMatterEdge.A10 | EMatterEdge.A11);
    }
    private static void CreateGroundA00A01A10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.GROUND_A00A01A11, 270f, Matter.ContactGA00A01A10, Matter.MatterGA00A01A10);
    }
    private static void ContactGA00A01A10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactG00G01G10(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterGA00A01A10()
    {
        return new SMatterInfo(EMatterKind.GROUND, EMatterEdge.A00 | EMatterEdge.A01 | EMatterEdge.A10);
    }
    private static void CreateGroundALL(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.GROUND_ALL, 0f, Matter.ContactGALL, Matter.MatterGALL);
    }
    private static void ContactGALL(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactGALL(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterGALL()
    {
        return new SMatterInfo(EMatterKind.GROUND, EMatterEdge.A_ALL);
    }

    private static void CreateWaterA00(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00, 0f, Matter.ContactWA00, Matter.MatterWA00);
    }
    private static void ContactWA00(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00);
    }
    private static void CreateWaterA01(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00, 90f, Matter.ContactWA01, Matter.MatterWA01);
    }
    private static void ContactWA01(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW01(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA01()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01);
    }
    private static void CreateWaterA10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00, 270f, Matter.ContactWA10, Matter.MatterWA10);
    }
    private static void ContactWA10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW10(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A10);
    }
    private static void CreateWaterA11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00, 180f, Matter.ContactWA11, Matter.MatterWA11);
    }
    private static void ContactWA11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A11);
    }
    private static void CreateWaterA00B11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00, 0f, Matter.ContactWA00, Matter.MatterWA00B11);
    }
    private static SMatterInfo MatterWA00B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.B11);
    }
    private static void CreateWaterA01B10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00, 90f, Matter.ContactWA01, Matter.MatterWA01B10);
    }
    private static SMatterInfo MatterWA01B10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01 | EMatterEdge.B10);
    }
    private static void CreateWaterA11B00(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00, 180f, Matter.ContactWA11, Matter.MatterWA11B00);
    }
    private static SMatterInfo MatterWA11B00()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A11 | EMatterEdge.B00);
    }
    private static void CreateWaterA10B01(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00, 270f, Matter.ContactWA10, Matter.MatterWA10B01);
    }
    private static SMatterInfo MatterWA10B01()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A10 | EMatterEdge.B01);
    }
    private static void CreateWaterA00A01(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01, 0f, Matter.ContactWA00A01, Matter.MatterWA00A01);
    }
    private static void ContactWA00A01(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00W01(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00A01()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A01);
    }
    private static void CreateWaterA01A11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01, 90f, Matter.ContactWA01A11, Matter.MatterWA01A11);
    }
    private static void ContactWA01A11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW01W11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA01A11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01 | EMatterEdge.A11);
    }
    private static void CreateWaterA10A11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01, 180f, Matter.ContactWA10A11, Matter.MatterWA10A11);
    }
    private static void ContactWA10A11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW10W11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA10A11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A10 | EMatterEdge.A11);
    }
    private static void CreateWaterA00A10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01, 270f, Matter.ContactWA00A10, Matter.MatterWA00A10);
    }
    private static void ContactWA00A10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00W10(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00A10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A10);
    }
    private static void CreateWaterA00A11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A01A10, 90f, Matter.ContactWA00A11, Matter.MatterWA00A11);
    }
    private static void ContactWA00A11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00W11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00A11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A11);
    }
    private static void CreateWaterA01A10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A01A10, 0f, Matter.ContactWA01A10, Matter.MatterWA01A10);
    }
    private static void ContactWA01A10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW01W10(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA01A10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01 | EMatterEdge.A10);
    }
    private static void CreateWaterA00A01A10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01A10, 0f, Matter.ContactWA00A01A10, Matter.MatterWA00A01A10);
    }
    private static void ContactWA00A01A10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00W01W10(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00A01A10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A01 | EMatterEdge.A10);
    }
    private static void CreateWaterA00A01A11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01A10, 90f, Matter.ContactWA00A01A11, Matter.MatterWA00A01A11);
    }
    private static void ContactWA00A01A11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00W01W11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00A01A11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A01 | EMatterEdge.A11);
    }
    private static void CreateWaterA01A10A11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01A10, 180f, Matter.ContactWA01A10A11, Matter.MatterWA01A10A11);
    }
    private static void ContactWA01A10A11(Matter matter, Human human)
    {
        matter.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW01W10W11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA01A10A11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01 | EMatterEdge.A10 | EMatterEdge.A11);
    }
    private static void CreateWaterA00A10A11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01A10, 270f, Matter.ContactWA00A10A11, Matter.MatterWA00A10A11);
    }
    private static void ContactWA00A10A11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00W10W11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00A10A11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A10 | EMatterEdge.A11);
    }
    private static void CreateWaterALL(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_ALL, 0f, Matter.ContactWALL, Matter.MatterWALL);
    }
    private static void ContactWALL(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactWALL(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWALL()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A_ALL);
    }
    private static void CreateWaterA00A01A10B11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01A10B11, 0f, Matter.ContactWA00A01A10B11, Matter.MatterWA00A01A10B11);
    }
    private static void ContactWA00A01A10B11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00W01W10G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00A01A10B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A01 | EMatterEdge.A10 | EMatterEdge.B11);
    }
    private static void CreateWaterA00A01A11B10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01A10B11, 90f, Matter.ContactWA00A01A11B10, Matter.MatterWA00A01A11B10);
    }
    private static void ContactWA00A01A11B10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00W01W11G10(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00A01A11B10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A01 | EMatterEdge.A11 | EMatterEdge.B10);
    }
    private static void CreateWaterA01A10A11B00(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01A10B11, 180f, Matter.ContactWA01A10A11B00, Matter.MatterWA01A10A11B00);
    }
    private static void ContactWA01A10A11B00(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW01W10W11G00(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA01A10A11B00()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01 | EMatterEdge.A10 | EMatterEdge.A11 | EMatterEdge.B00);
    }
    private static void CreateWaterA00A10A11B01(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01A10B11, 270f, Matter.ContactWA00A10A11B01, Matter.MatterWA00A10A11B01);
    }
    private static void ContactWA00A10A11B01(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00W10W11G01(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00A10A11B01()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A10 | EMatterEdge.A11 | EMatterEdge.B01);
    }
    private static void CreateWaterA00B01(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00B01, 0f, Matter.ContactWA00B01, Matter.MatterWA00B01);
    }
    private static void ContactWA00B01(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00G01(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00B01()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.B01);
    }
    private static void CreateWaterA01B11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00B01, 90f, Matter.ContactWA01B11, Matter.MatterWA01B11);
    }
    private static void ContactWA01B11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW01G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA01B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01 | EMatterEdge.B11);
    }
    private static void CreateWaterA11B10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00B01, 180f, Matter.ContactWA11B10, Matter.MatterWA11B10);
    }
    private static void ContactWA11B10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW11G10(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA11B10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A11 | EMatterEdge.B10);
    }
    private static void CreateWaterA10B00(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00B01, 270f, Matter.ContactWA10B00, Matter.MatterWA10B00);
    }
    private static void ContactWA10B00(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW10G00(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA10B00()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A10 | EMatterEdge.B00);
    }
    private static void CreateWaterA01B00(Matter matter)
    {
        CreateMatterMacro(matter, true, MatterList.EMatterTable.WATER_A00B01, 0f, Matter.ContactWA01B00, Matter.MatterWA01B00);
    }
    private static void ContactWA01B00(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW01G00(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA01B00()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01 | EMatterEdge.B00);
    }
    private static void CreateWaterA11B01(Matter matter)
    {
        CreateMatterMacro(matter, true, MatterList.EMatterTable.WATER_A00B01, 90f, Matter.ContactWA11B01, Matter.MatterWA11B01);
    }
    private static void ContactWA11B01(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW11G01(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA11B01()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A11 | EMatterEdge.B01);
    }
    private static void CreateWaterA10B11(Matter matter)
    {
        CreateMatterMacro(matter, true, MatterList.EMatterTable.WATER_A00B01, 180f, Matter.ContactWA10B11, Matter.MatterWA10B11);
    }
    private static void ContactWA10B11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW10G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA10B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A10 | EMatterEdge.B11);
    }
    private static void CreateWaterA00B10(Matter matter)
    {
        CreateMatterMacro(matter, true, MatterList.EMatterTable.WATER_A00B01, 270f, Matter.ContactWA00B10, Matter.MatterWA00B10);
    }
    private static void ContactWA00B10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00G10(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00B10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.B10);
    }
    private static void CreateWaterA00B01B10B11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00B01B10B11, 0f, Matter.ContactWA00B01B10B11, Matter.MatterWA00B01B10B11);
    }
    private static void ContactWA00B01B10B11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00G01G10G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00B01B10B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.B01 | EMatterEdge.B10 | EMatterEdge.B11);
    }
    private static void CreateWaterA01B00B10B11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00B01B10B11, 90f, Matter.ContactWA01B00B10B11, Matter.MatterWA01B00B10B11);
    }
    private static void ContactWA01B00B10B11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW01G00G10G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA01B00B10B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01 | EMatterEdge.B00 | EMatterEdge.B10 | EMatterEdge.B11);
    }
    private static void CreateWaterA10B00B01B11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00B01B10B11, 270f, Matter.ContactWA10B00B01B11, Matter.MatterWA10B00B01B11);
    }
    private static void ContactWA10B00B01B11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW10G00G01G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA10B00B01B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A10 | EMatterEdge.B00 | EMatterEdge.B01 | EMatterEdge.B11);
    }
    private static void CreateWaterA11B00B01B10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00B01B10B11, 180f, Matter.ContactWA11B00B01B10, Matter.MatterWA11B00B01B10);
    }
    private static void ContactWA11B00B01B10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW11G00G01G10(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA11B00B01B10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A11 | EMatterEdge.B00 | EMatterEdge.B01 | EMatterEdge.B10);
    }
    private static void CreateWaterA00B01B11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00B01B11, 0f, Matter.ContactWA00B01B11, Matter.MatterWA00B01B11);
    }
    private static void ContactWA00B01B11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00G01G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00B01B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.B01 | EMatterEdge.B11);
    }
    private static void CreateWaterA01B10B11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00B01B11, 90f, Matter.ContactWA01B10B11, Matter.MatterWA01B10B11);
    }
    private static void ContactWA01B10B11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW01G10G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA01B10B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01 | EMatterEdge.B10 | EMatterEdge.B11);
    }
    private static void CreateWaterA11B00B10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00B01B11, 180f, Matter.ContactWA11B00B10, Matter.MatterWA11B00B10);
    }
    private static void ContactWA11B00B10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW11G00G10(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA11B00B10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A11 | EMatterEdge.B00 | EMatterEdge.B10);
    }
    private static void CreateWaterA10B00B01(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00B01B11, 270f, Matter.ContactWA10B00B01, Matter.MatterWA10B00B01);
    }
    private static void ContactWA10B00B01(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW10G00G01(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA10B00B01()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A10 | EMatterEdge.B00 | EMatterEdge.B01);
    }
    private static void CreateWaterA01B00B10(Matter matter)
    {
        CreateMatterMacro(matter, true, MatterList.EMatterTable.WATER_A00B01B11, 0f, Matter.ContactWA01B00B10, Matter.MatterWA01B00B10);
    }
    private static void ContactWA01B00B10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW01G00G10(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA01B00B10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01 | EMatterEdge.B00 | EMatterEdge.B10);
    }
    private static void CreateWaterA11B00B01(Matter matter)
    {
        CreateMatterMacro(matter, true, MatterList.EMatterTable.WATER_A00B01B11, 90f, Matter.ContactWA11B00B01, Matter.MatterWA11B00B01);
    }
    private static void ContactWA11B00B01(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW11G00G01(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA11B00B01()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A11 | EMatterEdge.B00 | EMatterEdge.B01);
    }
    private static void CreateWaterA10B01B11(Matter matter)
    {
        CreateMatterMacro(matter, true, MatterList.EMatterTable.WATER_A00B01B11, 180f, Matter.ContactWA10B01B11, Matter.MatterWA10B01B11);
    }
    private static void ContactWA10B01B11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW10G01G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA10B01B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A10 | EMatterEdge.B01 | EMatterEdge.B11);
    }
    private static void CreateWaterA00B10B11(Matter matter)
    {
        CreateMatterMacro(matter, true, MatterList.EMatterTable.WATER_A00B01B11, 270f, Matter.ContactWA00B10B11, Matter.MatterWA00B10B11);
    }
    private static void ContactWA00B10B11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00G10G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00B10B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.B10 | EMatterEdge.B11);
    }
    private static void CreateWaterA01A10B00(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A01A10B00, 0f, Matter.ContactWA01A10B00, Matter.MatterWA01A10B00);
    }
    private static void ContactWA01A10B00(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW01W10G00(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA01A10B00()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01 | EMatterEdge.A10 | EMatterEdge.B00);
    }
    private static void CreateWaterA00A11B01(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A01A10B00, 90f, Matter.ContactWA00A11B01, Matter.MatterWA00A11B01);
    }
    private static void ContactWA00A11B01(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00W11G01(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00A11B01()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A11 | EMatterEdge.B01);
    }
    private static void CreateWaterA01A10B11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A01A10B00, 180f, Matter.ContactWA01A10B11, Matter.MatterWA01A10B11);
    }
    private static void ContactWA01A10B11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW01W10G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA01A10B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01 | EMatterEdge.A10 | EMatterEdge.B11);
    }
    private static void CreateWaterA00A11B10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A01A10B00, 270f, Matter.ContactWA00A11B10, Matter.MatterWA00A11B10);
    }
    private static void ContactWA00A11B10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00W11G10(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00A11B10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A11 | EMatterEdge.B10);
    }
    private static void CreateWaterA00A01B10B11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01, 0f, Matter.ContactWA00A01, Matter.MatterWA00A01B10B11);
    }
    private static SMatterInfo MatterWA00A01B10B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A01 | EMatterEdge.B10 | EMatterEdge.B11);
    }
    private static void CreateWaterA01A11B00B10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01, 90f, Matter.ContactWA01A11, Matter.MatterWA01A11B00B10);
    }
    private static SMatterInfo MatterWA01A11B00B10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01 | EMatterEdge.A11 | EMatterEdge.B00 | EMatterEdge.B10);
    }
    private static void CreateWaterA10A11B00B01(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01, 180f, Matter.ContactWA10A11, Matter.MatterWA10A11B00B01);
    }
    private static SMatterInfo MatterWA10A11B00B01()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A10 | EMatterEdge.A11 | EMatterEdge.B00 | EMatterEdge.B01);
    }
    private static void CreateWaterA00A10B01B11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01, 270f, Matter.ContactWA00A10, Matter.MatterWA00A10B01B11);
    }
    private static SMatterInfo MatterWA00A10B01B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A10 | EMatterEdge.B01 | EMatterEdge.B11);
    }
    private static void CreateWaterA00A11B01B10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A11B01B10, 0f, Matter.ContactWA00A11B01B10, Matter.MatterWA00A11B01B10);
    }
    private static void ContactWA00A11B01B10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW00W11G01G10(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00A11B01B10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A11 | EMatterEdge.B01 | EMatterEdge.B10);
    }
    private static void CreateWaterA01A10B00B11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A11B01B10, 90f, Matter.ContactWA01A10B00B11, Matter.MatterWA01A10B00B11);
    }
    private static void ContactWA01A10B00B11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        human.ContactW01W10G00G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA01A10B00B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01 | EMatterEdge.A10 | EMatterEdge.B00 | EMatterEdge.B11);
    }
    private static void CreateWaterA00B01B10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00B01B10B11, 0f, ContactWA00B01B10B11, MatterWA00B01B10);
    }
    private static SMatterInfo MatterWA00B01B10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.B01 | EMatterEdge.B10);
    }
    private static void CreateWaterA01B00B11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00B01B10B11, 90f, ContactWA01B00B10B11, MatterWA01B00B11);
    }
    private static SMatterInfo MatterWA01B00B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01 | EMatterEdge.B00 | EMatterEdge.B11);
    }
    private static void CreateWaterA11B01B10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00B01B10B11, 180f, ContactWA11B00B01B10, MatterWA11B01B10);
    }
    private static SMatterInfo MatterWA11B01B10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A11 | EMatterEdge.B01 | EMatterEdge.B10);
    }
    private static void CreateWaterA10B00B11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00B01B10B11, 270f, ContactWA10B00B01B11, MatterWA10B00B11);
    }
    private static SMatterInfo MatterWA10B00B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A10 | EMatterEdge.B00 | EMatterEdge.B11);
    }
    private static void CreateWaterA00A01B10(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01B10, 0f, ContactWA00A01B10, MatterWA00A01B10);
    }
    private static void ContactWA00A01B10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        //human.ContactW01W10G00G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00A01B10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A01 | EMatterEdge.B10);
    }
    private static void CreateWaterA01A11B00(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01B10, 90f, ContactWA01A11B00, MatterWA01A11B00);
    }
    private static void ContactWA01A11B00(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        //human.ContactW01W10G00G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA01A11B00()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01 | EMatterEdge.A11 | EMatterEdge.B00);
    }
    private static void CreateWaterA10A11B01(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01B10, 180f, ContactWA10A11B01, MatterWA10A11B01);
    }
    private static void ContactWA10A11B01(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        //human.ContactW01W10G00G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA10A11B01()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A10 | EMatterEdge.A11 | EMatterEdge.B01);
    }
    private static void CreateWaterA00A10B11(Matter matter)
    {
        CreateMatterMacro(matter, false, MatterList.EMatterTable.WATER_A00A01B10, 270f, ContactWA00A10B11, MatterWA00A10B11);
    }
    private static void ContactWA00A10B11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        //human.ContactW01W10G00G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00A10B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A10 | EMatterEdge.B11);
    }
    private static void CreateWaterA00A01B11(Matter matter)
    {
        CreateMatterMacro(matter, true, MatterList.EMatterTable.WATER_A00A01B10, 0f, ContactWA00A01B11, MatterWA00A01B11);
    }
    private static void ContactWA00A01B11(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        //human.ContactW01W10G00G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00A01B11()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A01 | EMatterEdge.B11);
    }
    private static void CreateWaterA01A11B10(Matter matter)
    {
        CreateMatterMacro(matter, true, MatterList.EMatterTable.WATER_A00A01B10, 90f, ContactWA01A11B10, MatterWA01A11B10);
    }
    private static void ContactWA01A11B10(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        //human.ContactW01W10G00G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA01A11B10()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A01 | EMatterEdge.A11 | EMatterEdge.B10);
    }
    private static void CreateWaterA10A11B00(Matter matter)
    {
        CreateMatterMacro(matter, true, MatterList.EMatterTable.WATER_A00A01B10, 180f, ContactWA10A11B00, MatterWA10A11B00);
    }
    private static void ContactWA10A11B00(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        //human.ContactW01W10G00G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA10A11B00()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A10 | EMatterEdge.A11 | EMatterEdge.B00);
    }
    private static void CreateWaterA00A10B01(Matter matter)
    {
        CreateMatterMacro(matter, true, MatterList.EMatterTable.WATER_A00A01B10, 270f, ContactWA00A10B01, MatterWA00A10B01);
    }
    private static void ContactWA00A10B01(Matter matter, Human human)
    {
        Vector3 pos = matter.gameObject.GetComponent<Transform>().position;
        float size = matter.gameObject.GetComponent<SpriteRenderer>().size.x;
        //human.ContactW01W10G00G11(pos.x, pos.y, size);
    }
    private static SMatterInfo MatterWA00A10B01()
    {
        return new SMatterInfo(EMatterKind.WATER, EMatterEdge.A00 | EMatterEdge.A10 | EMatterEdge.B01);
    }
}
