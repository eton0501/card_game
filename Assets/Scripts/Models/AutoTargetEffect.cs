using UnityEngine;
using SerializeReferenceEditor;
/// <summary>
/// 把目標選取方式(TargetMode)和效果(Effect)綁定成一組自動目標效果資料
/// </summary>

[System.Serializable]

public class AutoTargetEffect 
{
   [field:SerializeReference,SR] public TargetMode TargetMode{get;private set;}
   [field:SerializeReference,SR] public Effect Effect{get;private set;}
}
