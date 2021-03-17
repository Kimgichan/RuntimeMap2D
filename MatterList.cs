using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatterList : MonoBehaviour
{
    [SerializeField]
    private Sprite[] readTable;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Sprite SerchSprite(EMatterTable useSprite)
    {
        return readTable[(int)(useSprite)];
    }

    public enum EMatterTable
    {
        GROUND_A00A01 = 0,
        GROUND_A00A01A11,
        GROUND_A00A11,
        GROUND_A01,
        GROUND_ALL,
        WATER_A00,
        WATER_A00A01,
        WATER_A00A01A10,
        WATER_A00A01A10B11,
        WATER_A00B01,
        WATER_A00B01B10B11,
        WATER_A00B01B11,
        WATER_A01A10B00,
        WATER_A01A10,
        WATER_A00A11B01B10,
        WATER_ALL,
        WATER_A00A01B10
    }
}
