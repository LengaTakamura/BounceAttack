using UnityEngine;

public class DrawForward : MonoBehaviour
{
   [SerializeField]private Color _color = Color.white;
   [SerializeField] private float _length = 1f;
   private void OnDrawGizmos()
   {
      Gizmos.color = _color;
      Gizmos.DrawLine(transform.position,transform.position + transform.forward * _length);
   }
}
