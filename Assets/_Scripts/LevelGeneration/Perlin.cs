using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perlin : ScriptableObject, ISerializationCallbackReceiver {
   public class Column : Dictionary<int, float> { }
   public class Dict : Dictionary<int, Column> { }

   Dict map;
   [SerializeField]
   float celw;
   [SerializeField]
   float celh;
   [SerializeField]
   List<int> _x;
   [SerializeField]
   List<int> _y;
   [SerializeField]
   List<float> _v;

   public static Perlin Create(float w, float h) {
      Perlin res = ScriptableObject.CreateInstance<Perlin>();
      res.map = new Dict();
      res.celh = h;
      res.celw = w;
      return res;
   }

   void Reset() {
      if (map == null) {
         map = new Dict();
      } else {
         foreach (Column column in map.Values) {
            column.Clear();
         }
      }
   }

   void LoadPoint(int x, int y, float v) {
      if (!map.ContainsKey(x)) {
         map[x] = new Column();
      }
      Column c = map[x];
      c[y] = v;
   }

   public void OnBeforeSerialize() {
      _v = new List<float>();
      _x = new List<int>();
      _y = new List<int>();
      foreach (int x in map.Keys) {
         Column c = map[x];
         foreach (int y in c.Keys) {
            float v = c[y];
            _v.Add(v);
            _x.Add(x);
            _y.Add(y);
         }
      }
   }

   public void OnAfterDeserialize() {
      map = new Dict();
      int count = _v.Count;
      for (int i = 0; i < count; i++) {
         LoadPoint(_x[i], _y[i], _v[i]);
      }
      _v.Clear();
      _v = null;
      _x.Clear();
      _x = null;
      _y.Clear();
      _y = null;
   }

   public void SetSize(float w, float h) {
      Reset();
      celh = h;
      celw = w;
   }

   float GetValue(int x, int y) {
      if (!map.ContainsKey(x)) {
         map[x] = new Column();
      }
      Column column = map[x];
      if (!column.ContainsKey(y)) {
         column[y] = Random.value;
      }
      return column[y];
   }

   public float Sample(float x, float y) {
      int xa = Mathf.FloorToInt(x / celw);
      int ya = Mathf.FloorToInt(y / celh);
      float xp = (x - (xa * celw)) / celw;
      float yp = (y - (ya * celh)) / celh;
      float va = Mathf.Lerp(GetValue(xa, ya), GetValue(xa + 1, ya), xp);
      float vb = Mathf.Lerp(GetValue(xa, ya + 1), GetValue(xa + 1, ya + 1), xp);
      return Mathf.Lerp(va, vb, yp);
   }
}
