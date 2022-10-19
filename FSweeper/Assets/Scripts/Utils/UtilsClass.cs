using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnoKunG.Utils
{
    public static class UtilsClass
    {
        public const int sortingOrderDefault = 5000;

        // Create Simple World Text with default setting (customizable)
        public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontsize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = sortingOrderDefault)
        {
            if (color == null) color = Color.white;
            return CreateWorldText(text, parent, localPosition, fontsize, (Color)color, textAnchor, textAlignment, sortingOrder);
        }

        // Create detailed World Text
        public static TextMesh CreateWorldText(string text, Transform parent, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment,int sortingOrder)
        {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.text = text;
            textMesh.color = color;
            textMesh.fontSize = fontSize;
            textMesh.alignment = textAlignment;
            textMesh.anchor = textAnchor;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMesh;
        }

        // Get Mouse Position ignoring z position in 2d (z = 0)
        public static Vector3 GetMouseWorldPosition2D()
        {
            Vector3 worldPosition = GetMouseWorldPosition();
            worldPosition.z = 0;
            return worldPosition;
        }

        // Get Typical Mouse Position
        public static Vector3 GetMouseWorldPosition()
        {
            return GetMouseWorldPosition(Input.mousePosition, Camera.main);
        }

        // Get Mouse Position with specific Camera
        public static Vector3 GetMouseWorldPosition(Camera worldCamera)
        {
            return GetMouseWorldPosition(Input.mousePosition, worldCamera);
        }

        // Get Mouse Position with specific Position and Camera
        public static Vector3 GetMouseWorldPosition(Vector3 screenPosition, Camera  worldCamera) 
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }

        public static void SetSortingOrderOffset(GameObject gameObject, int sortingOrderOffset)
        {
            gameObject.GetComponent<Renderer>().sortingOrder = sortingOrderDefault + sortingOrderOffset;
        }
    }
}
