using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystem : MonoBehaviour
{
    private struct TinyTransform
    {
        public Vector3 pos;
        public Quaternion rot;
        public float angle;
        public float angleZ;
        public int generation;

        public TinyTransform(Vector3 pp, Quaternion rr, float aa = 0f, float aaZ = 0f, int l = 1)
        {
            pos = pp;
            rot = rr;
            angle = aa;
            angleZ = aaZ;
            generation = l;
        }

        public TinyTransform(TinyTransform tt)
        {
            pos = tt.pos;
            rot = tt.rot;
            angle = tt.angle;
            angleZ = tt.angleZ;
            generation = tt.generation;
        }

        public void Reset(Transform trans)
        {
            pos = trans.position;
            rot = trans.rotation;
            angle = 0f;
            angleZ = 0f;
            generation++;
        }
    }

    public GameObject branchPrefab;

    string axiom = "F";
    string alphabet = "F+-[]";

    Stack<TinyTransform> branches = new Stack<TinyTransform>();

    Dictionary<string, string> rules = new Dictionary<string, string>();
    string tree;

    [Range(0, 2)]
    public float width = 0.95f;
    [Range(0, 2)]
    public float length = 0.7f;
    [Range(0, 10)]
    public float angle = 1f;
    [Range(0, 180)]
    public float zAng = 60f;

    void Start()
    {
        rules["F"] = "FF+[+F-F-F]-[-F+F+F]";
        //rules["F"] = "F+[+F-F-[F]]--[F]";
        //rules["F"] = "F+[+F-F-[F]]--[F]";
        //rules["F"] = "F+[+F-F-]--[F]";
        rules["+"] = "+";
        rules["-"] = "-";

        tree = axiom;

        branches.Push(new TinyTransform(transform.position, transform.rotation, 0f, 0f, 1));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            tree = tree.Replace("F", rules["F"]);

            var head = branches.Peek();
            foreach (char c in tree)
            {
                float coefWidth = head.generation * width;
                float coefLength = length;
                float coefAngle = head.generation / angle;
                switch (c)
                {
                    case '+':
                        head.angle += Random.Range(10f / coefAngle, 30f / coefAngle);
                        head.angleZ += Random.Range(-zAng, zAng);
                        break;
                    case '-':
                        head.angle -= Random.Range(10f / coefAngle, 30f / coefAngle);
                        head.angleZ += Random.Range(-zAng, zAng);
                        break;
                    case '[':
                        branches.Push(new TinyTransform(head));
                        head = branches.Peek();
                        break;
                    case ']':
                        branches.Pop();
                        head = branches.Peek();
                        break;
                    case 'F':
                        GameObject go = Instantiate(branchPrefab, head.pos, head.rot);
                        go.GetComponentInChildren<MeshRenderer>().material.color = new Color(163f / 255f * (Mathf.Sqrt(head.generation) / 3f), 90f / 255f * (Mathf.Sqrt(head.generation) / 3f), 0);
                        Transform goT = go.transform;
                        goT.localScale = new Vector3(goT.localScale.x / coefWidth, goT.localScale.y * coefLength, goT.localScale.z / coefWidth);
                        goT.position += 2 * (goT.up * goT.localScale.y);
                        goT.Rotate(goT.forward, head.angle);
                        goT.Rotate(goT.up, head.angleZ);
                        head.Reset(goT);
                        branches.Pop();
                        branches.Push(head);
                        break;
                }
            }
        }
    }

    string ApplyRule(string tree)
    {
        string newTree = "";
        foreach (char c in tree)
        {
            switch (c)
            {
                case 'F':
                    newTree += rules["F"];
                    break;
                case '+':
                    newTree += rules["+"];
                    break;
                case '-':
                    newTree += rules["-"];
                    break;
            }
        }
        return newTree;
    }

}
