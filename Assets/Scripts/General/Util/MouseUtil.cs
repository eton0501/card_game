using UnityEngine;
/// <summary>
/// 負責將螢幕上2D滑鼠座標轉換為遊戲世界中的3D座標
/// </summary>
public class MouseUtil
{
    private static Camera camera=Camera.main;//取得主攝影機
    public static Vector3 GetMousePositionInWorldSpace(float zValue = 0f)//取得滑鼠在世界座標中的位置
    {
        Plane dragPlane=new(camera.transform.forward,new Vector3(0,0,zValue));//建立一個虛擬平面，用來接住從攝影機射出的射線
        Ray ray=camera.ScreenPointToRay(Input.mousePosition);//從攝影機向滑鼠位置射出一條射線
        if(dragPlane.Raycast(ray, out float distance))//計算射線與虛擬平面的交點
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
}
