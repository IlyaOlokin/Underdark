using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickUper
{
    bool TryPickUpItem(Item item, int amount);
}
