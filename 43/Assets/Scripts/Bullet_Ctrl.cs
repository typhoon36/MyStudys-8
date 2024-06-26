using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ctrl : MonoBehaviour
{
    Vector3 m_DirVec = Vector3.right;       //날아가야 할 방향 벡터
    float m_MoveSpeed = 15.0f;              //이동속도

    //float m_LifeTime = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        //m_LifeTime = 3.0f;
        Destroy(gameObject, 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //if(0.0f < m_LifeTime)
        //{
        //    m_LifeTime -= Time.deltaTime;
        //    if(m_LifeTime <= 0.0f)
        //        Destroy(gameObject);
        //}

        transform.position += m_DirVec * Time.deltaTime * m_MoveSpeed;

        if(CameraResolution.m_ScWMax.x + 1.0f < transform.position.x ||
           transform.position.x < CameraResolution.m_ScWMin.x - 1.0f ||
           CameraResolution.m_ScWMax.y + 1.0f < transform.position.y ||
           transform.position.y < CameraResolution.m_ScWMin.y - 1.0f)
        { //총알이 화면을 벗어나면... 즉시 제거
            Destroy(gameObject);
        }

    }//void Update()
}
