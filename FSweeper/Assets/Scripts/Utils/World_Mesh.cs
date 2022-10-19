using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZnoKunG.Utils
{
    public class World_Mesh
    {
        private const int sortingOrderDefault = 5000;

        public GameObject gameObject;
        public Transform transform;
        public Material material;
        private Vector3[] vertices;
        private Vector2[] uv;
        private int[] triangles;
        private Mesh mesh;
        public static World_Mesh Create(Vector3 position, float eulerZ, float width, float height, Material material, UVCoords uvCoords, int sortingOrdertOffset = 0)
        {
            return new World_Mesh(null, position, Vector3.one, eulerZ, width, height, material, uvCoords, sortingOrdertOffset);
        }
        public World_Mesh(Transform parent, Vector3 localPosition, Vector3 localScale, float eulerZ, float meshWidth, float meshHeight, Material material, UVCoords uvCoords, int sortingOrderOffset)
        {
            this.material = material;
            vertices = new Vector3[4];
            uv = new Vector2[4];
            triangles = new int[6];

            float meshWidthHalf = meshWidth / 2;
            float meshHeightHalf = meshHeight / 2;

            vertices[0] = new Vector3(-meshWidthHalf, meshHeightHalf);
            vertices[1] = new Vector3(meshWidthHalf, meshHeightHalf);
            vertices[2] = new Vector3(-meshWidthHalf, -meshHeightHalf);
            vertices[3] = new Vector3(meshWidthHalf, -meshHeightHalf);

            if (uvCoords == null)
            {
                uvCoords = new UVCoords(0, 0, material.mainTexture.width, material.mainTexture.height);
            }

            Vector2[] uvArray = GetUVRectanglesFromPixels(uvCoords.x, uvCoords.y, uvCoords.width, uvCoords.height, material.mainTexture.width, material.mainTexture.height);

            ApplyUVToUVArray(uvArray, ref uv);

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 2;
            triangles[4] = 1;
            triangles[5] = 3;

            mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            gameObject = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer));
            gameObject.transform.parent = parent;
            gameObject.transform.localPosition = localPosition;
            gameObject.transform.localScale = localScale;
            gameObject.transform.localEulerAngles = new Vector3(0, 0, eulerZ);

            gameObject.GetComponent<MeshFilter>().mesh = mesh;
            gameObject.GetComponent<MeshRenderer>().material = material;

            transform = gameObject.transform;

            SetSortingOrderOffset(sortingOrderOffset);
        }

        public class UVCoords
        {
            public int x, y, width, height;
            public UVCoords(int x, int y, int width, int height)
            {
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
            }
        }
        private Vector2 ConvertPixelsToUVCoordinates(int x, int y, int textureWidth, int textureHeight)
        {
            return new Vector2((float)x / textureWidth, (float)y / textureHeight);
        }
        private Vector2[] GetUVRectanglesFromPixels(int x, int y, int width, int height, int textureWidth, int textureHeight)
        {
            return new Vector2[]
            {
                ConvertPixelsToUVCoordinates(x, y + height, textureWidth, textureHeight),
                ConvertPixelsToUVCoordinates(x + width, y + height, textureWidth, textureHeight),
                ConvertPixelsToUVCoordinates(x, y, textureWidth, textureHeight),
                ConvertPixelsToUVCoordinates(x + width, y, textureWidth, textureHeight)
            };
        }

        private void ApplyUVToUVArray(Vector2[] uv, ref Vector2[] mainUV)
        {
            if (uv == null || uv.Length < 4 || mainUV == null || mainUV.Length < 4) throw new System.Exception();
            mainUV[0] = uv[0];
            mainUV[1] = uv[1];
            mainUV[2] = uv[2];
            mainUV[3] = uv[3];
        }

        public static int GetSortingOrder(Vector3 position, int offset, int baseSortingOrder = sortingOrderDefault)
        {
            return (int)(baseSortingOrder - position.y) + offset;
        }
        public void SetSortingOrderOffset(int sortingOrderOffset)
        {
            SetSortingOrder(GetSortingOrder(gameObject.transform.position, sortingOrderOffset));
        }

        public void SetSortingOrder(int sortingOrder)
        {
            gameObject.GetComponent<Renderer>().sortingOrder = sortingOrder;
        }

        public int GetSortingOrder()
        {
            return gameObject.GetComponent<Renderer>().sortingOrder;
        }

        public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles)
        {
            vertices = new Vector3[4 * quadCount];
            uvs = new Vector2[4 * quadCount];
            triangles = new int[6 * quadCount];
        }

        private static Quaternion[] QuaternionEulerArray;
        private static void CreateQuaternionEulerArray()
        {
            if (QuaternionEulerArray != null) return;
            QuaternionEulerArray = new Quaternion[360];
            for (int i = 0; i < 360; i++)
            {
                QuaternionEulerArray[i] = Quaternion.Euler(0, 0, i);
            }
        }

        private static Quaternion GetQuaternionEuler(float rotationFloat)
        { 
            int rotation = Mathf.RoundToInt(rotationFloat);
            rotation = rotation % 360;
            if (rotation < 0) rotation += 360;
            if (QuaternionEulerArray == null) CreateQuaternionEulerArray();
            return QuaternionEulerArray[rotation];
        }

        public static void SetMeshComponentsWithIndex(Vector3[] vertices, Vector2[] uvs, int[] triangles, int index, Vector3 position, float rotation, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
        {
            int vIndex = index * 4;
            int vIndex0 = vIndex;
            int vIndex1 = vIndex + 1;
            int vIndex2 = vIndex + 2;
            int vIndex3 = vIndex + 3;

            baseSize *= .5f;

            bool skewed = baseSize.x != baseSize.y;
            if (skewed)
            {
                vertices[vIndex0] = position + GetQuaternionEuler(rotation) * new Vector3(-baseSize.x, -baseSize.y);
                vertices[vIndex1] = position + GetQuaternionEuler(rotation) * new Vector3(-baseSize.x, baseSize.y);
                vertices[vIndex2] = position + GetQuaternionEuler(rotation) * new Vector3(baseSize.x, baseSize.y);
                vertices[vIndex3] = position + GetQuaternionEuler(rotation) * new Vector3(baseSize.x, -baseSize.y);
            } else
            {
                vertices[vIndex0] = position + GetQuaternionEuler(rotation - 180) * baseSize;
                vertices[vIndex1] = position + GetQuaternionEuler(rotation - 270) * baseSize;
                vertices[vIndex2] = position + GetQuaternionEuler(rotation - 0) * baseSize;
                vertices[vIndex3] = position + GetQuaternionEuler(rotation - 90) * baseSize;
            }

            // Relocate UVs
            uvs[vIndex0] = new Vector2(uv00.x, uv00.y);
            uvs[vIndex1] = new Vector2(uv00.x, uv11.y);
            uvs[vIndex2] = new Vector2(uv11.x, uv11.y);
            uvs[vIndex3] = new Vector2(uv11.x, uv00.y);

            // Create triangles
            int tIndex = index * 6;
            triangles[tIndex + 0] = vIndex0;
            triangles[tIndex + 1] = vIndex1;
            triangles[tIndex + 2] = vIndex2;

            triangles[tIndex + 3] = vIndex0;
            triangles[tIndex + 4] = vIndex2;
            triangles[tIndex + 5] = vIndex3;
        }
    }
}
