using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Gen : MonoBehaviour
{
    public GameObject[] MonPrefab;

    float m_SpDelta = 0.0f;     //���� �ֱ� ���� ����
    float m_DiffSpawn = 1.0f;   //���̵��� ���� ���� ���� �ֱ� ����

    //--- �̱��� ����
    public static Mon_Gen Inst = null;

    void Awake()
    {
        Inst = this;
    }
    //--- �̱��� ����

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_SpDelta -= Time.deltaTime;
        if(m_SpDelta < 0.0f)
        {
            m_SpDelta  = m_DiffSpawn;

            GameObject Go = null;
            Go = Instantiate(MonPrefab[0]); //���� ����
            float py = Random.Range(-3.0f, 3.0f);
            Go.transform.position = new Vector3(CameraResolution.m_ScWMax.x + 1.0f, py, 0.0f);

        }//if(m_SpDelta < 0.0f)

    }//void Update()
}
