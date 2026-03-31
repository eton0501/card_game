using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 負責在場景中建立EnemyView物件的類別
/// </summary>
public class EnemyViewCreator : Singleton<EnemyViewCreator>
{
   [SerializeField] private EnemyView enemyViewPrefab;//敵人的預製體
   public EnemyView CreateEnemyView(EnemyData enemyData,Vector3 position,Quaternion rotation)//在指定位置和旋轉角度建立一個新的EnemyView
    {
        EnemyView enemyView=Instantiate(enemyViewPrefab,position,rotation);//生成一個新的EnemyView物件
        enemyView.Setup(enemyData);//初始化敵人資料
        return enemyView;//回傳建立好的EnemyView
    }
}
