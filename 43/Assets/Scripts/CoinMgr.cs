using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinMgr : MonoBehaviour
{

    [Header("Coin Text")]
    public Transform m_HUD_Canvas = null;
    //데미지 표시용 변수
    public GameObject m_CoinPrefab = null;


    public void SpawnDamageText(float a_Value, Vector3 a_TextPos, Color a_Color)
    {
        if (m_CoinPrefab == null && m_HUD_Canvas == null)
            return;

        GameObject a_DmgClone = Instantiate(m_CoinPrefab);
        a_DmgClone.transform.SetParent(m_HUD_Canvas, false);
        a_DmgClone.transform.position = a_TextPos;

        Text a_CurText = a_DmgClone.GetComponentInChildren<Text>();
        if (0.0f < a_Value)

            a_CurText.text = "+" + (int)a_Value;
        else if (a_Value < 0.0f)
        {
            a_Value = Mathf.Abs(a_Value);
            a_CurText.text = "-" + (int)a_Value;
        }
        else

            a_CurText.text = a_Value.ToString();

        a_CurText.color = a_Color;
        Destroy(a_DmgClone, 1.1f);


    }
}
