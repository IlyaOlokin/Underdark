using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerUIManaBar : BarController
{
    [Inject]
    private void Cunstruct(Player player)
    {
        player.OnManaChanged += UpdateValue;
        player.OnMaxManaChanged += SetMaxValue;
    }
}
