using UnityEngine;
 
namespace RDTools.Runtime
{
    public static class CanvasExtensions {
        public static Vector2 WorldToCanvas(this Canvas canvas, Vector3 worldPoint, Camera camera) {
            Vector2 viewportPos = camera.WorldToViewportPoint(worldPoint);
            RectTransform canvasRect = (RectTransform)canvas.transform;
 
            return new Vector2((viewportPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
                (viewportPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));
        }
        
        public static Vector2 ScreenToCanvas(this Canvas canvas, Vector3 screenPoint, Camera camera)
        {
            Vector2 viewportPos = camera.ScreenToViewportPoint(screenPoint);
            RectTransform canvasRect = (RectTransform)canvas.transform;
 
            return new Vector2((viewportPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
                (viewportPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));
        }
    }  
}
