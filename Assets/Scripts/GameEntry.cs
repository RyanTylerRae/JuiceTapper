using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    public List<GameObject> m_counters = new List<GameObject>();
    private int m_counterIndex = 0;

    public GameObject m_playerPrefab = null;
    public GameObject m_customerPrefab = null;
    public GameObject m_juicePrefab = null;

    private GameObject m_player = null;
    public GameObject m_youLose = null;

    // customer spawn values
    public float m_difficultyIncreasePerWave = 0.0f;
    public int m_customersPerWaveMin = 0;
    public int m_customersPerWaveMax = 0;
    public float m_customerSpawnDelayMin = 0.0f;
    public float m_customerSpawnDelayMax = 0.0f;
    public float m_waveDelayMin = 0.0f;
    public float m_waveDelayMax = 0.0f;

    // custom spawn logic
    private float m_waveCounter = 0.0f;
    private float m_difficultyMultiplier = 1.0f;
    private int m_customersToSpawn = 0;
    private float m_customerSpawnCounter = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        if(m_playerPrefab != null && m_counters.Count > 0)
        {
            m_player = GameObject.Instantiate(m_playerPrefab, GetNodePosition(m_counters[0], "PlayerNode"), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_player == null || m_juicePrefab == null || m_customerPrefab == null || m_counters.Count == 0)
        {
            return;
        }
        
        HandleInput();
        HandleCustomers();
    }

    public Vector3 GetNodePosition(GameObject counter, string nodeName)
    {
        Vector3? position = GetNode(counter, nodeName).position;

        if (position.HasValue)
        {
            return position.Value;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Transform GetNode(GameObject counter, string nodeName)
    {
        return counter?.transform.Find(nodeName);
    }

    private void HandleInput()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            m_counterIndex = (m_counterIndex + 1) % m_counters.Count;
            m_player.transform.position = GetNodePosition(m_counters[m_counterIndex], "PlayerNode");
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            m_counterIndex = (m_counterIndex + m_counters.Count - 1) % m_counters.Count;
            m_player.transform.position = GetNodePosition(m_counters[m_counterIndex], "PlayerNode");
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject newJuice = GameObject.Instantiate(m_juicePrefab, GetNodePosition(m_counters[m_counterIndex], "JuiceNode"), Quaternion.identity);

            JuiceComponent juiceComponent = newJuice.GetComponent<JuiceComponent>();
            juiceComponent.m_counterBegin = GetNode(m_counters[0], "JuiceNode");
            juiceComponent.m_counterEnd = GetNode(m_counters[0], "CustomerNode");
            juiceComponent.OnJuiceDropped += OnGameOver;

            int counterIndex = m_counterIndex;
            juiceComponent.OnEmptyJuiceCanBeCollected += () => { TryCollectEmptyJuice(counterIndex); };
        }
    }

    private void HandleCustomers()
    {
        m_waveCounter -= Time.deltaTime * m_difficultyMultiplier;
        if(m_waveCounter < 0.0f)
        {
             m_waveCounter = Random.Range(m_waveDelayMin, m_waveDelayMax);
            m_customersToSpawn += Random.Range(m_customersPerWaveMin, m_customersPerWaveMax);
            m_difficultyMultiplier += m_difficultyIncreasePerWave;
        }

        if(m_customersToSpawn > 0)
        {
            m_customerSpawnCounter -= Time.deltaTime * m_difficultyMultiplier;
            if(m_customerSpawnCounter < 0.0f)
            {
                m_customerSpawnCounter = Random.Range(m_customerSpawnDelayMin, m_customerSpawnDelayMax);
                --m_customersToSpawn;

                int randomCounterIndex = Random.Range(0, m_counters.Count);
                GameObject newCustomer = GameObject.Instantiate(m_customerPrefab, GetNodePosition(m_counters[randomCounterIndex], "CustomerNode"), Quaternion.identity);

                CustomerComponent customerComponent = newCustomer.GetComponent<CustomerComponent>();
                customerComponent.m_counterBegin = GetNode(m_counters[0], "JuiceNode");
                customerComponent.m_counterEnd = GetNode(m_counters[0], "CustomerNode");
                customerComponent.OnCustomerFailed += OnGameOver;
                customerComponent.OnCustomerLeftHappy += OnCustomerLeftHappy;
            }
        }
    }

    private void OnGameOver()
    {
        //if(m_youLose != null && !m_youLose.activeSelf)
        //{
        //    m_youLose.SetActive(true);

        //    Destroy(m_player);
        //}
    }

    private void TryCollectEmptyJuice(int counterIndex)
    {
        // if the player isn't there, the glass was dropped
        if(m_counterIndex != counterIndex)
        {
            OnGameOver();
        }
    }

    private void OnCustomerLeftHappy()
    {
        // a good hook for ... something? :3
    }
}
