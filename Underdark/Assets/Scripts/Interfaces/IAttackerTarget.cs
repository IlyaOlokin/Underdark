using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackerTarget : IAttacker
{
    void Attack(IDamageable unit);
}
