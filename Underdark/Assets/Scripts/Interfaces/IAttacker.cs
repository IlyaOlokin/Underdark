using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;

public interface IAttacker
{
    void Attack();
    void Attack(IDamageable unit);
}
