using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCtrl : MonoBehaviour
{
    //##Ű���� �Է°� 
    float h = 0.0f;
    float v = 0.0f;

    float moveSpeed = 7.0f;
    Vector3 moveDir = Vector3.zero;
   

    //## ȭ������
    Vector3 HalfSize = Vector3.zero;
    Vector3 m_CacCurPos = Vector3.zero;
    

    //## �Ѿ� �߻� ����
    public GameObject m_BulletPrefab = null;
    public GameObject m_ShootPos = null;
    float m_ShootCool = 0.0f;       
    //�Ѿ� �߻� �ֱ� ���


    // Start is called before the first frame update
    void Start()
    {
        //## ĳ������ �ݻ����� ���ϱ�
       
        SpriteRenderer sprRend = gameObject.GetComponentInChildren<SpriteRenderer>();
        //sprRend.bounds.size.x   ��������Ʈ�� ���� ������
        //sprRend.bounds.size.y   ��������Ʈ�� ���� ������
        
        HalfSize.x = sprRend.bounds.size.x / 2.0f - 0.23f;
        //ĳ������ ���� �� ������(������ Ŀ�� ���� ����)
        HalfSize.y = sprRend.bounds.size.y / 2.0f - 0.05f; 
        //ĳ������ ���� �� ������
        HalfSize.z = 1.0f;
       
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        if(h != 0.0f || v != 0.0f)
        {
            moveDir = new Vector3(h, v, 0.0f);
            if (1.0f < moveDir.magnitude)
                moveDir.Normalize();

            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }//if(h != 0.0f || v != 0.0f)

        LimitMove();

        FireUpdate();
    }//void Update()

    //## ȭ�� ������ ������ ���ϰ� ����
    void LimitMove()
    {
        m_CacCurPos = transform.position;

        if(m_CacCurPos.x < CameraResolution.m_ScWMin.x + HalfSize.x)
            m_CacCurPos.x = CameraResolution.m_ScWMin.x + HalfSize.x;

        if (CameraResolution.m_ScWMax.x - HalfSize.x < m_CacCurPos.x)
            m_CacCurPos.x = CameraResolution.m_ScWMax.x - HalfSize.x;

        if(m_CacCurPos.y < CameraResolution.m_ScWMin.y + HalfSize.y)
            m_CacCurPos.y = CameraResolution.m_ScWMin.y + HalfSize.y;

        if (CameraResolution.m_ScWMax.y - HalfSize.y < m_CacCurPos.y)
            m_CacCurPos.y = CameraResolution.m_ScWMax.y - HalfSize.y;

        transform.position = m_CacCurPos;

    }

    //## �Ѿ� �߻�
    void FireUpdate()
    {
        if (0.0f < m_ShootCool)
            m_ShootCool -= Time.deltaTime;

        if(m_ShootCool <= 0.0f)
        {
            m_ShootCool = 0.15f;

            GameObject a_CloneObj = Instantiate(m_BulletPrefab);
            a_CloneObj.transform.position = m_ShootPos.transform.position;
        }
    }// void FireUpdate()
}