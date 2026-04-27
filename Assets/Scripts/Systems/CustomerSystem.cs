using UnityEngine;
using System;
using System.Collections.Generic;

public class CustomerSystem : MonoBehaviour
{
    [SerializeField] private Transform customersParent;
    [SerializeField] private float baseSpawnInterval = 2.5f;
    [SerializeField] private float minSpawnInterval = 0.75f;
    [SerializeField] private float spawnReductionPerLevel = 0.04f;
    [SerializeField] private float roadWidth = 100f;
    [SerializeField] private float roadVerticalOffset = 0f;

    private float timeSinceLastSpawn = 0f;
    private List<Customer> activeCustomers = new();
    private Queue<Customer> customerPool = new();
    private GameObject customerPrefab;
    private int consecutiveMisses = 0;

    public event Action OnCustomerSpawned;
    public event Action<Customer> OnCustomerServed;
    public event Action<Customer> OnCustomerMissed;

    public void Initialize()
    {
        if (customersParent == null)
        {
            GameObject parentObj = new("Customers");
            customersParent = parentObj.transform;
        }

        timeSinceLastSpawn = 0f;
    }

    public void Tick(float deltaTime)
    {
        timeSinceLastSpawn += deltaTime;
        float spawnInterval = GetSpawnInterval();

        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnCustomer();
            timeSinceLastSpawn = 0f;
        }

        // Update active customers
        for (int i = activeCustomers.Count - 1; i >= 0; i--)
        {
            Customer customer = activeCustomers[i];
            customer.Tick(deltaTime);

            if (customer.IsTimedOut)
            {
                OnCustomerMissed?.Invoke(customer);
                GameManager.Instance.Economy.ProcessMiss();
                consecutiveMisses++;
                ReturnCustomerToPool(customer);
                activeCustomers.RemoveAt(i);
            }
        }
    }

    private float GetSpawnInterval()
    {
        int spawnLevel = GameManager.Instance.Upgrades.GetUpgradeLevel("louder_bell");
        float interval = baseSpawnInterval * Mathf.Pow(1f - spawnReductionPerLevel, spawnLevel);
        return Mathf.Max(interval, minSpawnInterval);
    }

    private void SpawnCustomer()
    {
        bool isImpatient = UnityEngine.Random.value > 0.7f; // 30% impatient

        Customer customer = GetOrCreateCustomer(isImpatient);
        customer.SetPosition(GetRandomSpawnPosition());
        customer.Show();

        activeCustomers.Add(customer);
        OnCustomerSpawned?.Invoke();

        GameManager.Instance.Analytics.TrackEvent("customer_spawned", new() {
            { "type", isImpatient ? "impatient" : "normal" }
        });
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float truckX = GameManager.Instance.Truck.GetTruckPosition().x;
        // Spawn ahead of truck
        float spawnX = truckX + UnityEngine.Random.Range(150f, 300f);
        float spawnY = roadVerticalOffset + UnityEngine.Random.Range(-20f, 20f);
        return new Vector3(spawnX, spawnY, 0f);
    }

    private Customer GetOrCreateCustomer(bool isImpatient)
    {
        if (customerPool.Count > 0)
        {
            Customer customer = customerPool.Dequeue();
            customer.SetCustomerType(isImpatient);
            return customer;
        }

        // Create new customer
        GameObject customerObj = new("Customer_" + activeCustomers.Count);
        customerObj.transform.SetParent(customersParent);
        Customer customerComponent = customerObj.AddComponent<Customer>();
        customerComponent.SetCustomerType(isImpatient);
        return customerComponent;
    }

    private void ReturnCustomerToPool(Customer customer)
    {
        customer.Hide();
        customerPool.Enqueue(customer);
    }

    public void TapCustomer(Customer customer)
    {
        if (!activeCustomers.Contains(customer))
            return;

        bool isInRange = GameManager.Instance.Truck.IsCustomerInRange(customer.GetPosition());

        if (isInRange)
        {
            GameManager.Instance.Economy.ProcessSale();
            GameManager.Instance.Audio.PlaySound("sale");
            OnCustomerServed?.Invoke(customer);
            consecutiveMisses = 0;

            GameManager.Instance.Analytics.TrackEvent("first_sale", new() {
                { "time_since_start", Time.time.ToString("F1") }
            });
        }
        else
        {
            GameManager.Instance.Audio.PlaySound("miss");
            customer.PlayMissReaction();
            consecutiveMisses++;
        }

        ReturnCustomerToPool(customer);
        activeCustomers.Remove(customer);
    }

    public int GetActiveCustomerCount() => activeCustomers.Count;
    public List<Customer> GetActiveCustomers() => activeCustomers;
}

public class Customer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 position;
    private float patience = 5f;
    private float patienceTimer = 0f;
    private bool isImpatient = false;
    private bool isTimedOut = false;

    public Vector3 GetPosition() => transform.position;
    public bool IsTimedOut => isTimedOut;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
    }

    public void SetCustomerType(bool impatient)
    {
        isImpatient = impatient;
        patience = impatient ? 3f : 5f;
    }

    public void SetPosition(Vector3 newPosition)
    {
        position = newPosition;
        transform.position = newPosition;
        patienceTimer = 0f;
        isTimedOut = false;
    }

    public void Tick(float deltaTime)
    {
        patienceTimer += deltaTime;
        if (patienceTimer >= patience)
        {
            isTimedOut = true;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if (spriteRenderer != null)
        {
            // Draw simple customer sprite (will be replaced with real art)
            spriteRenderer.color = isImpatient ? Color.yellow : Color.white;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void PlayMissReaction()
    {
        // Animation will go here
    }
}
