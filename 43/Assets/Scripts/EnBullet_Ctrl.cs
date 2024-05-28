using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnBullet_Ctrl : MonoBehaviour
{
    Vector3 m_DirVec = Vector3.left;       //날아가야 할 방향 벡터
    float m_MoveSpeed = 15.0f;              //이동속도

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += m_DirVec * Time.deltaTime * m_MoveSpeed;

        if (CameraResolution.m_ScWMax.x + 1.0f < transform.position.x ||
           transform.position.x < CameraResolution.m_ScWMin.x - 1.0f ||
           CameraResolution.m_ScWMax.y + 1.0f < transform.position.y ||
           transform.position.y < CameraResolution.m_ScWMin.y - 1.0f)
        { //총알이 화면을 벗어나면... 즉시 제거
            Destroy(gameObject);
        }
    }
}
