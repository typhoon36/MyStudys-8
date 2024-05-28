using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HeroCtrl : MonoBehaviour
{
    public Damage_Ctrl damageCtrl; // ������ �ؽ�Ʈ�� ���� ���� ����

    public CoinMgr coinMgr; // ���� �ؽ�Ʈ�� ���� ���� ����

    //## ���ΰ� ü��
    float m_MaxHp = 200.0f;
    [HideInInspector]public float m_CurHp = 200.0f;
    public Image m_HPBar = null;


    //--- Ű���� �Է°� ���� ����
    float h = 0.0f;
    float v = 0.0f;

    float moveSpeed = 7.0f;
    Vector3 moveDir = Vector3.zero;
    //--- Ű���� �Է°� ���� ����

    //--- ���ΰ� ȭ�� ������ ���� �� ������ ���� ���� ����
    Vector3 HalfSize = Vector3.zero;
    Vector3 m_CacCurPos = Vector3.zero;
    //--- ���ΰ� ȭ�� ������ ���� �� ������ ���� ���� ����

    //--- �Ѿ� �߻� ����
    public GameObject m_BulletPrefab = null;
    public GameObject m_ShootPos = null;
    float m_ShootCool = 0.0f;       //�Ѿ� �߻� �ֱ� ���� ����
    //--- �Ѿ� �߻� ����

    // Start is called before the first frame update
    void Start()
    {
        //--- ĳ������ ���� �ݻ�����, ���� �ݻ����� ���ϱ�
        //���忡 �׷��� ��������Ʈ ������ ������
        SpriteRenderer sprRend = gameObject.GetComponentInChildren<SpriteRenderer>();
        //sprRend.bounds.size.x   ��������Ʈ�� ���� ������
        //sprRend.bounds.size.y   ��������Ʈ�� ���� ������
        // Debug.Log(sprRend.bounds.size);
        // (1.26, 1.58, 0.20)
        HalfSize.x = sprRend.bounds.size.x / 2.0f - 0.23f; //ĳ������ ���� �� ������(������ Ŀ�� ���� ����)
        HalfSize.y = sprRend.bounds.size.y / 2.0f - 0.05f; //ĳ������ ���� �� ������
        HalfSize.z = 1.0f;
        //���忡 �׷��� ��������Ʈ ������ ������
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


        // ���ΰ��� ü���� 0�� �Ǿ��� �� RŰ�� ������ ���� �ٽ� �ҷ���
        if (m_CurHp <= 0.0f && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("GameScene");
            Time.timeScale = 1.0f;
        }

        LimitMove();

        fireupdate();
    }//void Update()

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

    void fireupdate()
    {
        if (0.0f < m_ShootCool)
            m_ShootCool -= Time.deltaTime;

        if (m_ShootCool <= 0.0f)
        {
            m_ShootCool = 0.15f;

            GameObject a_cloneobj = Instantiate(m_BulletPrefab);
            a_cloneobj.transform.position = m_ShootPos.transform.position;
        }
    }// void fireupdate()

    //## �浹üũ
    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.tag == "Monster")
        {
           Monster_Ctrl a_RefMon = coll.gameObject.GetComponent<Monster_Ctrl>();
            if (a_RefMon != null)
                a_RefMon.TakeDamage(1000);

            TakeDamage(50.0f);
        }

        if(coll.tag == "Coin")
        {
            if(coinMgr != null)
            {
                coinMgr.SpawnDamageText(100, transform.position, Color.yellow);
            }

            Destroy(coll.gameObject);
        }

        if(coll.tag == "EnemyBullet")
        {
            TakeDamage(10.0f);
        }

        if(coll.tag == "Boss")
        {
            TakeDamage(200.0f);
        }


    }

    void TakeDamage(float a_Val)
    {
       
        if(m_CurHp <= 0.0f)
            return;
        
        m_CurHp -= a_Val;


        if(damageCtrl != null)
        {
            damageCtrl.SpawnDamageText(a_Val, transform.position, Color.red);
        }


        if (m_CurHp < 0.0f)
            m_CurHp = 0.0f;
           
        
        if(m_HPBar != null)
            m_HPBar.fillAmount = m_CurHp / m_MaxHp;

        //## ���ó��
        if(m_CurHp <= 0.0f)
        {
            //���� ����
            Time.timeScale = 0.0f;

           
        }

        
    }





}
