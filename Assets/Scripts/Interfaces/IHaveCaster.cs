using UnityEngine;
/// <summary>
/// 定義施放者資訊
/// </summary>
public interface IHaveCaster 
{
   CombatantView Caster{get;}//回傳施放者
}
