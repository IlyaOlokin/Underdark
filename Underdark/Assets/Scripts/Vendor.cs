using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Vendor : MonoBehaviour
{
    private VendorUI vendorUI;
    
    [Inject]
    private void Construct(VendorUI vendorUI)
    {
        this.vendorUI = vendorUI;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            vendorUI.OpenWindow();
        }
    }
}
