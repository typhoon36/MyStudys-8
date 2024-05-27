using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonType
{
    MT_Zombi,
    MT_Missile,
    MT_Boss
}

public class Monster_Ctrl : MonoBehaviour
{
    public MonType m_MonType = MonType.MT_Zombi;

    //## 몬스터 체력 
    float m_MaxHp = 200.0f;
    float m_CurHp = 200.0f;
    public Image m_HpBar = null;


    float m_Speed = 4.0f;   //이동속도
    Vector3 m_CurPos;       //위치 계산용 변수
    Vector3 m_SpawnPos;     //스폰 위치

    float m_CacPosY = 0.0f; //싸인 함수에 들어갈 누적 각도 계산용 변수
    float m_RandY   = 0.0f; //랜덤한 진폭값 저장용 변수
    float m_CycleSpeed = 0.0f;  //랜덤한 진동 속도 변수

    // Start is called before the first frame update
    void Start()
    {
        m_SpawnPos = transform.position;    //몬스터의 스폰 위치 저장
        m_RandY = Random.Range(0.5f, 2.0f); //Sin 함수의 랜덤 진폭
        m_CycleSpeed = Random.Range(1.8f, 5.0f);    //진동수 랜덤값
    }

    // Update is called once per frame
    void Update()
    {
        if (m_MonType == MonType.MT_Zombi)
            Zombi_AI_Update();

        if (this.transform.position.x < CameraResolution.m_ScWMin.x - 2.0f)
            Destroy(gameObject);    //왼쪽 화면을 벗어나면 즉시 제거
    }

    void Zombi_AI_Update()
    {
        m_CurPos = transform.position;
        m_CurPos.x += (-1.0f * m_Speed * Time.deltaTime);
        m_CacPosY += (Time.deltaTime * m_CycleSpeed);
        m_CurPos.y = m_SpawnPos.y + Mathf.Sin(m_CacPosY) * m_RandY;
        transform.position = m_CurPos;  
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.tag == "AllyBullet")
        {
            TakeDamage(80.0f);
            Destroy(coll.gameObject);
        }
    }//void OnTriggerEnter2D(Collider2D coll)

    public void TakeDamage(float a_Value)
    {
        if (m_CurHp <= 0.0f)  
            return;          

        m_CurHp -= a_Value;
        if (m_CurHp < 0.0f)
            m_CurHp = 0.0f;

        if (m_HpBar != null)
            m_HpBar.fillAmount = m_CurHp / m_MaxHp;

        if(m_CurHp <= 0.0f)
        { //몬스터 사망 처리

            //보상주기

            Destroy(gameObject);
        }
    }//public void TakeDamage(float a_Value)
}
