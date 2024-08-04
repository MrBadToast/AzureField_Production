using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_InventoryBehavior : StaticSerializedMonoBehaviour<UI_InventoryBehavior>
{
    //============================================
    //
    // [싱글턴 오브젝트]
    // 인벤토리를 표시하는 UI를 관리하는 클래스입니다.
    // OnEnable되면 자동으로 열립니다.
    // 
    //============================================

    [SerializeField] private Vector2 slotDistance;  // 아이템 슬롯 간격
    [SerializeField] private Vector2 offset;        // 아이템 슬롯 오프셋
    [SerializeField] private int rowCount;          // 가로줄 숫자

    [Title("References")]
    [SerializeField] private RectTransform slotViewport;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI noItemText;

    [SerializeField] private GameObject popUpWindow;
    [SerializeField] private Image popUpImage;
    [SerializeField] private TextMeshProUGUI popUpText;
    

    private MainPlayerInputActions input;

    private List<GameObject> instanciatedSlots;

    int currentScroll = 0;

    protected override void Awake()
    {
        base.Awake();
        input = new MainPlayerInputActions();
    }

    private void OnEnable()
    {
        input.Enable();
        input.UI.Navigate.performed += NavigateInventory;
    }

    /// <summary>
    /// 인벤토리 데이터 딕셔너리를 받아와 현재 UI를 세팅
    /// </summary>
    /// <param name="data"></param>
    public void SetInventory(Dictionary<ItemData, int> data)
    {
        if (instanciatedSlots == null) instanciatedSlots = new List<GameObject>();

        ClearInventory();

        KeyValuePair<ItemData, int>[] itemArray = data.ToArray();
        currentScroll = 0;

        if(itemArray.Length == 0) { noItemText.gameObject.SetActive(true); return; }
        else { noItemText.gameObject.SetActive(false); }

        for (int y = 0; y <= (int)(itemArray.Length / rowCount); y++)
        {
            for (int x = 0; x < Mathf.Clamp(itemArray.Length - y*rowCount,0,rowCount); x++)
            {
                GameObject newSlot = Instantiate(slotPrefab, slotViewport,false);
                newSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(slotDistance.x * x + offset.x, slotDistance.y * y + offset.y);
                InventorySlotSingle slot = newSlot.GetComponent<InventorySlotSingle>();
                slot.InitializeSlot(this,itemArray[x + y*rowCount].Key, itemArray[x + y *rowCount].Value);
                instanciatedSlots.Add(newSlot);
            }
        }
    }

    /// <summary>
    /// 조개 수량 텍스트를 설정
    /// </summary>
    /// <param name="data"></param>
    public void SetMoney(int value)
    {
        moneyText.text = value.ToString();
    }

    /// <summary>
    /// 인벤토리 슬롯들을 모두 제거
    /// </summary>
    public void ClearInventory()
    {
        foreach (var slot in instanciatedSlots)
        {
            Destroy(slot.gameObject);
        }

        instanciatedSlots.Clear();

        currentScroll = 0;
    }

    public void NavigateInventory(InputAction.CallbackContext context)
    {
        if(context.ReadValue<Vector2>() == Vector2.up)
        {
            ScrollInventoryUP();
        }
        else if(context.ReadValue<Vector2>() == Vector2.down)
        {
            ScrollInventoryDOWN();
        }               
    }

    public void ScrollInventoryUP()
    {
        if (currentScroll == 0) return;

        currentScroll--;
    }

    public void ScrollInventoryDOWN()
    {
        if (currentScroll >= instanciatedSlots.Count / rowCount) return;

        currentScroll++;
    }

    private void Update()
    {
        slotViewport.anchoredPosition = Vector2.Lerp(slotViewport.anchoredPosition, new Vector2(slotViewport.anchoredPosition.x, currentScroll * slotDistance.y), 0.2f);
    }

    private void OnDisable()
    {
        input.UI.Navigate.performed -= NavigateInventory;
        input.Disable();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        int itemCount = 20;
        float squareSize = 175;
        for (int y = 0; y < (int)(itemCount / rowCount); y++)
        {
            for (int x = 0; x < rowCount; x++)
            {
                Gizmos.DrawWireCube(slotViewport.position + new Vector3(slotDistance.x * x , -slotDistance.y * y , 0f) + new Vector3(offset.x,offset.y,0f) + squareSize * new Vector3(0.5f,-0.5f,0f) , squareSize * new Vector3(1,1,0));
            }
        }
    }

    public void OpenItemPopUp(ItemData item)
    {
        popUpWindow.SetActive(true);
        popUpText.text= item.ItemDiscription.GetLocalizedString();
        popUpImage.sprite = item.ItemPopUpImage;
    }

    public void CloseItemPopUp()
    {
        popUpText.text = "";
        popUpImage.sprite = null;
        popUpWindow.SetActive(false);
    }
}
