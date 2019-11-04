using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TM2SubMesh : MonoBehaviour {
    [HideInInspector]
    public Rect bounds;
    [HideInInspector]
    public bool dirty;
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.black * 0.25f;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}



[Serializable]
public class TM2SubMeshCollection : ISerializationCallbackReceiver {
    [Serializable]
    public struct Coord : IComparable<Coord> {
        public int x;
        public int y;

        public Coord(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public int CompareTo(Coord other) {
            int res = x - other.x;
            if (res == 0) {
                res = y - other.y;
            }
            return res;
        }
    }

    const int sliceWidth = 10;
    const int sliceHeight = 10;
    public float chunkSize;
    public Coord[] serializedCoords;
    public TM2SubMesh[] serializedSubMeshes;

    Map<Coord, TM2SubMesh> subMeshes = new Map<Coord, TM2SubMesh>();

    public Coord ToCoord(Vector2 point) {
        Coord result;
        result.x = Mathf.FloorToInt(point.x / (sliceWidth * chunkSize));
        result.y = Mathf.FloorToInt(point.y / (sliceHeight * chunkSize));
        return result;
    }

    public void OnAfterDeserialize() {
        Debug.Log("deserialized submeshes");
    }

    public void OnBeforeSerialize() {
        Debug.Log("serializing submeshes");
    }

    void MarkAllDirty() {
        foreach (KeyValuePair<Coord, TM2SubMesh> value in subMeshes) {
            value.Value.dirty = true;
        }
    }

    void MarkDirty(Rect r) {
        Coord min = ToCoord(r.min);
        Coord max = ToCoord(r.max);
        for (int y = min.y; y <= max.y; y++) {
            for (int x = min.x; x <= max.x; x++) {
                TM2SubMesh sm = subMeshes[new Coord(x, y)];
                sm.dirty = true;
            }
        }
    }

    void MarkDirty(Zone z) {
        MarkDirty(z.bounds);
    }

    void MarkDirty(ZoneLink link) {
        //todo: bresenham thing? probably not important
        //MarkDirty(link.bounds);
    }
}