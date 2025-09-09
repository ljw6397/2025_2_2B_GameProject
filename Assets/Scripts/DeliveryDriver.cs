using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeliveryDriver : MonoBehaviour
{
    [Header("배달원 설정")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 10.0f;

    [Header("상태")]
    public float currentMoney = 0;
    public float batteryLevel = 100f;
    public int deliveryCount = 0;

    //Event system
    [System.Serializable]
    public class DriverEvents
    {
        [Header("Move Event")]
        public UnityEvent OnMoveStarted;
        public UnityEvent OnMoveStoped;

        [Header("상태 변화 Event")]
        public UnityEvent<float> OnMoneyChanged;
        public UnityEvent<float> OnBatteryChanged;
        public UnityEvent<int> OnDeliveryCountChanged;

        [Header("경고 Event")]
        public UnityEvent OnLowBattery;
        public UnityEvent OnLowBatteryEmpty;
        public UnityEvent OnDeliveryCompleted;
    }

    public DriverEvents driverEvents;

    public bool isMoving = false;
    // Start is called before the first frame update

    //
    void Start()
    {
        //초기 상태 Event
        driverEvents.OnMoneyChanged?.Invoke(currentMoney);
        driverEvents.OnBatteryChanged?.Invoke(batteryLevel);
        driverEvents.OnDeliveryCountChanged ?.Invoke(deliveryCount);
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        UpdateBattery();
    }

    void HandleMovement()
    {
        if(batteryLevel <= 0)
        {
            if(isMoving)
            {

            }
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(horizontal, 0, vertical);

        if(moveDir.magnitude > 0.1f)
        {
            if(!isMoving)
            {
                StartMoving();
            }

            //이동 처리
            moveDir = moveDir.normalized;
            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

            //회전 처리
            if(moveDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            ChangeBattery(-Time.deltaTime * 3.0f); //이동할때마다 배터리 
        }
        else
        {
            if(isMoving)
            {
                StopMoving();
            }
        }
    }

    void ChangeBattery(float amount)
    {
        float oldBattery = batteryLevel;
        batteryLevel += amount;
        batteryLevel = Mathf.Clamp(batteryLevel, 0, 100);

        //배터리
        driverEvents.OnBatteryChanged?.Invoke(batteryLevel);

        //배터리 상태에 따른 경고
        if(oldBattery > 20f && batteryLevel <= 20f)
        {
            driverEvents.OnLowBattery?.Invoke();     //배터리 부족 
        }
        if (oldBattery > 0f && batteryLevel <= 0f)
        {
            driverEvents.OnLowBatteryEmpty?.Invoke();     //배터리 방전
        }
    }

    void StartMoving()
    {
        isMoving = true;
        driverEvents.OnMoveStarted?.Invoke(); // Mouse Ex
    }

    void StopMoving()
    {
        isMoving = false;
        driverEvents.OnMoveStoped?.Invoke();   //Mouse Ex
    }

    void UpdateBattery()
    {
        //아무것도 안해도 조금씩 배터리 소모
        if(batteryLevel > 0)
        {
            ChangeBattery(-Time.deltaTime * 0.5f);
        }
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        driverEvents.OnMoneyChanged?.Invoke(currentMoney);   //돈 획득 후 이벤트 처리
    }

    public void CompleteDelivery()       //배달 완료
    {
        deliveryCount++;
        float reward = Random.Range(3000, 8000);

        AddMoney(reward);
        driverEvents.OnDeliveryCountChanged.Invoke(deliveryCount);
        driverEvents.OnDeliveryCompleted?.Invoke();
    }

    public void ChargeBattery()
    {
        ChangeBattery(100f - batteryLevel);    //배터리 완충

    }
    public string GetStatusText()
    {
        return $"돈 : {currentMoney:F0} 원 | 배터리 : {batteryLevel:F1}% | 배달 : {deliveryCount} 건";
    }
    
    public bool CanMove()
    {
        return batteryLevel > 0f;
    }
}
