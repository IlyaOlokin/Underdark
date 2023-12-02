using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Portal : MonoBehaviour
{
    private FastTravelUI fastTravelUI;
    
    [Inject]
    private void Construct(FastTravelUI fastTravelUI)
    {
        this.fastTravelUI = fastTravelUI;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            fastTravelUI.transform.SetAsLastSibling();
            fastTravelUI.gameObject.SetActive(true);
        }
    }
}
