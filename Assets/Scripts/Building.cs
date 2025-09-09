using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour
{

    [Header("건물 정보")]
    public BuildingType BuildingType;
    public string bulidingName = "건물";

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
                    bulidingName = "음식점";
                    break;

                case BuildingType.Customer:
                    mat.color = Color.green;
                    bulidingName = "고객 집";
                    break;

                case BuildingType.ChargingStation:
                    mat.color = Color.yellow;
                    bulidingName = "충전소";
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
            Debug.Log($"{bulidingName}을 떠났습니다.");
        }
    }
    void HandleDriverService(DeliveryDriver driver)
    {
        switch(BuildingType)
        {
            case BuildingType.Restaurant:
                Debug.Log($"{bulidingName} 에서 음식을 픽업 했습니다.");
                break;

            case BuildingType.Customer:
                Debug.Log($"{bulidingName} 에서 배달 완료");
                driver.CompleteDelivery();
                break;

            case BuildingType.ChargingStation:
                Debug.Log($"{bulidingName} 에서 배터리를 충전 했습니다.");
                driver.ChargeBattery();
                break;
        }
    }
}
