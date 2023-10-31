using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExecutableSlotsHandler : MonoBehaviour
{
    public Button[] executableSlots;
    
    [SerializeField] private UIItem item1;
    [SerializeField] private UIItem item2;
    
    void Init(Inventory inventory)
    {
        //executableSlots[0].onClick.AddListener(inventory.GetExecutableItem1().Execute());
    }
    
    void Update()
    {
        
    }
}
