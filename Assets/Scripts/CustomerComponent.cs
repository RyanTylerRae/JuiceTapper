using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerComponent : MonoBehaviour
{
    public float m_slideSpeed = 0.0f;
    public float m_slideSpeedDrinking = 0.0f;

    public float m_drinkingDelayMin = 0.0f;
    public float m_drinkingDelayMax = 0.0f;
    private float m_drinkingCounter = 0.0f;
    private bool m_isDrinking = false;

    public float m_leaveEmptyJuiceChance = 1.0f;

    public Transform m_counterBegin = null;
    public Transform m_counterEnd = null;

    // the customer was not served enough drinks! you fool!
    public delegate void CustomerFailedDelegate();
    public CustomerFailedDelegate OnCustomerFailed;

    // the customer was served!
    public delegate void CustomerLeftHappyDelegate();
    public CustomerLeftHappyDelegate OnCustomerLeftHappy;

    private void Update()
    {
        transform.position += new Vector3(m_isDrinking ? m_slideSpeedDrinking : m_slideSpeed, 0.0f, 0.0f) * Time.deltaTime;

        if(m_isDrinking)
        {
            m_drinkingCounter -= Time.deltaTime;
            if(m_drinkingCounter < 0.0f)
            {
                m_drinkingCounter = 0.0f;
                m_isDrinking = false;
            }
        }

        if (transform.position.x < m_counterBegin.position.x)
        {
            if (OnCustomerFailed != null)
            {
                OnCustomerFailed.Invoke();
            }

            Destroy(this.gameObject);
        }
        else if(transform.position.x > m_counterEnd.position.x)
        {
            if(OnCustomerLeftHappy != null)
            {
                OnCustomerLeftHappy.Invoke();
            }

            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(m_isDrinking)
        {
            return;
        }

        JuiceComponent juiceComponent = collision.gameObject.GetComponent<JuiceComponent>();
        if(juiceComponent != null)
        {
            bool leaveEmptyJuice = Random.Range(0.0f, 1.0f) < m_leaveEmptyJuiceChance;

            if(leaveEmptyJuice)
            {
                juiceComponent.SetFull(false);
            }

            m_drinkingCounter = Random.Range(m_drinkingDelayMin, m_drinkingDelayMax);
            m_isDrinking = true;

            if(!leaveEmptyJuice)
            {
                Destroy(juiceComponent.gameObject);
            }
        }
    }
}
