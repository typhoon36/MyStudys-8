using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HeroCtrl : MonoBehaviour
{
    public Damage_Ctrl damageCtrl; // 데미지 텍스트를 띄우기 위한 변수

    public CoinMgr coinMgr; // 코인 텍스트를 띄우기 위한 변수

    //## 주인공 체력
    float m_MaxHp = 200.0f;
    [HideInInspector]public float m_CurHp = 200.0f;
    public Image m_HPBar = null;


    //--- 키보드 입력값 변수 선언
    float h = 0.0f;
    float v = 0.0f;

    float moveSpeed = 7.0f;
    Vector3 moveDir = Vector3.zero;
    //--- 키보드 입력값 변수 선언

    //--- 주인공 화면 밖으로 나갈 수 없도록 막기 위한 변수
    Vector3 HalfSize = Vector3.zero;
    Vector3 m_CacCurPos = Vector3.zero;
    //--- 주인공 화면 밖으로 나갈 수 없도록 막기 위한 변수

    //--- 총알 발사 변수
    public GameObject m_BulletPrefab = null;
    public GameObject m_ShootPos = null;
    float m_ShootCool = 0.0f;       //총알 발사 주기 계산용 변수
    //--- 총알 발사 변수

    // Start is called before the first frame update
    void Start()
    {
        //--- 캐릭터의 가로 반사이즈, 세로 반사이즈 구하기
        //월드에 그려진 스프라이트 사이즈 얻어오기
        SpriteRenderer sprRend = gameObject.GetComponentInChildren<SpriteRenderer>();
        //sprRend.bounds.size.x   스프라이트의 가로 사이즈
        //sprRend.bounds.size.y   스프라이트의 세로 사이즈
        // Debug.Log(sprRend.bounds.size);
        // (1.26, 1.58, 0.20)
        HalfSize.x = sprRend.bounds.size.x / 2.0f - 0.23f; //캐릭터의 가로 반 사이즈(여백이 커서 조금 줄임)
        HalfSize.y = sprRend.bounds.size.y / 2.0f - 0.05f; //캐릭터의 세로 반 사이즈
        HalfSize.z = 1.0f;
        //월드에 그려진 스프라이트 사이즈 얻어오기
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


        // 주인공의 체력이 0이 되었을 때 R키를 누르면 씬을 다시 불러옴
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

    //## 충돌체크
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

        //## 사망처리
        if(m_CurHp <= 0.0f)
        {
            //게임 종료
            Time.timeScale = 0.0f;

           
        }

        
    }





}
