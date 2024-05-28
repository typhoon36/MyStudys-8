using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGenerator : MonoBehaviour
{
    public GameObject[] MonPrefab;
    public int monKillCount = 0;
    public bool isBossSpawned = false;

    float m_SpDelta = 0.0f;     //스폰 주기 계산용 변수
    float m_DiffSpawn = 3.0f;   //난이도에 따른 몬스터 스폰 주기 변수


    //--- 싱글톤 패턴
    public static MonsterGenerator Inst = null;

    void Awake()
    {
        Inst = this;
    }
    //--- 싱글톤 패턴

    void Update()
    {
        m_SpDelta -= Time.deltaTime;

        if (m_SpDelta <= 0.0f)
        {
            m_SpDelta = Random.Range(1.0f, m_DiffSpawn); // 스폰 간격을 랜덤하게 설정

            if (monKillCount >= 7 && !isBossSpawned)
            {
                // 보스 스폰
                SpawnBoss();
                isBossSpawned = true;
            }
            else if (!isBossSpawned)
            {
                // 일반 몬스터 스폰
                float py = Random.Range(-3.0f, 3.0f);
                GameObject Go = Instantiate(MonPrefab[0]); //좀비 스폰
                Go.transform.position = new Vector3(CameraResolution.m_ScWMax.x + 1.0f, py, 0.0f);
                StartCoroutine(SpawnMon(Random.Range(2.0f, 4.0f), py)); // 2초에서 4초 후에 미사일 스폰
            }
        }
    }

    public void SpawnBoss()
    {
        float py = Random.Range(-3.0f, 3.0f);
        GameObject Go = Instantiate(MonPrefab[2]); //보스 스폰
        Go.transform.position = new Vector3(5.0f, py, 0.0f);
    }



    public IEnumerator SpawnMon(float delay, float py)
    {
        yield return new WaitForSeconds(delay);

        if (MonPrefab[1] != null)
        {
            GameObject Go = Instantiate(MonPrefab[1]); //미사일 스폰
            Go.transform.position = new Vector3(CameraResolution.m_ScWMax.x + 1.0f, py, 0.0f);
        }

       
    }

    public IEnumerator RespawnBoss(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 보스가 존재하지 않을 때만 보스를 스폰
        if (MonPrefab[2] != null && GameObject.FindGameObjectWithTag("Boss") == null)
        {
            float py = Random.Range(-3.0f, 3.0f);
            GameObject Go = Instantiate(MonPrefab[2]); //보스 재스폰
            Go.transform.position = new Vector3(CameraResolution.m_ScWMax.x + 1.0f, py, 0.0f);
        }
    }

}
