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

    //--- ���� ü�� ����
    float m_MaxHp = 200.0f;
    float m_CurHp = 200.0f;
    public Image m_HpBar = null;
    //--- ���� ü�� ����

    float m_Speed = 1.0f;   // �̵��ӵ�
    Vector3 m_CurPos;       // ��ġ ���� ����
    Vector3 m_SpawnPos;     // ���� ��ġ

    float m_CacPosY = 0.0f; // ���� �Լ��� �� ���� ���� ���� ����
    float m_RandY = 0.0f; // ������ ������ ����� ����
    float m_CycleSpeed = 0.0f;  // ������ ���� �ӵ� ����

    // MissileMonster ���� ����
    public float speed = 1f;
    public float detectionRange = 6.0f;
    private Transform player;
    private bool isChasing = false;

    //## ���� ������ 
    public GameObject m_CoinPrefab = null;

    //--- �Ѿ� �߻� ����
    public GameObject Bullet_Prefab = null;
    public GameObject ShootPos = null;
    float m_ShootCool = 0.0f;       //�Ѿ� �߻� �ֱ� ���� ����
    float shootTime = 0.0f; // �Ѿ� �߻� �ð��� ����ϱ� ���� ����
                            //--- �Ѿ� �߻� ����

    Damage_Ctrl damageCtrl; // ������ �ؽ�Ʈ�� ���� ���� ����



    void Start()
    {
        m_SpawnPos = transform.position;    // ������ ���� ��ġ ����
        m_RandY = Random.Range(0.5f, 2.0f); // Sin �Լ��� ���� ����
        m_CycleSpeed = Random.Range(1.8f, 5.0f);    // ������ ������

        player = GameObject.FindGameObjectWithTag("Player").transform;

        damageCtrl = GameObject.Find("Damage_Ctrl").GetComponent<Damage_Ctrl>();


        // ���� ���� ī�޶� ����
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
            Destroy(gameObject);    // ���� ȭ���� ����� ��� ����

        if (m_MonType == MonType.MT_Missile)
            Missile_AI_Update();

        if (this.transform.position.x < CameraResolution.m_ScWMin.x - 2.0f)
            Destroy(gameObject);    // ���� ȭ���� ����� ��� ����

        // �Ѿ� �߻� �ֱ⸦ ����մϴ�.
        m_ShootCool -= Time.deltaTime;
        if (m_ShootCool <= 0f)
        {
            // �Ѿ��� �߻��մϴ�.
            Shoot();

            // �Ѿ� �߻� �ֱ⸦ �缳���մϴ�.
            m_ShootCool = 1f;  // 1�ʸ��� �Ѿ��� �߻��ϵ��� ����
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
                // ���� ����
                EnterBoss();
                break;
            case BossState.Attacking:
                // ���� ����
                AttackBoss();
                break;
            case BossState.Dead:
                // ��� �� �罺�� ����
                RespawnBoss();
                break;
        }
    }
    void EnterBoss()
    {
        // ���� ���� ���� ����
        currentState = BossState.Attacking;

        // ���� ���� �� ī�޶� ��鸲 ȿ�� ����
        StartCoroutine(ShakeCamera(1.0f, 0.1f));

        // ���� ���� ����
        StartCoroutine(AttackBoss());
    }


    // �Ѿ� �ӵ� ���� �߰�
    public float bulletSpeed = 10f;

    // AttackBoss �޼��带 �ڷ�ƾ���� ����
    IEnumerator AttackBoss()
    {
        // ## 360�� �ñ��� 3��
        for (int j = 0; j < 3; j++)
        {
            // 360���� �Ѿ��� �߻��մϴ�.
            for (int i = 0; i < 360; i += 10) // 10�� �������� �Ѿ��� �߻��մϴ�.
            {
                // �Ѿ��� �����մϴ�.
                GameObject bullet = Instantiate(Bullet_Prefab, ShootPos.transform.position, Quaternion.identity);

                // �Ѿ��� ������ �����մϴ�.
                float radian = i * Mathf.Deg2Rad; // ������ �������� ��ȯ�մϴ�.
                Vector2 direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
                bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
            }

            // �� �߻� ���̿� �ణ�� �����̸� �ξ� ��� �Ѿ��� ���ÿ� �߻���� �ʵ��� �մϴ�.
            yield return new WaitForSeconds(0.5f);
        }

        // ## ���ΰ��� ���� �ܹ� ��� 7��
        for (int i = 0; i < 7; i++)
        {
            // ���ΰ��� ��ġ�� ã���ϴ�.
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // ���ΰ��� ���ϴ� ������ ����մϴ�.
                Vector3 direction = (player.transform.position - transform.position).normalized;

                // �Ѿ��� �����ϰ�, ���ΰ��� ���ϴ� �������� �߻��մϴ�.
                GameObject bullet = Instantiate(Bullet_Prefab, ShootPos.transform.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
            }

            // �Ѿ� ���̿� �ణ�� �����̸� �ξ� ��� �Ѿ��� ���ÿ� �߻���� �ʵ��� �մϴ�.
            yield return new WaitForSeconds(0.2f);
        }
    }

    void RespawnBoss()
    {
        // ��� �� �罺�� ���� ����
        float respawnTime = Random.Range(25.0f, 30.0f);

        // respawnTime �Ŀ� ���� �罺�� ���� ����
        StartCoroutine(RespawnAfterSeconds(respawnTime));
    }

    IEnumerator RespawnAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        

        // ������ ���¸� '����' ���·� ����
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
        if (m_CurHp <= 0.0f)  // �� ���Ͱ� �̹� �׾� ������...
            return;           // �������� ������ �ʿ� ������ ���� ��Ű�ڴٴ� ��


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
        { // ���� ��� ó��

            // �����ֱ�
            Instantiate(m_CoinPrefab, transform.position, Quaternion.identity);


            // ������ �׾��� �� �罺��
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
