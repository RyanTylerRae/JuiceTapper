using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuiceComponent : MonoBehaviour
{
    private bool m_isFull = true;

    public float m_slideSpeedFull;
    public float m_slideSpeedEmpty;

    public Transform m_counterBegin = null;
    public Transform m_counterEnd = null;

    public delegate void JuiceDroppedDelegate();
    public JuiceDroppedDelegate OnJuiceDropped;

    public delegate void EmptyJuiceCanBeCollectedDelegate();
    public EmptyJuiceCanBeCollectedDelegate OnEmptyJuiceCanBeCollected;

    public void SetFull(bool isFull)
    {
        m_isFull = isFull;

        transform.Find("JuiceFull").gameObject.SetActive(m_isFull);
        transform.Find("JuiceEmpty").gameObject.SetActive(!m_isFull);
    }

    private void Update()
    {
        transform.position += new Vector3((m_isFull ? m_slideSpeedFull : m_slideSpeedEmpty), 0.0f, 0.0f) * Time.deltaTime;

        if (transform.position.x > m_counterEnd.position.x)
        {
            if (OnJuiceDropped != null)
            {
                OnJuiceDropped.Invoke();
            }

            Destroy(this.gameObject);
        }
        else if(transform.position.x < m_counterBegin.position.x)
        {
            if(m_isFull && OnJuiceDropped != null)
            {
                OnJuiceDropped.Invoke();
            }
            else if(!m_isFull && OnEmptyJuiceCanBeCollected != null)
            {
                OnEmptyJuiceCanBeCollected.Invoke();
            }

            Destroy(this.gameObject);
        }
    }
}
