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

public enum BossState
{
    Entering,
    Attacking,
    Dead
}

public class Monster_Ctrl : MonoBehaviour
{
    BossState currentState = BossState.Entering;

    public MonType m_MonType = MonType.MT_Zombi;

    //--- 몬스터 체력 변수
    float m_MaxHp = 200.0f;
    float m_CurHp = 200.0f;
    public Image m_HpBar = null;
    //--- 몬스터 체력 변수

    float m_Speed = 1.0f;   // 이동속도
    Vector3 m_CurPos;       // 위치 계산용 변수
    Vector3 m_SpawnPos;     // 스폰 위치

    float m_CacPosY = 0.0f; // 싸인 함수에 들어갈 누적 각도 계산용 변수
    float m_RandY = 0.0f; // 랜덤한 진폭값 저장용 변수
    float m_CycleSpeed = 0.0f;  // 랜덤한 진동 속도 변수

    // MissileMonster 관련 변수
    public float speed = 1f;
    public float detectionRange = 6.0f;
    private Transform player;
    private bool isChasing = false;

    //## 코인 프리팹 
    public GameObject m_CoinPrefab = null;

    //--- 총알 발사 변수
    public GameObject Bullet_Prefab = null;
    public GameObject ShootPos = null;
    float m_ShootCool = 0.0f;       //총알 발사 주기 계산용 변수
    float shootTime = 0.0f; // 총알 발사 시간을 계산하기 위한 변수
                            //--- 총알 발사 변수

    Damage_Ctrl damageCtrl; // 데미지 텍스트를 띄우기 위한 변수



    void Start()
    {
        m_SpawnPos = transform.position;    // 몬스터의 스폰 위치 저장
        m_RandY = Random.Range(0.5f, 2.0f); // Sin 함수의 랜덤 진폭
        m_CycleSpeed = Random.Range(1.8f, 5.0f);    // 진동수 랜덤값

        player = GameObject.FindGameObjectWithTag("Player").transform;

        damageCtrl = GameObject.Find("Damage_Ctrl").GetComponent<Damage_Ctrl>();


        // 보스 등장 카메라 연출
        if (m_MonType == MonType.MT_Boss)
        {
           StartCoroutine(ShakeCamera(1.0f, 0.1f));
        }

        if (m_MonType == MonType.MT_Boss)
        {
            m_MaxHp = 500.0f;
        }

        m_CurHp = m_MaxHp;

    }

    // Update is called once per frame
    void Update()
    {
        if (m_MonType == MonType.MT_Zombi)
            Zombi_AI_Update();

        if (this.transform.position.x < CameraResolution.m_ScWMin.x - 2.0f)
            Destroy(gameObject);    // 왼쪽 화면을 벗어나면 즉시 제거

        if (m_MonType == MonType.MT_Missile)
            Missile_AI_Update();

        if (this.transform.position.x < CameraResolution.m_ScWMin.x - 2.0f)
            Destroy(gameObject);    // 왼쪽 화면을 벗어나면 즉시 제거

        // 총알 발사 주기를 계산합니다.
        m_ShootCool -= Time.deltaTime;
        if (m_ShootCool <= 0f)
        {
            // 총알을 발사합니다.
            Shoot();

            // 총알 발사 주기를 재설정합니다.
            m_ShootCool = 1f;  // 1초마다 총알을 발사하도록 설정
        }

        if (m_MonType == MonType.MT_Boss)
            Boss_AI_Update();


    }

    IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Vector3 originalPosition = Camera.main.transform.position;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            Camera.main.transform.position = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        Camera.main.transform.position = originalPosition;
    }



    void Missile_AI_Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            isChasing = true;
        }
        else if (distanceToPlayer > detectionRange)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            MoveLeft();
        }
    }

    void MoveLeft()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void Zombi_AI_Update()
    {
        m_CurPos = transform.position;
        m_CurPos.x += (-1.0f * m_Speed * Time.deltaTime);
        m_CacPosY += (Time.deltaTime * m_CycleSpeed);
        m_CurPos.y = m_SpawnPos.y + Mathf.Sin(m_CacPosY) * m_RandY;
        transform.position = m_CurPos;
    }

    void Boss_AI_Update()
    {
        switch (currentState)
        {
            case BossState.Entering:
                // 등장 로직
                EnterBoss();
                break;
            case BossState.Attacking:
                // 공격 로직
                AttackBoss();
                break;
            case BossState.Dead:
                // 사망 후 재스폰 로직
                RespawnBoss();
                break;
        }
    }
    void EnterBoss()
    {
        // 보스 등장 로직 구현
        currentState = BossState.Attacking;

        // 보스 등장 시 카메라 흔들림 효과 시작
        StartCoroutine(ShakeCamera(1.0f, 0.1f));

        // 보스 공격 시작
        StartCoroutine(AttackBoss());
    }


    // 총알 속도 변수 추가
    public float bulletSpeed = 10f;

    // AttackBoss 메서드를 코루틴으로 변경
    IEnumerator AttackBoss()
    {
        // ## 360도 궁국기 3발
        for (int j = 0; j < 3; j++)
        {
            // 360도로 총알을 발사합니다.
            for (int i = 0; i < 360; i += 10) // 10도 간격으로 총알을 발사합니다.
            {
                // 총알을 생성합니다.
                GameObject bullet = Instantiate(Bullet_Prefab, ShootPos.transform.position, Quaternion.identity);

                // 총알의 방향을 설정합니다.
                float radian = i * Mathf.Deg2Rad; // 각도를 라디안으로 변환합니다.
                Vector2 direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
                bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
            }

            // 각 발사 사이에 약간의 딜레이를 두어 모든 총알이 동시에 발사되지 않도록 합니다.
            yield return new WaitForSeconds(0.5f);
        }

        // ## 주인공을 향해 단발 쏘기 7발
        for (int i = 0; i < 7; i++)
        {
            // 주인공의 위치를 찾습니다.
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // 주인공을 향하는 방향을 계산합니다.
                Vector3 direction = (player.transform.position - transform.position).normalized;

                // 총알을 생성하고, 주인공을 향하는 방향으로 발사합니다.
                GameObject bullet = Instantiate(Bullet_Prefab, ShootPos.transform.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
            }

            // 총알 사이에 약간의 딜레이를 두어 모든 총알이 동시에 발사되지 않도록 합니다.
            yield return new WaitForSeconds(0.2f);
        }
    }

    void RespawnBoss()
    {
        // 사망 후 재스폰 로직 구현
        float respawnTime = Random.Range(25.0f, 30.0f);

        // respawnTime 후에 보스 재스폰 로직 구현
        StartCoroutine(RespawnAfterSeconds(respawnTime));
    }

    IEnumerator RespawnAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        

        // 보스의 상태를 '등장' 상태로 변경
        currentState = BossState.Entering;
    }


    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "AllyBullet")
        {
            TakeDamage(80.0f);
            Destroy(coll.gameObject);
        }
    }

    public void TakeDamage(float a_Value)
    {
        if (m_CurHp <= 0.0f)  // 이 몬스터가 이미 죽어 있으면...
            return;           // 데미지를 차감할 필요 없으니 리턴 시키겠다는 뜻


        if(m_MonType == MonType.MT_Boss)
        {
            a_Value *= 0.8f;
        }


        m_CurHp -= a_Value;

      



        if (m_CurHp < 0.0f)
            m_CurHp = 0.0f;

        if (m_HpBar != null)
            m_HpBar.fillAmount = m_CurHp / m_MaxHp;


        if(damageCtrl != null)
        {
            damageCtrl.SpawnDamageText(a_Value, transform.position, Color.blue);
        }

        if (m_CurHp <= 0.0f)
        { // 몬스터 사망 처리

            // 보상주기
            Instantiate(m_CoinPrefab, transform.position, Quaternion.identity);


            // 보스가 죽었을 때 재스폰
            if (m_MonType == MonType.MT_Boss)
            {
                MonsterGenerator.Inst.monKillCount = 0;
                MonsterGenerator.Inst.isBossSpawned = false;
            }
            else
            {
                MonsterGenerator.Inst.monKillCount++;
            }

            Destroy(gameObject);
        }
    }



    void Shoot()
    {
        if (0.0f < m_ShootCool)
            m_ShootCool -= Time.deltaTime;

        if (m_ShootCool <= 0.0f)
        {
            m_ShootCool = 0.15f;

            if (Bullet_Prefab != null && ShootPos != null)
            {
                GameObject a_cloneobj = Instantiate(Bullet_Prefab);
                a_cloneobj.transform.position = ShootPos.transform.position;
            }
            else
            {
              return;
            }
        }
    }


}
