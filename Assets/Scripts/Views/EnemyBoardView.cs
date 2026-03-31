using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Collections;
/// <summary>
/// 負責管理場上所有敵人的視覺呈現
/// </summary>
public class EnemyBoardView : MonoBehaviour
{
    [SerializeField] private List<Transform> slots;//先設定好每個敵人的站位，一個slot可以站一個敵人
    public List<EnemyView> EnemyViews{get;private set;}=new();//目前場上所有敵人的EnemyView清單
    public void AddEnemy(EnemyData enemyData)//根據EnemyData在場上生成一個敵人
    {
        Transform slot=slots[EnemyViews.Count];//用目前場上的敵人數量決定新的敵人要站哪個位置
        EnemyView enemyView=EnemyViewCreator.Instance.CreateEnemyView(enemyData,slot.position,slot.rotation);//在指定位置生成敵人視覺物件
        enemyView.transform.parent=slot;//將敵人物件設為slot的子物件
        EnemyViews.Add(enemyView);//將新敵人加入清單
    }
    public IEnumerator RemoveEnemy(EnemyView enemyView)//從場上移除指定的敵人
    {
        EnemyViews.Remove(enemyView);//從清單中刪除
        Tween tween=enemyView.transform.DOScale(Vector3.zero,0.25f);//播放縮小至消失的死亡動畫
        yield return tween.WaitForCompletion();//等待動畫播完
        Destroy(enemyView.gameObject);//摧毀物件
    }
}
