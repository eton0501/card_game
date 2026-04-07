using UnityEngine;

/// <summary>
/// 控制手動指定目標時的箭頭顯示。
/// </summary>


public class ArrowView : MonoBehaviour
{
    [SerializeField] private GameObject arrowHead;//箭頭頭部物件
    [SerializeField] private LineRenderer lineRenderer;//箭身物件
    private Vector3 startPosition;//箭頭起點
    private void Update()
    {
        Vector3 endPosition=MouseUtil.GetMousePositionInWorldSpace();//取得滑鼠目前在世界座標中的位置當作箭頭終點
        Vector3 direction=-(startPosition-arrowHead.transform.position).normalized;//計算箭頭方向向量並正規化
        lineRenderer.SetPosition(1,endPosition-direction*0.5f);//設定線段終點
        arrowHead.transform.position=endPosition;//把箭頭頭部移到滑鼠位置
        arrowHead.transform.right=direction;//
    }
    public void SetupArrow(Vector3 startPosition)//外部初始化箭頭時呼叫
    {
        this.startPosition=startPosition;
        lineRenderer.SetPosition(0,startPosition);
        lineRenderer.SetPosition(1,MouseUtil.GetMousePositionInWorldSpace());
    }
}
