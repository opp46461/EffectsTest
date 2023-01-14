using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class RawImageWaterWave : MonoBehaviour
{
    //初始波浪高度(偏距)
    [SerializeField, Range(0, 1)] float heightPercent = 0.5f;
    //波浪高度(值越接近于0,波浪越矮)(振幅)
    [SerializeField] float waveHeightParame = 0.1f;
    //波浪宽度(值越接近于0,波浪越宽)(角速度)
    [SerializeField] float waveWidthParame = 7;
    //波动速度(初相)
    [SerializeField] float speed = 1;
    //默认颜色
    [SerializeField] Color32 defaultColor;

    //RawImage组件
    RawImage waveImage;
    //待显示图片
    Texture2D showTexture;
    //像素点
    Color32[] pixelsBaseArr;
    Color32[] pixelsDrawArr;

    void Start()
    {
        //获取组件
        waveImage = GetComponent<RawImage>();
        //备份贴图
        showTexture = waveImage.texture as Texture2D;
        showTexture = Instantiate(showTexture);
        waveImage.texture = showTexture;
        //初始像素点
        pixelsBaseArr = showTexture.GetPixels32();
        //变化像素点
        pixelsDrawArr = new Color32[pixelsBaseArr.Length];
    }

    void Update()
    {
        Wave();
    }

    private void FixedUpdate()
    {
    }

    public void Wave()
    {
        //拷贝初始像素点
        System.Array.Copy(pixelsBaseArr, pixelsDrawArr, pixelsBaseArr.Length);
        //遍历所有Texture像素点
        for (int x = 0; x < showTexture.width; x++)
        {
            //偏移系数
            float tmpOffectParame = waveHeightParame * Mathf.Sin(waveWidthParame * x / showTexture.width + Time.time * speed);
            //限制极值
            float tmpHeightParame = Mathf.Clamp(heightPercent + tmpOffectParame, 0, 1);
            //计算高度
            int tmpHeight = (int)(tmpHeightParame * showTexture.height);
            //高度大于指定高度的像素点,显示默认色值
            for (int y = tmpHeight; y < showTexture.height; y++)
            {
                //将showTexture的width与height转化为pixelsDrawArr像素点数组的下标
                pixelsDrawArr[y * showTexture.width + x] = defaultColor;
            }
        }
        //Texture赋值
        showTexture.SetPixels32(pixelsDrawArr);
        showTexture.Apply();
    }

}