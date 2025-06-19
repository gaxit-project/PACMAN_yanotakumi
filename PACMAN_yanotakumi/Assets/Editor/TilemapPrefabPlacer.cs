using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class TilemapPrefabPlacer : EditorWindow
{
    private Tilemap tilemap; // 配置元のTilemap
    private GameObject prefab; // 配置するPrefab
    private bool clearTilesAfterPlacement = false; // 配置後にTilemapのタイルを削除するか

    [MenuItem("ツール/TilemapからPrefab配置ツール")]
    public static void ShowWindow()
    {
        GetWindow<TilemapPrefabPlacer>("TilemapからPrefab配置");
    }

    private void OnGUI()
    {
        GUILayout.Label("TilemapからPrefab配置ツール", EditorStyles.boldLabel);

        // TilemapとPrefabを選択
        tilemap = (Tilemap)EditorGUILayout.ObjectField("タイルマップ", tilemap, typeof(Tilemap), true);
        prefab = (GameObject)EditorGUILayout.ObjectField("プレハブ", prefab, typeof(GameObject), false);
        clearTilesAfterPlacement = EditorGUILayout.Toggle("配置後にタイルを削除", clearTilesAfterPlacement);

        if (GUILayout.Button("Prefabを配置"))
        {
            if (tilemap == null || prefab == null)
            {
                Debug.LogWarning("タイルマップとプレハブを選択してください！");
                return;
            }

            PlacePrefabs();
        }
    }

    private void PlacePrefabs()
    {
        Undo.RegisterCompleteObjectUndo(tilemap.gameObject, "TilemapからPrefab配置");

        BoundsInt bounds = tilemap.cellBounds;

        // タイル配置範囲を取得
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(cellPosition);

                if (tile != null)
                {
                    Vector3 worldPos = tilemap.GetCellCenterWorld(cellPosition);
                    GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    instance.transform.position = worldPos;
                    instance.transform.SetParent(tilemap.transform);

                    // タイルを削除するオプション
                    if (clearTilesAfterPlacement)
                    {
                        tilemap.SetTile(cellPosition, null);
                    }

                    Undo.RegisterCreatedObjectUndo(instance, "Prefab配置");
                }
            }
        }

        Debug.Log("Prefab配置完了！");
    }
}
