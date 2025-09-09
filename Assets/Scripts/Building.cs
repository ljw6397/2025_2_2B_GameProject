using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour
{

    [Header("�ǹ� ����")]
    public BuildingType BuildingType;
    public string bulidingName = "�ǹ�";

    [System.Serializable]
    public class BuildingEvents
    {
        public UnityEvent<string> OnDriverEntered;
        public UnityEvent<string> OnDriverExited;
        public UnityEvent<BuildingType> OnServiceUsed;

    }

    public BuildingEvents buildingEvents;


     void Start()
    {
        SetupBuilding();
    }
    void SetupBuilding()
    {
        Renderer renderer = GetComponent<Renderer>();
        if(renderer != null)
        {
            Material mat = renderer.material;
            switch(BuildingType)
            {
                case BuildingType.Restaurant:
                    mat.color = Color.red;
                    bulidingName = "������";
                    break;

                case BuildingType.Customer:
                    mat.color = Color.green;
                    bulidingName = "�� ��";
                    break;

                case BuildingType.ChargingStation:
                    mat.color = Color.yellow;
                    bulidingName = "������";
                    break;
            }
        }

        Collider col = GetComponent<Collider>();
        if(col != null) { col.isTrigger = true; }
    }
     void OnTriggerEnter(Collider other)
    {
        DeliveryDriver driver = other.GetComponent<DeliveryDriver>();
        if(driver != null)
        {
            buildingEvents.OnDriverEntered?.Invoke(bulidingName);
            HandleDriverService(driver);
        }
    }

     void OnTriggerExit(Collider other)
    {
        DeliveryDriver driver = other.GetComponent<DeliveryDriver>();
        if (driver != null)
        {
            buildingEvents.OnDriverExited?.Invoke(bulidingName);
            Debug.Log($"{bulidingName}�� �������ϴ�.");
        }
    }
    void HandleDriverService(DeliveryDriver driver)
    {
        switch(BuildingType)
        {
            case BuildingType.Restaurant:
                Debug.Log($"{bulidingName} ���� ������ �Ⱦ� �߽��ϴ�.");
                break;

            case BuildingType.Customer:
                Debug.Log($"{bulidingName} ���� ��� �Ϸ�");
                driver.CompleteDelivery();
                break;

            case BuildingType.ChargingStation:
                Debug.Log($"{bulidingName} ���� ���͸��� ���� �߽��ϴ�.");
                driver.ChargeBattery();
                break;
        }
    }
}
