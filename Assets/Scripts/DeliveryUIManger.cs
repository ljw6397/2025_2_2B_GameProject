using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryUIManger : MonoBehaviour
{
    [Header("UI 요소")]
    public Text statusText;
    public Text messageText;
    public Slider batterySlider;
    public Image batteryFill;

    [Header("게임 오브젝트")]
    public DeliveryDriver driver;

    void Start()
    {
        if(driver != null)
        {
            //이벤트 구독
            driver.driverEvents.OnMoneyChanged.AddListener(UpdateMoney);
            driver.driverEvents.OnBatteryChanged.AddListener(UpdateBattery);
            driver.driverEvents.OnDeliveryCountChanged.AddListener(UpdateDeliveryCount);
            driver.driverEvents.OnMoveStarted.AddListener(OnMoveStarted);
            driver.driverEvents.OnMoveStoped.AddListener(OnMoveStopped);
            driver.driverEvents.OnLowBatteryEmpty.AddListener(OnBatteryEmpty);
            driver.driverEvents.OnDeliveryCompleted.AddListener(OnDeliveryCompleted);
        }

        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if(statusText != null && driver != null)
        {
            statusText.text = driver.GetStatusText();
        }
    }

    void ShowMessage(string message, Color color)
    {
        if(messageText != null)
        {
            messageText.text = message;
            messageText.color = color;
            StartCoroutine(ClearMessageAgterDelay(2f));
        }
    }
    IEnumerator ClearMessageAgterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
          if(messageText != null)
        {
            messageText.text = "";
        }

    }
    void UpdateMoney(float money)
    {
        ShowMessage($"돈 : {money} 원", Color.green);
    }

    void UpdateBattery(float battery)
    {
        if(batterySlider != null)
        {
            batterySlider.value = battery / 100f;
        }

        if(batteryFill != null)
        {
            if (battery > 50f)
                batteryFill.color = Color.green;
            else if (battery > 20f)
                batteryFill.color = Color.yellow;
            else
                batteryFill.color = Color.red;
        }
    }

    void UpdateDeliveryCount(int count)
    {
        ShowMessage($"배달 완료 : {count}건", Color.blue);
    }

    void OnMoveStarted()
    {
        ShowMessage("이동 시작", Color.cyan);
    }
    void OnMoveStopped()
    {
        ShowMessage("이동 정지", Color.gray);
    }

    void OnLowBattery()
    {
        ShowMessage("배터리 부족", Color.red);
    }

    void OnBatteryEmpty()
    {
        ShowMessage("배달 완료!", Color.green);
    }
    void OnDeliveryCompleted()
    {
        ShowMessage("배달 완료!", Color.green);
    }
    void UpdateUI()
    {
        if(driver != null)
        {
            UpdateBattery(driver.batteryLevel);
            UpdateDeliveryCount(driver.deliveryCount);
        }
    }
}
