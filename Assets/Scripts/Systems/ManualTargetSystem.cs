using UnityEngine;
/// <summary>
/// 管理手動選取目標，開始瞄準時顯示箭頭，結束瞄準用Raycast判斷是否選到敵人
/// </summary>
public class ManualTargetSystem : Singleton<ManualTargetSystem>
{
    [SerializeField] private ArrowView arrowView;//箭頭視覺物件
    [SerializeField] private LayerMask targetLayerMask;//Raycast可命中的目標圖層遮罩
    public void StartTargeting(Vector3 startPosition)//開始手動瞄準
    {
        arrowView.gameObject.SetActive(true);//開啟箭頭物件
        arrowView.SetupArrow(startPosition);//初始化箭頭起點與初始線段
    }
    public EnemyView EndTargeting(Vector3 endPosition)//結束瞄準
    {
        arrowView.gameObject.SetActive(false);//關閉箭頭顯示
        if(Physics.Raycast(endPosition,Vector3.forward,out RaycastHit hit,10f,targetLayerMask)//從endPosition往forward發射射線檢測
        && hit.collider !=null//如果有打到有效碰撞器
        && hit.transform.TryGetComponent(out EnemyView enemyView))//如果命中物件有EnemyView元件
        {
            return enemyView;//回傳該敵人
        }
        return null;//沒選到就回傳null
    }
}
